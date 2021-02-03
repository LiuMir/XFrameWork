using UnityEngine;
using System.IO;

public class PathUtility:Singleton<PathUtility>
{
    //绝对路径获取Asset路径
    public string GetAssetPath(string path)
    {
        string str = "";
        if (path.Length>0)
        {
            string tempPath = path.Replace('\\', '/');
            str = tempPath.Substring(tempPath.IndexOf("Assets"));
        }
        return str;
    }

    public string GetAbOutPath()
    {
        string path = Application.dataPath + "/BundlePath";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    // 获取tStreamingAsset目录（仅可读）
    public string GetStreamingAssetPath()
    {
        return "";
    }

    // 获取持久化目录（可读可写）
    public string GetPersistentDataPath()
    {
        return "";
    }

    //资源更新版本Url
    public string GetResUpdateVerUrl()
    {
        return "";
    }

    // 资源版本文件路径（存放在persistentDataPath目录）
    public string GetResVerFilePath()
    {
        return "";
    }

    // 获取资源列表文件url
    public string GetResListFileUrl()
    {
        return "";
    }

    // 获取资源列表文件路径（存放在persistentDataPath目录）
    public string GetResListFilePath()
    {
        return "";
    }

}
