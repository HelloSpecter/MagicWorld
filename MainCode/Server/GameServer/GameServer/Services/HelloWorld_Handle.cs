using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using log4net;
using SkillBridge;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    /// <summary>
    /// 测试HelloWorld消息的测试
    /// </summary>
    class HelloWorld_Handle:Singleton<HelloWorld_Handle>
    {
        public void Init()
        {

        }
        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<HelloWorldRequest>(this.OnHelloWorldRequest);

        }
        void OnHelloWorldRequest(NetConnection<NetSession> sender,HelloWorldRequest request)
        {
            Log.Info("Hellownum is " + request.Hellownum);
            Log.Info("Helloworld is " + request.Helloworld);
        }

        public void Stop()
        {


        }
    }
    
}
