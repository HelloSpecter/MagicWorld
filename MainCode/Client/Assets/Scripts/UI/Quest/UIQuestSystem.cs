using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Managers;
using Common.Data;

public class UIQuestSystem : UIWindow
{
    public Text title;
    public GameObject itemPrefab;
    public TabView Tabs;
    public ListView ListMain;
    public ListView ListBranch;
    public UIQuestInfo questInfo;
    private bool showAvailableList = false;



    // Use this for initialization
    void Start()
    {
        this.ListMain.onItemSelected += this.OnQuestSelected;
        this.ListBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
    }

    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }

    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }


    public void OnQuestSelected(ListView.ListViewItem item)
    {
        if (item.owner== this.ListMain && this.ListBranch.SelectedItem != null)
        {
            this.ListBranch.SelectedItem.Selected = false;
            this.ListBranch.SelectedItem = null;
        }

        else if (item.owner == this.ListBranch && this.ListMain.SelectedItem != null)
        {
            this.ListMain.SelectedItem.Selected = false;
            this.ListMain.SelectedItem = null;
        }

        //当任务列表中的任务条目被选中时
        UIQuestItem questItem = item as UIQuestItem;//里氏转换：父类 => 子类
        //在它右侧任务信息栏设置（显示）任务的具体信息
        this.questInfo.SetQuestInfo(questItem.quest);
    }

    /// <summary>
    /// 初始化任务列表
    /// </summary>
    void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.AllQuests)//从任务管理器中拉取所有可用的任务
        {
            if (showAvailableList)//如果当前选择的是可接受的任务（即还未开始的可接的任务）
            {
                if (kv.Value.Info != null)//网络任务信息若不为空，则说明该任务已经接受过了（在服务器端有相关数据），故跳过
                {
                    continue;
                }
            }
            else//如果当前选择的是正在进行任务
            {
                if (kv.Value.Info == null)//如果网络任务信息为空，则该任务还没有接受过，故跳过
                {
                    continue;
                }
            }

            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.ListMain.transform : this.ListBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);//设置任务信息（具体在UI面板上的内容显示）
            if (kv.Value.Define.Type == QuestType.Main)
            {
                this.ListMain.AddItem(ui as ListView.ListViewItem);
            }
            else
            {
                this.ListBranch.AddItem(ui as ListView.ListViewItem);
            }
        }
    }


    void ClearAllQuestList()
    {
        this.ListMain.RemoveAll();
        this.ListBranch.RemoveAll();
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
