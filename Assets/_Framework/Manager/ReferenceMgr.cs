using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ReferenceMgr:Singleton<ReferenceMgr>{
    private readonly ConcurrentDictionary<Type, Stack<IBaseReference>> referencePool = new ConcurrentDictionary<Type, Stack<IBaseReference>>();
  
    // 获取一个引用类
    public T Acquire<T>() where T :IBaseReference, new()
    {
        if (referencePool.TryGetValue(typeof(T), out Stack<IBaseReference> iReferences) && iReferences.Count > 0 )
        {
            return (T)iReferences.Pop();
        }
        else
        {
            if (!referencePool.ContainsKey(typeof(T)))
            {
                iReferences = new Stack<IBaseReference>();
                referencePool.TryAdd(typeof(T), iReferences);
            }
            return new T();
        }
    }
    
    // 回收一个引用类
    public void Release(IBaseReference type)
    {
        if (referencePool.TryGetValue(type.GetType(), out Stack<IBaseReference> iReferences))
        {
            type.Clear();
            int count = 3; // 默认存三个
            CountLimitAttribute attr = type.GetType().GetCustomAttribute<CountLimitAttribute>();
            if (null != attr)
            {
                count = attr.Count;
            }
            if (iReferences.Count < count) //超过指定个数就不要了
            {
                iReferences.Push(type);
            }
            else
            {
                type = null;
            }
        }
        else
        {
            Debug.LogError(type.GetType().Name + "不是通过类引用创建，请检查！！！");
        }
    }

    //清除指定缓存对象
    public void ClearOneType(IBaseReference type)
    {
        Type t = type.GetType();
        if (referencePool.TryRemove(t, out Stack<IBaseReference> iReferences))
        {
            iReferences.Clear();
        }
    }

    //清除缓存对象
    public void ClearAll()
    {
        referencePool.Clear();
    }
}
