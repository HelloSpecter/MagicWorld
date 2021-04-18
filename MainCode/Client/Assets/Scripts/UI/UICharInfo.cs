using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UICharInfo : MonoBehaviour {
    public Text CharName;
    public Text CharLv;
    public Text CharLoc;
    public Text CharClass;




    public void SetValue(NCharacterInfo characterInfo)
    {
        CharName.text = characterInfo.Name;
        CharLv.text = "Lv:" +characterInfo.Level.ToString();
        CharLoc.text = characterInfo.mapId.ToString();
        CharClass.text = characterInfo.Class.ToString();



    }
	// Use this for initialization
	void Start () {
         

    }
	
	// Update is called once per frame
	void Update () {

	}
}
