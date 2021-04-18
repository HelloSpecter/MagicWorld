using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using log4net;
using SkillBridge;
using Network;
using SkillBridge.Message;
using GameServer.Managers;
using GameServer.Entities;
using System.ComponentModel;
using Common.Data;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        public void Init()
        {





        }

        /// <summary>
        /// 测试用：添加1个User（包含Player*1、Character*2）
        /// </summary>
        //void AddUser()
        //{
        //    DBService.Instance.Init();
        //    TUser user_Test = DBService.Instance.Entities.Users.Add(new TUser()
        //    {
        //        ID = 1,
        //        Password = "123",
        //        Player = new TPlayer()
        //        {
        //            ID = 1
        //        },
        //        RegisterDate = DateTime.Now,
        //        Username = "Test01"


        //    });
        //    TCharacter Char_Test1 = new TCharacter()
        //    {
        //        Class = 1,
        //        ID = 1,
        //        Level = 10,
        //        MapID = 1,
        //        Name = "AAA",
        //        TID = 1,
        //        Player = user_Test.Player
        //    };
        //    TCharacter Char_Test2 = new TCharacter()
        //    {
        //        Class = 2,
        //        ID = 2,
        //        Level = 11,
        //        MapID = 1,
        //        Name = "BBB",
        //        TID = 2,
        //        Player = user_Test.Player
        //    };
        //    TCharacter Char_Test3 = new TCharacter()
        //    {
        //        Class = 3,
        //        ID = 3,
        //        Level = 13,
        //        MapID = 1,
        //        Name = "CCC",
        //        TID = 3,
        //        Player = user_Test.Player
        //    };

        //    user_Test.Player.Characters.Add(Char_Test1);
        //    user_Test.Player.Characters.Add(Char_Test2);
        //    user_Test.Player.Characters.Add(Char_Test3);
        //    DBService.Instance.Entities.SaveChanges();


        //}

        public void Start()
        {
            //AddUser();
            //Log.Info("已增加测试用例！");


            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegisterRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogInRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);




        }

        void OnRegisterRequest(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest:User:{0} Pass:{1}", request.User, request.Passward);
            sender.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser()
                {
                    Username = request.User,
                    Password = request.Passward,
                    Player = player,
                    RegisterDate =System.DateTime.Now,
                });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }
            sender.SendResponse();



            ////初始化UserRegisterResponse

            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.userRegister = new UserRegisterResponse();
            ////使用DBService服务，用Linq语句查询数据库
            //DBService.Instance.Init();
            //TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            ////检查数据库中是否有重复项，若有则返回报错（用户已存在）
            //if (user != null)
            //{
            //    msg.Response.userRegister.Result = Result.Failed;
            //    msg.Response.userRegister.Errormsg = "已存在相同用户名，请使用其他用户名再试一次！";
            //    Log.Info("已存在相同用户名，请使用其他用户名再试一次！");
            //}
            ////检查数据库中是否有重复项，若没有则添加数据，保存数据库，并返回成功
            //if (user == null)
            //{
            //    TUser newUser = DBService.Instance.Entities.Users.Add(new TUser());
            //    TPlayer newPlayer = new TPlayer();
            //    newUser.Username = request.User;
            //    newUser.Password = request.Passward;
            //    newUser.Player = newPlayer;
            //    DBService.Instance.Entities.SaveChanges();
            //    msg.Response.userRegister.Result = Result.Success;
            //    msg.Response.userRegister.Errormsg = newUser.Username + "注册成功";
            //    Log.Info("新用户" + newUser.Username + "已写入数据库");

            //    //将当前会话User用户设为刚刚新建的User，若不赋值则后面新建角色会报错
            //    sender.Session.User = newUser;
            //    //---
            //}
            ////返回内容给UserRegisterResponse
            //byte[] data;
            //data = PackageHandler.PackMessage(msg);
            //sender.SendData(data, 0, data.Length);
            //Log.Info("响应数据已发送");


        }

        void OnLogInRequest(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest:User:{0} Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            if (user== null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;

                //将当前用户下的所有角色数据添加到Response中
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.configId = c.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            Log.Info(sender.Session.Response.userLogin.Result.ToString() + sender.Session.Response.userLogin.Errormsg);

            sender.SendResponse();

            Log.Info("UserLoginResponse响应数据已发送");

            ////初始化响应数据
            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.userLogin = new UserLoginResponse();

            ////检查数据库中当前用户是否存在
            //DBService.Instance.Init();
            //TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            //if (user != null)
            //{
            //    if (user.Password == request.Passward)
            //    {
            //        msg.Response.userLogin.Result = Result.Success;
            //        msg.Response.userLogin.Errormsg = null;
            //        //记录当前会话Session
            //        sender.Session.User = user;

            //        //提取该User下的userinfo、playerinfo、characters
            //        msg.Response.userLogin.Userinfo = new NUserInfo();
            //        msg.Response.userLogin.Userinfo.Player = new NPlayerInfo();

            //        msg.Response.userLogin.Userinfo.Id = (int)user.ID;
            //        msg.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
            //        foreach (var db_char in user.Player.Characters)
            //        {
            //            NCharacterInfo characterInfo = new NCharacterInfo()
            //            {
            //                Id = db_char.ID,
            //                Tid = db_char.ID,
            //                Name = db_char.Name,
            //                Type = CharacterType.Player,
            //                Class = (CharacterClass)db_char.Class,
            //                Level = db_char.Level,
            //                mapId = db_char.MapID
            //            };
            //            msg.Response.userLogin.Userinfo.Player.Characters.Add(characterInfo);
            //        }



            //    }
            //    else
            //    {
            //        msg.Response.userLogin.Result = Result.Failed;
            //        msg.Response.userLogin.Errormsg = "密码错误";
            //    }
            //}
            //if (user == null)
            //{
            //    msg.Response.userLogin.Result = Result.Failed;
            //    msg.Response.userLogin.Errormsg = "用户名不存在";
            //}

            ////发送响应数据
            //byte[] data = PackageHandler.PackMessage(msg);
            //sender.SendData(data, 0, data.Length);
            //Log.Info(msg.Response.userLogin.Result.ToString() + msg.Response.userLogin.Errormsg);
            //Log.Info("响应数据已发送");

        }

        void OnCreateCharRequest(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            //初始化UserCreateCharacterResponse

            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.createChar = new UserCreateCharacterResponse();

            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            

            //使用DBService服务，用Linq语句查询数据库
            DBService.Instance.Init();
            TCharacter sameChar = DBService.Instance.Entities.Characters.Where(u => u.Name == request.Name).FirstOrDefault();




            //检查数据库中是否有重复项，若有则返回报错（角色名称已被其他角色占用）
            if (sameChar != null)
            {
                sender.Session.Response.createChar.Result= Result.Failed;
                sender.Session.Response.createChar.Errormsg = "已存在相同角色名，请使用其他名字再试一次！";
                //msg.Response.createChar.Result = Result.Failed;
                //msg.Response.createChar.Errormsg = "已存在相同角色名，请使用其他名字再试一次！";

                Log.Info("已存在相同角色名，请使用其他名字再试一次！");

                //return;//这里手残打了return，导致创建角色失败后，不能给客户端反馈失败响应
            }
            //检查数据库中是否有重复项，若没有则添加数据，保存数据库，并返回成功
            if (sameChar == null)
            {
                //查询数据库中与请求对应的User账户
                TUser whereAddChar = DBService.Instance.Entities.Users.Where(u => u.ID == sender.Session.User.ID).FirstOrDefault();

                if (whereAddChar == null)
                {
                    Log.Error("用户为空！");
                    return;
                }

                //根据创建请求，创建1个新的TChar（没有隶属关系）
                TCharacter newChar = new TCharacter()
                {


                    Name = request.Name,
                    Class = (int)request.Class,
                    MapID = 1,
                    ID = 1,
                    TID = (int)request.Class,
                    Level = 1,
                    Player = whereAddChar.Player,
                    //初始位置X、Y坐标，Z是地图场景地平线高度
                    MapPosX = 5000,
                    MapPosY = 4000,
                    MapPosZ = 820,
                    Gold = 100000,//初始金币为10万金币
                    Equips = new byte[28],
                };

                //新创建角色时，创建1个初始化的背包
                var bag = new TCharacterBag();
                bag.Owner = newChar;
                bag.Items = new byte[0];
                bag.Unlocked = 20;//初始解锁20个格子
                //TCharacterItem item = new TCharacterItem();
                newChar.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);
                newChar = DBService.Instance.Entities.Characters.Add(newChar);
                //---

                //将新创建的TChar与用户User绑定
                whereAddChar.Player.Characters.Add(newChar);

                //新建角色时，默认添加2种道具
                newChar.Items.Add(new TCharacterItem()
                {
                    Owner = newChar,
                    ItemID =1,
                    Count =20,
                });
                newChar.Items.Add(new TCharacterItem()
                {
                    Owner=newChar,
                    ItemID=2,
                    Count=20,
                });
                //---

                //测试:添加装备道具
                newChar.Items.Add(new TCharacterItem()
                {
                    Owner = newChar,
                    ItemID=2001,
                    Count=2,
                });
                //---

                //TChar添加到当前对话Session用户的角色列表下
                sender.Session.User.Player.Characters.Add(newChar);

                //Important：保存对数据库的修改
                DBService.Instance.Entities.SaveChanges();

                sender.Session.Response.createChar.Result = Result.Success;
                sender.Session.Response.createChar.Errormsg = request.Class + "-" + request.Name + "-创建成功";

                //msg.Response.createChar.Result = Result.Success;
                //msg.Response.createChar.Errormsg = request.Class + "-" + request.Name + "-创建成功";


                //遍历当前Session用户下的所有角色，添加至创建请求对应的返回响应，Respon的Chars列表内
                foreach (var c in sender.Session.User.Player.Characters)
                {
                    NCharacterInfo characterInfo = new NCharacterInfo()
                    {
                        Id = c.ID,
                        configId = c.TID,//将TChar.ID(数据库中的ID位置)赋值给NChar.Tid
                        Name = c.Name,
                        Type = CharacterType.Player,
                        Class = (CharacterClass)c.Class,
                        Level = c.Level,
                        mapId = c.MapID
                    };
                    //msg.Response.createChar.Characters.Add(characterInfo);
                    sender.Session.Response.createChar.Characters.Add(characterInfo);
                }

                Log.Info(request.Class + "-" + request.Name + "-已写入数据库");
                Log.Info("Session.User.Player.Chars 当前数量为:" + sender.Session.User.Player.Characters.Count);
            }
            //返回内容给UserRegisterResponse
            //byte[] data;
            //data = PackageHandler.PackMessage(msg);
            //sender.SendData(data, 0, data.Length);
            sender.SendResponse();
            Log.Info("响应数据已发送");


        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.charId);
            Log.InfoFormat("UserGameEnterRequest:characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddChar(dbchar);
            //在缓存中保存登录游戏的用户会话session
            SessionManager.Instance.AddSession(character.Id, sender);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();

            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";

            //进入成功，发送初始角色信息
            sender.Session.Response.gameEnter.Character = character.Info;
            sender.SendResponse();

            sender.Session.Character = character;
            sender.Session.PostResponser = character;//初始化后处理器，后处理逻辑由角色Character执行
            
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);

            //////从当前Session中取得：被选择进入游戏的TChar
            ////TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.charId);

            ////从当前Session中取得：被选择进入游戏的TChar
            //TCharacter dbchar = new TCharacter();
            //foreach (var item in sender.Session.User.Player.Characters)
            //{
            //    if (item.ID == request.charId)
            //    {
            //        dbchar = item;
            //        Log.InfoFormat("UserGameEnterRequest: characterID:{0},Name:{1},Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            //        break;
            //    }

            //}

            ////发送GameEnter的Response
            ////NetMessage msg = new NetMessage();
            ////msg.Response = new NetMessageResponse();
            ////msg.Response.gameEnter = new UserGameEnterResponse();

            
            

            //if (dbchar != null)
            //{

            //    //msg.Response.gameEnter.Result = Result.Success;
            //    //msg.Response.gameEnter.Errormsg = null;

                

            //}

            //else
            //{
            //    msg.Response.gameEnter.Result = Result.Failed;
            //    msg.Response.gameEnter.Errormsg = "选择了空游戏角色/游戏角色不存在！";
            //    byte[] data = PackageHandler.PackMessage(msg);
            //    sender.SendData(data, 0, data.Length);
            //}



            ////发送CharEnterMap的Response
            //if (dbchar != null)
            //{
            //    //根据Tchar产生实体Char对象，并保存至Session会话中
            //    sender.Session.Character = CharacterManager.Instance.AddChar(dbchar);
            //    //---

            //    //发送角色信息（身上拥有的物品）
            //    msg.Response.gameEnter.Character = sender.Session.Character.Info;
            //    //---
                
            //    /*
            //    //道具系统测试
            //    //int ItemId = 2;
            //    //Character  character = sender.Session.Character;
            //    //bool hasItem = character.ItemManager.HasItem(ItemId);
            //    //Log.InfoFormat("HasItem:[{0}][{1}]", ItemId, hasItem);
            //    //if (hasItem)
            //    //{
            //    //    //sender.Session.Character.ItemManager.RemoveItem(ItemId, 1);
            //    //}
            //    //else
            //    //{
            //    //    character.ItemManager.AddItem(1, 200);//叠加限制为99，这里测试叠加逻辑是否正确
            //    //    character.ItemManager.AddItem(2, 100);
            //    //    character.ItemManager.AddItem(3, 30);
            //    //    character.ItemManager.AddItem(4, 120);
            //    //}
            //    //Models.Item item = character.ItemManager.GetItem(ItemId);
            //    //Log.InfoFormat("Item:[{0}][{1}]", ItemId, item);
            //    //DBService.Instance.Save();
            //    //---
            //    */

            //    byte[] data = PackageHandler.PackMessage(msg);
            //    sender.SendData(data, 0, data.Length);

            //    //sender.Session.Character = character;

            //    MapManager.Instance[dbchar.MapID].CharacterEnter(sender, sender.Session.Character);

            //}
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {

            //NetMessage msg = new NetMessage();
            //msg.Response = new NetMessageResponse();
            //msg.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();

            Character CurChar = sender.Session.Character;

            //删除缓存中的用户会话
            SessionManager.Instance.RemoveSession(CurChar.Id);

            //保存Char当前位置
            this.SaveChar(CurChar);

            //msg.Response.gameLeave.Result = Result.Success;
            //msg.Response.gameLeave.Errormsg = "";

            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "";


            //byte[] data = PackageHandler.PackMessage(msg);
            //sender.SendData(data, 0, data.Length);

            sender.SendResponse();

            //执行角色退出操作（删除逻辑角色）
            CharacterLeave(CurChar);

        }

        /// <summary>
        /// 保存Char当前数据至数据库
        /// </summary>
        /// <param name="CurChar"></param>
        public void SaveChar(Character CurChar)
        {
            //保存当前数据至数据库（位置等信息）
            TCharacter SaveChar = DBService.Instance.Entities.Characters.Where(u => u.ID == CurChar.Data.ID).FirstOrDefault();
            if (SaveChar != null)
            {
                SaveChar.MapID = CurChar.Info.mapId;
                SaveChar.MapPosX = CurChar.Position.x;
                SaveChar.MapPosY = CurChar.Position.y;
                SaveChar.MapPosZ = CurChar.Position.z;
                SaveChar.Equips = CurChar.Data.Equips;

                DBService.Instance.Entities.SaveChanges();
                Log.Error("角色当前位置信息已保存至数据库!");
            }
            //-------------------
        }

        /// <summary>
        /// 角色退出逻辑（OnGameLeave中重构）
        /// </summary>
        /// <param name="CurChar"></param>
        public void CharacterLeave(Character CurChar)
        {
            Log.InfoFormat("CharacterLeave:characterID:{0}:{1}", CurChar.Id, CurChar.Info.Name);
            SaveChar(CurChar);
            CharacterManager.Instance.RemoveChar(CurChar.Id);//Char.Id即为entityId
            CurChar.Clear();
            MapManager.Instance[CurChar.Info.mapId].CharacterLeave(CurChar);

        }

        public void Stop()
        {




        }


    }

}
