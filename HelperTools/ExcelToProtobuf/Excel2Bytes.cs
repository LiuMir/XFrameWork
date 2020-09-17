using Google.Protobuf;
using Google.Protobuf.Collections;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

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
            if (Directory.Exists(Common.dataFileDirPath)) // 导数据前删除里面的所有文件
            {
                Directory.Delete(Common.dataFileDirPath, true);
            }
            Directory.CreateDirectory(Common.dataFileDirPath);

            if (Directory.Exists(Common.lawsDataFileDirPath))
            {
                Directory.Delete(Common.lawsDataFileDirPath, true);
            }
            Directory.CreateDirectory(Common.lawsDataFileDirPath);

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
            int Nums = sheet.LastRowNum; // 行数
            if (Nums > 4)
            {
                object configIns = assembly.CreateInstance(sheet.SheetName + "_config"); // 创建数据容器类
                Type configType = configIns.GetType();
                PropertyInfo propertyInfo = configType.GetProperty("Datas"); // 取到容器字段
                object DatasVal = propertyInfo.GetValue(configIns); // 取到容器

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

                IRow nameRow = sheet.GetRow(1); // 字段行数据
                IRow typeRow = sheet.GetRow(3); // 类型行数据
                for (int i = 4; i < Nums; i++)
                {
                    IRow rowData = sheet.GetRow(i);
                    if (rowData.GetCell(0).ToString() == "__END__")  // 结束标志放在第一列 
                    {
                        break;
                    }
                    object dataIns = assembly.CreateInstance(sheet.SheetName + "_data"); // 数据实例
                    MethodInfo addMethod = propertyInfo.PropertyType.GetMethod("Add", new Type[] { dataIns.GetType() }); // 获取容器Add方法,需要标明具体添加哪种类型
                    addMethod?.Invoke(DatasVal, new[] { dataIns });
                    for (int j = 0; j < validColIndex.Count; j++)
                    {
                        string type = typeRow.GetCell(validColIndex[j]) !=null ? typeRow.GetCell(validColIndex[j]).ToString() : "";
                        string fieldName = nameRow.GetCell(validColIndex[j]) != null ? nameRow.GetCell(validColIndex[j]).ToString() : "";
                        string value = rowData.GetCell(validColIndex[j]) != null ? rowData.GetCell(validColIndex[j]).ToString() : "";
                        if (value.Length > 0)
                        {
                            //采用字段不使用属性是因为repeated没有set方法
                            FieldInfo fieldInfo = dataIns.GetType().GetField(Common.GetPbFieldName(fieldName), BindingFlags.NonPublic|BindingFlags.Instance); 
                            object realVal = GetRealVal(type, value); //获取真实值
                            fieldInfo.SetValue(dataIns, realVal);
                        }
                    }
                }
                Serialize(configIns, sheet.SheetName);
                WriteLawsData(configIns, sheet.SheetName);
            }
        }

        // 根据类型获取真实值
        //  *********** 根据自己的填表规则进行修改 ***********
        private static object GetRealVal(string type, string value)
        {
            switch (type)
            {
                case "int":
                case "key":
                    return int.Parse(value);
                case "string":
                    return value;
                case "intarray":
                case "intArray":
                    return HandlerArray(value, (val) => { return int.Parse(val); } );
                case "floatarray":
                    return HandlerArray(value, (val) => { return float.Parse(val); });
                case "stringarray":
                case "stringArray":
                    return HandlerArray(value, (val) => { return val.Trim(); });
                default:
                    Console.WriteLine("excel填写的类型错误！！！");
                    return value;
            }
        }

        // 返回数组，[]包裹 暂时以逗号分隔，根据自己的需求进行更改
        private static RepeatedField<T> HandlerArray<T>(string value, Func<string,T> func)
        {
            if (null == value)
            {
                return null;
            }
            string val = value.TrimStart('[');
            val = val.TrimEnd(']');
            string[] datas = val.Split(',');
            RepeatedField<T> newValue = new RepeatedField<T>();
            for (int i = 0; i < datas.Length; i++)
            {
                newValue.Add(func(datas[i]));
            }
            return newValue;
        }

        // 序列化后二进制文件
        private static void Serialize(object obj, string sheetName)
        {
            string fileName = sheetName + ".bytes";
            using (FileStream fs = new FileStream(Path.Combine(Common.dataFileDirPath, fileName), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                MessageExtensions.WriteTo(obj as IMessage, fs);
            }
        }

        // 位置数据，为了加载数据的时候不用全部加载（按需加载）
        // 自定义选择 如果需要自己编写 .proto文件
        // 作为优化的备选，SerializePos里面的逻辑都可以删除
        // 思路为：
        // 一、修改序列化方法（Serialize),对xxxconfig类里面的datas遍历序列化后依次写进文件
        // 二、对datas里每一个对象进行相应的计算，例如：index、pos、length
        private static void SerializePos(object obj, string sheetName)
        {
            string fileName = sheetName + "Pos.bytes";
            object configIns = assembly.CreateInstance("clientDataPosConfig"); // 创建数据容器类
            PropertyInfo propertyInfo = configIns.GetType().GetProperty("Datas"); // 取到容器字段
            object DatasVal = propertyInfo.GetValue(configIns); // 取到容器

            PropertyInfo objPInfo = obj.GetType().GetProperty("Datas"); // 取到容器字段
            IList datas = objPInfo.GetValue(obj) as IList;
            int pos = 0;
            for (int i = 0; i < datas.Count; i++)
            {
                object dataIns = assembly.CreateInstance("clientDataPos"); // 创建数据类
                MethodInfo addMethod =  propertyInfo.PropertyType.GetMethod("Add", new Type[] { dataIns.GetType() });
                addMethod?.Invoke(DatasVal, new[] { dataIns }); // 往容器中添加数据
                
                int length = (datas[i] as IMessage).ToByteArray().Length; // 使用Pb自带的转字节数组
                dataIns.GetType().GetField("index_", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(dataIns, i); // 数据在第几行
                dataIns.GetType().GetField("length_", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(dataIns, length);// 当前行数据长度
                dataIns.GetType().GetField("pos_", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(dataIns, pos);// 当前行数据开始位置
                pos += length;
            }
            // 数据写进文件
            using (FileStream fs = new FileStream(Path.Combine(Common.dataFileDirPath, fileName), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                MessageExtensions.WriteTo(configIns as IMessage, fs);
            }
        }

        // 明文数据
        private static void WriteLawsData(object obj, string sheetName)
        {
            string fileName = sheetName + ".txt";
            using (FileStream fs = new FileStream(Path.Combine(Common.lawsDataFileDirPath, fileName), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string content = obj.ToString();
                byte[] datas = Encoding.Default.GetBytes(content);
                byte[] newDatas = Encoding.Convert(Encoding.Default, Encoding.UTF8, datas); // 不转换一下就出现乱码的情况 
                fs.Write(newDatas, 0, newDatas.Length);
            }
        }
        
    }
}
