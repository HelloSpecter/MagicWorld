using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public enum NpcType
    {
        none,
        Functional,
        Task
    }

    public enum NpcFunction
    {
        none,
        InvokeShop,
        InvokeInstance
    }
    public class NpcDefine
    {


        public int ID { get; set; }
        public string Name { get; set; }
        public string Descript { get; set; }
        public NpcType Type { get; set; }
        public NpcFunction Function { get; set; }
        public int Param { get; set; }


    }
}
