using UnityEngine;

public class DebugUtility:Singleton<DebugUtility>
{
    //输出错误信息
    public void Error(string msg)
    {
        Debug.LogError($"{msg}");
    }

}
