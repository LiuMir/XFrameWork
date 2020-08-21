using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetCoroutine : CustomYieldInstruction
{
    public override bool keepWaiting => !CheckisDone();
    public bool IsDone { get; private set; }
    public float Progress => GetProgress();
    
    public Action<AssetCoroutine> completed;

    private List<ABInfo> abInfoList;
    private List<string> requestPaths;
    private List<AssetBundleCreateRequest> requests;

    public AssetCoroutine(string path)
    {
        requests = new List<AssetBundleCreateRequest>();
        requestPaths = new List<string>();
        abInfoList = new List<ABInfo>();
        LoadAssetBundleAsync(path);
    }

    // 异步加载
    private void LoadAssetBundleAsync(string path)
    {
        IsDone = false;
        CollectRequestPaths(path);
        for (int i = 0; i < requestPaths.Count; i++)
        {
            requests.Add(AssetBundle.LoadFromFileAsync(requestPaths[i]));
        }
    }

    //检测是否完成
    private bool CheckisDone()
    {
        if (IsDone)
        {
            return IsDone;
        }
        for (int i = 0; i < requests.Count; i++)
        {
            if (!requests[i].isDone)
            {
                return IsDone;
            }
        }
        IsDone = true;

        for (int i = 0; i < requests.Count; i++)
        {
            abInfoList[i].RefCount++;
            abInfoList[i].Bundle = requests[i].assetBundle;
            ABMgr.Instance.TryAddBundle(abInfoList[i]);
        }
        completed?.Invoke(this);
        return IsDone;
    }
    
    //收集需要加载的路径
    private void CollectRequestPaths(string path)
    {
        string[] paths = ABMgr.Instance.GetAllDependencies(path);
        if ( (!ABMgr.Instance.ExistsAB(path)) && (!requestPaths.Contains(path))) // 避免abInfo加入重复ABInfo
        {
            ABInfo info = ReferenceMgr.Instance.Acquire<ABInfo>();
            info.fullPath = path;
            info.DependenceList = paths.ToList();
            requestPaths.Add(path); //  requestPaths 与 abInfoList 需要一一对应
            abInfoList.Add(info);
        }
        for (int i = 0; i < paths.Length; i++)
        {
            if (!ABMgr.Instance.ExistsAB(paths[i]))
            {
                CollectRequestPaths(paths[i]);// 取到依赖的依赖……
            }
        }
    }

    //获取当前的加载进度
    private float GetProgress()
    {
        if (IsDone)
        {
            return 1.0f;
        }

        int total = requests.Count;
        float curProgress = 0.0f;
        for (int i = 0; i < total; i++)
        {
            curProgress += requests[i].progress;
        }
        return curProgress / total;
    }

}
