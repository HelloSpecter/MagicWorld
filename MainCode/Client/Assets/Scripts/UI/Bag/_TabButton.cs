//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class TabButton : MonoBehaviour {

//    public Sprite ActiveImage;//在编辑器内设置
//    private Sprite normaleImage;
//    public  TabView tabView;
//    public int TabIndex;//在编辑器内设置
//    bool selected;
//    Image tabImage;

//	void Start () {

//        tabImage = this.GetComponent<Image>();
//        normaleImage = tabImage.sprite;
//        this.GetComponent<Button>().onClick.AddListener(OnClick);
//	}
	
//    void OnClick()
//    {
//        this.tabView.OnSelect(this.TabIndex);


//    }

//    public void Selected(bool selected)
//    {
//        tabImage.overrideSprite = selected ? ActiveImage : normaleImage;
//    }




//	// Update is called once per frame
//	void Update () {
		


//	}
//}
