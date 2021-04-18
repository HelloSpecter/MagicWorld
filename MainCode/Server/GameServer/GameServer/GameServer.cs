﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;
        NetService network;

        public bool Init()
        {
            network = new NetService();
            network.Init(8000);
            HelloWorld_Handle.Instance.Init();
            UserService.Instance.Init();
            DBService.Instance.Init();
            DataManager.Instance.Load();
            MapService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            FriendService.Instance.Init();
            TeamService.Instance.Init();
            //var a = DBService.Instance.Entities.Characters.Where(s => s.TID == 1);
            //Console.WriteLine("{0}",a.FirstOrDefault<TCharacter>().Name);

            thread = new Thread(new ThreadStart(this.Update));//启动1个进程

            return true;
        }

        public void Start()
        {
            network.Start();
            HelloWorld_Handle.Instance.Start();
            UserService.Instance.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            var mapManager = MapManager.Instance;
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);//每100ms跑1帧（1s跑10帧）
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapManager.Update();
            }
        }
    }
}
