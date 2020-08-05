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

}
