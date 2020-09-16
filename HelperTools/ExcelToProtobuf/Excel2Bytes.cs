using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToProtobuf
{
    // excel数据转二进制数据
    public class Excel2Bytes
    {
        private static Assembly assembly;

        public static void Data(string dllDir, string excelDir)
        {
            string dllFilePath = Path.Combine(dllDir, Common.dllName);
            if (!File.Exists(dllFilePath))
            {
                Console.WriteLine($"{dllFilePath}:dll文件路径不存在！！！");
                return;
            }
            assembly = Assembly.LoadFile(dllFilePath);
            string[] excelFilePaths = ReadExcel.GetAllExcelPaths(excelDir);
            for (int i = 0; i < excelFilePaths.Length; i++)
            {
                OpenExcel(excelFilePaths[i]);
            }
        }

        private static void OpenExcel(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);

                if (workbook != null)
                {
                    int sheetNums = workbook.NumberOfSheets;
                    for (int i = 0; i < sheetNums; i++)
                    {
                        ISheet sheet = workbook.GetSheetAt(i);
                        WriteData(sheet);
                    }
                }
            }
        }

        // 三个文件：序列化后二进制文件、位置文件、明文数据
        private static void WriteData(ISheet sheet)
        {
            if (!sheet.SheetName.Contains("st_")) // 只有以特定开头的表名才需要生成bytes文件
            {
                return;
            }
            object classIns = null;
            int Nums = sheet.LastRowNum; // 行数
            if (Nums > 4)
            {
                string typeName = sheet.SheetName + "_data";
                classIns = assembly.CreateInstance(typeName);
                string fileName = typeName + ".bytes";

                IRow row = sheet.GetRow(0); // 第一行数据
                int cellNum = row.LastCellNum;

                List<int> validColIndex = new List<int>(); //有效列 #是注释列
                for (int i = 0; i < cellNum; i++)
                {
                    if (row.GetCell(i).StringCellValue == "#") continue;
                    if (row.GetCell(i).StringCellValue != "__END__")
                    {
                        validColIndex.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 4; i < Nums; i++)
                {
                    IRow rowData = sheet.GetRow(i);
                    if (row.GetCell(0).StringCellValue == "__END__")  // 结束标志放在第一列 
                    {
                        break;
                    }
                    for (int j = 0; j < validColIndex.Count; j++)
                    {
                        // TODO 取数据 赋值
                    }
                }

            }
        }

    }
}
