using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Managers;

public class UIMain : MonoSingleton<UIMain> {
    public Text avatarName;
    public Text avatarLv;
    public UITeam TeamWindow;
	// Use this for initialization
	protected override void OnStart () {
        
        this.UpdateAvatar();


	}
	
	// Update is called once per frame
	void Update () {



    }

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avatarLv.text = User.Instance.CurrentCharacter.Level.ToString();

    }


    public void Back2CharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");


        Services.UserService.Instance.SendGameLeave();

        //UIWorldElementManager.Instance.Destroy();
        //GameObjectManager.Instance.Destroy();
        //MainPlayerCamera.Instance.Destroy();

        //CharacterManager.Instance.Clear();

    }

    public void OnClickTest()
    {
        UIManager.Instance.Show<UITest>();
    }

    public void OnClickBag()
    {

        UIManager.Instance.Show<UIBag>();

    }

    public void OnClickCharEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriends()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {

    }

    public void OnClickSetting()
    {

    }

    public void OnClickSkill()
    {

    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

}
