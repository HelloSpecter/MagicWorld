using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SkillBridge.Message;
using ProtoBuf;
using Services;
using Managers;


public class LoadingManager : MonoBehaviour {

    public GameObject UIRegister;
    public GameObject UILogIn;
    public GameObject UILoading;
    public GameObject UITips;
    public  Slider m_slider;
    bool isloading = false;
    public  bool needload = false;
    // Use this for initialization


    IEnumerator Start ()
    {
        Debug.Log("Start LoadingManager");
        //日志写出
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");


        UITips.SetActive(true);
        UIRegister.SetActive(false);
        UILogIn.SetActive(false) ;
        UILoading.SetActive(false) ;
        yield return new WaitForSeconds(1.0f);
        UILoading.SetActive(true);
        isloading = true;
        UITips.GetComponent<Animation>().Play();

        yield return DataManager.Instance.LoadData();//异步加载地图、角色定义、传送点、刷怪点等字典信息

        //TestManager.Instance.Init();//NPC系统测试使用
        ShopManager.Instance.Init();//初始化商店管理器(注册Npc响应事件)

        Services.UserService.Instance.Init();//初始化UserService服务
        Services.MapService.Instance.Init();//初始化地图MapService服务
        Services.ItemService.Instance.Init();//初始化ItemService服务
        Services.StatusService.Instance.Init();//初始化StatusService服务
        Services.QuestService.Instance.Init();//初始化QuestService服务
        Services.FriendService.Instance.Init();//初始化FriendService服务
        Services.TeamService.Instance.Init();//初始化TeamService服务
        //Services.GuildService.Instance.Init();//初始化GuildService服务


        UILogIn.SetActive(true);
        isloading = false;
        //SceneManager.Instance.LoadScene("CharSelect");
        yield return null;



    }
    // Update is called once per frame
    void Update ()
    {
        if (UITips.GetComponent<Image>().color.a < 0.1f) UITips.SetActive(false);
        if (UILoading.GetComponent<Image>().color.a < 0.1f) UILoading.SetActive(false);
        if (UILogIn.GetComponent<Image>().color.a < 0.1f) UILogIn.SetActive(false);
        if (UIRegister.GetComponent<Image>().color.a < 0.1f) UIRegister.SetActive(false);
        if (m_slider.value > 99f&&isloading)
        {
            UILogIn.SetActive(true);
            UILoading.SetActive(false);
        }
        if (needload)
        {
            if (!isloading)
            {
                isloading = true;
                UILoading.SetActive(true);
                needload = false;
                isloading = false;
            }
        }
    }
}
