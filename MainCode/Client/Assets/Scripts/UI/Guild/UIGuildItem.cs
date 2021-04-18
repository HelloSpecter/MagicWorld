using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using SkillBridge.Message;
using System;

class UIGuildItem : ListView.ListViewItem {

    public NGuildInfo Info;

    public Text ID;
    public Text Name;
    public Text Count;
    public Text LeaderName;

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


    internal void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (this.ID != null) this.ID.text = this.Info.Id.ToString();
        if (this.Name != null) this.Name.text = this.Info.GuildName;
        if (this.Count != null) this.Count.text = this.Info.memberCount.ToString();
        if (this.LeaderName != null) this.LeaderName.text = this.Info.leaderName;
    }
}
