using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using System;
using Managers;
using Models;
using SkillBridge.Message;

public class UICharEquip : UIWindow {
    public Text title;
    public Text money;
    public GameObject itemPrefab;
    public GameObject itemEquipedPrefab;
    public Transform itemListRoot;
    public List<Transform> slots;
    private EquipItem equipItem = null;
    
	// Use this for initialization
	void Start () {
       this.RefreshUI();
        EquipManager.Instance.OnEquipChanged += this.RefreshUI;
	}

    private void OnDestroy()
    {
        //在销毁时及时注销事件，以免累计注册导致服务端1次Response,客户端多次响应
        EquipManager.Instance.OnEquipChanged -= this.RefreshUI;
    }

    private void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedItems();
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    /// <summary>
    /// 初始化所有装备的列表
    /// </summary>
    void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                //判断是否已经被装备在身上
                if (EquipManager.Instance.Contains(kv.Key))
                {
                    continue;
                }
                GameObject go = Instantiate(itemPrefab, itemListRoot);
                EquipItem equipListItem = go.GetComponent<EquipItem>();
                equipListItem.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    void ClearAllEquipList()
    {
        if (itemListRoot.childCount>0)
        {

            //遍历装备列表节点下所有EquipListItem并清除
            foreach (var item in itemListRoot.GetComponentsInChildren<EquipItem>())
            {
                Destroy(item.gameObject);
            }
        }
    }

    void ClearEquipedList()
    {

        foreach (var item in slots)
        {
            //如果此节点（装备槽位）下有装备物品
            if (item.childCount>1)
            {
                Destroy(item.GetChild(1).gameObject);
            }
        }

    }

    /// <summary>
    /// 初始化已经装备的列表
    /// </summary>
    void InitEquipedItems()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            {
                if (item != null)
                {
                    GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                    EquipItem equipItem = go.GetComponent<EquipItem>();
                    equipItem.SetEquipItem(i, item, this, true);
                }

            }
        }


    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }

    // Update is called once per frame
    void Update () {
		
	}

    //修复多个物品可以被同时选择
    public void ItemSelected(EquipItem equipItem)
    {
        if (this.equipItem!=null)
        {
            this.equipItem.Selected = false;

        }
        this.equipItem = equipItem;
        this.equipItem.Selected = true;
        
    }
}
