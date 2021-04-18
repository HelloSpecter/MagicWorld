using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    public class Item
    {
        TCharacterItem dbItem;
        public int ItemId;
        public int Count;

        public Item(TCharacterItem dbItem)
        {
            this.dbItem = dbItem;
            this.ItemId = dbItem.ItemID;
            this.Count = dbItem.Count;

        }

        public void Add(int count)
        {
            this.Count += count;
            this.dbItem.Count = this.Count;

        }

        public void Remove(int count)
        {
            this.Count -= count;
            this.dbItem.Count = this.Count;

        }

        public bool Use(int count=1)
        {
            return false;

        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.ItemId, this.Count);
        }

        public ItemDefine Define;

        public Item(NItemInfo item)
        {

            this.Define = DataManager.Instance.Items[item.Id];

        }




    }
        
}
