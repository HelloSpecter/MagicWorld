using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : UIWindow {

     

    // Use this for initialization
    void Start () {
        base.OnClose += this.OnClick;

    }
	

     void OnClick(UIWindow sender, WindowResult result)
    {
        MessageBox.Show("Name is :[" + sender.name + "]");
    }

    //public void SetTitle(string name)
    //{
       
    //}

    // Update is called once per frame
    void Update () {
		
	}
}
