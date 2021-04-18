using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Common.Data;
using Models;

namespace Managers
{
    public  class MinimapManager:Singleton<MinimapManager>
    {
        public UIMiniMap miniMap;

        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get
            {
                return minimapBoundingBox;
            }
        }



        //自定义1个属性，进行逻辑上的排除 空异常 处理
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurCharObject==null)
                {
                    return null;
                }

                return User.Instance.CurCharObject.transform;
            }


        }



        public Sprite LoadMiniMap()
        {
            MapDefine mapDefine = User.Instance.CurMapDefine;
            Sprite mapImg = Resloader.Load<Sprite>("UI/Minimap/" + mapDefine.MiniMap);
            Debug.LogError("已获取地图[" + mapDefine.ID + "]" + mapDefine.Name);
            return mapImg;


        }


        public void UpdateMinimap(Collider minimapBoundingBox)
        {

            this.minimapBoundingBox = minimapBoundingBox;
            if (this.miniMap!=null)
            {

                this.miniMap.UpdateMap();

            }

        }


    }
}
