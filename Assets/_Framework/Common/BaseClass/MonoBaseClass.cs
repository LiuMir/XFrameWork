using System.Collections;
using System.Reflection;
using UnityEngine;

public abstract class MonoBaseClass : BaseReference
{
    public GameObject Go;
    private float destoryTime = 3f;
    private int timerID = 0;
    public virtual void Awake()
    {
        DestoryTimeAttribute attr = this.GetType().GetCustomAttribute<DestoryTimeAttribute>();
        if (null != attr)
        {
            destoryTime = attr.Time;
        }
    }
    public virtual void OnOpen()
    {
        Go?.SetActive(true);
        if (Go && timerID != 0)
        {
            TimerMgr.Instance.RemoveTimer(timerID);
        }
    }
    public virtual void OnClose()
    {
        Go?.SetActive(false);
        if (Go)
        {
            timerID = TimerMgr.Instance.AddTimer(destoryTime, 1, () =>
            {
                Object.DestroyImmediate(Go);
                OnDestory();
            });
        }
    }
    public abstract void OnDestory();
    public override void Clear() { }
    
}