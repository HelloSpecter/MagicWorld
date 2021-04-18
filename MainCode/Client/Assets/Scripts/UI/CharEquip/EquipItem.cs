using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Managers;

public class EquipItem : MonoBehaviour,IPointerClickHandler {
    public Image icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text limitCategory;

    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;
    

    private bool selected;
    public bool Selected
    {
        get
        {
            return this.selected;
        }
        set
        {
            this.selected = value;
            this.background.overrideSprite = this.selected ? selectBg : normalBg;
        }
    }

    public int Index { get; set; }
    private UICharEquip owner;
    private Item item;
    private bool isEquiped = false;

    public void SetEquipItem(int idx,Item item, UICharEquip owner,bool equiped)
    {
        this.owner = owner;
        this.Index = idx;
        this.item = item;
        this.isEquiped = equiped;

        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = this.item.Define.Level.ToString();
        if (this.limitClass != null) this.limitClass.text = this.item.Define.LimitClass.ToString();
        if (this.limitCategory != null) this.limitCategory.text = this.item.Define.Category;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);

    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;
            }
            else
            {
                //this.Selected = true;
                owner.ItemSelected(this);
            }
        }
    }

    void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备[{0}]", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        //定义1个匿名委托，并注册到msg.OnYes上
        msg.OnYes = () =>
         {
             this.owner.UnEquip(this.item);
         };
        //---
    }

    void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备[{0}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            //得到 目前装备 将要装备到的部位上的现有装备
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            //若这个部位已经装备了其他物品，则执行替换逻辑
            if (oldEquip != null)
            {
                var newmsg = MessageBox.Show(string.Format("要替换掉[{0}]吗", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                newmsg.OnYes = () =>
                 {
                     this.owner.DoEquip(this.item);
                 };
            }
            //如果该部位是空的，直接装备
            else
            {
                this.owner.DoEquip(this.item);
            }
        };


    }


}
