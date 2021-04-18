using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Managers;

public class UIMiniMap : MonoBehaviour
{

    public Image miniMap;
    public Image arrow;
    public Text mapName;
    public Collider minimapBoundingBox;




    private Transform playerTransform;
    // Use this for initialization
    void Start()
    {
        Debug.LogWarning("UIMinimap Start"+ this.GetInstanceID());
        MinimapManager.Instance.miniMap = this;
        this.UpdateMap();

    }

    public  void UpdateMap()
    {
        
        this.mapName.text = User.Instance.CurMapDefine.Name;

        miniMap.overrideSprite = MinimapManager.Instance.LoadMiniMap();

        

        miniMap.SetNativeSize();
        miniMap.transform.localPosition = Vector3.zero;

        //更新包围盒
        this.minimapBoundingBox = MinimapManager.Instance.MinimapBoundingBox;
        //将其绑定的playerTransform清空，目的：Update中重新获取User.Instance.CurCharObject.transform（因为：新进入地图时，CurCharObject重新建立了）
        this.playerTransform = null;
    }


    // Update is called once per frame
    void Update()
    {
        //避免：返回角色选择时，当前角色未创建而导致的小地图绑定操作引发 空异常 
        if (playerTransform==null)
        {
            this.playerTransform = MinimapManager.Instance.PlayerTransform;
        }


        //避免在切换场景等边界情况时出现空异常错误
        if (minimapBoundingBox == null || playerTransform == null)
        {
            return;
        }

        float realWidth = minimapBoundingBox.bounds.size.x;
        float realHeight = minimapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;

        //将游戏中角色的正方向同步到小地图的导航图标
        Quaternion quaternion = Quaternion.Euler(-90, 0, 0);
        Vector3 newTemp = quaternion * this.playerTransform.forward;
        this.arrow.transform.up = newTemp;
    }
}
