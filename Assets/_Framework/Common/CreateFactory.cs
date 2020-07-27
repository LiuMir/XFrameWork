using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CreateFactory:Singleton<CreateFactory>
{
    //创建引用对象
    public T Create<T>()where T : BaseClass, new()
    {
        return ReferenceMgr.Instance.Acquire<T>();
    }

    //创建非嵌入式GameObject
    public T Create<T>(string path, Transform parent = null, Action<GameObject> action = null) where T : MonoBaseClass, new()
    {
        T t = ReferenceMgr.Instance.Acquire<T>();
        if (null == t.Go)
        { 
            ResourcesMgr.Instance.LoadGameObj(path, parent, (go) => {
                t.Go = go;
                t?.Awake();
                t?.OnOpen();
            });
        }
        else
        {
            t?.OnOpen();
            action?.Invoke(t.Go);
        }
        return t;
    }

    //创建嵌入式GameObject ----> 子GameObject
    public T Create<T>(string path, MonoBaseClass parent = null, Action<T> action = null) where T : MonoBaseClass, new()
    {
        T t = ReferenceMgr.Instance.Acquire<T>();
        if (null == t.Go)
        {
            parent?.Childs.Push(t);
             ResourcesMgr.Instance.LoadGameObj(path, parent?.Go.transform, (go)=> {
                t.Go = go;
                t?.Awake();
                t?.OnOpen();
                action?.Invoke(t);
            });
        }
        else
        {
            t?.OnOpen();
            parent?.Childs.Push(t);
            action?.Invoke(t);
        }
        return t;
    }

    //创建嵌入式GameObject ----> 父GameObject
    public T Create<T>(string path, Transform parent = null, Action<T> action = null) where T : MonoBaseClass, new()
    {
        T t = ReferenceMgr.Instance.Acquire<T>();
        if (null == t.Go)
        {
           ResourcesMgr.Instance.LoadGameObj(path, parent, (go) => {
                t.Go = go;
                t?.Awake();
                t?.OnOpen();
                action?.Invoke(t);
            });
        }
        else
        {
            t?.OnOpen();
            action?.Invoke(t);
        }
        return t;
    }

}

