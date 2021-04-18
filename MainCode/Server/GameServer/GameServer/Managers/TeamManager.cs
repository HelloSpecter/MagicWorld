using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using GameServer.Entities;
using GameServer.Services;
using Common;
using SkillBridge.Message;
using Network;
using Common.Data;

namespace GameServer.Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();//方便遍历
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();

        public void Init()
        {

        }

        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            this.CharacterTeams.TryGetValue(characterId, out team);
            return team;
        }

        public void AddTeamMember(Character leader,Character member)
        {
            if (leader.Team == null)//队长的队伍为空，则当前第1次组队，创建1个新队伍
            {
                leader.Team = CreateTeam(leader);
            }

            leader.Team.AddMember(member);
        }

        /// <summary>
        /// 可复用队伍队列
        /// 队伍表创建出来，即使成员全部解散也不销毁，新创建队伍时使用空的队伍，避免在内存中重复->创建、销毁，降低性能
        /// </summary>
        /// <param name="leader"></param>
        /// <returns></returns>
        Team CreateTeam(Character leader)
        {
            Team team = null;
            for (int i = 0; i < this.Teams.Count; i++)
            {
                team = this.Teams[i];
                if (team.Members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;
                }
            }

            team = new Team(leader);
            this.Teams.Add(team);
            team.Id = this.Teams.Count;
            return team;

        }

    }
}
