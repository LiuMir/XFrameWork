using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DownLoadInfo:IDisposable
{
    public string Url;
    public string SavePath;
    public Action FinishAction;

    public void Dispose()
    {
        Url = "";
        SavePath = "";
        FinishAction = null;
    }
}

public class DownLoadFileUtility:Singleton<DownLoadFileUtility>
{
    private const int MaxDownLoadNum = 10; // 最大同时下载数量
    private readonly Dictionary<string, UnityWebRequest> m_workingDownLoadQue = new Dictionary<string, UnityWebRequest>(); // 工作中队列
    private readonly Queue<UnityWebRequest> m_stopDownLoadQue = new Queue<UnityWebRequest>(); // 空闲下载队列
    private readonly Queue<DownLoadInfo> m_needDownLoadRes = new Queue<DownLoadInfo>(); // 需要下载的资源

    // 下载文件
    public async void DownLoadResFile(DownLoadInfo downLoadInfo)
    {
        if (null != downLoadInfo)
        {
            if (m_workingDownLoadQue.Count < MaxDownLoadNum)
            {
                UnityWebRequest webRequest = m_stopDownLoadQue.Dequeue();
                if (null == webRequest)
                {
                    webRequest = new UnityWebRequest(downLoadInfo.Url)
                    {
                        timeout = 5 // 网络超时设置
                    };
                }
                else
                {
                    webRequest.url = downLoadInfo.Url;
                }
                using (webRequest.downloadHandler = new DownloadHandlerFile(downLoadInfo.SavePath) {
                    removeFileOnAbort = true
                }) // 想缓存DownloadHandlerFile，但是缓存后没找到修改保存路径的方法
                {
                    await webRequest.SendWebRequest(); // 等待文件下载完成
                    if (webRequest.isDone && webRequest.isNetworkError)
                    {
                        DebugUtility.Instance.Error($"请求下载  {downLoadInfo.Url}  失败，失败原因为:{webRequest.error}");
                    }
                    m_workingDownLoadQue.Remove(downLoadInfo.Url);
                    m_stopDownLoadQue.Enqueue(webRequest); // 闲置下来的请求放入容器中重复使用
                    downLoadInfo.FinishAction?.Invoke();
                    if (m_needDownLoadRes.Count > 0) // 每次下载完检测一下是否有需要下载
                    {
                        using (DownLoadInfo Info =  m_needDownLoadRes.Dequeue())
                        {
                            DownLoadResFile(Info);
                        }
                    }
                }
            }
            else
            {
                m_needDownLoadRes.Enqueue(downLoadInfo);
            }
        }
    }

    // 释放下载相关缓存
    public void Clear()
    {
        m_workingDownLoadQue.Clear(); // 理论上不应该还有下载中的队列
        m_stopDownLoadQue.Clear();
        m_needDownLoadRes.Clear();
    }
    
    
}