using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using GameServer.Services;
using Common;
using SkillBridge.Message;


namespace GameServer.Managers
{
    class ItemManager
    {

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public Character owner;

        public ItemManager(Character owner)
        {
            this.owner = owner;
            foreach (var newItem in this.owner.Data.Items)
            {

                Item item = new Item(newItem);
                Items.Add(newItem.ItemID, item);
            }

        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool UseItem(int ItemId, int count = 1)
        {
            Item item = null;

            if (Items.TryGetValue(ItemId, out item))
            {
                if (item.Count < count)
                {
                    return false;
                }

                //使用物品的逻辑

                //---

                item.Remove(count);
                return true;

            }
            return false;
        }

        /// <summary>
        /// 查询是否持有某物品
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public bool HasItem(int ItemId)
        {

            Item item = null;

            if (Items.TryGetValue(ItemId, out item))
            {
                if (item.Count > 0)
                {
                    return true;
                }

                return false;
            }

            return false;


        }

        /// <summary>
        /// 获得某个物品
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public Item GetItem(int ItemId)

        {
            Item item = null;
            Items.TryGetValue(ItemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}:{2}]", this.owner.Data.ID, ItemId, item);
            return item;

        }

        /// <summary>
        /// 给角色物品列表添加物品
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddItem(int ItemId,int count)
        {
            Item item = null;
            if (Items.TryGetValue(ItemId,out item))
            {
                item.Add(count);
                //return true;//这里加了return会导致后面的逻辑未能执行

            }
            else
            {
                TCharacterItem characterItem = new TCharacterItem()
                {
                    CharacterID = owner.Data.ID,
                    Owner = owner.Data,
                    ItemID=ItemId,
                    Count = count
                

                };
                
                owner.Data.Items.Add(characterItem);
                item = new Item(characterItem);
                this.Items.Add(ItemId, item);

                
            }
            //物品变化（增加）时，调用StatusManager的AddItemChange方法
            this.owner.StatusManager.AddItemChange(ItemId, count, StatusAction.Add);

            //保存新修改的Item至DB中
            Log.InfoFormat("[{0}] AddItem [{1}] addCount:{2}", this.owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;

        }


        /// <summary>
        /// 移除一定数量的Item
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveItem(int ItemId,int count)
        {
            Item item = null;
            if (Items.TryGetValue(ItemId,out item))
            {

                if (item.Count<count)
                {
                    return false;
                }

                item.Remove(count);

                //物品变化（增加）时，调用StatusManager的AddItemChange方法
                this.owner.StatusManager.AddItemChange(ItemId, count, StatusAction.Delete);

                Log.InfoFormat("[{0}]RemoveItem[{1}] removeCount:{2}", this.owner.Data.ID, item, count);
                //DBService.Instance.Save();
                return true;

            }

            return false;
        }



        public void GetItemInfos(List<NItemInfo> list)
        {

            foreach (var item in Items)
            {
                list.Add(new NItemInfo()
                {
                    Id = item.Value.ItemId,
                    Count = item.Value.Count

                });
            }

        }

    }
}
