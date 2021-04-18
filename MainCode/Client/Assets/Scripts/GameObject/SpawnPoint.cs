using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;
using Common.Data;
using Services;


public class SpawnPoint : MonoBehaviour {

    public int ID;
    public Mesh mesh = null;

	// Use this for initialization
	void Start () {

        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;

	}

    void Update () {
		


	}

    //离开编辑器即失效：
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Vector3 pos = this.transform.position + Vector3.up * this.transform.localScale.y * .5f;//pos点为球心+半径=>原球的Y方向顶点
        Gizmos.color = Color.red;
        if (this.mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh, pos, this.transform.rotation, this.transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        //绘制箭头
        UnityEditor.Handles.ArrowHandleCap(0, pos, this.transform.rotation, 1f, EventType.Repaint);

    }


#endif
    


}
