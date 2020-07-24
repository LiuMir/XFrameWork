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

    public T Create<T>(string path, Transform parent = null, Action<GameObject> action = null) where T : MonoBaseClass, new()
    {
        T t = ReferenceMgr.Instance.Acquire<T>();
        if (null == t.Go)
        { 
            t.Go = ResourcesMgr.Instance.LoadGameObj(path, parent, action);
            t?.Awake();
        }
        return t;
    }
}

