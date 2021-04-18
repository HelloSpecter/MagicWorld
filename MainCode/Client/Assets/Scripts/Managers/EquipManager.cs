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
    
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();
        public event OnEquipChangeHandler OnEquipChanged;
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
        byte[] data;

        /// <summary>
        /// 装备系统初始化
        /// </summary>
        /// <param name="data"></param>
        unsafe public void Init(byte[] data)
        {
            this.data = data;
            this.ParseEquipData(data);
        }

        #region 将装备 穿、脱 请求发送到服务端
        //将装备 穿、脱 请求发送到服务端
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }
        //---
        #endregion

        #region 客户端对装备更换做出的响应

        public void OnEquipItem(Item equip)
        {
            //当前槽位物品没有发生变化
            if (this.Equips[(int)equip.EquipInfo.Slot]!=null&&this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }

            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            //触发装备更换事件
            if (OnEquipChanged != null)
            {
                OnEquipChanged();
            }

        }

        public  void OnUnEquipItem(EquipSlot slot)
        {
            if(this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;
                //触发装备更换事件
                if (OnEquipChanged != null)
                {
                    OnEquipChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取当前装备格子里的物品
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }


        /// <summary>
        /// 与背包类似，解析byte数据
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed(byte* pt = this.data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId>0)
                    {
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    }
                    else
                    {
                        Equips[i] = null;
                    }
                }

            }
        }


        /// <summary>
        /// 打包为byte数据
        /// </summary>
        /// <returns></returns>
        unsafe public byte[] GetEquipData()
        {
            fixed(byte* pt = data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                    {
                        *itemid = 0;
                    }
                    else
                    {
                        *itemid = Equips[i].Id;
                    }

                }
            }

            return this.data;
        }

        /// <summary>
        /// 检查是否已经装备了某个物品
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public bool Contains(int equipId)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                {
                    return true;
                }
            }
            return false;

        }

    }
}
