using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Managers;
using UnityEngine;
using Common.Data;
using Entities;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable//IDisposable——释放非托管资源
    {

        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);

        }


        public void Init()
        {

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void SendBuyItem(int shopId,int shopItemId)
        {

            Debug.Log("SendBuyItem");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(message);

        }

        private void OnItemBuy(object sender,ItemBuyResponse response)
        {
            MessageBox.Show("购买结果:" + response.Result + "\n" + response.Errormsg, "购买完成");


        }

        //记录发送装备操作请求的Item对象，便于对Response做出响应
        Item pendingEquip = null;
        bool isEquip;
        public bool SendEquipItem(Item equip,bool isEquip)
        {
            if (pendingEquip!=null)
            {
                return false;
            }

            Debug.Log("-->SendEquipItem");
            pendingEquip = equip;
            this.isEquip = isEquip;
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;


        }



        private void OnItemEquip(object sender, ItemEquipResponse response)
        {
            if (response.Result==Result.Success)
            {
                if (pendingEquip!=null)
                {
                    if (this.isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(this.pendingEquip);
                    }
                    else
                    {
                        EquipManager.Instance.OnUnEquipItem(this.pendingEquip.EquipInfo.Slot);
                    }
                    pendingEquip = null;
                }
            }
        }


    }
}
