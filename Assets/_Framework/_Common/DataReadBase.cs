using Google.Protobuf;
using System;
using System.IO;
using UnityEngine;

//  TDataArray 对应XXX_config  TReader对应继承DataReadBase的子类 
// 例如 public class DataTest : DataReadBase<st_main_menu_config, DataTest>{}
public class DataReadBase<TDataArray, TReader> where TDataArray : IMessage, new()  where TReader : new()
{
    #region 单例实现
    private static readonly Lazy<TReader> lazy = new Lazy<TReader>(() => new TReader());
    protected DataReadBase() { }
    public static TReader Instance
    {
        get { return lazy.Value; }
    }
    #endregion

    private TDataArray datas; //表数据
    //private bool isLoading = false; // 是否加载中

    //获取表数据
    public TDataArray GetAllData()
    {
        if (null == datas)
        {
            datas = LoadDatas();
        }
        return datas;
    }

    // 获取表文件路径(子类重写)
    public virtual string GetFilPath() { return ""; }

    // 解析表数据
    // TODO表文件需要加载，后续接入加载机制并且加入表文件加载中就不再加载（异步加载）
    private TDataArray LoadDatas()
    {
        try
        {
            using (FileStream fs = new FileStream(GetFilPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                TDataArray msg = new TDataArray();
                msg = (TDataArray)msg.Descriptor.Parser.ParseFrom(bytes);
                return msg;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            return default;
        }
    }
}

