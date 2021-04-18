using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSend : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Network.NetClient.Instance.Init("127.0.0.1", 8000);
        Network.NetClient.Instance.Connect();

        ////设置消息
        //NetMessage msg = new NetMessage();
        //msg.Request = new NetMessageRequest();
        //msg.Request.helloWorldRequest = new HelloWorldRequest();
        //msg.Request.helloWorldRequest.Hellownum = 5;
        //msg.Request.helloWorldRequest.Helloworld = "HelloWorld";
        ////发送消息
        //Network.NetClient.Instance.SendMessage(msg);
        //Debug.Log(msg.ToString() + "消息已发送");



    }

    // Update is called once per frame
    void Update () {
		
	}
}
