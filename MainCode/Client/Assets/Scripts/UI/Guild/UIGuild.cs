using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Services;
using SkillBridge.Message;
using Managers;
using UnityEngine.UI;

class UIGuild : MonoBehaviour
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;

    void start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate = null;
    }

    void UpdateUI()
    {
        //更新公会界面左侧UI
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
    }

    void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIMemberItem ui = go.GetComponent<UIMemberItem>();
            ui.SetMemberInfo(item);
        }
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickAppliesList()
    {

    }

    public void OnClickLeave()
    {

    }

    public void OnClickChat()
    {

    }

    public void OnClickKickout()
    {

    }

    public void OnClickPromote()
    {

    }

    public void OnClickDepose()
    {

    }
}
