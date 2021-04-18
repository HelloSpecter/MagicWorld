using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Services;


namespace GameServer.Managers
{
    class EquipManager:Singleton<EquipManager>
    {
        
         public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
         {
            Character character = sender.Session.Character;

            //服务端二级校验，防外挂
            if (!character.ItemManager.Items.ContainsKey(itemId))
            {
                return Result.Failed;
            }
            //---

            this.UpdateEquip(character.Data.Equips, slot, itemId, isEquip);

            TCharacter Tchar = DBService.Instance.Entities.Characters.Where(u => u.ID == character.Data.ID).FirstOrDefault();
            if (Tchar!=null)
            {
                //Test1:success
                //byte[] newdata = new byte[28];
                //newdata = (byte[])character.Data.Equips.Clone();
                //Tchar.Equips = newdata;
                //---

                ////Test2:Failed
                //Tchar.Equips = (byte[])character.Data.Equips;
                ////---

                //Test3:success
                Tchar.Equips = (byte[])character.Data.Equips.Clone();
                //---
            }

            DBService.Instance.Entities.SaveChanges();

            Log.WarningFormat("角色装备数据已保存：");
           

                Log.WarningFormat(Tchar.Equips.ToString());
            


            return Result.Success;
         }

         unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
         {
            //与背包系统类似，解析字节数据
            fixed(byte* pt = equipData)
            {
                int* slotid = (int*)(pt + slot * sizeof(int));
                if (isEquip)
                {
                    *slotid = itemId;
                }
                else
                {
                    *slotid = 0;
                }
            }

         }
    }
}
