using System.IO;
using System.Security.Cryptography;
using System.Text;

public sealed class ToolUtility
{
    private static readonly StringBuilder stringBuilder = new StringBuilder();

    // 获取文件的Md5字符串 
    public static string GetFileMD5Str(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            int len = (int)fileStream.Length;
            byte[] data = new byte[len];
            fileStream.Read(data, 0, len);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            stringBuilder.Clear();
            foreach (byte b in result)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
    }
}
