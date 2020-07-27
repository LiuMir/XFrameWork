using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjMgr:Singleton<GameObjMgr>
{
    //显示非嵌入式GameObject
    public T Show<T>(string path, Transform parent = null, Action<GameObject> action = null)where T:MonoBaseClass,new()
    {
        return CreateFactory.Instance.Create<T>(path, parent, action);
    }

    //显示嵌入式GameObject ----> 子GameObject
    public T ShowChild<T>(string path, MonoBaseClass parent = null, Action<T> action = null) where T : MonoBaseClass, new()
    {
        return CreateFactory.Instance.Create(path, parent, action);
    }

    //显示嵌入式GameObject ----> 父GameObject
    public T ShowParent<T>(string path, Transform parent = null, Action<T> action = null) where T : MonoBaseClass, new()
    {
        return CreateFactory.Instance.Create(path, parent, action);
    }

    //隐藏GameObject
    public void Hide(MonoBaseClass mb) 
    {
        ReferenceMgr.Instance.Release(mb);
        mb?.OnClose();
    }

    //销毁GameObject
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