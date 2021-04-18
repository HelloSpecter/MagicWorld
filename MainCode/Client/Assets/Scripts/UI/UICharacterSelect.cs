using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Common;
using UnityEngine.UI;
using SkillBridge.Message;
using log4net;
using Network;
using System;
using UnityEngine.Events;
using Models;

public class UICharacterSelect : MonoBehaviour
{

    // Use this for initialization
    public GameObject Panel_Create;
    public GameObject Panel_Select;
    public GameObject[] Titles;
    public Text[] Descb;
    private int curCharClass = 0;
    public UICharacterView view;
    public GameObject uiChar;
    public Transform uiList;
    public InputField CharName;

    private List<GameObject> uiChars = new List<GameObject>();
    
    int selectCharidx = -1;



    #region CharacterSelect


    public void InitCharacterSelect(bool init)
    {
        Panel_Select.SetActive(true);
        Panel_Create.SetActive(false);
        if (init)//删除旧表，刷新角色列表
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
            //
            foreach (var old in uiChars)
            {
                Debug.LogError(uiChars.IndexOf(old) + "___" + old.name);
            }
            //
            Refresh();

            if (uiChars.Count > 0)
            {
                //如果上次角色选择是空的（第一次进入选择界面）
                if ( User.Instance.CurIdx< 0)
                {
                    //默认(按下)选择第1个角色
                    OnSelectCharacter(0, uiChars[0]);

                }
                else
                {
                    OnSelectCharacter(User.Instance.CurIdx, uiChars[User.Instance.CurIdx]);
                }
            }

            //if (uiChars.Count>0 && Models.User.Instance.CurrentCharacter==default(NCharacterInfo))//如果含有至少1个角色,且CurrentCharacter没有被赋值，默认选中并显示第1个角色
            //{

            //    //Button btn = uiChars[0].GetComponent<Button>();
            //    OnSelectCharacter(0,uiChars[0]);
            //    //btn.onClick.Invoke();
            //    return;
            //}
            //if (uiChars != null && Models.User.Instance.CurrentCharacter != default(NCharacterInfo))
            //{
            //    for (int i = 0; i < Models.User.Instance.Info.Player.Characters.Count; i++)
            //    {
            //        if (Models.User.Instance.CurrentCharacter.Id== Models.User.Instance.Info.Player.Characters[i].Id)
            //        {
            //            Button btn = uiChars[i].GetComponent<Button>();
            //            btn.onClick.Invoke();
            //            return;
            //        }
            //    }
            //    return;
            //}

        }

    }


    /// <summary>
    /// 刷新选择列表中每个对象，取消“被选择时图片”的效果
    /// </summary>
    void ImgRefresh()
    {
        foreach (var item in uiChars)
        {
            item.transform.Find("SelectedImg").gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 更新CharList
    /// </summary>
    public void Refresh()
    {
        if (Models.User.Instance.Info != null)
        {


            if (Models.User.Instance.Info.Player.Characters != null)
            {



                for (int i = 0; i < Models.User.Instance.Info.Player.Characters.Count; i++)

                {

                    int c_idx;

                    GameObject go = Instantiate(uiChar, uiList);

                    UICharInfo uiCharInfo = go.GetComponent<UICharInfo>();

                    uiCharInfo.SetValue(Models.User.Instance.Info.Player.Characters[i]);

                    c_idx = i;

                    Button button = go.GetComponent<Button>();

                    button.onClick.AddListener(() => { OnSelectCharacter(c_idx, go); });


                    uiChars.Add(go);

                    go.SetActive(true);





                }
            }
        }
    }


    /// <summary>
    /// 选中某个角色时的逻辑
    /// </summary>
    /// <param name="idx"></param>
    void OnSelectCharacter(int idx, GameObject go)
    {
        ImgRefresh();
        GameObject selectedImg = go.transform.Find("SelectedImg").gameObject;
        selectedImg.SetActive(true);

        //传值给View，刷新模型视图
        selectCharidx = idx;

        //传值给User，刷新当前选择的逻辑角色(CurChar)
        var selectChar = Models.User.Instance.Info.Player.Characters[selectCharidx];
        //Models.User.Instance.CurrentCharacter = selectChar;
        view.CurrentCharacter = (int)(selectChar.Class) - 1;
        Debug.LogErrorFormat("Character Selected:Name:{0},Lv:{1},Class:{2}", selectChar.Name, selectChar.Level, selectChar.Class);

    }


    #endregion


    #region CharacterCreate

    public void InitCharacterCreate()
    {

        Panel_Create.SetActive(true);
        Panel_Select.SetActive(false);
        curCharClass = 0;
        view.CurrentCharacter = curCharClass;

        foreach (var item in Titles)
        {
            item.SetActive(false);
        }
        Titles[curCharClass].SetActive(true);

        Descb[curCharClass].text = DataManager.Instance.Characters[curCharClass + 1].Description;

    }
    public void OnClickCharClass(int charclass)
    {

        curCharClass = charclass;
        view.CurrentCharacter = curCharClass;
        for (int i = 0; i < Titles.Length; i++)
        {
            Titles[i].SetActive(i == charclass);
        }
        Descb[charclass].text = DataManager.Instance.Characters[charclass + 1].Description;

    }

    public void OnCreateChar()
    {
        if (string.IsNullOrEmpty(CharName.text))
        {
            MessageBox.Show("请输入角色名称！");
            return;
        }

        //这里加入else if 增加命名规则筛选

        else
        {
            UserService.Instance.SendCharCreate(CharName.text, curCharClass);


        }
    }


    private void OnCharCreate(Result result, string errmsg)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show("未能成功创建角色：" + errmsg, "错误", MessageBoxType.Error);
            CharName.text = null;
        }



    }



    #endregion


    #region CharacterEnterGame

    public void OnPlayClick()
    {
        User.Instance.CurIdx = selectCharidx;
        UserService.Instance.SendCharEnter(Models.User.Instance.Info.Player.Characters[selectCharidx]);




    }




    //void OnCharEnter(int mapId)
    //{

    //    string SceneName = DataManager.Instance.Maps[mapId].Resource;
    //    SceneManager.Instance.LoadScene(SceneName);



    //}




    #endregion







    void Start()
    {
        InitCharacterSelect(true);
        Services.UserService.Instance.OnCharCreate = this.OnCharCreate;
        //Services.UserService.Instance.OnCharEnter = this.OnCharEnter;

        //不登录的测试方法:
        //DataManager.Instance.Load();
        //end

    }


    // Update is called once per frame
    void Update()
    {

    }
}
