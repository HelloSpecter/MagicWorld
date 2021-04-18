using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using System.Runtime.InteropServices;

namespace Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BagItem
    {

        public ushort ItemId;
        public ushort Count;

        public static BagItem zero = new BagItem { ItemId = 0, Count = 0 };

        public BagItem(int itemId,int count)
        {
            this.ItemId = (ushort)itemId;
            this.Count = (ushort)count;
        }

        public static bool operator == (BagItem lhs,BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;
        }

        public static bool operator !=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }

    }
    
       
       

    
}