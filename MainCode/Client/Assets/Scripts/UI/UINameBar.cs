using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using Models;

[ExecuteInEditMode]
public class UINameBar : MonoBehaviour {
    public Image avatar;
    public Text characterName;
    //public Text AvatarName;
    public  Character Character;

	void Start () {
        if (Character!=null)
        {
            if (Character.Info.Type == SkillBridge.Message.CharacterType.Monster)
            {
                this.avatar.gameObject.SetActive(false);
            }
            else
            {
                this.avatar.gameObject.SetActive(true);
            }
        }
	}
	


	void Update () {

        this.UpdateInfo();
        //this.transform.forward = Camera.main.transform.forward;

    }

    void UpdateInfo()
    {
        if (this.Character != null)
        {
            string Name = "Lv." + this.Character.Info.Level + " " + this.Character.Info.Name;

            if (string.IsNullOrEmpty(this.characterName.text)|| this.characterName.text != Name)
            {
                this.characterName.text = Name;
            } 

         }
    }

}
