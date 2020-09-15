using System.Collections;
using System.Collections.Generic;

public class CoroutineMgr:Singleton<CoroutineMgr>
{
    private readonly Dictionary<string, IEnumerator> ieDic = new Dictionary<string, IEnumerator>();

    // 开启一个协程
    public void StartCoroutine(string name, IEnumerator ie)
    {
        if (!ieDic.ContainsKey(name))
        {
            ieDic.Add(name, ie);
        }
        GameEnter.Instance.StartCoroutine(ie);
    }

    // 关闭一个协程
    public void StopCoroutine(string name)
    {
        if (ieDic.ContainsKey(name))
        {
            IEnumerator ie = ieDic[name];
            GameEnter.Instance.StopCoroutine(ie);
            ieDic.Remove(name);
        }
    }

    //清除所有的协程
    public void ClearAllCoroutine()
    {
        foreach (var item in ieDic)
        {
            IEnumerator ie = item.Value;
            GameEnter.Instance.StopCoroutine(ie);
        }
        ieDic.Clear();
    }
}
