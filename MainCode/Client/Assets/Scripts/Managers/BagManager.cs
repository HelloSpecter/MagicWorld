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

namespace Managers
{
    
    class BagManager:Singleton<BagManager>
    {
        public int Unlocked;
        public byte[] Date;
        public BagItem[] Items;
        public  UIBag uiBag;
        NBagInfo Info;


        /// <summary>
        /// 在进入游戏时完成背包物品的初始化（逻辑）
        /// </summary>
        /// <param name="info"></param>
        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            this.Date = info.Items;
            Items = new BagItem[this.Unlocked];
            if (this.Date != null && this.Date.Length>=this.Unlocked)
            {
            Analysis(this.Unlocked, this.Date);

            }
            else//如果Date是空的，初始化Items
            {
                this.Info.Items = new byte[this.Unlocked * sizeof(BagItem)];
                this.Reset();
            }
        }
       

        /// <summary>
        /// 重置背包
        /// </summary>
        public void Reset()
        {
            int i = 0;

            //逐个分析物品管理器中存储的物品，超过堆叠数量的依次往后满数量（堆叠限制）放置
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Count<=kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else
                {
                    int count = kv.Value.Count;
                    while (count>kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }




        }


        /// <summary>
        /// 解析byte[]格式的Items
        /// </summary>
        /// <param name="unLocked"></param>
        /// <param name="bagItems"></param>
        unsafe void Analysis(int unLocked,byte[] bagItems)
        {
            byte[] data = bagItems;
            //fixed以内：内存空间被固定
            fixed(byte* pt=data)
            {
                for (int i = 0; i < unLocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));//item是地址，BagItem的（首）地址
                    this.Items[i] = *item;//使用*运算符，加载item中地址指向的结构体变量
                }
            }


        }

        /// <summary>
        /// 压缩Items数据至byte[]格式
        /// </summary>
        /// <param name="unLocked"></param>
        /// <param name="Items"></param>
        /// <returns></returns>
        unsafe public  NBagInfo GetBagInfo(int unLocked,BagItem[] Items)
        {
            int infoSize = unLocked * sizeof(BagItem);
            byte[] bagInfo = new byte[infoSize];

            fixed(byte* pt = bagInfo)
            {
                for (int i = 0; i < unLocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }


            }
            Info.Items = bagInfo;
            return this.Info;


        }
        /// <summary>
        /// 背包物品（逻辑物品）增加
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="count"></param>
        public void AddItem(int ItemId,int count)
        {
            ushort addCount = (ushort)count;
            for (int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemId==ItemId)
                {
                    //能增加的最大数量
                    ushort canAdd = (ushort)(DataManager.Instance.Items[ItemId].StackLimit - this.Items[i].Count);
                    if (canAdd >= addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    //若增加数量超过堆叠上限，在下次循环中加到下个格子
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }

            //若仍未添加完，剩下的添加到空的格子里
            if (addCount>0)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    //如果格子是空的，将（剩余的）新增加的物品放进去
                    if (this.Items[i].ItemId == 0)
                    {
                        this.Items[i].ItemId = (ushort)ItemId;
                        this.Items[i].Count = addCount;
                        break;
                    }
                }
            }
            if (uiBag!=null)
            {
                //刷新背包
                uiBag.OnReset();

            }

        }

        public void RemoveItem(int itemId,int count)
        {


        }

        public void setGold()
        {
            if (uiBag!=null)
            {
                uiBag.SetGold();

            }
        }

    }
}
