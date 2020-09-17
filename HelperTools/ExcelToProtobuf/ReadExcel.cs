using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;

namespace ExcelToProtobuf
{
    // excel转.proto
    public class ReadExcel
    {
        private static string protoFileName = "clientData.proto";
        private static string outFilePath ="";

        //获取所有的excel路径
        public static string[] GetAllExcelPaths(string excelDir)
        {
            return Directory.GetFiles(excelDir, "*.xlsx", SearchOption.AllDirectories);
        }

        //创建客户端协议文件
        public static void CreateClientFile(string outPathDir)
        {
            outFilePath = Path.Combine(outPathDir, protoFileName);
            using (FileStream fileStream = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
            {
                string preContent = "syntax = \"proto3\"; \noption optimize_for = LITE_RUNTIME;\n\n";
                byte[] datas = Encoding.Default.GetBytes(preContent);
                fileStream.Write(datas, 0, datas.Length);
            }
        }

        //打开excel
        public static void OpenExcel(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                
                if (workbook != null)
                {
                    int sheetNums = workbook.NumberOfSheets;
                    for (int i = 0; i < sheetNums; i++)
                    {
                        ISheet sheet = workbook.GetSheetAt(i);
                        CreateProtoFile(sheet);
                    }
                }
            }
        }

        //创建.proto文件
        private static void CreateProtoFile(ISheet sheet)
        {
            if (!sheet.SheetName.Contains("st_")) // 只有以特定开头的表名才生成proto文件
            {
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            string content = "message fileName_data {\ncontent}\n\n";
            int Nums = sheet.LastRowNum;
            if (Nums >= 4) //加上结束标志行大于规则数量才算是有效的表
            {
                IRow row = sheet.GetRow(0);
                int cellNum = row.LastCellNum;
                int index = 0;
                for (int i = 0; i < cellNum; i++)
                {
                    if (row.GetCell(i).StringCellValue == "#") continue; // 客户端、服务器所需的字段不一样，筛选条件也不一样，自己做调整
                    if (row.GetCell(i).StringCellValue != "__END__")
                    {
                        string typeName = GetProtoType(sheet.GetRow(3).GetCell(i).StringCellValue.Trim()); // 类型名字
                        string valName = sheet.GetRow(1).GetCell(i).StringCellValue; // 字段名字
                        index++;
                        stringBuilder.Append($"\t{typeName}");
                        stringBuilder.Append($"\t{valName}");
                        stringBuilder.Append($"\t=\t{index};\n");
                    }
                    else
                    {
                        break;
                    }
                }

                if (stringBuilder.Length>0)
                {
                    content = content.Replace("fileName", sheet.SheetName);
                    content = content.Replace("content", stringBuilder.ToString());
                    CreateFile(content);
                    CreateConfigFile(sheet.SheetName);
                    System.Console.WriteLine(sheet.SheetName + "转换完成！！！\n");
                }
            }
        }

        //获取proto3类型
        private static string GetProtoType(string typeName)
        {
            switch (typeName)
            {
                case "int":
                case "key":
                    return "sint32";
                case "string":
                    return "string";
                case "intarray":
                case "intArray":
                    return "repeated int32";
                case "floatarray":
                    return "repeated float";
                case "stringarray":
                case "stringArray":
                    return "repeated string";
                default:
                    return typeName;
            }
        }

        //创建文件
        private static void CreateFile(string content)
        {
            if (outFilePath .Length>0 && content.Length > 0)
            {
                using (FileStream fs = new FileStream(outFilePath, FileMode.Append, FileAccess.Write))
                {
                    byte[] datas = Encoding.Default.GetBytes(content);
                    fs.Write(datas, 0, datas.Length);
                }
            }
        }

        //创建配置proto文件
        private static void CreateConfigFile(string msgName)
        {
            string content = "message msg_config {\n\trepeated msg_data datas = 1;\n}\n";
            content = content.Replace("msg", msgName);
            using (FileStream fs = new FileStream(outFilePath, FileMode.Append, FileAccess.Write))
            {
                byte[] datas = Encoding.Default.GetBytes(content);
                fs.Write(datas, 0, datas.Length);
            }
        }

    }
}
