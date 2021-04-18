using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Services;

namespace Managers
{
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            //注册NPC的响应事件
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;

        }

        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId,out shop))
            {

                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                {
                    //设置商店
                    uiShop.SetShop(shop);
                }


            }



        }

        public bool BuyItem(int shopId,int shopItemId)
        {

            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }

        
    }

}
