using System;
using UnityEngine;

public class GameEnter:MonoSingle<GameEnter>
{
    public event Action UpdateEvent;
    public event Action LateUpdateEvent;
    public event Action FixedUpdateEvent;
    public event Action OnApplicationQuitEvent;
    public event Action OnApplicationPauseEvent;
    
    private void Update()
    {
        UpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateEvent?.Invoke();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        OnApplicationQuitEvent?.Invoke();
    }

    private void OnApplicationPause(bool pause)
    {
        OnApplicationPauseEvent?.Invoke();
    }



    public GameObject Button;
    public Transform Canvas;

    protected override void Awake()
    {
        base.Awake();
        ButtonTest();
    }

    private void ButtonTest()
    {
        Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
            Debug.LogError("lzh Open");
            GameObjMgr.Instance.Show<NewUI>("NewUI", Canvas);
        });
    }


}
