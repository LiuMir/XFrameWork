using System;
using System.IO;

namespace ExcelToProtobuf
{
    // .cs文件转dll 
    public class Compiler2Dll
    {
        // 目标文件夹、输出文件夹
        public static bool Compiler(string targetPath, string outPath, string pbDllPath)
        {
            string dllFilePath = Path.Combine(outPath, Common.dllName);
            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine($"{targetPath}：需要编译的C#文件夹不存在！！！");
                return false;
            }
            if (!File.Exists(pbDllPath))
            {
                Console.WriteLine($"{pbDllPath}：pbDllPath文件不存在！！！");
                return false;
            }
            if (File.Exists(dllFilePath))
            {
                File.Delete(dllFilePath);
            }
            var commond = @"/t:library /out:{0} /r:{1}  {2}\*.cs";
            // csc.exe一定要选则VS（Roslyn）安装目录下的，千万不要选择框架下
            // 遇到csc不是有效命令，在环境变量中添加一下就好了
            commond = "csc " + string.Format(commond, dllFilePath, pbDllPath, targetPath);
            Cmd(commond);
            return true;
        }

        // 调用CMD编译cs文件 --> dll
        private static string Cmd(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            process.StandardInput.WriteLine(cmd);
            process.StandardInput.AutoFlush = true;
            process.StandardInput.WriteLine("exit");

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            process.Close();
            return output;
        }
    }
}
