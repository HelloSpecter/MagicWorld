using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using UnityEngine.UI;
using Managers;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    public  GameObject NamebarPerfab;
    public  GameObject npcStatusPrefab;
    Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    // Use this for initialization
    protected override void OnStart () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddCharacterNameBar(Transform owner,Character character)
    {
        GameObject goNameBar = Instantiate(NamebarPerfab, this.transform);
        goNameBar.name = "NameBar" + character.entityId;//注意 entityId 值是否为空值
        goNameBar.GetComponent<UIWorldElement>().Owner = owner;
        goNameBar.GetComponent<UINameBar>().Character = character;
        goNameBar.SetActive(true);
        this.elementNames[owner] = goNameBar;
        



    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementNames.ContainsKey(owner))
        {
            Destroy(this.elementNames[owner]);//删除owner身上的血条UI组件
            this.elementNames.Remove(owner);//同时在管理器的字典内将其删除

        }


    }

    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        //这里增加判空，防止Prefab被Destroy后，空异常报错
        GameObject ui=null;
        this.elementStatus.TryGetValue(owner, out ui);

        if (ui!=null)
        {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().Owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }

    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }


    }


    public void Destroy()
    {
        Destroy(this.gameObject);
    }

}
