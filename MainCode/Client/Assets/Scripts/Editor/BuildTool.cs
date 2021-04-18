using UnityEditor;
using UnityEngine;
using System.IO;
public class BuildTool
{
    [MenuItem("BuildTool/Clear AssetBundles")]
    static void ClearAllAssetsBundles()
    {
        var allBundles = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundle in allBundles)
        {
            AssetDatabase.RemoveAssetBundleName(bundle, true);
            Debug.LogFormat("BuildTool:Remove Old Bundle:{0}", bundle);
        }
    }

    [MenuItem("BuildTool/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        //参数列表：输出目录；打包选项（是否自动更新、关联依赖项、压缩等）；打包的目标平台
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
    }
}
