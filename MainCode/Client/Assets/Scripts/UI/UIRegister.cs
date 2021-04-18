using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Network;
using SkillBridge.Message;
using SkillBridge;
using Services;

public class UIRegister : MonoBehaviour {

    // Use this for initialization
    public GameObject UILogin;
    public InputField Username;
    public InputField Password;
    public InputField Confirm;
    public Sprite EnterIcon;
    public Button Enter;
    bool isEnter = false;
    public Canvas UICanvas;
    public Sprite RegisterIcon;

    private void OnEnable()
    {
        Start();
    }
    void Start () {
        UserService.Instance.OnRegister = this.OnRegister;
        //MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnRegister);
        Enter.image.sprite = RegisterIcon;
        Enter.image.SetNativeSize();
        Username.text = null;
        Password.text = null;
        Confirm.text = null;
        isEnter = false;
    }
    void OnRegister(Result result, string errmsg)
    {
        if (result==Result.Success)
        {
            MessageBox.Show("注册结果:" + result.ToString() + "-" + errmsg);
            Enter.image.sprite = EnterIcon;
            Enter.image.SetNativeSize();
            isEnter = true;
            
        }
        else
        {
            MessageBox.Show("注册结果:" + result.ToString() + "-" + errmsg);
            Username.text = null;
            Password.text = null;
            Confirm.text = null;
        }
    }
    //void OnRegister(object sender, UserRegisterResponse response )
    //{
    //    MessageBox.Show(response.Result.ToString() + response.Errormsg);
    //    Debug.LogFormat("OnUserRegister:{0} {1}", response.Result, response.Errormsg);
    //}

    // Update is called once per frame
    void Update () {
    }
    /// <summary>
    /// 按下进入游戏后，连接服务器，并尝试进行新用户注册
    /// </summary>
    public void OnClickEnter()
    {
        if (!isEnter)//注册模式
        {
            if (string.IsNullOrEmpty(Username.text))
            {
                MessageBox.Show("请输入账号！");
                return;
            }
            if (string.IsNullOrEmpty(Password.text))
            {
                MessageBox.Show("请输入密码！");
                return;
            }
            if (string.IsNullOrEmpty(Confirm.text))
            {
                MessageBox.Show("请输入确认密码！");
                return;
            }
            if (Confirm.text != Password.text)
            {
                MessageBox.Show("两次密码输入不一致，请重新输入");
                Confirm.text = null;
                Password.text = null;
                return;
            }
            Services.UserService.Instance.Init();
            Services.UserService.Instance.SendRegister(Username.text, Password.text);
            Debug.LogError("注册信息已发送！");
            //MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnRegister);
        }
        else//登录模式
        {
            //UICanvas.GetComponent<LoadingManager>().needload = true;
            UserService.Instance.SendLogIn(Username.text, Password.text);
            Debug.LogError("登录信息已发送！");
            //SceneManager.Instance.LoadScene("CharSelect");




        }

    }
    /// <summary>
    /// 按下返回键，返回登录页面
    /// </summary>
    public void OnClickBack()
    {
        this.UILogin.SetActive(true);
        gameObject.SetActive(false);

    }







}
