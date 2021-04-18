using Common.Data;
using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Managers;
using Network;
using GameServer.Models;
using Common;

namespace GameServer.Entities
{
    class Character : CharacterBase,IPostResponser
    {
       
        public TCharacter Data;

        public ItemManager ItemManager;
        public QuestManager QuestManager;
        public StatusManager StatusManager;
        public FriendManager FriendManager;
        public Team Team;
        public int TeamUpdateTS;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Info = new NCharacterInfo();
            this.Id = cha.ID;
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.Name = cha.Name;

            ////如果TChar角色的等级未被定义，则赋予初始等级(初始等级由角色配置表定义)
            //cha.Level = cha.Level <= 0 ? DataManager.Instance.Characters[cha.TID].InitLevel: cha.Level;

            //this.Info.Level = cha.Level;

            //测试任务系统
            this.Info.Level = 10;


            this.Info.configId = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.configId];

            //实例化ItemManager
            this.ItemManager = new ItemManager(this);
            //读取该Character的物品表至协议中的网络数据
            this.ItemManager.GetItemInfos(this.Info.Items);

            //初始化背包,并将数据库数据传递给协议中的网络数据
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Bag.Unlocked = this.Data.Bag.Unlocked;

            //初始化装备
            this.Info.Equips = this.Data.Equips;

            //实例化StatusManager
            this.StatusManager = new StatusManager(this);

            //初始化任务系统
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);

            //初始化好友系统
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
        }

        public long Gold
        {
            get { return this.Data.Gold; }
            set
            {
                if (this.Data.Gold == value)
                {
                    return;
                }
                //当前金币量-之前的金币量，将其Delta值传给StatusManager的AddGoldChange方法
                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));
                //---
                this.Data.Gold = value;
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            this.FriendManager.PostProcess(message);

            if (this.Team != null)
            {
                Log.InfoFormat("PostProcess > Team:characterID:{0}:{1} {2}-{3}", this.Id, this.Info.Name, TeamUpdateTS, this.Team.timestamp);
                if (TeamUpdateTS < this.Team.timestamp)
                {
                    TeamUpdateTS = Team.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }
        }

        /// <summary>
        /// 角色离开时调用
        /// </summary>
        public void Clear()
        {
            this.FriendManager.OfflineNotify();
        }


        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Info.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level
            };
        }

    }
}
