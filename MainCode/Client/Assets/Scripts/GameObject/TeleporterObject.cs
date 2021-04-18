using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;
using Common.Data;
using Services;


public class TeleporterObject : MonoBehaviour {

    public int ID;
    Mesh mesh = null;

	// Use this for initialization
	void Start () {

        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;

	}



    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("角色进入传送点:[" + ID + "]");

        PlayerInputController playerInputController = other.GetComponent<PlayerInputController>();

        if (playerInputController != null && playerInputController.isActiveAndEnabled)
        {

            TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];

            //如果传送点定义文件缺失，报错并返回
            if (td == null)
            {
                Debug.LogErrorFormat("TeleporterObject:Character [{0}] Enter Teleporter [{1}],But TeleporterDefine not exister", playerInputController.character.Info.Name, this.ID);

                return;

            }
            //--------------

            Debug.LogFormat("TeleporterObject:Character [{0}] Enter Teleporter [{1}:{2}]", playerInputController.character.Info.Name, td.ID, td.Name);

            if (td.LinkTo > 0)
            {

                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                {
                    MapService.Instance.SendMapTeleport(this.ID);
                }
                else
                {
                    Debug.LogErrorFormat("Teleporter ID:{0} LinkID {1} error!", td.ID, td.LinkTo);
                }


            }





        }


    }

    
    void Update () {
		


	}

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        //设置绘制颜色：绘制为绿色

        Gizmos.color = Color.green;

        //绘制网格
        if (this.mesh!=null)
        {
            //显示效果
            //Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * .5f, this.transform.rotation, this.transform.localScale);
            Gizmos.DrawWireMesh(this.mesh, this.transform.position, this.transform.rotation, this.transform.localScale);

        }
        //编辑器扩展开发，设置小箭头方向
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position,  this.transform.rotation, 1f, EventType.Repaint);

    }


#endif
    


}
