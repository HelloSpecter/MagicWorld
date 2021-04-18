using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Common;
using System;
using Managers;
using SkillBridge.Message;

public class FriendManager : Singleton<FriendManager>
{
    //所有好友
    public List<NFriendInfo> allFriends;

    public void Init(List<NFriendInfo> friends)
    {
        this.allFriends = friends;
    }

}
