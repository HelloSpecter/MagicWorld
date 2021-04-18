using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Managers;
using UnityEngine;
using Common.Data;
using Entities;

namespace Services
{
    public class StatusService : Singleton<StatusService>, IDisposable//IDisposable——释放非托管资源
    {
        public delegate bool StatusNotifyHandler(NStatus status);

        Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();

        HashSet<StatusNotifyHandler> handles = new HashSet<StatusNotifyHandler>();
        public void Init()
        {


        }

        /// <summary>
        /// 注册通知事件的方法
        /// </summary>
        /// <param name="function"></param>
        /// <param name="action"></param>
        public void ResgisterStatusNofity(StatusType function,StatusNotifyHandler action)
        {
            if (handles.Contains(action))
            {
                return;
            }
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
            handles.Add(action);
        }

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            foreach (NStatus status in notify.Status)
            {
                Notify(status);
            }
        }

        /// <summary>
        /// 根据协议中的状态通知，对客户端数据进行变化操作
        /// </summary>
        /// <param name="status"></param>
        private void Notify(NStatus status)
        {
            Debug.LogFormat("StatusNotify:[{0}][{1}]{2}:{3}", status.Type, status.Action, status.Id, status.Value);

            if (status.Type == StatusType.Money)
            {
                if (status.Action == StatusAction.Add)
                {
                    User.Instance.AddGold(status.Value);
                }
                else if(status.Action == StatusAction.Delete)
                {
                    User.Instance.AddGold(-status.Value);
                }
            }

            //若状态变化不是金钱，调用该状态变化对应的StatusNotifyHandler
            StatusNotifyHandler handler;
            if (eventMap.TryGetValue(status.Type,out handler))
            {
                handler(status);
            };

        }



    }
}
