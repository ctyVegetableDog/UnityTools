using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 提供场景同步，异步加载
/// </summary>
public class SceneMgr : BaseSingleton<SceneMgr>
{
    // 场景同步加载，通过场景名
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    // 开启场景异步加载，并在加载结束后做些事
    public void LoadSceneAysnc(string name, UnityAction callback)
    {
        MonoManager.GetInstance().StartCoroutine(LoadAysnc(name, callback));
        callback();
    }

    // 进行场景异步加载，更新进度条，并在加载结束后做些事
    private IEnumerator LoadAysnc(string name, UnityAction callback)
    {
        // 场景异步加载
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        // 场景未加载结束就一直更新进度
        while (!operation.isDone)
        {
            // 更新进度，传float要拆装箱啊
            EventCenter.GetInstance().Publish("进度条更新", operation.progress);
            yield return operation.progress;
        }
        callback();
        yield return operation;
    }
}
