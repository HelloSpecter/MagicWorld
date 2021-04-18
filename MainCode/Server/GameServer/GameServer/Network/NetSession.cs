using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer;
using GameServer.Entities;
using SkillBridge.Message;
using GameServer.Services;

namespace Network
{
    class NetSession:INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }//定义接口

        internal void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null)
            {
                UserService.Instance.CharacterLeave(this.Character);
            }
            
        }


        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }
                return response.Response;

            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                ////判断角色是否为空，状态是否有变化
                //if (this.Character != null && this.Character.StatusManager.HasStatus)
                //{
                //    //若状态有变化，将状态变化通知加在message里
                //    this.Character.StatusManager.ApplyResponse(Response);
                //}

                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(Response);
                }

                //Response的Message发送后，立即将response清空
                byte[] data = PackageHandler.PackMessage(response);
                response = null;
                //---
                return data;
            }
            return null;
        }

        ////测试使用
        //public int[] a { get; set; }
    }
}
