using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 给未继承Monobehavior的类提供Update方法和开启协程功能
/// 使用委托接收其他类中的MonoBehavior，然后在自己的update里调用这些委托
/// </summary>
public class MonoController : MonoBehaviour
{
    // 委托
    private event UnityAction updateEvent;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        updateEvent();
    }
    // 订阅
    public void Subscribe(UnityAction action)
    {
        updateEvent += action;
    }
    // 取消订阅
    public void Unsubscribe(UnityAction action)
    {
        updateEvent -= action;
    }
}
