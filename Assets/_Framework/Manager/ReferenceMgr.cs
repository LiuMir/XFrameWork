using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ReferenceMgr:Singleton<ReferenceMgr>{
    private readonly ConcurrentDictionary<Type, Stack<BaseReference>> referencePool = new ConcurrentDictionary<Type, Stack<BaseReference>>();
  
    // 获取一个引用类
    public T Acquire<T>() where T :BaseReference, new()
    {
        if (referencePool.TryGetValue(typeof(T), out Stack<BaseReference> iReferences) && iReferences.Count > 0 )
        {
            return iReferences.Pop() as T;
        }
        else
        {
            if (!referencePool.ContainsKey(typeof(T)))
            {
                iReferences = new Stack<BaseReference>();
                referencePool.TryAdd(typeof(T), iReferences);
            }
            return new T();
        }
    }
    
    // 回收一个引用类
    public void Release(BaseReference type)
    {
        if (referencePool.TryGetValue(type.GetType(), out Stack<BaseReference> iReferences))
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
    public void ClearOneType(BaseReference type)
    {
        Type t = type.GetType();
        if (referencePool.TryRemove(t, out Stack<BaseReference> iReferences))
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
