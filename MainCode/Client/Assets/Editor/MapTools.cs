using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTools
{

    //编辑器上方新建1个菜单项"菜单项名称/二级菜单名称"
    [MenuItem("Map Tools/Export Teleporters")]
    //---------------

    //使用静态方法
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        //获取当前场景
        Scene current = EditorSceneManager.GetActiveScene();
        string curScene = current.name;
        //---

        //如果当前场景已经被修改一过，但却未保存，则提示保存，并返回
        if (current.isDirty)
        {

            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;

        }
        //---

        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();

        //对每张地图中的传送点进行检查
        foreach (var map in DataManager.Instance.Maps)
        {
            //读取每个Map（Scene）的路径地址
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            //判断Map对应的路径地址下文件(Scene场景)是否存在
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} is not existed!", sceneFile);
                continue;
            }

            //EditorSceneManager是编辑器模式下的场景管理器,打开每个Map所对应的场景
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters)
            {

                //校验传送点Id是否有效
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0}中配置的 Teleporter:[{1}] 不正确，请检查ID", map.Value.Resource, teleporter.ID), "确定");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];

                if (def.MapID!=map.Value.ID)
                {

                    EditorUtility.DisplayDialog("错误",string.Format("地图:{0}中配置的 Teleporter:[{1}] 不正确，请检查ID", map.Value.Resource, teleporter.ID), "确定");
                    return;

                }
                //---

                //将Unity中的世界坐标转换为逻辑坐标
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
                //---
            }




        }

        DataManager.Instance.SaveTeleporters();
        
        //恢复到运行此插件前的场景
        EditorSceneManager.OpenScene("Assets/Levels/" + curScene + ".unity");
        //---

        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");



    }

    [MenuItem("Map Tools/Export SpawnPoints")]

    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        if (DataManager.Instance.SpawnPoints==null)
        {
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        }

        foreach (var map in DataManager.Instance.Maps)
        {
            //读取每个Map（Scene）的路径地址
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            //判断Map对应的路径地址下文件(Scene场景)是否存在
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} is not existed!", sceneFile);
                continue;
            }

            //EditorSceneManager是编辑器模式下的场景管理器,打开每个Map所对应的场景
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            SpawnPoint[] spawnpoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }

            foreach (var sp in spawnpoints)
            {

                //校验传送点Id是否有效
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }

                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];

                def.ID = sp.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);

            }
        }

        DataManager.Instance.SaveSpawnPoints();

        //恢复到运行此插件前的场景
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        //---

        EditorUtility.DisplayDialog("提示", "刷怪点导出完成", "确定");

    }


}

