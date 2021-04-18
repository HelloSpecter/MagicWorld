using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Common.Data;

public class UIIconItem : MonoBehaviour {

    public Image MainImage;
    public Image SecondImage;
    public Text MainText;
    
    
    // Use this for initialization
	void Start () {
		
	}
	
    //替换当前Icon及其数量
    public void SetMainIcon(string iconName,string text)
    {

        this.MainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.MainText.text = text;

    }

    //重载这个方法，用于传入ID的情况
    public void SetMainIcon(int iconId,int count)
    {
        ItemDefine item = null;
        DataManager.Instance.Items.TryGetValue(iconId, out item);
        if (item==null)
        {
            Debug.LogWarning("读取物品ID错误，请检查！");
            return;
        }
        string iconName = item.Icon;
        string text = count.ToString();
        this.SetMainIcon(iconName, text);

    }


	// Update is called once per frame
	void Update () {
		
	}
}
