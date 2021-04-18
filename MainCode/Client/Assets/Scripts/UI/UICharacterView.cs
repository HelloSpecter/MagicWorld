using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;

public class UICharacterView : MonoBehaviour {
    public GameObject[] Characters;//0-战  1-法  2-猎

    public GameObject Panel_Create;
    public GameObject Panel_Select;

    private int currentCharacter=-1;
    public int CurrentCharacter {
        get
        {
            return CurrentCharacter;
        }
        set
        {
            currentCharacter = value;
            UpdataCharacter();//每次设置当前角色对象（索引时），重新加载视图
        }
    }


    public void UpdataCharacter()
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            Characters[i].SetActive(i == this.currentCharacter);
        }
    }


	// Use this for initialization
	void Start () {
        UpdataCharacter();

    }
	
	// Update is called once per frame
	void Update () {
        ////根据当前选择的逻辑角色CurChar，创建角色模型视图
        //if (Models.User.Instance.CurrentCharacter!=default(NCharacterInfo)&& !Panel_Create.activeSelf&& Panel_Select.activeSelf)
        //{
        //    CurrentCharacter = (int)Models.User.Instance.CurrentCharacter.Class - 1;
        //}
	}
}
