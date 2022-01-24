using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单一元素池
public class ObjectDrawer
{
    // 该类元素的父节点
    private GameObject parent;
    // 该类元素池
    private Queue<GameObject> objQueue;
    // 获取当前元素池中元素个数
    public int Count { get { return objQueue.Count; } private set { } }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="root">整个缓存池的根节点，将单一元素池交给缓存池</param>
    /// <param name="name">单一元素池根节点名</param>
    public ObjectDrawer(GameObject root, string name)
    {
        // 设置该类元素的父节点为根节点
        parent = new GameObject();
        parent.name = name;
        parent.transform.SetParent(root.transform);
        // 初始化该类元素池
        objQueue = new Queue<GameObject>(100);
    }

    // 将元素入池
    public void Push(GameObject obj)
    {
        // 设置禁用
        obj.SetActive(false);
        // 交给单一元素池的父节点管理
        obj.transform.SetParent(parent.transform);
        // 该元素入池
        objQueue.Enqueue(obj);
    }
    // 从池中取出元素
    public GameObject Get()
    {
        GameObject obj = objQueue.Dequeue();
        // 从父节点接管
        obj.transform.parent = null;
        // 激活
        obj.SetActive(true);
        return obj;
    }
    
}


/// <summary>
/// 缓存池，避免游戏对象的频繁创建
/// </summary>
public class Pool : BaseSingleton<Pool>
{
    // 不同对象存入不同的List，根据对象名找到list，使用队列，因为不需要随机访问，且插入删除效率高点
    private Dictionary<string, ObjectDrawer> objectDic = new Dictionary<string, ObjectDrawer>();
    // 给所有用缓存池管理的GameObject指定根节点，便于管理
    private GameObject root = null;
    /// <summary>
    /// 存入元素
    /// </summary>
    /// <param name="name">元素类型名，使用缓存池管理的对象，类型名就是游戏对象名</param>
    /// <param name="obj">存入的游戏对象</param>
    public void Push(string name, GameObject obj)
    {
        // 初始化根节点
        if (root == null)
        {
            root = new GameObject();
        }
        // 初始化单一元素池
        if (!objectDic.ContainsKey(name))
        {
            // 给个初始值避免多次GC
            objectDic[name] = new ObjectDrawer(root, name);
        }
        // 入池
        objectDic[name].Push(obj);
    }
    /// <summary>
    /// 取出元素，若当前池中没有对应类型的元素，则创建一个
    /// </summary>
    /// <param name="name">资源Resources下路径名</param>
    /// <returns>取出的元素</returns>
    public GameObject Get(string name)
    {
        GameObject obj = null;
        // 若池存在，且池中还有元素
        if (objectDic.ContainsKey(name) && objectDic[name].Count > 0)
        {
            obj = objectDic[name].Get();
        }
        else
        {
            // 没有则创建，Resource的资源名要和对应名称相同
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
        }
        return obj;
    }
    // 清空缓存池
    public void Clear()
    {
        objectDic.Clear();
    }
}
