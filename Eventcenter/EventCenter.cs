using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 怪物死亡时想通知玩家获得经验，并通知UI显示加分
可以在玩家和UI中注册怪物死亡事件，怪物死亡时发布其死亡事件
 */

/// <summary>
/// 事件中心，用于管理所有事件，包括事件的订阅和发布
/// </summary>
public class EventCenter : BaseSingleton<EventCenter>
{
    // 事件列表，目前通过string访问事件（得改）
    private Dictionary<string, UnityAction<object>> eventDic = new Dictionary<string, UnityAction<object>>();

    /// <summary>
    /// 发布某个事件，表示该事件被触发了
    /// </summary>
    /// <param name="name">触发的事件名</param>
    /// <param name="pulisher">触发事件的物体</param>
    public void Publish(string name, object pulisher)
    {
        if (eventDic.ContainsKey(name)) eventDic[name](pulisher);
    }

    /// <summary>
    /// 订阅某个事件
    /// </summary>
    /// <param name="name">注册的事件名</param>
    /// <param name="action">当该事件触发时，执行的方法</param>
    public void Subscribe(string name, UnityAction<object> action)
    {
        if (eventDic.ContainsKey(name)) eventDic[name] += action;
        else eventDic.Add(name, action);
    }

    /// <summary>
    /// 取消订阅某个事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void Unsubscribe(string name, UnityAction<object> action)
    {
        if (eventDic.ContainsKey(name)) eventDic[name] -= action;
    }

    /// <summary>
    /// 取消所有订阅的事件
    /// </summary>
    public void UnsubscribeAll()
    {
        eventDic.Clear();
    }
}
