using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DestoryTime(5)]
public class NewUI : MonoBaseClass
{
    private Button btn;

    public override void Awake()
    {
        base.Awake();
        BtnTest();
        Debug.LogError("lzh Awake New UI");
    }

    public override void OnOpen()
    {
        base.OnOpen();
        Debug.LogError("lzh OnOpen New UI");
    }

    public override void OnClose()
    {
        base.OnClose();
        Debug.LogError("lzh OnClose New UI");
    }

    public override void Clear()
    {
        base.Clear();
        Debug.LogError("lzh Clear New UI");
    }

    public override void OnDestory()
    {
        Debug.LogError("lzh OnDestory New UI");
    }

    private void BtnTest()
    {
        btn = this.Go.transform.Find("Panel/Button").GetComponent<Button>();
        btn.onClick.AddListener(() => {
            GameObjMgr.Instance.Hide(this);
        });
    }
}
