using System;
using System.Collections.Generic;

/// <summary>
/// 消息管理中心，目前最大支持传四个参数。若需要扩展，可以自己添加
/// </summary>
public class NotificationMgr:Singleton<NotificationMgr>
{
    private readonly Dictionary<EventCode, Delegate> eventList = new Dictionary<EventCode, Delegate>();

    //注册消息前检测一下
    private void RegistCheck(EventCode eventCode, Delegate callBack)
    {
        if (!eventList.ContainsKey(eventCode))
        {
            eventList.Add(eventCode, null);
        }
        Delegate d = eventList[eventCode];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception($"当前事件与委托类型不匹配，当前事件所对应的委托是{d.GetType()}，要添加的委托类型为{callBack.GetType()}");
        }
    }

    //移除消息前检测一下
    private void RemoveCheck(EventCode eventCode, Delegate callBack)
    {
        if (eventList.ContainsKey(eventCode))
        {
            Delegate d = eventList[eventCode];
            if (d == null)
            {
                throw new Exception($"移除监听错误：事件{eventCode}没有对应的委托");
            }else if (d.GetType() != callBack.GetType())
            {
                throw new Exception($"移除监听错误：尝试为事件{eventCode}移除不同类型的委托，当前委托类型为{d.GetType()}，要移除的委托类型为{callBack.GetType()}");
            }
        }
    }

    //如果消息ID没有对应的委托，就直接移除消息ID
    private void RemoveMsgKey(EventCode eventCode)
    {
        if (null == eventList[eventCode])
        {
            eventList.Remove(eventCode);
        }
    }

    //注册无参数
    public void RegistMsg(EventCode eventCode, Action action)
    {
        RegistCheck(eventCode, action);
        eventList[eventCode] = (Action)eventList[eventCode] + action;
    }

    //注册一个参数
    public void RegistMsg<T>(EventCode eventCode, Action<T> action)
    {
        RegistCheck(eventCode, action);
        eventList[eventCode] = (Action<T>)eventList[eventCode] + action;
    }

    //注册两个参数
    public void RegistMsg<T, X>(EventCode eventCode, Action<T, X> action)
    {
        RegistCheck(eventCode, action);
        eventList[eventCode] = (Action<T, X>)eventList[eventCode] + action;
    }

    //注册三个参数
    public void RegistMsg<T, X, Y>(EventCode eventCode, Action<T, X, Y> action)
    {
        RegistCheck(eventCode, action);
        eventList[eventCode] = (Action<T, X, Y>)eventList[eventCode] + action;
    }

    //注册四个参数
    public void RegistMsg<T, X, Y, Z>(EventCode eventCode, Action<T, X, Y, Z> action)
    {
        RegistCheck(eventCode, action);
        eventList[eventCode] = (Action<T, X, Y, Z>)eventList[eventCode] + action;
    }

    //移除无参数
    public void RemoveMsg(EventCode eventCode, Action action)
    {
        RemoveCheck(eventCode, action);
        eventList[eventCode] = (Action)eventList[eventCode] - action;
        RemoveMsgKey(eventCode);
    }

    //移除一个参数
    public void RemoveMsg<T>(EventCode eventCode, Action<T> action)
    {
        RemoveCheck(eventCode, action);
        eventList[eventCode] = (Action<T>)eventList[eventCode] - action;
        RemoveMsgKey(eventCode);
    }

    //移除两个参数
    public void RemoveMsg<T, X>(EventCode eventCode, Action<T, X> action)
    {
        RemoveCheck(eventCode, action);
        eventList[eventCode] = (Action<T, X>)eventList[eventCode] - action;
        RemoveMsgKey(eventCode);
    }

    //移除三个参数
    public void RemoveMsg<T, X, Y>(EventCode eventCode, Action<T, X, Y> action)
    {
        RemoveCheck(eventCode, action);
        eventList[eventCode] = (Action<T, X, Y>)eventList[eventCode] - action;
        RemoveMsgKey(eventCode);
    }

    //移除四个参数
    public void RemoveMsg<T, X, Y, Z>(EventCode eventCode, Action<T, X, Y, Z> action)
    {
        RemoveCheck(eventCode, action);
        eventList[eventCode] = (Action<T, X, Y, Z>)eventList[eventCode] - action;
        RemoveMsgKey(eventCode);
    }

    //无参数 发送消息
    public void SendMsg(EventCode eventCode)
    {
        if (eventList.TryGetValue(eventCode, out Delegate d))
        {
            (d as Action)?.Invoke();
        }
    }

    //一个参数 发送消息
    public void SendMsg<T>(EventCode eventCode, T arg)
    {
        if (eventList.TryGetValue(eventCode, out Delegate d))
        {
            (d as Action<T>)?.Invoke(arg);
        }
    }

    //两个参数 发送消息
    public void SendMsg<T, X>(EventCode eventCode, T arg0, X arg1)
    {
        if (eventList.TryGetValue(eventCode, out Delegate d))
        {
            (d as Action<T, X>)?.Invoke(arg0, arg1);
        }
    }

    //三个参数 发送消息
    public void SendMsg<T, X, Y>(EventCode eventCode, T arg0, X arg1, Y arg2)
    {
        if (eventList.TryGetValue(eventCode, out Delegate d))
        {
            (d as Action<T, X, Y>)?.Invoke(arg0, arg1, arg2);
        }
    }

    //四个参数 发送消息
    public void SendMsg<T, X, Y,  Z>(EventCode eventCode, T arg0, X arg1, Y arg2, Z arg3)
    {
        if (eventList.TryGetValue(eventCode, out Delegate d))
        {
            (d as Action<T, X, Y, Z>)?.Invoke(arg0, arg1, arg2, arg3);
        }
    }

}

