using UnityEngine;
using UnityEngine.UI;
using Models;
using Common;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButtons;
    public GameObject submitButtons;

    private void Start()
    {

    }

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();//更新UI信息
        if (this.quest.Info == null)
        {//若任务是未完成状态，则显示“接受”和“拒绝”的Button
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {//若任务是已完成状态，则只显示“完成”的Button
                openButtons.SetActive(false);
                submitButtons.SetActive(true);
            }
            else
            {//错误情况，2套按钮均隐藏
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }


    }

    void UpdateQuest()
    {
        if (this.quest != null)
        {
            if (this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(quest);
            }
        }
    }
}