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
using UnityEngine.Events;


namespace Services
{
    public class GuildService : Singleton<GuildService>, IDisposable//IDisposable——释放非托管资源
    {
        public UnityAction<bool> OnGuildCreateResult;
        public UnityAction OnGuildUpdate;
        public UnityAction<List<NGuildInfo>> OnGuildListResult;

        public void Init()
        {

        }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

        /// <summary>
        /// 发送创建公会
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="notice"></param>
        public void SendGuildCreate(string guildName,string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到公会创建响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("OnGuildCreateResponse:{0}", response.Result);
            if (OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0} 公会创建成功", response.guildInfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0}公会创建失败", response.guildInfo.GuildName), "公会");
            }
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="id"></param>
        public  void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildJoinReq = new GuildJoinRequest();
            msg.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            msg.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accept,GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildJoinRes = new GuildJoinResponse();
            msg.Request.guildJoinRes.Result = Result.Success;
            msg.Request.guildJoinRes.Apply = request.Apply;
            msg.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(msg);
        }

        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm, "允许", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }

        /// <summary>
        /// 收到加入公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功", "申请结果");
            }
            else
            {
                MessageBox.Show("加入公会失败", "申请结果");
            }
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild : {0} {1} {2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
            {
                this.OnGuildUpdate();
            }
        }

        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildMemberRemoveRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(msg);
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("退出公会成功", "公会");
            }
            else
            {
                MessageBox.Show("离开公会失败", "公会", MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

       /// <summary>
       /// 收到公会列表刷新
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="message"></param>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult != null)
            {
                this.OnGuildListResult(response.Guilds);
            }
        }

        

        

        

        
    }
}
