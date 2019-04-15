using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/************************* 
* 作者： wenyueyun 
* 时间： 2018/6/27 11:05:30 
* 描述： BuildAssetBundle 
*************************/
public class BuildAssetBundle
{
    public static void UpdateAllAssetBundleName()
    {
        UpdateAtlasAssetBundleName();
        UpdateConfigAssetBundleName();
        UpdateAudioAssetBundleName();
        UpdateModelAssetBundleName();
        UpdateUIAssetBundleName();
        UpdateMapAssetBundleName();
        UpdateFontAssetBundleName();
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    public static void UpdateAtlasAssetBundleName()
    {
        Debug.Log("开始设置Atlas名字------------->");
        string path = "Assets/Res/Atlas";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;
            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = "Atlas/" + file.Directory.Name;
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
            }
            importer.spritePackingTag = "atlas_"+ file.Directory.Name.ToLower();
            importer.assetBundleName = abName.ToLower();
            importer.SaveAndReimport();
        }
        AssetDatabase.Refresh();
        Debug.Log("Atlas名字设置完成");
    }

    public static void UpdateFontAssetBundleName()
    {
        Debug.Log("开始设置Font名字------------->");
        string path = "Assets/Res/Font";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;

            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = assetPath.Replace("Assets/Res/", "").Split('.')[0];
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = abName.ToLower();
        }
        AssetDatabase.Refresh();
        Debug.Log("Font名字设置完成");
    }

    public static void UpdateConfigAssetBundleName()
    {
        Debug.Log("开始设置Config名字------------->");
        string path = "Assets/Res/Config";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;
            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = "Config/Data";
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = abName.ToLower();
        }
        AssetDatabase.Refresh();
        Debug.Log("Config名字设置完成");
    }

    public static void UpdateAudioAssetBundleName()
    {
        Debug.Log("开始设置Audio名字------------->");
        string path = "Assets/Res/Audio";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;
            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = assetPath.Replace("Assets/Res/", "").Split('.')[0];
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = abName.ToLower();
        }
        AssetDatabase.Refresh();
        Debug.Log("Audio名字设置完成");
    }

    public static void UpdateModelAssetBundleName()
    {
        Debug.Log("开始设置Model名字------------->");
        string path = "Assets/Res/Model";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;
            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = assetPath.Replace("Assets/Res/", "").Split('.')[0];
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = abName.ToLower();
        }
        AssetDatabase.Refresh();
        Debug.Log("Model名字设置完成");
    }

    public static void UpdateUIAssetBundleName()
    {
        Debug.Log("开始设置UI名字------------->");
        string path = "Assets/Res/UI";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;
            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = assetPath.Replace("Assets/Res/", "").Split('.')[0];
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = abName.ToLower();
        }
        AssetDatabase.Refresh();
        Debug.Log("UI名字设置完成");
    }

    public static void UpdateMapAssetBundleName()
    {
        Debug.Log("开始设置Map名字------------->");
        string path = "Assets/Res/Map";
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Extension == ".meta") continue;
            string assetPath = EditorUtil.FullPathToAssetPath(file.FullName);
            string abName = assetPath.Replace("Assets/Res/", "").Split('.')[0];
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = abName.ToLower();
        }
        AssetDatabase.Refresh();
        Debug.Log("Map名字设置完成");
    }

    //构建资源
    public static void Build()
    {
        Debug.Log("开始构建资源------------->");

        if(!Directory.Exists(Application.dataPath + "/StreamingAssets"))
        {
            Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
        }

        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
        BuildPipeline.BuildAssetBundles(Application.dataPath + "/StreamingAssets/", options, EditorUserBuildSettings.activeBuildTarget);
        EditorUtility.UnloadUnusedAssetsImmediate();
        Debug.Log("资源构建完毕");
        AssetDatabase.Refresh();
    }
}
