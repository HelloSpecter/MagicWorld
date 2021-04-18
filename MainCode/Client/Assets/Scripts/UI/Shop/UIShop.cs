using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using UnityEngine.UI;
using Managers;

public class UIShop : UIWindow {

    ShopDefine shop;
    public Text Title;
    public GameObject ShopItem;//UIShop的占位Prefab
    public Transform[] Pages;//指向商店显示页的Content
    public Transform[] ShopItems;//商店格子
    private int ShopItemsNum=0;
    private ShopItem selectedItem = null;

	// Use this for initialization
	void Start () {
        StartCoroutine(InitShop());
	}
	
    IEnumerator InitShop()
    {
        int page = 0;
        ShopItems = new Transform[10 * Pages.Length];
        foreach (var item in DataManager.Instance.ShopItems[shop.ID])
        {
            //判断当前Item在配置表内是否被启用
            if (item.Value.Status>0)
            {
                if (ShopItemsNum % 10 == 0 && ShopItemsNum != 0)
                {
                    page++;
                }
                if (page >= this.Pages.Length)
                {
                    Debug.LogWarning("商店物品数量超过商店显示页上限!");
                }
                //实例化占位符至Content父节点下
                GameObject newItem = Instantiate(ShopItem, Pages[page]);
                //实例化Item添加至ShopItems管理下
                ShopItems[ShopItemsNum] = newItem.transform;
                //设置Item的Icon、Name、Count等属性
                newItem.GetComponent<ShopItem>().SetShopItem(item.Key,item.Value, this);

                ShopItemsNum++;

            }
        }

        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.Title.text = shop.Name;

    }

    public void SelectShopItem(ShopItem shopItem)
    {
        

        if(selectedItem!=null)
        {
            selectedItem.Selected = false;
           
        }

        selectedItem = shopItem;
    }

    public void OnClickBuy()
    {

        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择需要购买的道具", "购买提示");
            return;
        }

        if (!ShopManager.Instance.BuyItem(this.shop.ID,this.selectedItem.shopItemId))
        {
            //未购买成功的提示
        }
       
    }

	// Update is called once per frame
	void Update () {
		
	}




}
