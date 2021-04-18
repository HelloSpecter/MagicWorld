using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using SkillBridge.Message;
using System;
using Models;

class UIMemberItem : ListView.ListViewItem {

    public NGuildMemberInfo Member;

    public Text Name;
    public Text Level;
    public Text Class;
    public Text Type;
    public Text JoinDate;
    public Text State;

    public Image Image;
    private Sprite normalBg;
    public Sprite SelectedBg;

    private void Start()
    {
        this.Image = gameObject.GetComponent<Image>();
        this.normalBg = this.Image.sprite;
    }

    public override void onSelected(bool selected)
    {
        this.Image.overrideSprite = selected ? SelectedBg : normalBg;
    }

    internal void SetMemberInfo(NGuildMemberInfo item)
    {
        this.Member = item;
        if (this.Name != null) this.Name.text = this.Member.Info.Name;
        if (this.Level != null) this.Level.text = this.Member.Info.Level.ToString();
        if (this.Class != null) this.Class.text = this.Member.Info.Class.ToString();
        if (this.Type != null) this.Type.text = this.Member.Title.ToString();
        if (this.JoinDate != null) this.JoinDate.text = this.Member.joinTime.ToString();
        if (this.State != null) this.State.text = this.Member.Status == 1 ? "在线" : "离线";
    }
}
