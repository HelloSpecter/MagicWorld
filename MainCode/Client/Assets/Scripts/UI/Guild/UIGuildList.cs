using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Services;
using SkillBridge.Message;

class UIGuildList : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    private UIGuildItem selectedItem;

    // Use this for initialization
    void Start()
    {
        this.listMain.onItemSelected += this.OnGuildMembersSelected;
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;//只有当公会UI面板打开时才注册UI刷新逻辑

        GuildService.Instance.SendGuildListRequest();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    public void OnGuildMembersSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    /// <summary>
    /// 初始化公会列表
    /// </summary>
    /// <param name="guilds"></param>
    void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    void ClearList()
    {
        //清空公会列表
        this.listMain.RemoveAll();
    }

    public void OnClickJoin()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要加入的公会");
            return;
        }
        MessageBox.Show(string.Format("确定要加入公会[{0}]吗", selectedItem.Info.GuildName), "申请加入公会", MessageBoxType.Confirm, "申请加入", "取消").OnYes += () =>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
        };
    }


}
