using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

/************************* 
* 作者： wenyueyun 
* 时间： 2018/6/28 20:42:49 
* 描述： Tool 
*************************/
public class Tool
{

    [MenuItem("工具/资源/ABName_Atlas")]
    public static void UpdateAtlasAssetBundleName()
    {
        BuildAssetBundle.UpdateAtlasAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_Font")]
    public static void UpdateFontAssetBundleName()
    {
        BuildAssetBundle.UpdateFontAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_Config")]
    public static void UpdateConfigAssetBundleName()
    {
        BuildAssetBundle.UpdateConfigAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_Audio")]
    public static void UpdateAudioAssetBundleName()
    {
        BuildAssetBundle.UpdateAudioAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_Model")]
    public static void UpdateModelAssetBundleName()
    {
        BuildAssetBundle.UpdateModelAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_UI")]
    public static void UpdateUIAssetBundleName()
    {
        BuildAssetBundle.UpdateUIAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_Map")]
    public static void UpdateMapAssetBundleName()
    {
        BuildAssetBundle.UpdateMapAssetBundleName();
    }

    [MenuItem("工具/资源/ABName_All")]
    public static void UpdateAllAssetBundleName()
    {
        BuildAssetBundle.UpdateAllAssetBundleName();
    }

    [MenuItem("工具/服务器/编辑器")]
    public static void SwitchNone()
    {
        Debug.Log("编辑器");
        EditorUtil.ModifyConfig("cdn_url", "");
    }

    [MenuItem("工具/服务器/本地服")]
    public static void SwitchLocal()
    {
        Debug.Log("本地服");
        string ip = "";
        //获取计算机IP
        IPHostEntry computerIp = Dns.GetHostEntry(Dns.GetHostName());
        for (int i = 0; i < computerIp.AddressList.Length; i++)
        {
            if (computerIp.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ip = computerIp.AddressList[i].ToString();
            }
        }
        string cdn = string.Format("http://{0}:4141/Resources/{1}", ip, EditorUtil.Platform);
        EditorUtil.ModifyConfig("cdn_url", cdn);

        if (System.Diagnostics.Process.GetProcessesByName("hfs").Length == 0)
        {
            Debug.Log("hfs:         " + Application.dataPath  + @"\..\..\Tool\hfs\hfs.exe");
            Application.OpenURL(Application.dataPath + @"\..\..\Tool\hfs\hfs.exe");
        }
    }

    [MenuItem("工具/服务器/内网")]
    public static void SwitchIntranet()
    {
        Debug.Log("内网");
        string cdn = string.Format("http://{0}:4141/Resources/{1}", "192.168.50.247", EditorUtil.Platform);
        EditorUtil.ModifyConfig("cdn_url", cdn);
    }

    [MenuItem("工具/服务器/外网")]
    public static void SwitchInternet()
    {
        Debug.Log("外网");
        string cdn = string.Format("http://{0}:4141/Resources/{1}", "120.77.209.15", EditorUtil.Platform);
        EditorUtil.ModifyConfig("cdn_url", cdn);
    }

    [MenuItem("工具/构建资源")]
    public static void BuildRes()
    {
        BuildAssetBundle.Build();
    }

    [MenuItem("工具/MD5")]
    public static void MD5()
    {
        EditorUtil.MD5(Application.dataPath + "/StreamingAssets/");
    }

    [MenuItem("工具/发布")]
    public static void Release()
    {
        EditorUtil.ModifyConfig("res_version", EditorUtil.Version());
        //复制到对应的发布资源目标
        EditorUtil.CopyDir(Application.dataPath + "/StreamingAssets/", EditorUtil.resources_path + EditorUtil.Platform + "/StreamingAssets/");
        Debug.Log("复制到对应的发布资源目标");
    }

    [MenuItem("工具/一键发布")]
    public static void OneKey()
    {
        //构建配置表
        //XmlBuilder.BuildeXml();
        //更新资源
        BuildAssetBundle.UpdateAllAssetBundleName();
        //构建资源
        BuildAssetBundle.Build();
        //生成MD5
        EditorUtil.MD5(Application.dataPath + "/StreamingAssets/");
        //生成资源配置版本号
        EditorUtil.ModifyConfig("res_version", EditorUtil.Version());
        //复制到对应的发布资源目标
        EditorUtil.CopyDir(Application.dataPath + "/StreamingAssets/", EditorUtil.resources_path + EditorUtil.Platform + "/StreamingAssets/");
        Debug.Log("一键发布成功");
    }
}
