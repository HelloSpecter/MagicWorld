using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Models;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪管理器
    /// </summary>
     class SpawnManager 
    {
        //刷怪器列表
        private List<Spawner> Rules = new List<Spawner>();

        private Map Map;

        public void Init(Map map)
        {
            this.Map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.define.ID))
            {
                foreach (var define in DataManager.Instance.SpawnRules[map.define.ID].Values)
                {
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }
        }

        public  void Update()
        {
            //没有规则返回
            if (Rules.Count == 0)
            {
                return;
            }
            //存在至少1条刷怪规则时，调用刷怪规则
            for (int i = 0; i < this.Rules.Count; i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
