public class AssertUtility:Singleton<AssertUtility>
{
    //判断对象是否为空，若为空并且有错误信息则输出错误信息, 空返回false
    public bool AssertNull(object  obj, string errorMsg = "")
    {
        if (null == obj)
        {
            if (errorMsg.Length > 0)
            {
                DebugUtility.Instance.Error(errorMsg);
            }
            return false;
        }
        return true;
    }

}

