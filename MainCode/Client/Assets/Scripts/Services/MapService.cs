using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Managers;
using UnityEngine;
using Common.Data;
using Entities;

namespace Services
{
    public class MapService : Singleton<MapService>, IDisposable//IDisposable——释放非托管资源
    {

        public int CurMapId;

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharLeave);

            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharEnter);

        }

        public void Init()
        {



        }

        private void OnMapCharEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogError("Respon.Char Num is:" + response.Characters.Count);
            foreach (var item in response.Characters)
            {
                if (User.Instance.CurrentCharacter == null || (item.Type == CharacterType.Player && User.Instance.CurrentCharacter.Id == item.Id))
                {
                    User.Instance.CurrentCharacter = item;
                    //item.Type = CharacterType.Player;
                }
                Debug.LogError("角色正在加入...");
                CharacterManager.Instance.AddChar(item);

            }

            if (response.mapId != CurMapId)
            {
                EnterMap(response.mapId);
                CurMapId = response.mapId;
            }

        }

        private void EnterMap(int mapId)
        {
            Debug.Log("正在切换地图...");

            MapDefine map = DataManager.Instance.Maps[mapId];
            User.Instance.CurMapDefine = map;

            string mapSouce = DataManager.Instance.Maps[mapId].Resource;
            SceneManager.Instance.LoadScene(mapSouce);
        }

        private void OnMapCharLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.Log("OnMapCharLeave:[" + response.entityId + "]");

            if (response.entityId != User.Instance.CurrentCharacter.entityId)
            {
                CharacterManager.Instance.Clear();
                User.Instance.CurrentCharacter = null;
            }
            else
            {
                CharacterManager.Instance.RemoveChar(response.entityId);
            }
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {

            Debug.LogFormat("MapEntityUpdateRequest:ID:{0},POS:{1},DIR:{2},SPD:{3}", entity.Id, entity.Position, entity.Direction, entity.Speed);

            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.mapEntitySync = new MapEntitySyncRequest();
            msg.Request.mapEntitySync.entitySync = new NEntitySync()
            {

                Entity = entity,
                Event = entityEvent,
                Id = entity.Id

            };

            NetClient.Instance.SendMessage(msg);

        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            //用于调试：字符串输出合并
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse:Entity{0}", response.entitySyncs.Count);
            sb.AppendLine();

            foreach (NEntitySync Nentity in response.entitySyncs)
            {
                //收到其他角色状态变化，将其状态传给EntityManager
                EntityManager.Instance.OnEntitySync(Nentity);
                sb.AppendFormat("      [{0}]event:{1} entity{2}", Nentity.Id, Nentity.Event, Nentity.Entity.String());
                sb.AppendLine();

            }
            Debug.Log(sb.ToString());

        }

        /// <summary>
        /// 向服务器发出地图传送请求
        /// teleporterID:地图传送点编号
        /// </summary>
        /// <param name="传送点ID"></param>
        internal void SendMapTeleport(int teleporterID)
        {

            Debug.LogFormat("MapTeleportRequest:teleporterID:{0}", teleporterID);
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.mapTeleport = new MapTeleportRequest();
            msg.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(msg);

        }

    }
}
