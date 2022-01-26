using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 使用单例模式提供访问MonoController的方法
/// 给未继承Monobehavior的类提供Update方法和开启协程功能
/// </summary>
public class MonoManager : BaseSingleton<MonoManager>
{
    private MonoController monoController;
    public MonoManager()
    {
        GameObject obj = new GameObject();
        monoController = obj.AddComponent<MonoController>();
    }
    // 订阅
    public void Subscribe(UnityAction action)
    {
        monoController.Subscribe(action);
    }
    // 取消订阅
    public void Unsubscribe(UnityAction action)
    {
        monoController.Unsubscribe(action);
    }
    // 用函数名带参数开启协程
    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return monoController.StartCoroutine(methodName, value);
    }
    // 用迭代器开始协程
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return monoController.StartCoroutine(routine);
    }
    // 用函数名开始协程
    public Coroutine StartCoroutine(string methodName)
    {
        return monoController.StartCoroutine(methodName);
    }

}
