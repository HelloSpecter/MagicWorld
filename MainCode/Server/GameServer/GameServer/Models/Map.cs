using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Data;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Services;
using GameServer.Managers;


namespace GameServer.Models
{
    class Map
    {
        internal class MapChar
        {
            public  NetConnection<NetSession> sender;
            public  NCharacterInfo charInfo;
            public  Character character;
            internal MapChar(NetConnection<NetSession> sender, Character Char)
            {
                this.character = Char;
                this.sender = sender;
                this.charInfo = Char.Info;
            }
        }
        Dictionary<int, MapChar> MapChars = new Dictionary<int, MapChar>();
        public  MapDefine define;

        

        public  int ID
        {
            get
            {
                return define.ID;
            }
        }

        

        /// <summary>
        /// 刷怪管理器
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Char"></param>
        //新建实例
        SpawnManager SpawnManager = new SpawnManager();

        public MonsterManager MonsterManager = new MonsterManager();

        /// <summary>
        /// Map带参(MapDefine)构造函数
        /// </summary>
        /// <param name="mapDefine"></param>
        public Map(MapDefine mapDefine)
        {
            this.define = mapDefine;
            //实例化怪物管理器
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
            //---
        }

        public void Update()
        {
            SpawnManager.Update();//处理与时间相关的逻辑
        }

        /// <summary>
        /// 通知其他玩家，有怪物进入地图
        /// </summary>
        /// <param name="monster"></param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter:Map:{0} monsterId:{1}", this.define.ID, monster.Id);
            foreach (var kv in this.MapChars)
            {
                this.AddCharIntoMap(kv.Value.sender, monster.Info);
            }
        }

        public void CharacterEnter(NetConnection<NetSession> sender, Character Char)
        {

            Log.InfoFormat("CharacterEnter:Map:{0} characterId:{1}", this.define.ID, Char.Id);

            Char.Info.mapId = this.ID;
            this.MapChars[Char.Id] = new MapChar(sender, Char);

            sender.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            sender.Session.Response.mapCharacterEnter.mapId = this.define.ID;

            foreach (var kv in this.MapChars)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != Char)
                {
                    this.AddCharIntoMap(kv.Value.sender, Char.Info);
                }
            }

            foreach (var kv in this.MonsterManager.Monsters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            sender.SendResponse();
        }

            ////定义Message消息
            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            ////将新加入的角色加入Message的Chars里
            //Char.Info.mapId = this.ID;
            

            //msg.Response.mapCharacterEnter.Characters.Add(Char.Info);

            ////Log.ErrorFormat("当前Response.mapCharEnter.Chars Num is" + msg.Response.mapCharacterEnter.Characters.Count);

            //msg.Response.mapCharacterEnter.mapId = this.ID;

            ////遍历地图角色列表，①将该角色加入message Chars里；②将新角色的信息发送给其他地图角色
            //foreach (var oldChar in MapChars)
            //{
            //    msg.Response.mapCharacterEnter.Characters.Add(oldChar.Value.charInfo);
            //    SendCharIntoMapResponse(oldChar.Value.sender, Char);
            //}
            //Log.ErrorFormat("当前Response.mapCharEnter.Chars Num is" + msg.Response.mapCharacterEnter.Characters.Count);


            ////将进入的角色加入地图角色的字典内
            //MapChar mapChar = new MapChar(Sender, Char);
            //MapChars[Char.entityId] = mapChar;


            //Log.ErrorFormat("当前MapChars Num is" + MapChars.Count);


            ////NewChar加入地图角色列表，返回Message给NewChar客户端

            //byte[] data = PackageHandler.PackMessage(msg);
            //Sender.SendData(data, 0, data.Length);


        public void AddCharIntoMap(NetConnection<NetSession> sender, NCharacterInfo info)
        {
            //若response已经在其他地方创建过，则只添值，不重复创建
            if (sender.Session.Response.mapCharacterEnter == null)
            {
                sender.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                sender.Session.Response.mapCharacterEnter.mapId = this.define.ID;
            }

            sender.Session.Response.mapCharacterEnter.Characters.Add(info);
            sender.SendResponse();

            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            ////ATN
            //msg.Response.mapCharacterEnter.Characters.Add(NewChar.Info);

            //msg.Response.mapCharacterEnter.mapId = NewChar.Info.mapId;
            //byte[] data = PackageHandler.PackMessage(msg);
            //Sender.SendData(data, 0, data.Length);
            //Log.Info(NewChar.entityId + " Message of Enter Map: [" + this.ID + "] has already sent to " + Sender.Session.User.Username);

        }

        public void CharacterLeave(Character LeaveChar)
        {


            foreach (MapChar mapchar in MapChars.Values)
            {
                this.SendCharLeaveMap(mapchar.sender, LeaveChar);
            }

            Log.InfoFormat("MapChars中删除:[" + LeaveChar.Id+"]");
            this.MapChars.Remove(LeaveChar.Id);

        }
                

        void SendCharLeaveMap(NetConnection<NetSession> sender, Character LeaveChar)
        {
            sender.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            sender.Session.Response.mapCharacterLeave.entityId = LeaveChar.entityId;
            sender.SendResponse();


            //Log.Info("Send [" + LeaveChar.Id + "] Leave to " + Sender.Session.User.Username);

            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            //msg.Response.mapCharacterLeave.characterId = LeaveChar.Id;



            //byte[] data = PackageHandler.PackMessage(msg);
            //Sender.SendData(data, 0, data.Length);
            //Log.Info("MapCharacterLeaveResponse of [" + LeaveChar.Id+ "] has sent!");

        }


        public  void  UpdateEntity(NEntitySync entitySync)
        {

            
            foreach (var mapchar in MapChars)
            {
                //保存NEntity信息
                if (mapchar.Value.character.entityId==entitySync.Id)
                {
                    mapchar.Value.character.Position = entitySync.Entity.Position;
                    mapchar.Value.character.Direction = entitySync.Entity.Direction;
                    mapchar.Value.character.Speed = entitySync.Entity.Speed;

                }

                //将同步信息：NEntity转发(广播)给其他角色（当前地图）
                else
                {
                    MapService.Instance.SendEntityUpdate(mapchar.Value.sender,entitySync);
                }
            }




            



        }

        
        









    }
}
