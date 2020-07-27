using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[DestoryTime(5)]
public class DeleteBtnUI : MonoBaseClass
{
    public override void Awake()
    {
        base.Awake();
        Debug.LogError("lzh DeleteBtnUI Awake");
    }

    public override void Clear()
    {
        base.Clear();
        Debug.LogError("lzh DeleteBtnUI Clear");
    }

    public override void OnClose()
    {
        base.OnClose();
        Debug.LogError("lzh DeleteBtnUI OnClose");
    }

    public override void OnDestory()
    {
        Debug.LogError("lzh DeleteBtnUI OnDestory");
    }

    public override void OnOpen()
    {
        base.OnOpen();
        Debug.LogError("lzh DeleteBtnUI OnOpen");
    }
}
