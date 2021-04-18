using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using log4net;
using SkillBridge.Message;
using Models;
using Managers;

namespace Services
{
    public  class UserService : Singleton<UserService>, IDisposable//IDisposable——释放非托管资源
    {

        public UnityEngine.Events.UnityAction<Result, string> OnRegister;//事件
        public UnityEngine.Events.UnityAction<Result, string> OnLogIn;
        public UnityEngine.Events.UnityAction<Result, string> OnCharCreate;
        //public UnityEngine.Events.UnityAction<int> OnCharEnter;
        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogIn);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCharCreate);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharEnter);

        }

       

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogIn);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCharCreate);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse> (this.OnGameLeave);

            //MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharEnter);




            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            //Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                    if (this.OnLogIn!=null)
                    {
                        this.OnLogIn(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        #region SendRequest
        /// <summary>
        /// 封装注册信息并发送给服务器
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }


        /// <summary>
        /// 封装登录信息并发送给服务器
        /// </summary>
        /// <param name="user"></param>
        /// <param name="psw"></param>
        public void SendLogIn(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }

            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }

        }

        /// <summary>
        /// 封装角色创建信息并发送给服务器
        /// </summary>
        /// <param name="charName"></param>
        /// <param name="curClass"></param>
        public void SendCharCreate(string charName, int curClass)
        {
            Debug.LogFormat("UserCharCreateRequest::Name :{0} Class:{1}", charName, (CharacterClass)(curClass + 1));
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.createChar = new UserCreateCharacterRequest();
            msg.Request.createChar.Name = charName;
            msg.Request.createChar.Class = (CharacterClass)(curClass + 1);

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(msg);
            }
            else
            {
                this.pendingMessage = msg;
                this.ConnectToServer();
            }
        }

        /// <summary>
        /// 封装选中的角色开始游戏并发送给服务器
        /// </summary>
        /// <param name="NCharacterInfo Char"></param>
        public void SendCharEnter(NCharacterInfo Char)
        {

            Debug.LogFormat("CharPlay::ID :{0} Name:{1}  Claa:{2}", Char.configId, Char.Name, Char.Class);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            //message.Request.gameEnter.charId = Char.Tid;
            message.Request.gameEnter.charId = User.Instance.CurIdx;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }

            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        #endregion

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            //Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }

        }
        void OnUserLogIn(object sender, UserLoginResponse response)
        {
            if (response.Result==Result.Success)
            {
                //将返回信息传给Models下User，作为本地存储

                Models.User.Instance.SetupUserInfo(response.Userinfo);
                Debug.LogFormat ("SetupUserInfo()->User.userInfo");

            }
            


            //考虑MessageBox设计的只能有1个实例，屏蔽this，在具体UI脚本逻辑内调用（注册OnLogIn）
            //Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnLogIn != null)
            {
                this.OnLogIn(response.Result, response.Errormsg);
            }
        }
        void  OnUserCharCreate(object sender, UserCreateCharacterResponse response)
        {
            if (response.Result==Result.Success)
            {
                
                //if (Models.User.Instance.Info==null)
                //{
                //    Models.User.Instance.SetupUserInfo(new NUserInfo());
                //    Models.User.Instance.Info.Player = new NPlayerInfo();
                    


                //}

                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
                //
                //Models.User.Instance.CurrentCharacter = response.Characters.Last();
                //
                Debug.LogFormat("角色创建成功");


            }
            

            if (this.OnCharCreate != null)
            {
                this.OnCharCreate(response.Result, response.Errormsg);
            }
        }


        //void OnMapCharEnter(object sender, MapCharacterEnterResponse response)
        //{
        //    NCharacterInfo info = response.Characters.First();
        //    Models.User.Instance.CurrentCharacter = info;
        //    if (this. OnCharEnter!=default(UnityEngine.Events.UnityAction<int>))
        //    {
        //        this.OnCharEnter(response.mapId);
        //    }


        //}



        public  void OnUserGameEnter(object sender, UserGameEnterResponse response)
        {
            if (response.Result==Result.Success)
            {
                Debug.Log("正在进入游戏");

                if (response.Character!=null)
                {
                    User.Instance.CurrentCharacter = response.Character;//当前游戏角色（逻辑）的赋值被提前
                    //初始化道具管理器
                    ItemManager.Instance.Init(response.Character.Items);
                    //初始化背包
                    BagManager.Instance.Init(response.Character.Bag);
                    //初始化装备系统
                    EquipManager.Instance.Init(response.Character.Equips);
                    //初始化任务系统
                    QuestManager.Instance.Init(response.Character.Quests);
                    //初始化好友系统
                    FriendManager.Instance.Init(response.Character.Friends);
                }

            }
            else
            {
                Debug.LogError(response.Result + response.Errormsg);
            }
            



        }



        public  void SendGameLeave()
        {

            //当前角色离开消息发送给服务器
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.gameLeave = new UserGameLeaveRequest();
            Debug.LogError("角色正在退出游戏......");

            NetClient.Instance.SendMessage(msg);

        }


        private void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            //当离开游戏时，清空当前地图及当前角色
            MapService.Instance.CurMapId = 0;
            

        }


    }
}
