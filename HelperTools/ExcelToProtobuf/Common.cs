﻿using System.Linq;

namespace ExcelToProtobuf
{
    public class Common
    {
        public static string dllName = "clientData.dll"; 
        public static string dataFileDirPath = "E:/UnityDemo/FrameWork/HelperTools/ExcelToProtobuf/Data"; // 序列化二进制数据文件夹
        public static string posDataFileDirPath = "E:/UnityDemo/FrameWork/HelperTools/ExcelToProtobuf/Data"; // 位置文件数据文件夹
        public static string lawsDataFileDirPath = "E:/UnityDemo/FrameWork/HelperTools/ExcelToProtobuf/LawsData"; // 明文数据文件夹

        // 首字母小写为了获取字段名字
        public static string GetPbFieldName(string str)
        {
            return str.First().ToString().ToLower() + str.Substring(1)+"_";  // 加个下划线是因为pb生成的字段就是首字母小写并加上下划线
        }
    }
}
