using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Common.Data;
using Managers;

public class TestManager:Singleton<TestManager> {

    // Use this for initialization

     

    public void Init()
    {
        
        NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeInstance, this.OnInvokeInstance);
        NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, this.OnInvokeShop);




    }

    bool OnInvokeInstance(NpcDefine npcDefine)
    {
        MessageBox.Show("正在和[" + npcDefine.Name + "]对话......");


        return true;
    }

    bool OnInvokeShop(NpcDefine npcDefine)
    {
        UITest uiTest=UIManager.Instance.Show<UITest>();
        

        return true;
    }
}
