using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public  Dictionary<int, Character> Chars = new Dictionary<int, Character>();
        public UnityEngine.Events.UnityAction<Character> OnCharEnter;
        public UnityEngine.Events.UnityAction<Character> OnCharLeave;



        public void Clear()
        {
            //（Qu）使用 数组keys，而不是直接Chars的Keys，来做foreach，是不是出于性能角度考虑
            int[] keys = this.Chars.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveChar(key);


            }


            this.Chars.Clear();



        }

        public void AddChar(NCharacterInfo ChaInfo)
        {
            //当前添加的逻辑对象不在列表内
            if (!Chars.ContainsKey(ChaInfo.Entity.Id))
            {

                Character character = new Character(ChaInfo);

                Chars[ChaInfo.entityId] = character;

                //添加EntityManager里的Entity
                EntityManager.Instance.AddEntity(character);

                Debug.Log("Add new Char to CharManager-Chars");

                Debug.Log("OnCharEnter is " + (OnCharEnter != null));
            }

            Debug.LogError("CharacterManager Chars Num is" + Chars.Count);

            if (OnCharEnter != null)
            {

                Debug.LogError("OnCharEnter(Chars[ChaInfo.Id])......");

                OnCharEnter(Chars[ChaInfo.Entity.Id]);

            }


        }

        public void RemoveChar(int entityId)
        {
            Debug.Log("RemoveCharacter:[" + entityId + "]");
            if (this.Chars.ContainsKey(entityId))
            {

                //删除EntityManager里的Entity
                EntityManager.Instance.RemoveEntity(this.Chars[entityId].Info.Entity);

                if (OnCharLeave!=null)
                {

                    OnCharLeave(this.Chars[entityId]);

                }

                this.Chars.Remove(entityId);

            }
        }



    }
}
