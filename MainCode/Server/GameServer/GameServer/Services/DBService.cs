using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }



        float time = DateTime.Now.Ticks;
        /// <summary>
        /// 将物品（背包）变化的数据保存至DB的Save方法
        /// </summary>
        public void Save(bool asnc = false)
        {
            ////设定DB存储间隔
            //if (DateTime.Now.Ticks - time > 30.0f)
            //{
            if (asnc)
            {
                entities.SaveChangesAsync();
            }
            else
            {
                entities.SaveChanges();
            }

            //    time = DateTime.Now.Ticks;
            //} 


        }
    }
}
