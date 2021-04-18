//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TabView : MonoBehaviour {
//    public TabButton[] TabButtons;
//    public GameObject[] Pages;
//    int selectIndex = -1;

//	// Use this for initialization
//	IEnumerator Start () {

//        for (int i = 0; i < Pages.Length; i++)
//        {
//            TabButtons[i].tabView = this;
//            TabButtons[i].TabIndex = i;

//        }
        
//        yield return new WaitForEndOfFrame();
//        OnSelect(0);
//	}
	
//	// Update is called once per frame
	

//    public  void OnSelect(int tabIndex)
//    {
//        if (this.selectIndex!=tabIndex)
//        {

//            selectIndex = tabIndex;
//            for (int i = 0; i < TabButtons.Length; i++)
//            {
//                TabButtons[i].Selected(i == selectIndex);
//                Pages[i].SetActive(i == selectIndex);
//            }
            

//        }

//    }

//    //public  void pageSelect(int index)
//    //{

//    //    for (int i = 0; i < TabButtons.Length; i++)
//    //    {
            

//    //    }

//    //}


//    void Update () {
		
//	}

//}
