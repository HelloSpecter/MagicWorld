using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;

public class UITeam : MonoBehaviour {

    public Text teamTitle;
    public UITeamItem[] Members;//5个队伍成员
    public ListView list;

    // Use this for initialization
    void Start () {

        if (User.Instance.TeamInfo == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (var item in Members)
        {
            this.list.AddItem(item);
        }

    }

    private void OnEnable()
    {
        UpdateTeamUI();
    }

    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            UpdateTeamUI();
        }
    }

    public void UpdateTeamUI()
    {
        if (User.Instance.TeamInfo == null)
        {
            return;
        }

        this.teamTitle.text = string.Format("我的队伍({0}/5)", User.Instance.TeamInfo.Members.Count);
        for (int i = 0; i < 5; i++)
        {
            //根据队伍人数，显示队伍成员信息条并添加成员信息
            if (i<User.Instance.TeamInfo.Members.Count)
            {
                this.Members[i].SetMemberInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                this.Members[i].gameObject.SetActive(true);
            }
            //若队伍只有3个人，第4条和第5条队伍成员信息条隐藏
            else
            {
                this.Members[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClickLeave()
    {
        MessageBox.Show("确定要离开队伍吗", "退出队伍", MessageBoxType.Confirm, "确定离开", "取消").OnYes = () =>
            {
                TeamService.Instance.SendTeamLeaveRequest(User.Instance.TeamInfo.Id);
            };
    }

    void Update () {
		
	}
}
