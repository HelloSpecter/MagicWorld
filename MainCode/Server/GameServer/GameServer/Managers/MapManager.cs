using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;
using Common.Data;

namespace GameServer.Managers
{
    class MapManager:Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();
        public void Init()
        {
            foreach (var item in DataManager.Instance.Maps)
            {
                Map map = new Map(item.Value);
                this.Maps[map.ID] = map;
                Log.Info("New Map > ID:[" + item.Value.ID + "] Name:[" + item.Value.Name + "] 已加载!");
            }
            
        }
        public Map this[int mapId]
        {
            get
            {
                return Maps[mapId];
            }
        }
        public void Update()
        {
            foreach (var map in Maps.Values)
            {
                map.Update();
            }
        }

    }
}
