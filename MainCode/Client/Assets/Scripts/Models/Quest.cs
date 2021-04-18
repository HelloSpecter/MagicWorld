using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using Common.Data;

namespace Models
{


    public class Quest
    {
        public QuestDefine Define;//本地配置表
        public NQuestInfo Info;   //来自网络（服务器）的任务信息

        public Quest()
        {
            
        }

        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }
    }
}