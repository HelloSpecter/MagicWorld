using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {
        public NCharacterInfo Info;

        public Common.Data.CharacterDefine Define;

        public int Id
        {
            get { return this.Info.Id; }
        }

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return this.Info.Type == CharacterType.Player; }
        }

        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer)
                {
                    return false;
                }
                return this.Info.Id == Models.User.Instance.CurrentCharacter.Id;
            }
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.configId];
            ////
            ////this.entityId = info.Id;//暂时以NCharInfo.ID作为entityID
            ////

            ////修正:entityId、EntityData.Id、NEntity-Id三者值统一
            //this.entityId = this.EntityData.Id;
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }
    }
}
