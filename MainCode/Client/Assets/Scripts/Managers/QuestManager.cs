using Common.Data;
using Models;
using SkillBridge.Message;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine.Events;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0,//无任务
        Complete,//拥有已完成可提交任务
        Available,//拥有可接受任务
        Incomplete,//拥有未完成任务
    }

    class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> QuestInfos;
        public Dictionary<int, Quest> AllQuests = new Dictionary<int, Quest>();

        //int=>NPCID
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> onQuestStatusChanged;
        public UnityAction<int> onNpcClose;

        int curNpcId;
        public void Init(List<NQuestInfo> quests)
        {
            this.QuestInfos = quests;
            AllQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }


        void InitQuests()
        {
            //初始化已有任务
            //foreach (var info in this.QuestInfos)
            //{
            //    Quest quest = new Quest(info);
            //    //将接受的任务信息，传给任务发出的NPC
            //    this.AddNpcQuest(quest.Define.AcceptNPC, quest);
            //    //将完成任务的信息，传给将要提交任务的NPC
            //    this.AddNpcQuest(quest.Define.SubmitNPC, quest);
            //    //将网络中的任务信息，根据任务ID添加到总任务列表内
            //    this.AllQuests[quest.Info.QuestId] = quest;
            //}

            foreach (var info in this.QuestInfos)
            {
                Quest quest = new Quest(info);
                this.AllQuests[quest.Info.QuestId] = quest;
            }

            this.CheckAvailableQuests();

            foreach (var kv in this.AllQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        /// <summary>
        /// 检查可接任务
        /// </summary>
        void CheckAvailableQuests()
        {

            //初始化可用任务
            foreach (var kv in DataManager.Instance.Quests)
            {

                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                {//职业限制
                    continue;
                }
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                {//等级限制
                    continue;
                }
                if (this.AllQuests.ContainsKey(kv.Key))
                {//不重复添加任务
                    continue;
                }

                if (kv.Value.PreQuest > 0)
                {//若有前置任务，判断是否完成了前置
                    Quest preQuest;
                    if (this.AllQuests.TryGetValue(kv.Value.PreQuest, out preQuest))//获取前置任务
                    {
                        if (preQuest.Info == null)
                        {//前置任务未开始(服务端没有存储前置任务的任何信息)
                            continue;
                        }
                        if (preQuest.Info.Status != QuestStatus.Finished)
                        {//前置任务未完成
                            continue;
                        }

                    }
                    else//前置任务未达到接受条件
                    {
                        continue;
                    }
                }

                Quest quest = new Quest(kv.Value);
                this.AllQuests[quest.Define.ID] = quest;


            }

        }

        private void AddNpcQuest(int npcID, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcID))
            {
                this.npcQuests[npcID] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }

            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Available,out availables))
            {
                //若还没有可接任务列表，则创建1个
                availables = new List<Quest>();
                //创建完绑定至该NPC的任务列表下
                this.npcQuests[npcID][NpcQuestStatus.Available] = availables;
            }

            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Complete,out complates))
            {
                //同上：已完成任务列表
                complates = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Complete] = complates;
            }

            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                //同上：未完成任务列表
                incomplates = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Incomplete] = incomplates;
            }

            if (quest.Info == null)
            {
                if (npcID == quest.Define.AcceptNPC && !this.npcQuests[npcID][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcID][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC == npcID && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcID][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcID][NpcQuestStatus.Complete].Add(quest);
                    }
                }

                if (quest.Define.SubmitNPC == npcID && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcID][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcID][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }

            }

        }

        /// <summary>
        /// 获取 NPC 任务的状态情况
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId,out status)) //获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return NpcQuestStatus.Incomplete;
                }
            }
            return NpcQuestStatus.None;
        }

        /// <summary>
        /// 打开 NPC 对话框
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool OpenNpcQuest(int npcId)
        {
            curNpcId = npcId;
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId,out status)) //获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count>0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
                }
            }

            return false;

        }

        bool ShowQuestDialog(Quest quest)
        {
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);

                dlg.OnClose += OnQuestDialogClose;//注册窗口关闭事件
                return true;
            }
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                {
                    MessageBox.Show(quest.Define.DialogIncomplete);
                }
            }

            return true;
        }

        void OnQuestDialogClose(UIWindow sender,UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes)
            {//UI被关闭时，若按下的是Yes（接受任务或完成任务）
                MessageBox.Show(dlg.quest.Define.DialogAccept);
                if (dlg.quest.Info == null)
                {//如果当前这个任务还没有接受过的记录，则向服务端发送接受任务请求
                    QuestService.Instance.SendQuestAccept(dlg.quest);
                }
                else if (dlg.quest.Info.Status == QuestStatus.Complated)
                {//同上，发送任务完成的请求
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
                }
            }
            else if(result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }

            this.onNpcClose(curNpcId);
        }

        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            this.npcQuests.Clear();
            Quest result;
            if (this.AllQuests.ContainsKey(quest.QuestId))
            {
                //若服务器传来的是已经在任务表中老任务，则更新其任务状态
                this.AllQuests[quest.QuestId].Info = quest;
                result = this.AllQuests[quest.QuestId];
            }
            else
            {
                //若服务器传来的是新任务，则创建该任务并放置到任务列表中
                result = new Quest(quest);
                this.AllQuests[quest.QuestId] = result;
            }

            //更新Npc身上的任务列表

            CheckAvailableQuests();

            foreach (var kv in this.AllQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
            //----

            //触发任务状态变化的事件

            if (onQuestStatusChanged != null)
            {
                onQuestStatusChanged(result);
            }


            return result;
        }


        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public  void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }


    }




}
