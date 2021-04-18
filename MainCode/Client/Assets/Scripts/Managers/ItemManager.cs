using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Entities;
using SkillBridge.Message;
using Models;
using UnityEngine;
using Common.Data;
using Services;

namespace Managers
{
    
    class ItemManager:Singleton<ItemManager>
    {

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public void Init(List<NItemInfo> items)
        {

            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);


            }

            StatusService.Instance.ResgisterStatusNofity(StatusType.Item, OnItemNotify);
        }

        private bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }

            return true;
        }

        private void AddItem(int itemId,int count)
        {
            Item item = null;
            //若本地有Item增加数量，若没有则新建Item
            if (this.Items.TryGetValue(itemId,out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
            }
            //将新加物品添加到背包中
            BagManager.Instance.AddItem(itemId, count);
        }

        void RemoveItem(int itemId,int count)
        {
            if (!this.Items.ContainsKey(itemId))
            {
                return;
            }

            Item item = this.Items[itemId];

            if (item.Count < count)
            {
                return;
            }

            item.Count -= count;
            //将背包中的物品移除
            BagManager.Instance.RemoveItem(itemId, count);

        }



        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;

        }



    }
}
