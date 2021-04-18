using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;



namespace GameServer.Managers
{
    public class EntityManager:Singleton<EntityManager>
    {
        public List<Entity> AllEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();
        int idx = 0;

        public void AddEntity(int mapId,Entity entity)
        {
            AllEntities.Add(entity);
            Log.InfoFormat("AllEntities中添加了1个entity");


            //生成唯一ID
            entity.EntityData.Id= ++this.idx;
            Log.InfoFormat("生成entityID为:[" + entity.entityId + "]");



            //使用TryGetValue，减少1次不必要的查找，同时避免了判断Key值是否存在而引发的“给定关键字不在字典中。”的错误
            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId,out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
                Log.InfoFormat(" MapEntities:MapId[{0}]--Entities 目录已生成!", mapId);
            }

            entities.Add(entity);
            Log.Info("EntityID:[" + entity.entityId + "],At" + entity.Position + ",已被添加至MapId[" + mapId + "]目录");



        }


        public void RemoveEntiy(int mapId,Entity entity)
        {

            if (AllEntities.Contains(entity))
            {
                AllEntities.Remove(entity);
                Log.InfoFormat("AllEntities中删除:[{0}]", entity.entityId);
            }
            if (MapEntities[mapId].Contains(entity))
            {
                MapEntities[mapId].Remove(entity);
                Log.InfoFormat("MapEntities中删除:MapId[{0}]--EntityId[{1}]", mapId, entity.entityId);
            }

        }




    }
}
