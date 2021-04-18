using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    public class UIElement
    {
        public string Resources;//UI窗口Prefab地址
        public bool Cache;//是否启用缓存
        public GameObject Instance = null;//实例
    }

    public Dictionary<Type, UIElement> UIElements = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        this.UIElements.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest", Cache = true });
        this.UIElements.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
        this.UIElements.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
        this.UIElements.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
        this.UIElements.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false});
        this.UIElements.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
        this.UIElements.Add(typeof(UIFriends), new UIElement() { Resources = "UI/UIFriends", Cache = false });
        this.UIElements.Add(typeof(UIGuild), new UIElement() { Resources = "UI/Guild/UIGuild", Cache = false });
        this.UIElements.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/Guild/UIGuildList", Cache = false });
        this.UIElements.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/Guild/UIGuildPopNoGuild", Cache = false });
        this.UIElements.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/Guild/UIGuildPopCreate", Cache = false });

    }

    ~UIManager()
    {
    }

    /// <summary>
    /// Show UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Show<T>()
    {
        Type type = typeof(T);

        if (this.UIElements.ContainsKey(type))
        {
            UIElement info = this.UIElements[type];
            if (info.Instance == null)
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                {
                    return default(T);
                }

                info.Instance = GameObject.Instantiate(prefab) as GameObject;

            }
            else
            {
                info.Instance.SetActive(true);
            }

            return info.Instance.GetComponent<T>();
        }

        Debug.Log("UIManager中没有注册此UI：" + type.ToString());
        return default(T);


    }


    public void Close(Type type)
    {
        if (this.UIElements.ContainsKey(type))
        {
            UIElement info = UIElements[type];
            if (info.Instance != null)
            {
                if (info.Cache)
                {
                    info.Instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(info.Instance);
                    info.Instance = null;
                }
            }
        }
        else
        {
            Debug.Log(type.ToString() + "未被添加至UI管理器，Close失败");
            return;
        }
    }


    //public void Show(Type ui)
    //{

    //    if (UIElements[ui].Instance == null)
    //    {
    //        UIElements[ui].Instance = Resloader.Load<GameObject>(UIElements[ui].Resources);
    //    }

    //    if (UIElements[ui].Cache)
    //    {
    //    UIElements[ui].Instance.SetActive(true);

    //    }
    //}

    //public void Close(Type ui)
    //{

    //    if (UIElements[ui].Cache)
    //    {
    //        UIElements[ui].Instance.SetActive(false);

    //    }
    //    else
    //    {
    //        UnityEngine.Object.Destroy(UIElements[ui].Instance);
    //    }


    //}





}
