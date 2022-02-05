using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// UI层级枚举
/// </summary>
public enum E_UI_Layer
{
    System,
    Top,
    Mid,
    Bot
}

/// <summary>
/// 
/// </summary>
public class UIMananger : BaseSingleton<UIMananger>
{
    // Canvas预制体所在的AB包名
    public string pathUrl { get { return "UI"; } }
    // Canvas预制体名
    public string canvasName { get { return "MainCanvas"; } }

    public Transform mainCanvas; // 主Canvas
    // 最上层，放置系统层级UI，比如游戏菜单等
    private Transform systemTrans;
    // 上层
    private Transform topTrans;
    // 中层
    private Transform midTrans;
    // 底层
    private Transform botTrans;
    // 存放所有的Panel，存放的Panel需要继承BasePanel
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    public UIMananger()
    {
        // 从AB包中加载Canvas，Canvas包含system, top, mid和bot4个层级
        GameObject canvas = ABLoader.GetInstance().LoadRes<GameObject>(pathUrl, canvasName);
        GameObject.Instantiate(canvas);
        GameObject.DontDestroyOnLoad(canvas);
        mainCanvas = canvas.transform;
        systemTrans = mainCanvas.Find("system");
        systemTrans = mainCanvas.Find("top");
        systemTrans = mainCanvas.Find("mid");
        systemTrans = mainCanvas.Find("bot");

        GameObject eventSystem = ABLoader.GetInstance().LoadRes<GameObject>(pathUrl, "EventSystem");
        GameObject.Instantiate(eventSystem);
        GameObject.DontDestroyOnLoad(eventSystem);
    }

    /// <summary>
    /// 显示某个panel，从AB包中加载。
    /// Panel应该是AB包中的某个预制体，该预制体是一个Panel并且上面绑定了一个类型为T的脚本，该脚本继承自BasePanel
    /// </summary>
    /// <param name="bundleName">AB包名</param>
    /// <param name="panelName">panel名</param>
    /// <param name="layer">放在哪个层级</param>
    /// <param name="callback">当加载完成后想做的事，比如设置Text啊，设置Button颜色之类的</param>
    public void ShowPanel<T>(string bundleName, string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callback = null) where T : BasePanel
    {
        // 若当前该面板已经显示了，则直接调用callback使用它
        if (panelDic.ContainsKey(panelName))
        {
            if (callback != null)
            {
                callback(panelDic[panelName] as T);
            }
            return;
        }
        // 否则，创建它并使用，这里还有点问题，就是在异步加载没有结束时，其他地方又异步加载了，会有问题
        ABLoader.GetInstance().LoadResAsync<GameObject>(bundleName, panelName, (obj) => {
            Transform father = botTrans;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    father = midTrans;
                    break;
                case E_UI_Layer.Top:
                    father = topTrans;
                    break;
                case E_UI_Layer.System:
                    father = systemTrans;
                    break;
            }
            // 设置panel的大小和位置
            obj.transform.SetParent(father);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            // 获取加载好的panel预设体上的panel脚本（需要继承BasePanel）
            T panel = obj.GetComponent<T>();

            if (callback != null)
                callback(panel);

            panel.ShowSelf(); // 调用其被显示时的方法
            panelDic.Add(panelName, panel);

        });
    }

    /// <summary>
    /// 关闭某个panel
    /// </summary>
    /// <param name="panelName">panel名</param>
    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HielSelf(); // 其被隐藏
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

    /// <summary>
    /// 从名称获取Panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T GetPanel<T>(string name) where T : BasePanel
    {
        if (panelDic.ContainsKey(name))
        {
            return panelDic[name] as T;
        }
        else return null;

    }
}
