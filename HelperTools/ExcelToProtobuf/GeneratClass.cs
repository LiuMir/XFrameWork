using System;
using System.Diagnostics;
using System.IO;

namespace ExcelToProtobuf
{
    // .proto文件转class
    public class GeneratClass
    {
        public static bool CallProtoc(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"{path}：没有BuildProtos.bat文件！！！");
                return false;
            }
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            try
            {
                Process proc = new Process();
                proc.StartInfo.WorkingDirectory = dir;
                proc.StartInfo.FileName = fileName;
                proc.Start();
                proc.WaitForExit();
                return true;

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"protoc.exe调用异常，异常信息为：{ex.ToString()}");
                return false;
            }
        }
    }
}
