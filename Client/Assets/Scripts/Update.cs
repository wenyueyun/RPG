using Game.Core.Config;
using Game.Core.Util;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Game.Core.Manager
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/6/26 13:55:47 
	* 描述： UpdateManager 
	*************************/
    public class Update : MonoBehaviour
    {
        private void Start()
        {
            if (!Directory.Exists(GameConfig.persistentDataPath))
            {
                Directory.CreateDirectory(GameConfig.persistentDataPath);
            }
#if DEV
            gameObject.SendMessage("StartGame");
#else
            StartCoroutine(LoadConfig());
#endif
        }

        private IEnumerator LoadConfig()
        {
            //加载本地配置文件
            Action<string, WWW> loadLocalConfig = (string path, WWW www) =>
            {
                GameConfig.InitLocal(www.text);
            };
            yield return StartCoroutine(LoadAsset("config.json", true, loadLocalConfig));

            //加载服务器配置文件
            Action<string, WWW> loadRemoteConfig = (string path, WWW www) =>
            {
                GameConfig.InitRemote(www.text);
            };
            yield return StartCoroutine(LoadAsset("config.json", false, loadRemoteConfig));

            CheckUpdate();
        }


        //判断是否更新
        private void CheckUpdate()
        {
            if (GameConfig.IsUpdateApp())
            {
                LogUtil.LogGreen("开始更新客户端");
                return;
            }

            if (GameConfig.IsUpdateRes())
            {
                LogUtil.LogGreen("开始更新资源");
                StartCoroutine(CheckMD5());
                return;
            }

            //不需要更新，直接进入游戏
            LogUtil.LogGreen("不需要更新，直接进入游戏");
            UpdateFinish();
        }


        private void UpdateFinish()
        {
            LogUtil.LogRed("StartGame");
            gameObject.SendMessage("StartGame");
            GameObject.Destroy(this);
        }

        //查找哪些资源需要更新
        private int loadCount = 0;
        private List<AssetInfo> diffList;
        private IEnumerator CheckMD5()
        {
            //加载本地MD5文件
            Action<string, WWW> loadLocalMD5 = (string path, WWW www) =>
            {
                GameConfig.InitLocalMD5(www.text);
            };
            yield return StartCoroutine(LoadAsset("md5.json", true, loadLocalMD5));

            //加载服务器MD5文件
            Action<string, WWW> loadRemoteMD5 = (string path, WWW www) =>
            {
                diffList = GameConfig.InitRemoteMD5(www.text);
            };
            yield return StartCoroutine(LoadAsset("md5.json", false, loadRemoteMD5));

            if (diffList.Count > 0)
            {
                yield return UpdateAsset();
            }
            else
            {
                UpdateFinish();
            }

        }

        private IEnumerator UpdateAsset()
        {
            for (int i = 0; i < diffList.Count; i++)
            {
                AssetInfo info = diffList[i];
                //加载服务器MD5文件
                Action<string, WWW> loadRes = (string path, WWW www) =>
                {
                    string dirPath = Path.Combine(GameConfig.persistentDataPath, path);
                    dirPath = dirPath.Substring(0, dirPath.LastIndexOf("/"));
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    using (FileStream fs = File.Open(Path.Combine(GameConfig.persistentDataPath, path), FileMode.Create))
                    {
                        using (BinaryWriter sw = new BinaryWriter(fs))
                        {
                            sw.Write(www.bytes);
                            sw.Close();
                        }
                        fs.Close();
                    }
                    LogUtil.LogYellow(string.Format("加载资源：{0}", path));
                    loadCount++;
                    if (loadCount >= diffList.Count)
                    {
                        //保存新的配置文件
                        File.WriteAllText(GameConfig.persistentDataPath + "/md5.json", JsonFormat(GameConfig.remoteMD5));
                        File.WriteAllText(GameConfig.persistentDataPath + "/config.json", JsonFormat(GameConfig.remoteConfig));
                        UpdateFinish();
                    }
                };
                yield return StartCoroutine(LoadAsset(info.path, false, loadRes));
            }
        }

        private IEnumerator LoadAsset(string path, bool local = true, Action<string, WWW> loadSucc = null)
        {
            string url;
            WWW www;
            if (local)
            {
                url = Path.Combine(GameConfig.persistentDataPath, path);
                if (!File.Exists(url))
                {
                    url = Path.Combine(Application.streamingAssetsPath, path);
                }
#if UNITY_EDITOR
                url = "file:///" + url;
#endif
            }
            else
            {
                url = GameConfig.cdnUrl + "/StreamingAssets/" + path;
            }

            LogUtil.LogYellow("url------->" + url);

            www = new WWW(url);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
        
                if (loadSucc != null)
                {
                    loadSucc(path, www);
                }
            }
            else
            {
                LogUtil.LogRed(www.error);
            }
            www.Dispose();
            www = null;
        }
        //将json数据进行格式化
        public string JsonFormat(string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sReader = new StringReader(str);
            JsonTextReader jReader = new JsonTextReader(sReader);
            object readerObj = serializer.Deserialize(jReader);
            if (readerObj != null)
            {
                StringWriter sWriter = new StringWriter();
                JsonTextWriter jWriter = new JsonTextWriter(sWriter)
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                };
                serializer.Serialize(jWriter, readerObj);
                return sWriter.ToString();
            }
            else
            {
                return str;
            }
        }
    }
}
