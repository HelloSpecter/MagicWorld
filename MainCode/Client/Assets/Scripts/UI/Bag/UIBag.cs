using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;

public class UIBag : UIWindow {

    public Text GoldText;

    public Transform[] Pages;

    public GameObject BagItem;

    List<Image> slots;

	// Use this for initialization
	void Start () {
        //初始化时，使slots绑定所有背包格子(bag1 and bag2)
        if (slots==null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.Pages.Length; page++)
            {
                slots.AddRange(this.Pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        //加载背包时，协程启动背包初始化(实体)
        StartCoroutine(this.InitBags());
	}

    /// <summary>
    ///（协程）根据背包的逻辑数据初始化背包实体
    /// </summary>
    /// <returns></returns>
    public  IEnumerator InitBags()
    {
        //从背包管理器中初始化每个背包格子
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId>0)
            {
                //复制模具物品（占位物品）至格子位置
                GameObject go = Instantiate(BagItem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }

        }
        //未解锁的格子设为灰色（锁定）
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }

        this.SetGold();

        BagManager.Instance.uiBag = this;
        yield return null;
    }

    public void SetGold()
    {

        this.GoldText.text = User.Instance.CurrentCharacter.Gold.ToString();

    }

    void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount>0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }

    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(this.InitBags());
    }

    // Update is called once per frame
    void Update () {
        
	}
}
