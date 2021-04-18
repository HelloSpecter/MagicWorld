using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Services;

class UIGuildPopCreate : UIWindow
{
    public InputField inputName;
    public InputField inputNotice;

    private void Start()
    {   
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }

    private void OnDestroy()
    {
        
        GuildService.Instance.OnGuildCreateResult = null;
    }

    //重写Yes点击：为避免创建产生报错，点击创建时窗口暂时不关闭
    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (inputName.text.Length < 4 || inputName.text.Length > 10)
        {
            MessageBox.Show("公会名称为4-10个字符", "错误", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
            return;
        }
        if (inputNotice.text.Length < 3 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("公会宣言需为3-50个字符", "错误", MessageBoxType.Error);
            return;
        }
        
        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);

    }

    void OnGuildCreated(bool result)
    {
        //当服务器返回角色创建成功时，才关闭该UI窗口
        if (result)
        {
            this.Close(WindowResult.Yes);
        }
    }




}
