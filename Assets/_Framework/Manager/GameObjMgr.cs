using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjMgr:Singleton<GameObjMgr>
{
    public void Show<T>(string path, Transform parent = null)where T:MonoBaseClass,new()
    {
        T t = CreateFactory.Instance.Create<T>(path, parent);
        t?.OnOpen();
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