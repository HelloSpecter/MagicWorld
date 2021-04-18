using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Managers;
using Common.Data;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪管理器
    /// </summary>
     class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;
        /// <summary>
        /// 刷新时间
        /// </summary>
        private float spawnTime = 0;

        /// <summary>
        /// 消失时间
        /// </summary>
        private float unspawnTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define,Map map)
        {
            this.Define = define;
            this.Map = map;

            //加载刷怪点
            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {//由刷怪规则指向具体的刷怪点
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpawnRule[{0}] SpawnPoint[{1}] not existed", this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        /// <summary>
        /// 判断每帧是否满足刷怪要求
        /// </summary>
        public void Update()
        {
            if (this.CanSpawn())
            {
                //满足则执行刷怪
                this.Spawn();
            }
        }

        bool CanSpawn()
        {
            //是否已经执行过刷怪
            if (this.spawned)
            {
                return false;
            }
            //unspawnTime为怪物上次被消灭的时间
            if (this.unspawnTime + this.Define.SpawnPeriod > Time.time)
            {
                return false;
            }

            return true;
        }

        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}]Spawn[{1}:Mon:{2},Lv:{3}] At Point:{4}", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }

    }
}
