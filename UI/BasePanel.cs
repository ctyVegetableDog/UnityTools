using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Paneld基类，其子类绑定到Panel上
/// 提供对该Panel下UI组件的查询和该Panel的显示隐藏
/// 不要直接使用
/// </summary>
public class BasePanel : MonoBehaviour
{
    // 用来存放所有UI控件，因为一个GameObject上可以绑定多个UI组件，所以用List把他们都存起来
    private Dictionary<string, List<UIBehaviour>> controllDic = new Dictionary<string, List<UIBehaviour>>();
    private void Awake()
    {
        FindAllChildren<Button>();
        FindAllChildren<Text>();
        FindAllChildren<Image>();
    }

    protected T GetChildrenByName<T>(string name) where T : UIBehaviour
    {
        if (controllDic.ContainsKey(name))
        {
            foreach (UIBehaviour each in controllDic[name])
            {
                if (each is T) return each as T;
            }
        }
        return null;
    }

    /// <summary>
    /// 找到该Panel下某种类型的全部元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    private void FindAllChildren<T>() where T : UIBehaviour
    {
        // 找到全部元素
        T[] allChildren = this.GetComponentsInChildren<T>();
        string objName;
        foreach (T each in allChildren)
        {
            objName = each.gameObject.name;
            if (controllDic.ContainsKey(objName))
            {
                controllDic[objName].Add(each);
            }
            else
            {
                controllDic.Add(objName, new List<UIBehaviour>() { each});
            }
        }
    }
    // 显示该panel
    public virtual void ShowSelf() { }
    // 隐藏该panel
    public virtual void HielSelf() { }

}
