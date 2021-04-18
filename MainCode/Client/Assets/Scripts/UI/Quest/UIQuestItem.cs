using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;

public class UIQuestItem : ListView.ListViewItem {

    public Text title;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    public Quest quest;
    bool isEquiped = false;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
        {
            this.title.text = this.quest.Define.Name;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
