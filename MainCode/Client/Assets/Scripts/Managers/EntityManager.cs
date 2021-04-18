using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Entities;
using SkillBridge.Message;




namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @event);
    }
    class EntityManager:Singleton<EntityManager>
    {
        //entities用于维护客户端所有（逻辑）角色
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary <int , IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId,IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }


        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity Nentity)
        {
            this.entities.Remove(Nentity.Id);
            if (notifiers.ContainsKey(Nentity.Id))
            {
                notifiers[Nentity.Id].OnEntityRemoved();
                notifiers.Remove(Nentity.Id);

            }


        }

        internal void OnEntitySync(NEntitySync data)
        {
            //根据NEntitySync查询并替换客户端中的逻辑实体
            Entity entity = null;
            //TryGetValue是查询存在与否并返回值得1种高效用法
            entities.TryGetValue(data.Id, out entity);

            if (entity!=null)
            {
                if (data.Entity!=null)
                {
                    entity.EntityData = data.Entity;
                }

                //当Entity变化时，同时出发事件通知
                if (notifiers.ContainsKey(data.Id))
                {
                    //通知数据变化
                    notifiers[entity.entityId].OnEntityChanged(entity);

                    //通知状态变化
                    notifiers[entity.entityId].OnEntityEvent(data.Event);

                }



            }





        }
    }
}
