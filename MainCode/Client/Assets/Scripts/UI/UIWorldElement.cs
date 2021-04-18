using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour
{
    public Transform Owner;
    public float Height = 2f;


    void Start()
    {

    }

    void Update()
    {
        if (Owner != null)
        {

            this.transform.position = Owner.position + Vector3.up * Height;//血条UI始终显示在角色上方

        }
        if (Camera.main != null)
        {

            this.transform.forward = Camera.main.transform.forward;
        }
    }
}
