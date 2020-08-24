using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

//AB信息
public class ABInfo:IBaseReference
{
    public int RefCount;
    public string fullPath;
    public AssetBundle Bundle;
    public List<string> DependenceList;

    public ABInfo()
    {
        RefCount = 0;
        fullPath = "";
        Bundle = null;
        DependenceList = new List<string>();
    }

    public void Clear()
    {
        DependenceList.Clear();
        RefCount = 0;
        Bundle.Unload(true);
        Bundle = null;
    }
}

public class ABMgr:Singleton<ABMgr>
{
    private readonly ConcurrentDictionary<string, ABInfo> loadedABs = new ConcurrentDictionary<string, ABInfo>();
    private AssetBundleManifest assetBundleManifest;

    //减少ab引用,当ab引用为0时会调用unload(true)
    public void SubABRefCount(string abName)
    {
        ABInfo abInfo = ReferenceMgr.Instance.Acquire<ABInfo>();
        if (loadedABs.TryGetValue(abName, out abInfo))
        {
            abInfo.RefCount--;
            if (abInfo.RefCount <= 0 && AssertUtility.Instance.AssertNull(abInfo.Bundle))
            {
                ReferenceMgr.Instance.Release(abInfo);
            }
            for (int i = 0; i < abInfo.DependenceList.Count; i++)
            {
                SubABRefCount(abInfo.DependenceList[i]);
            }
        }
    }

    //增加ab引用
    public void AddABRefCount(string abName)
    {
        ABInfo abInfo = ReferenceMgr.Instance.Acquire<ABInfo>();
        if (loadedABs.TryGetValue(abName, out abInfo))
        {
            abInfo.RefCount++;
        }
    }

    //异步加载AB
    public IEnumerator LoadABAsync(string path)
    {
        AssetCoroutine assetCoroutine = ReferenceMgr.Instance.Acquire<AssetCoroutine>();
        assetCoroutine.SetLoadPath(path);
        yield return assetCoroutine; 
        // TODO加载完成后回调可能需要加下
    }

    // 同步加载AB
    public void LoadABsync(string path)
    {
        string[] paths = GetAllDependencies(path);
        if (!ExistsAB(path))
        {
            ABInfo abInfo = ReferenceMgr.Instance.Acquire<ABInfo>();
            abInfo.fullPath = path;
            abInfo.DependenceList = paths.ToList();
            abInfo.RefCount++;
            abInfo.Bundle = AssetBundle.LoadFromFile(path);
            TryAddBundle(abInfo);
        }
        for (int i = 0; i < paths.Length; i++)
        {
            LoadABsync(paths[i]);
        }
    }

    //加载AssetBundleManifest
    public void LoadABManifest(string path)
    {
        AssetBundle bundle =  AssetBundle.LoadFromFile(path);
        assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        AssertUtility.Instance.AssertNull(assetBundleManifest, $"AssetBundleManifest加载失败，路径为：{path}");
    }

    //是否存在对应AssetBundle
    public bool ExistsAB(string path)
    {
        if (loadedABs.TryGetValue(path, out ABInfo abinfo) && AssertUtility.Instance.AssertNull(abinfo.Bundle))
        {
            return true;
        }
        return false;
    }

    // 获取AB包所有的依赖
    public string[] GetAllDependencies(string path)
    {
        if (!AssertUtility.Instance.AssertNull(assetBundleManifest))
        {
            LoadABManifest(""); // TODO assetBundleManifest 路径
        }
        return assetBundleManifest.GetAllDependencies(path);
    }

    // 缓存已加载assetBundle
    public void TryAddBundle(ABInfo abInfo)
    {
        if (!loadedABs.ContainsKey(abInfo.fullPath))
        {
            loadedABs.TryAdd(abInfo.fullPath, abInfo);
        }
    }

    // 尝试获取assetBundle
    public AssetBundle TryGetBundle(string path)
    {
        AssetBundle assetBundle = null;
        if (ExistsAB(path))
        {
            ABInfo abInfo = ReferenceMgr.Instance.Acquire<ABInfo>();
            if (loadedABs.TryGetValue(path, out abInfo) && AssertUtility.Instance.AssertNull(abInfo.Bundle, $"路径为{path}的bundle加载逻辑出现错误！！！"))
            {
                assetBundle = abInfo.Bundle;
            }
        }
        else
        {
            Debug.LogError($"路径为{path}的bundle还没有加载，请先调用加载的API！！！");
        }
        return assetBundle;
    }
}
