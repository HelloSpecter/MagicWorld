using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Common.Data;

public class ShopItem : MonoBehaviour,ISelectHandler {
    public Image Icon;
    public Text Name;
    public Text Price;
    public Text Num;
    public Text limitClass;

    //选中效果
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    //---

    private UIShop shop;
    public  int shopItemId;
    private ShopItemDefine shopItem;
    private ItemDefine item = null;

    private bool isSelect = false;
    public bool Selected
    {
        get { return this.isSelect; }
        set
        {
            this.isSelect = value;
            //若被选中，则替换背景为选中样式
            this.background.overrideSprite = isSelect ? selectedBg : normalBg;
        }

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetShopItem(int id,ShopItemDefine shopItemDefine,UIShop owner)
    {
        DataManager.Instance.Items.TryGetValue(shopItemDefine.ItemID, out item);

        if (item != null)
        {
            this.shop = owner;
            this.shopItemId = id;
            this.shopItem = shopItemDefine;

            this.Icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
            this.Name.text = item.Name;
            this.Price.text = shopItem.Price.ToString();
            this.Num.text = "x"+shopItemDefine.Count.ToString();
            this.limitClass.text = this.item.LimitClass.ToString();
        }

    }


    public void OnSelect(BaseEventData eventData)
    {


        if (shop != null)
        {
            this.Selected = true;
            //告诉商店(Owner)，自己被选中了
            this.shop.SelectShopItem(this);
        }



    }
}
