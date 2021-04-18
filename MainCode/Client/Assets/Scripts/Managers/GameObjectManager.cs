using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Entities;
using Managers;
using Object = UnityEngine.Object;
using Controller;
using Models;

namespace Managers
{
 class GameObjectManager:MonoSingleton<GameObjectManager>
    {
        public  Dictionary<int, GameObject> Chars = new Dictionary<int, GameObject>();

        protected override void OnStart()
        {
            

            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharEnter += this.OnCharEnter;
            CharacterManager.Instance.OnCharLeave += this.OnCharLeave;
            Debug.LogError("CharacterManager.Instance.OnCharEnter 已注册 ");
        }
        
        private void OnDestroy()
        {
            CharacterManager.Instance.OnCharEnter -= this.OnCharEnter;
            CharacterManager.Instance.OnCharLeave -= this.OnCharLeave;


        }
        
        IEnumerator InitGameObjects()
        {
            Debug.LogError("IEnumerator InitGameObjects() Start!");

            foreach (var item in CharacterManager.Instance.Chars.Values)
            {
                if (item!=null)
                {
                CreateCharObject(item);
                yield return null;

                }
            }


        }

        void OnCharEnter(Character cha)
        {


            CreateCharObject(cha);


        }

        void OnCharLeave(Character cha)
        {
            if (!Chars.ContainsKey(cha.entityId))
            {

                return;

            }
            //做判空，Chars中的游戏对象可能在其他地方被销毁，此处避免空异常
            if (Chars[cha.entityId]!=null)
            {
                Destroy(Chars[cha.entityId]);
                this.Chars.Remove(cha.entityId);

            }

        }

        void CreateCharObject(Character NewChar)
        {
            if (!this.Chars.ContainsKey(NewChar.entityId) || this.Chars[NewChar.entityId] == null)
            //if (!this.Chars.ContainsKey(NewChar.entityId))
            {
                //获取新角色所对应的模型地址（模型数据源）
                Object obj = Resloader.Load<Object>(NewChar.Define.Resource);
                Debug.Log("成功读取模型地址：" +"Tid:["+NewChar.Info.configId+"]--" +NewChar.Define.Resource);
                if (obj==null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", NewChar.Define.TID, NewChar.Define.Resource);
                    return;
                }

                GameObject go = (GameObject)Instantiate(obj,this.transform);
                go.name = "Char_" + NewChar.Id+ "_" + NewChar.Info.Class + "_" + NewChar.Name;


                //添加新对象至 <对象管理器> 字典
                Chars[NewChar.entityId] = go;

                //为角色增加血条UI
                UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, NewChar);

                this.InitGameObject(Chars[NewChar.entityId], NewChar);

            }




        }

        public  void InitGameObject(GameObject go,Character NewChar)
        {

            //将服务器的角色当前状态赋给新建对象go
            go.transform.position = GameObjectTool.LogicToWorld(NewChar.position);
            go.transform.forward = GameObjectTool.LogicToWorld(NewChar.direction);


            Debug.Log("新对象已创建：" + NewChar.Name);

            PlayerInputController pc = go.GetComponent<PlayerInputController>();
            EntityController ec = go.GetComponent<EntityController>();

            if (ec != null)
            {
                ec.Entity = NewChar;
                ec.IsPlayer = NewChar.IsCurrentPlayer;

                Debug.Log("EntityController 初始化完毕");

            }

            if (pc != null)
            {
                if (NewChar.IsCurrentPlayer)//判断是否为当前开始游戏的玩家角色
                {
                    //调整镜头对准角色
                    MainPlayerCamera.Instance.Player = go;
                    Debug.Log("Camera Ready!");

                    //赋值给Models/当前游戏对象
                    User.Instance.CurCharObject = go;


                    pc.enabled = true;
                    pc.character = NewChar;
                    pc.entityController = ec;

                    Debug.Log("PlayerInputController 初始化完毕");


                }

                else
                {
                    pc.enabled = false;
                }
            }
        }

        public void Destroy()
        {
            Destroy(this.gameObject);
        }

    }
}
