using Game.Core.Util;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Core.Config
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/6/28 9:17:44 
	* 描述： GameConfig 
	*************************/
    public class GameConfig
    {
        public  static string persistentDataPath =Path.Combine( Application.persistentDataPath, "StreamingAssets");
        
        public static string appVersion;
        public static string resVersion;
        public static string cdnUrl;
        public static Dictionary<string, AssetInfo> md5Dic = new Dictionary<string, AssetInfo>();

        public static void InitLocal(String text)
        {
            JsonData json = JsonMapper.ToObject(text);
            appVersion = json["app_version"].ToString();
            resVersion = json["res_version"].ToString();
            cdnUrl = json["cdn_url"].ToString();
        }

        public static string remoteAppVersion;
        public static string remoteResVersion;
        public static string remoteConfig;
        public static string remoteMD5;
        public static void InitRemote(string text)
        {
            remoteConfig = text;
            JsonData json = JsonMapper.ToObject(text);
            remoteAppVersion = json["app_version"].ToString();
            remoteResVersion = json["res_version"].ToString();
            cdnUrl = json["cdn_url"].ToString();
        }

        //是否更新APP
        public static bool IsUpdateApp()
        {
            LogUtil.Log("appVersion--->" + appVersion);
            LogUtil.Log("remoteAppVersion--->" + remoteAppVersion);
            if (VersionToLong(remoteAppVersion) > VersionToLong(appVersion))
            {
                return true;
            }
            return false;
        }

        //是否更新Res
        public static bool IsUpdateRes()
        {
            //LogUtil.Log("resVersion--->" + resVersion);
            //LogUtil.Log("remoteResVersion--->" + remoteResVersion);
            if (long.Parse(remoteResVersion) > long.Parse(resVersion))
            {
                return true;
            }
            return false;
        }

        private static long VersionToLong(string version)
        {
            string[] arrVersion = version.Split(new char[] { '.' });
            return long.Parse(string.Join("", arrVersion));
        }

        public static void InitLocalMD5(string text)
        {
            md5Dic.Clear();
            JsonData json = JsonMapper.ToObject(text);
            for (int i = 0; i < json.Count; i++)
            {
                JsonData data = json[i];
                AssetInfo info = new AssetInfo();
                info.path = data["path"].ToString();
                info.md5 = data["md5"].ToString();
                info.size = long.Parse(data["size"].ToString());
                md5Dic.Add(info.path, info);
            }
        }

        public static List<AssetInfo> InitRemoteMD5(string text)
        {
            remoteMD5 = text;
            List<AssetInfo> result = new List<AssetInfo>();
            JsonData json = JsonMapper.ToObject(text);
            for (int i = 0; i < json.Count; i++)
            {
                JsonData data = json[i];
                AssetInfo info = new AssetInfo();
                info.path = data["path"].ToString();
                info.md5 = data["md5"].ToString();
                info.size = long.Parse(data["size"].ToString());

                if (md5Dic.ContainsKey(info.path))
                {
                    AssetInfo localInfo = md5Dic[info.path];
                    if(!localInfo.md5.Equals(info.md5))
                    {
                        result.Add(info);
                    }
                }
                else
                {
                    result.Add(info);
                }
            }
            return result;
        }

        public static string AssetPath(string path)
        {
            string url = Path.Combine(persistentDataPath, path);
            if(File.Exists(url))
            {
                return url;
            }
            return Path.Combine(Application.streamingAssetsPath, path);
        }
    }


    public class AssetInfo
    {
        public string path;
        public string md5;
        public long size;
    }
}
