using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Common.Data;
using Models;
using SkillBridge.Message;
using Managers;
using System;

public class NpcController : MonoBehaviour
{
    public int NpcId;
    SkinnedMeshRenderer renderer;
    Animator anim;
    Color origin;
    bool inInteractive = false;
    NpcDefine npcDefine = null;

    NpcQuestStatus questStatus;

    //绑定获取SkinnedMeshRenderer、Animator、Color（origin）的值
    void Start()
    {
        renderer =  this.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.GetComponent<Animator>();
        origin = this.renderer.sharedMaterial.color;

        DataManager.Instance.Npcs.TryGetValue(NpcId, out npcDefine);



        StartCoroutine(Action());

        this.RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += this.OnQuestStatusChanged;
        QuestManager.Instance.onNpcClose += this.OnNpcClose;
    }

    void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }

    void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.NpcId);
        //根据Npc当前任务状态，在其头顶上方显示其任务状态图标
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance !=null)
        {
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 随机（Relax）动作触发方法
    /// </summary>
    /// <returns></returns>
    IEnumerator Action()
    {
        while (true)
        {
                if (inInteractive)
                {
                    yield return new WaitForSeconds(6);
                }
                else
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(2,20));
                }
                anim.SetTrigger("Relax");
        }
    }

    /// <summary>
    /// 执行调用方法（通过协程）
    /// </summary>
    void Interactive()
    {
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }


    }

    /// <summary>
    /// 根据NpcDefine调用其NpcDefine.Function（NpcManager中）所对应的方法（由其他系统注册）
    /// </summary>
    /// <returns></returns>
    IEnumerator DoInteractive()
    {
        
        yield return FaceToPlayer();

        if (NpcManager.Instance.Interactive(NpcId))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3);
        inInteractive = false;




    }

    Vector3 oldDir;
    /// <summary>
    /// Npc朝向玩家的协程方法
    /// </summary>
    /// <returns></returns>
    IEnumerator FaceToPlayer()
    {
        //保存之前的位置
        oldDir = this.transform.position;

        Vector3 faceToPlayer = User.Instance.CurCharObject.transform.position - this.transform.position;
        while (Vector3.Angle(this.transform.forward, faceToPlayer) > 5)
        {
            this.transform.forward = Vector3.Lerp(this.transform.forward, faceToPlayer, Time.deltaTime * 1.0f);
        }

        yield return null;
    }

    void  OnNpcClose(int npcId)
    {
        if (this.NpcId == npcId)
        {
            StartCoroutine(FaceReturn());
        }
    }

    /// <summary>
    /// 玩家与Npc交互结束后，将其方向转回原位置
    /// </summary>
    /// <returns></returns>
    IEnumerator FaceReturn()
    {
        if (oldDir != default(Vector3))
        {

        this.transform.forward = Vector3.Lerp(this.transform.forward, oldDir, Time.deltaTime * 1.0f);

        }

        yield return null;
    }

    #region 当鼠标经过Npc时，高亮对象
    //当鼠标经过Npc时，高亮对象

    private void OnMouseDown()
    {
        this.Interactive();

    }


    private void OnMouseOver()
    {
        HighLight(true);
    }


    private void OnMouseEnter()
    {
        HighLight(true);

    }


    private void OnMouseExit()
    {
        HighLight(false);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSelect"></param>
    void HighLight(bool isSelect)
    {
        if (isSelect)
        {
            if (renderer.sharedMaterial.color != Color.white)
            {
                renderer.sharedMaterial.color = Color.white;
            }
        }
        else
        {
            if (renderer.sharedMaterial.color != origin)
            {
                renderer.sharedMaterial.color = origin;

            }
        }


    }

    //---
    #endregion

    
}
