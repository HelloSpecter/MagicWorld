using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIWindow : MonoBehaviour
{

    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;

    public virtual Type Type
    {
        get
        {
            return this.GetType();
        }
    }


    public enum WindowResult
    {

        None = 0,
        Yes,
        No

    }

    /// <summary>
    /// 基类关闭窗口的方法(WindowResult result = WindowResult.None)这是重载了1个空（默认）参数的方法
    /// </summary>
    /// <param name="result"></param>
    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(this.Type);
        if (this.OnClose != null)
        {
            //触发关闭UI的事件
            this.OnClose(this, result);
        }
        //触发1次后注销该方法
        this.OnClose = null;

    }

    public virtual void OnCloseClick()
    {
        this.Close();
    }

    public virtual void OnYesClick()
    {

        this.Close(WindowResult.Yes);
    }

    public virtual void OnNoClick()
    {

        this.Close(WindowResult.No);
    }
}

    


    //private void OnMouseDown()
    //{
    //    Debug.LogFormat(this.name + "Clicked");
    //}






