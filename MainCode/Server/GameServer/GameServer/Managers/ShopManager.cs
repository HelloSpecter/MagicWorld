using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using GameServer.Entities;
using SkillBridge.Message;
using Network;
using GameServer.Managers;
using Common;
using GameServer.Services;

namespace GameServer.Managers
{
    class ShopManager:Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender,int shopId,int shopItemId)
        {
            //验证商店Id是否正确
            if (!DataManager.Instance.Shops.ContainsKey(shopId))
            {
                return Result.Failed;
            }

            //将商店Id转换为物品Id（在服务端进行转换，进行简单的反外挂机制）
            ShopItemDefine shopItem;
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId,out shopItem))
            {
                Log.InfoFormat("BuyItem-character-{0}-Item-{1}-Count-{2}-Price-{3}", sender.Session.Character.Id, shopItem.ItemID, shopItem.Count, shopItem.Price);
                if (sender.Session.Character.Gold >= shopItem.Price)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }
            }
            return Result.Failed;

        }





    }
}
