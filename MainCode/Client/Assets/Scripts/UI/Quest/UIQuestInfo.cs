using UnityEngine;
using UnityEngine.UI;
using Models;


public class UIQuestInfo : MonoBehaviour
{
    public Text title;
    public Text[] targets;
    public Text description;
    public UIIconItem[] rewardItems;
    public Text rewardMoney;
    public Text rewardExp;


    private void Start()
    {
        
    }

    public void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);

        if (quest.Info == null)
        {
            this.description.text = quest.Define.Dialog;
        }

        else
        {
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
        }

        foreach (var item in rewardItems)
        {
            item.gameObject.SetActive(false);
        }

        if (quest.Define.RewardItem1!=0)
        {
            rewardItems[0].gameObject.SetActive(true);
            rewardItems[0].SetMainIcon(quest.Define.RewardItem1, quest.Define.RewardItem1Count);
        }

        if (quest.Define.RewardItem2 != 0)
        {
            rewardItems[1].gameObject.SetActive(true);
            rewardItems[1].SetMainIcon(quest.Define.RewardItem2, quest.Define.RewardItem2Count);
        }

        if (quest.Define.RewardItem3 != 0)
        {
            rewardItems[2].gameObject.SetActive(true);
            rewardItems[2].SetMainIcon(quest.Define.RewardItem3, quest.Define.RewardItem3Count);
        }


        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();//手动刷新<ContentSizeFitter>组件
        }

    }

    public void OnClickAbandon()
    {

    }
}