using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AB包加载器
/// </summary>

public class ABLoader : Singleton<ABLoader>
{
    // 包的路径
    public string PathUrl { get { return Application.streamingAssetsPath + "/"; } }
    // 主包名
    public string MainABName { get { return "IOS"; } }
    // 主包
    private AssetBundle mainAB = null;
    // 主包中的关键配置
    private AssetBundleManifest manifest = null;
    // 存储已经加载过的AB包避免重复加载
    Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 加载单个包，若该包未被加载，则加载其和其所有依赖包
    /// </summary>
    /// <param name="targetBundleName">需加载的AB包名</param>
    public void LoadSingleAssetBundle(string targetBundleName)
    {
        // 加载主包
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            // 加载主包中的关键配置文件
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        // 查看需要加载包的依赖
        string[] str = manifest.GetAllDependencies(targetBundleName);
        // 加载依赖包
        for (int i = 0; i < str.Length; ++i)
        {
            // 分别加载每个依赖包
            if (!abDic.ContainsKey(str[i]))
            {
                // 加载此包，然后将其加入字典
                AssetBundle assetBundle = AssetBundle.LoadFromFile(PathUrl + str[i]);
                abDic.Add(str[i], assetBundle);
            }
        }
        // 加载目标包
        if (!abDic.ContainsKey(targetBundleName))
        {
            // 加载此包，然后将其加入字典
            AssetBundle assetBundle = AssetBundle.LoadFromFile(PathUrl + targetBundleName);
            abDic.Add(targetBundleName, assetBundle);
        }
    }

    /// <summary>
    /// 同步加载资源，不指定类型
    /// </summary>
    /// <param name="targetBundleName">目标AssetBundle名</param>
    /// <param name="resourceName">包中资源名</param>
    /// <returns>加载出的资源</returns>
    public Object LoadRes(string targetBundleName, string resourceName)
    {
        // 加载目标包
        LoadSingleAssetBundle(targetBundleName);
        // 加载资源
        AssetBundle targetBundle = abDic[targetBundleName];
        Object res = targetBundle.LoadAsset(resourceName);
        return res;
    }

    /// <summary>
    /// 同步加载资源，指定类型
    /// </summary>
    /// <param name="targetBundleName">目标AB包名</param>
    /// <param name="resourceName">目标资源名</param>
    /// <param name="type">资源类型</param>
    /// <returns>加载出的资源</returns>
    public Object LoadRes(string targetBundleName, string resourceName, System.Type type)
    {
        // 加载目标包
        LoadSingleAssetBundle(targetBundleName);
        // 加载资源
        AssetBundle targetBundle = abDic[targetBundleName];
        Object res = targetBundle.LoadAsset(resourceName, type);
        return res;
    }

    /// <summary>
    /// 同步加载资源，泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetBundleName">目标AB包名</param>
    /// <param name="resourceName">目标资源名</param>
    /// <returns>加载出的AB包</returns>
    public T LoadRes<T>(string targetBundleName, string resourceName) where T : Object
    {
        // 加载目标包
        LoadSingleAssetBundle(targetBundleName);
        // 加载资源
        AssetBundle targetBundle = abDic[targetBundleName];
        T res = targetBundle.LoadAsset<T>(resourceName);
        return res;
    }

    /// <summary>
    /// 异步加载资源，不指定类型。AB包依然是同步加载，资源异步加载，加载完后在回掉函数中处理对象
    /// </summary>
    /// <param name="targetBundleName">目标包名，该包同步加载</param>
    /// <param name="resourceName">目标资源名，异步加载</param>
    /// <param name="callback">回调函数</param>
    public void LoadResAsync(string targetBundleName, string resourceName, UnityAction<Object> callback)
    {
        // 对外接口只负责启动协程
        StartCoroutine(LoadResAsyncHelper(targetBundleName, resourceName, callback));
    }
    /// <summary>
    /// 使用协程加载资源，并在回调函数中使用
    /// </summary>
    /// <param name="targetBundleName">目标包名</param>
    /// <param name="resourceName">目标资源名</param>
    /// <param name="callback">回调函数</param>
    /// <returns></returns>
    private IEnumerator LoadResAsyncHelper(string targetBundleName, string resourceName, UnityAction<Object> callback)
    {

        // 加载目标包
        LoadSingleAssetBundle(targetBundleName);
        // 加载资源
        AssetBundle targetBundle = abDic[targetBundleName];
        AssetBundleRequest res = targetBundle.LoadAssetAsync(resourceName);
        yield return res;
        // 回调函数，通过委托传递给外部，外部来使用
        callback(res.asset);
        yield return null;
    }

    /// <summary>
    /// 异步加载资源，指定类型。AB包依然是同步加载，资源异步加载，加载完后在回掉函数中处理对象
    /// </summary>
    /// <param name="targetBundleName">目标包名，该包同步加载</param>
    /// <param name="resourceName">目标资源名，异步加载</param>
    /// <param name="type">目标资源类型</param>
    /// <param name="callback">回调函数</param>
    public void LoadResAsync(string targetBundleName, string resourceName, System.Type type, UnityAction<Object> callback)
    {
        // 对外接口只负责启动协程
        StartCoroutine(LoadResAsyncHelper(targetBundleName, resourceName, type, callback));
    }
    /// <summary>
    /// 使用协程加载资源，并在回调函数中使用
    /// </summary>
    /// <param name="targetBundleName">目标包名</param>
    /// <param name="resourceName">目标资源名</param>
    /// <param name="type">目标资源类型</param>
    /// <param name="callback">回调函数</param>
    /// <returns></returns>
    private IEnumerator LoadResAsyncHelper(string targetBundleName, string resourceName, System.Type type, UnityAction<Object> callback)
    {

        // 加载目标包
        LoadSingleAssetBundle(targetBundleName);
        // 加载资源
        AssetBundle targetBundle = abDic[targetBundleName];
        AssetBundleRequest res = targetBundle.LoadAssetAsync(resourceName, type);
        yield return res;
        // 回调函数，通过委托传递给外部，外部来使用
        callback(res.asset);
        yield return null;
    }

    /// <summary>
    /// 异步加载资源，使用泛型。AB包依然是同步加载，资源异步加载，加载完后在回掉函数中处理对象
    /// </summary>
    /// <param name="targetBundleName">目标包名，该包同步加载</param>
    /// <param name="resourceName">目标资源名，异步加载</param>
    /// <param name="callback">回调函数</param>
    public void LoadResAsync<T>(string targetBundleName, string resourceName, UnityAction<T> callback) where T:Object
    {
        // 对外接口只负责启动协程
        StartCoroutine(LoadResAsyncHelper<T>(targetBundleName, resourceName, callback));
    }
    /// <summary>
    /// 使用协程加载资源且使用泛型，并在回调函数中使用
    /// </summary>
    /// <param name="targetBundleName">目标包名</param>
    /// <param name="resourceName">目标资源名</param>
    /// <param name="callback">回调函数</param>
    /// <returns></returns>
    private IEnumerator LoadResAsyncHelper<T>(string targetBundleName, string resourceName, UnityAction<T> callback) where T:Object
    {

        // 加载目标包
        LoadSingleAssetBundle(targetBundleName);
        // 加载资源
        AssetBundle targetBundle = abDic[targetBundleName];
        AssetBundleRequest res = targetBundle.LoadAssetAsync<T>(resourceName);
        yield return res;
        // 回调函数，通过委托传递给外部，外部来使用
        callback(res.asset as T);
        yield return null;
    }


    /// <summary>
    /// 卸载单个包，若该包存在则将其卸载，否则什么也不做
    /// </summary>
    /// <param name="targetBundleName"></param>
    public void Unload(string targetBundleName)
    {
        //若该包已被加载才能卸载
        if (abDic.ContainsKey(targetBundleName))
        {
            // 卸载该包且保留场景中资源
            abDic[targetBundleName].Unload(false);
            // 将该包从字典中移除
            abDic.Remove(targetBundleName);
        }
    }
    /// <summary>
    /// 卸载所有已加载的AB包
    /// </summary>
    public void UnloadAll()
    {
        // 卸载所有包
        AssetBundle.UnloadAllAssetBundles(false);
        // 清空字典
        abDic.Clear();
    }
}
