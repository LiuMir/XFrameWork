// 思路： 打APK或者IPA是编辑器存入版本1.0资源列表文件（MD5形式） --> 文件路径、md5值、文件size、状态（增加、修改、删除）
// 增量更新（版本回退） 每次打包基于1.0上生成一个 1.1、1.2……的patch资源列表文件 
// 检测是否下载资源列表文件 如果需要下载 则下载版本最大的资源列表文件与本地资源列表文件对比  
// 如果对比下来需要对本地ab文件（增加、修改、删除）操作 则更新本地ab文件 （ab只删除persistentDataPath目录下 不删除streamingAssetsPath下） 

// 另一种方式 上述 制作的每个1.1、1.2……patch资源文件都压缩成一个压缩包（版本不同或者同版本未下载完 只需要对比一个文件）
// 需要进行解压以及趟解压过程中坑

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Linq;
using System.Text;
using System;
using Google.Protobuf;

enum ResFileStatus:byte
{
    ADD,
    MODIFY,
    DELETE,
}

public class UpdateResUtility:Singleton<UpdateResUtility>
{
    private const string m_ResListFileName = "ResListFile.txt";
    private string onlineVerStr = "";
    private bool isContinueUpdate = false; // 上次没更新完（相同版本，如果版本不同不管上次是否更新完毕）
    private readonly StringBuilder stringBuilder = new StringBuilder(30, 500); //限制拼接容器大小

    // 检测是否需要更新（1、增量更新  2、版本回退）
    public async Task<bool> CheckUpdate()
    {
        bool isNeedUpdate = false;
        using (UnityWebRequest webRequest = new UnityWebRequest(PathUtility.Instance.GetResUpdateVerUrl()))
        {
            await webRequest.SendWebRequest();
            if (webRequest.isDone && !webRequest.isNetworkError)
            {
                onlineVerStr = webRequest.downloadHandler.text;
                WriteVerInfo2File(onlineVerStr, ref isNeedUpdate);
            }
            else
            {
                DebugUtility.Instance.Error("请求资源版本信息失败，失败原因为: " + webRequest.error);
            }
        }
        return isNeedUpdate;
    }

    // 版本信息写入文件
    private void WriteVerInfo2File(string onlineVerStr, ref bool isNeedUpdate)
    {
        string path = PathUtility.Instance.GetResVerFilePath();
        if (File.Exists(path))
        {
            string[] verDataStr = File.ReadAllText(path).Split('|'); // 长度应该为2（1、资源版本号 2、资源是否下载完成 -- 0 未完成 1 完成）
            isContinueUpdate = onlineVerStr.Equals(verDataStr[0]) && verDataStr[1] == "0";
            if (isContinueUpdate | !onlineVerStr.Equals(verDataStr[0]))
            { // 同版本未下载完 | 不同版本
                File.WriteAllText(path, onlineVerStr+"|0");
                isNeedUpdate = true;
            }
        }
        else
        {
            File.Create(path);
            if (onlineVerStr.Length > 0)
            {
                File.WriteAllText(path, onlineVerStr+"|0");
                isNeedUpdate = true;
            }
        }
    }

    // 下载资源列表文件
    public async void DownloadResListFile()
    {
        using (UnityWebRequest webRequest = new UnityWebRequest($"{PathUtility.Instance.GetResListFileUrl()}/{m_ResListFileName}"))
        {
            await webRequest.SendWebRequest();
            if (webRequest.isDone && !webRequest.isNetworkError)
            {
                ResListInfo onlineResListInfo = ResListInfo.Descriptor.Parser.ParseFrom(webRequest.downloadHandler.data) as ResListInfo;
                List<SingleResInfo> updateList = new List<SingleResInfo>(onlineResListInfo.Info.Count); // 预先设置容量
                FileStream fs = new FileStream(PathUtility.Instance.GetResListFilePath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (File.Exists(PathUtility.Instance.GetResListFilePath())) // 之前更新过小版本
                {
                    int count = (int)fs.Length;
                    byte[] fileData = new byte[count];
                    fs.Read(fileData, 0, count);
                    ResListInfo curResListInfo = ResListInfo.Descriptor.Parser.ParseFrom(fileData) as ResListInfo;
                    GetUpdateResPaths(onlineResListInfo, curResListInfo, ref updateList);
                }
                else // 第一次更新小版本
                {
                    GetUpdateResPaths(onlineResListInfo, null, ref updateList); 
                }
                ModifyLocalResFile(updateList, ()=>{
                    byte[] onlineData = onlineResListInfo.ToByteArray();
                    fs.Write(onlineData, 0, onlineData.Length);
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Close();
                    fs.Dispose();
                });
            }
            else
            {
                DebugUtility.Instance.Error("请求资源列表文件失败，失败原因为: " + webRequest.error);
            }
        }
    }

    // 获取需要更新的资源路径
    private void GetUpdateResPaths(ResListInfo onlineResListInfo, ResListInfo curResListInfo, ref List<SingleResInfo> updateList)
    {
        if (curResListInfo !=null)
        {
            if (!onlineResListInfo.Ver.Equals(curResListInfo.Ver))
            {
                // TODO  需要测试一下Linq性能
                updateList = onlineResListInfo.Info.Where(onlineData=>!curResListInfo.Info.Where(curData=> onlineData.Md5 == curData.Md5 
                    && onlineData.Path == curData.Path && onlineData.Status == curData.Status && onlineData.Size == curData.Size).Any()).ToList();
            }
            else
            {
                foreach (var item in onlineResListInfo.Info) // TODO 检测文件是否需要更新的计算方式 感觉不是很好（后续有更好的方案再改）
                {
                    stringBuilder.Clear();
                    stringBuilder.Append(PathUtility.Instance.GetPersistentDataPath());
                    stringBuilder.Append("/");
                    stringBuilder.Append(item.Path);
                    string localFileMd5 = ToolUtility.GetFileMD5Str(stringBuilder.ToString()); 
                    if (localFileMd5 != item.Md5)
                    {
                        updateList.Add(item);
                    }
                }
            }
        }
        else
        {
            updateList.AddRange(onlineResListInfo.Info);
        }
    }

    // 修改本地资源文件
    private async void ModifyLocalResFile(List<SingleResInfo> updateList, Action finishAction = null)
    {
        int count = updateList.Count;
        int finishCount = 0;
        for (int i = 0; i < count; i++)
        {
            stringBuilder.Clear();
            if (updateList[i].Status == (byte)ResFileStatus.DELETE) // 删除的话只删除persistentDataPath目录下
            {
                stringBuilder.Append(PathUtility.Instance.GetPersistentDataPath());
                stringBuilder.Append(updateList[i].Path);
                if (File.Exists(stringBuilder.ToString()))
                {
                    File.Delete(stringBuilder.ToString());
                }
            }
            else // 新增、修改都是需要从服务器中下载文件
            {
                stringBuilder.Append(PathUtility.Instance.GetResListFileUrl());
                stringBuilder.Append(updateList[i].Path);
            }
            using (DownLoadInfo downLoadInfo = new DownLoadInfo {
                Url = stringBuilder.ToString(),
                SavePath = stringBuilder.ToString(), // TODO 下载后 保存文件的路径
                FinishAction = () => {
                    finishCount++;
                },
            })
            {
                DownLoadFileUtility.Instance.DownLoadResFile(downLoadInfo);
            }
        }
        while (finishCount != count)
        {
            await Task.Yield();
        }
        finishAction?.Invoke();

        string path = PathUtility.Instance.GetResVerFilePath(); // 资源文件全部更新完毕 更新一下标志位
        File.WriteAllText(path, onlineVerStr + "|1");

        DownLoadFileUtility.Instance.Clear(); // 资源文件全部下载完毕 清除一下相关的缓存
    }

    private void Clear()
    {
        onlineVerStr = "";
        stringBuilder.Clear();
    }
}