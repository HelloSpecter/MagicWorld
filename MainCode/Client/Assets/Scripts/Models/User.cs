using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using Common.Data;
using Managers;

namespace Models
{

    public class User : Singleton<User>
    {

        private NUserInfo userInfo = null;
        public NUserInfo Info
        {
            get
            {
                return userInfo;
            }
        }
        public void SetupUserInfo(NUserInfo userInfo)
        {
            this.userInfo = userInfo;
        }

        public MapDefine CurMapDefine { get; set; }

        public GameObject CurCharObject { get; set; }

        public NCharacterInfo CurrentCharacter { get; set; }

        public int CurIdx = -1;

        public NTeamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
            BagManager.Instance.setGold();
        }


    }
}
