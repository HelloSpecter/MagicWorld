using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Common;
using System;
using Managers;

public class NpcManager :Singleton<NpcManager>
{ 

    public delegate bool Action(NpcDefine npcDefine);
    public Dictionary<NpcFunction, Action> NpcResponse = new Dictionary<NpcFunction, Action>();

    /// <summary>
    /// 从DataManager中获取当前Id所对应的NPC的定义文件
    /// </summary>
    /// <param name="npcId"></param>
    /// <returns></returns>
    public NpcDefine GetNpcDefine(int npcId)
    {
        NpcDefine npcDefine = null;
        DataManager.Instance.Npcs.TryGetValue(npcId, out npcDefine);
        return npcDefine;
    }


    /// <summary>
    /// 注册Npc响应事件
    /// </summary>
    /// <param name="npcFunction"></param>
    /// <param name="action"></param>
    public void RegisterNpcEvent(NpcFunction npcFunction,Action action)
    {
        if (!NpcResponse.ContainsKey(npcFunction))
        { 
            NpcResponse[npcFunction] = action;
        }
        else
        {
            NpcResponse[npcFunction] += action;
        }

    }


    public bool Interactive(int npcId)
    {
        if (!DataManager.Instance.Npcs.ContainsKey(npcId))
        {
            Debug.LogError("当前NPC:[" + npcId + "],不在配置表内！");
            return false;
        }

        return Interactive(DataManager.Instance.Npcs[npcId]);


    }

    public bool Interactive(NpcDefine npcDefine)
    {
        if (DoTaskInteractive(npcDefine))
        {
            return true;
        }
        else if (npcDefine.Type == NpcType.Functional)
        {
            return DoFunctionInteractive(npcDefine);
        }
        return false;
    }




     bool  DoNoneInteractive(NpcDefine npcDefine)
    {
        Debug.Log("---> DoNoneInteractive()");

        return false;

    }

    bool  DoTaskInteractive(NpcDefine npcDefine)
    {

        var status = QuestManager.Instance.GetQuestStatusByNpc(npcDefine.ID);
        if (status == NpcQuestStatus.None)
        {
            return false;
        }
        //显示NPC对话框
        return QuestManager.Instance.OpenNpcQuest(npcDefine.ID);

    }


     bool  DoFunctionInteractive(NpcDefine npcDefine)
    {
        Debug.Log("---> DoFunctionInteractive(NpcDefine)");

        //二次检验如果类型不对，返回False
        if (npcDefine.Type != NpcType.Functional)
        {
            return false;
        }
        //如果NpcResponse中不包含这种Function类型，返回False
        if (!NpcResponse.ContainsKey(npcDefine.Function))
        {
            return false;


        }

        return  NpcResponse[npcDefine.Function](npcDefine);
        



    }




}
