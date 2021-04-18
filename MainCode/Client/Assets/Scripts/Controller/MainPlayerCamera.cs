using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera> {

    public Transform ViewPoint;
    public GameObject Player;
    public Camera PlayerCamera;
	// Use this for initialization
	protected override void OnStart () {

        


    }
	
	// Update is called once per frame
	void Update () {
		



	}
    private void LateUpdate()
    {
        if (Player==null)
        {
            Player = User.Instance.CurCharObject;


        }
        if (Player==null)
        {
            return;
        }

        this.transform.position = Player.transform.position;
        this.transform.rotation = Player.transform.rotation;

    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
