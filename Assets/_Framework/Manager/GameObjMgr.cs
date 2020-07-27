using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjMgr:Singleton<GameObjMgr>
{
    //显示非嵌入式UI
    public T Show<T>(string path, Transform parent = null, Action<GameObject> action = null)where T:MonoBaseClass,new()
    {
        return CreateFactory.Instance.Create<T>(path, parent, action);
    }

    //嵌入式UI ----> 子UI
    public T ShowChild<T>(string path, MonoBaseClass parent = null, Action<T> action = null) where T : MonoBaseClass, new()
    {
        return CreateFactory.Instance.Create(path, parent, action);
    }

    //创建嵌入式UI ----> 父UI
    public T ShowChild<T>(string path, Transform parent = null, Action<T> action = null) where T : MonoBaseClass, new()
    {
        return CreateFactory.Instance.Create(path, parent, action);
    }


    public void Hide(MonoBaseClass mb) 
    {
        ReferenceMgr.Instance.Release(mb);
        mb?.OnClose();
    }
    
    public void Destory(MonoBaseClass mb)
    {
        ReferenceMgr.Instance.Release(mb);
        if (mb?.Go)
        {
            UnityEngine.Object.Destroy(mb.Go);
        }
        mb?.OnDestory();
    }

}