﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using Network;
using Common;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        

        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);

        }
        public void Init()
        {
            TeamManager.Instance.Init();
        }

        /// <summary>
        /// 收到组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnTeamInviteRequest(NetConnection<NetSession> sender,TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteRequest::FromId:{0} FromName:{1} ToID:{2} ToName:{3}", request.FromId, request.FromName, request.ToId);
            //TODO:执行前置数据校验

            //开始逻辑
            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "好友不在线";
                sender.SendResponse();
                return;
            }

            if (target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已经有队伍";
                sender.SendResponse();
                return;
            }

            //转发请求
            Log.InfoFormat("ForwardTeamInviteRequest::FromId:{0} FromName:{1} ToID:{2} ToName:{3}", request.FromId, request.FromName, request);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();

        }

        /// <summary>
        /// 收到组队响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnTeamInviteResponse(NetConnection<NetSession> sender,TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteResponse::character:{0} Result:{1} FromId:{2} ToID:{3}", character.Id, response.Result, response.Request);
            sender.Session.Response.teamInviteRes = response;
            if (response.Result == Result.Success)
            {
                //接受了组队请求
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者已下线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }
        
        void OnTeamLeave(NetConnection<NetSession> sender,TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamLeave::character:{0} TeamID:{1}:{2}", character.Id, request.TeamId, request.characterId);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.Session.Response.teamLeave.characterId = request.characterId;

            character.Team.Leave(character);
            sender.SendResponse();
        }

    }
}
