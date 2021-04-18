using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using UnityEngine;
using Managers;
using SkillBridge.Message;

namespace Controller
{
    public  class EntityController : MonoBehaviour,IEntityNotify
    {
        public Entity Entity;
        public  Animator anim;
        public Rigidbody rig;
        AnimatorStateInfo state;
        public bool IsPlayer;

        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 LastPosition;
        Quaternion rotation;
        Quaternion lastRotation;



        private void Start()
        {
            //anim = GetComponent<Animator>();

            if (Entity!=null)
            {
                //注册成为EntityManager的1个事件接受者
                EntityManager.Instance.RegisterEntityChangeNotify(Entity.entityId, this);
                this.UpdateTransform();
            }


            if (!this.IsPlayer)
            {
                rig.useGravity = false;
            }

        }



        void UpdateTransform()
        {
            this.Position = GameObjectTool.LogicToWorld(Entity.position);
            this.Direction = GameObjectTool.LogicToWorld(Entity.direction);
            this.rig.MovePosition(this.Position);
            this.transform.forward = this.Direction;
            this.LastPosition = this.Position;
            this.lastRotation = this.rotation;



        }


        private void OnDestroy()
        {
            if (Entity!=null)
            {
                Debug.LogFormat("{0} isDestroy: entityID:{1} POS:{2} DIR:{3} SPD:{4}", this.name, Entity.entityId, Entity.position, Entity.direction, Entity.speed);

            }

            if (UIWorldElementManager.Instance!=null)
            {
                //Debug.LogError("["+Entity.entityId + "] UI_NameBar is Destroyed!");//这里报空异常：考虑是Entity已经被删除了
                UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
            }

        }

        private void FixedUpdate()
        {
            if (this.Entity==null)
            {
                return;
            }
            this.Entity.OnUpdate(Time.fixedDeltaTime);

            if (!this.IsPlayer)
            {
                this.UpdateTransform();
            }


            
        }

        /// <summary>
        /// 处理EntityEvent事件，根据Event类型，设置角色状态变化（站立、跑、跳）
        /// </summary>
        /// <param name="entityEvent"></param>
        public  void  OnEntityEvent(EntityEvent entityEvent)
        {
            switch (entityEvent)
            {
                case EntityEvent.None:
                    break;
                case EntityEvent.Idle:
                    anim.SetBool("Move", false);
                    anim.SetTrigger("Idle");
                    break;
                case EntityEvent.MoveFwd:
                    anim.SetBool("Move", true);
                    break;
                case EntityEvent.MoveBack:
                    anim.SetBool("Move", true);
                    break;
                case EntityEvent.Jump:
                    anim.SetTrigger("Jump");
                    break;
                default:
                    break;
            }



        }

        /// <summary>
        /// 实现IEntityNotify接口所定义的OnEntityRemoved方法
        /// </summary>
        public void OnEntityRemoved()
        {
            if (UIWorldElementManager.Instance!=null)
            {
                //删除当前角色头上的血条
                UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
            }
            //删除当前EntityController对象  //Debug:这里Destroy对象会导致程序报空异常
            //Destroy(this.gameObject);
        }

        /// <summary>
        /// 实现IEntityNotify接口所定义的OnEntityChanged方法
        /// </summary>
        /// <param name="entity"></param>
        public void OnEntityChanged(Entity entity)
        {
            Debug.LogFormat("OnEntityChanged:ID:{0} POS:{1} DIR:{2} SPD{3}", entity.entityId, entity.position, entity.direction, entity.speed);
            
            //测试：解决退出游戏,第2次进入后游戏同步失败的问题
            this.Entity = entity;

        }
    }
}
