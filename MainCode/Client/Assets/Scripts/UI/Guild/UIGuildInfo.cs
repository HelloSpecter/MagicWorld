﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Common;

class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildID;
    public Text leader;
    public Text notice;
    public Text memberNumber;

    private NGuildInfo info;

    public NGuildInfo Info
    {
        get
        {
            return this.info;
        }
        set
        {
            this.info = value;
            this.UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (this.info == null)
        {
            this.guildName.text = "None";
            this.guildID.text = "ID:0";
            this.leader.text = "会长：无";
            this.notice.text = "none";
            //todo:check1
            //this.memberNumber.text = string.Format("成员数量:0/{0}", GameDefine.GuildMaxMemberCount);
        }
        else
        {
            this.guildName.text = this.Info.GuildName;
            this.guildID.text = "ID:" + this.Info.Id;
            this.leader.text = "公会：" + this.Info.leaderName;
            this.notice.text = this.Info.Notice;
            //todo:check1
            //this.memberNumber.text = string.Format("成员数量:{0}/{1}", this.info.memberCount, GameDefine.GuildMaxMemberCount);
        }
    }





}
