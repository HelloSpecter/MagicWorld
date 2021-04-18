using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using SkillBridge.Message;
using GameServer.Managers;
using GameServer.Entities;
using Network;
using log4net;
using Common.Data;

namespace GameServer.Services
{
    class MapService:Singleton<MapService>
    {
        public MapService()
        {
            //MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);

        }

        public void Init()
        {

            MapManager.Instance.Init();

        }

        //private void OnMapCharacterEnter(NetConnection<NetSession> sender, MapCharacterEnterRequest message)
        //{
        //    throw new NotImplementedException();
        //}

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;

            //request.entitySync.Entity.String()是扩展方法，用于将Entity的具体内容输出
            Log.InfoFormat("OnMapEntitySync:characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());

            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);

        }

        internal void SendEntityUpdate(NetConnection<NetSession> Sender,NEntitySync entitySync)
        {

            NetMessage msg = new NetMessage();
            msg.Response = new NetMessageResponse();

            msg.Response.mapEntitySync = new MapEntitySyncResponse();

            //这里Response的EntitySync是集合形式：提高发送性能，而不是1个1个发送其他玩家的改变
            msg.Response.mapEntitySync.entitySyncs.Add(entitySync);


            byte[] data = PackageHandler.PackMessage(msg);
            Sender.SendData(data, 0, data.Length);

        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            //获取当前游戏角色
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport:characterID:{0}:{1} TeleporterId:{2}", character.Id, character.Data, request.teleporterId);

            //校验1：传送点定义文件是否有这个ID（传送请求ID）
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {

                Log.WarningFormat("Request.teleporterId:[" + request.teleporterId + "]" + "is not existed!");
                return;//漏加return会导致继续向下进行的错误逻辑
            }

            //获取原地点的TeleporterDefine

            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];

            //校验2：传送ID对应的目标点是否正确

            if (source.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {

                Log.WarningFormat("source.LinkTo:[" + source.LinkTo + "]" + "is not existed!");
                return;//漏加return会导致继续向下进行的错误逻辑

            }

            //获取目标地点的TeleporterDefine

            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            //原地图离开，更新角色信息（位置、方向），目标地图进入

            MapManager.Instance[source.MapID].CharacterLeave(character);
            EntityManager.Instance.MapEntities[source.MapID].Remove(character);

            character.Position = target.Position;
            character.Direction = target.Direction;

            MapManager.Instance[target.MapID].CharacterEnter(sender,character);
            //EntityManager.Instance.MapEntities[target.MapID]

                //使用TryGetValue，减少1次不必要的查找，同时避免了判断Key值是否存在而引发的“给定关键字不在字典中。”的错误
            List<Entity> entities = null;
            if (!EntityManager.Instance.MapEntities.TryGetValue(target.MapID, out entities))
            {
                entities = new List<Entity>();
                EntityManager.Instance.MapEntities[target.MapID] = entities;
            }

            entities.Add(character);

            Log.Info("EntityID:[" + character.entityId + "],At" + character.Position + ",已被添加至Map[" + target.MapID + "]目录");

        }



    }
}
