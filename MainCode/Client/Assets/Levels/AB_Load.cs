using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Collections;

public class AB_Load : MonoBehaviour {

    private void Start()
    {
        StartCoroutine(LoadCharacter("Archer"));
    }

     IEnumerator LoadCharacter(string assetBundleName)
    {
        string uri = "file:///Assets/AssetBundles/character/" + assetBundleName.ToLower() + ".asset";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        string uri2 = "file:///Assets/AssetBundles/character/" + assetBundleName.ToLower() + "_mat"+".asset";
        UnityWebRequest request2 = UnityWebRequestAssetBundle.GetAssetBundle(uri2);
        yield return request2.SendWebRequest();
        AssetBundle bundle2 = DownloadHandlerAssetBundle.GetContent(request2);

        GameObject gameObject = bundle.LoadAsset<GameObject>(assetBundleName);
        Instantiate(gameObject);
    }
}
