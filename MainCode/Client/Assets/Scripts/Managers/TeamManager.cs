using System;
using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using Models;

public class TeamManager : Singleton<TeamManager>
{
    public void Init()
    {

    }

    public void UpdateTeamInfo(NTeamInfo team)
    {
        User.Instance.TeamInfo = team;
        ShowTeamUI(team != null);
    }

    public void ShowTeamUI(bool show)
    {
        if (UIMain.Instance != null)//这里进行校验，确保主UI是存在的，避免空异常（切换场景时，主UI会销毁重建）
        {
            UIMain.Instance.ShowTeamUI(show);
        }
    }
}
