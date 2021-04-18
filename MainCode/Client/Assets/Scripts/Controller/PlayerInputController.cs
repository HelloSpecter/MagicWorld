using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using Entities;
using UnityEngine;
using Services;

namespace Controller
{
    public  class PlayerInputController:MonoBehaviour
    {
        public Rigidbody rig;
        CharacterState state;
        public  Character character;
        public EntityController entityController;
        public float rotateSpeed = 2.0f;
        public int Speed;
        public float turnAngle = 10;





        private void Start()
        {
            state = CharacterState.Idle;

            //if (this.character == null)
            //{
            //    DataManager.Instance.Load();
            //    NCharacterInfo cinfo = new NCharacterInfo();
            //    cinfo.Id = 1;
            //    cinfo.Name = "Test";
            //    cinfo.Tid = 1;
            //    cinfo.Entity = new NEntity();
            //    cinfo.Entity.Position = new NVector3();
            //    cinfo.Entity.Direction = new NVector3();
            //    cinfo.Entity.Direction.X = 0;
            //    cinfo.Entity.Direction.Y = 100;
            //    cinfo.Entity.Direction.Z = 0;
            //    this.character = new Character(cinfo);

            //    if (entityController != null) entityController.Entity = this.character;
            //}

        }

        private void FixedUpdate()
        {
            if (character==null)
            {
                return;
            }


            //角色意外坠落（切换地图卡顿）时，将其拉回地面
            if (this.transform.position.y<-2f)
            {
                this.transform.position = new Vector3(this.transform.position.x, 10, this.transform.position.z);
            }


            float v = Input.GetAxis("Vertical");

            #region 前后移动
            //向前移动
            if (v>0.01)
            {
                if (state!=CharacterState.Move)
                {
                    state = CharacterState.Move;
                    this.character.MoveForward();
                    this.SendEntityEvent(EntityEvent.MoveFwd);



                }

                this.rig.velocity = this.rig.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
                //this.rig.velocity = this.rig.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed ) / 100f;

            }
            //向后移动
            else if (v<-0.01)
            {
                if (state != CharacterState.Move)
                {
                    state = CharacterState.Move;
                    this.character.MoveBack();
                    this.SendEntityEvent(EntityEvent.MoveBack);



                }

                this.rig.velocity = this.rig.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;



            }

            //未移动
            else
            {
                if (state!=CharacterState.Idle)
                {
                    state = CharacterState.Idle;
                    this.rig.velocity = Vector3.zero;
                    this.character.Stop();
                    this.SendEntityEvent(EntityEvent.Idle);



                }




            }

            #endregion

            float h = Input.GetAxis("Horizontal");
            #region 左右旋转
            if (h<-0.1||h>0.1)
            {
                //使角色实体旋转
                this.transform.Rotate(0, h * rotateSpeed, 0);

                //以下代码为完成 逻辑同步、刚体同步
                Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
                Quaternion rot = new Quaternion();
                rot.SetFromToRotation(dir, this.transform.forward);

                //当角色旋转角度超过turnAngle时，更新角色逻辑（在Entity里会将逻辑状态更新为实体状态）的方向
                if (rot.eulerAngles.y>this.turnAngle&&rot.eulerAngles.y<(360-this.turnAngle))
                {

                    character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                    //更新刚体方向
                    rig.transform.forward = this.transform.forward;
                    this.SendEntityEvent(EntityEvent.None);
                }

            }



            #endregion

            #region 跳跃
            if (Input.GetButtonDown("Jump"))
            {
                this.SendEntityEvent(EntityEvent.Jump);


            }
            #endregion








        }

        Vector3 lastPos;
        float lastSync = 0;

        private void LateUpdate()
        {
            if (character==null)
            {
                return;
            }
            //计算实时速度
            Vector3 offset = this.rig.transform.position - lastPos;
            this.Speed = (int)(offset.magnitude * 100f / Time.deltaTime);


            this.lastPos = this.rig.transform.position;

            if ((GameObjectTool.WorldToLogic(this.rig.transform.position)-this.character.position).magnitude>50)
            {
                //当实际移动距离大于50时，更新角色逻辑的位置
                this.character.SetPosition(GameObjectTool.WorldToLogic(this.rig.transform.position));
                this.SendEntityEvent(EntityEvent.None);

            }

            //更新角色位置，<Qu>放在LateUpdate，考虑是因为其他物体（外力等）对角色刚体的影响，要体现在角色实体对象上
            this.transform.position = this.rig.transform.position;

        }


        void SendEntityEvent(EntityEvent entityEvent)
        {
            if (entityController!=null)
            {
            entityController.OnEntityEvent(entityEvent);

            }
            //角色的状态变化，通过MapService发送给服务端
            MapService.Instance.SendMapEntitySync(entityEvent, character.EntityData);


        }



    }
}
