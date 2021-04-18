using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;
public class UILogin : MonoBehaviour {

    public GameObject UIResgister;
    public InputField Username;
    public InputField Password;
    public Canvas UICanvas;
    // Use this for initialization
    public void OnClickRegister()
    {
        this.UIResgister.SetActive(true);
        gameObject.SetActive(false);
    }
    public  void OnClickLogin()
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
        UserService.Instance.SendLogIn(Username.text, Password.text);
        Debug.LogError("登录信息已发送！");
    }

    void Start () {
		UserService.Instance.OnLogIn=this.OnLogIn;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnLogIn(Result result,string errmsg)
    {
        if (result == Result.Success)
        {
            MessageBox.Show("登录成功,准备角色选择" + errmsg, "提示", MessageBoxType.Information);
            //UICanvas.GetComponent<LoadingManager>().needload = true;
            SceneManager.Instance.LoadScene("CharSelect");
            //SceneManager.Instance.LoadScene("abc");
        }
        else
        {
            if (errmsg=="密码错误")
            {
                Password.text = null;
                MessageBox.Show("登陆失败:"+ errmsg);

            }
            if (errmsg=="用户名不存在")
            {
                Username.text = null;
                Password.text = null;
                MessageBox.Show(errmsg+",请重新输入或注册新用户！");


            }
            
        }
    }
}
