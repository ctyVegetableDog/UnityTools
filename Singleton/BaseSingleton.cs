using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 没有monobehavior的单例
/// </summary>
public class BaseSingleton<T> where T:new()
{
    // 构造函数私有
    protected BaseSingleton() { }
    // 唯一实例
    private static T instance;
    // 返回唯一实例
    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = new T();
        }
        return instance;
    }
}
