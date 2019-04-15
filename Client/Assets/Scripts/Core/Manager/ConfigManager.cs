using Game.Code.Core.Res;
using Game.Core.Common;
using Game.Core.Config;
using Game.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.Core.Manager
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2019/3/27 17:15:54 
	* 描述： ConfigManager 
	*************************/
    public class ConfigManager : Singleton<ConfigManager>
    {
        private Dictionary<string, byte[]> datas = new Dictionary<string, byte[]>();
        public void Initialize()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            AssetManager.Instance.LoadConfig(ResType.Config, ResConfig.GetABPath(ResType.Config, ""), "", LoadComplete);
        }

        private void LoadComplete(AssetBundleData value)
        {
            LogUtil.LogYellow("配置表加载成功");
            TextAsset[] assets = value.ab.LoadAllAssets<TextAsset>();
            for (int i = 0; i < assets.Length; i++)
            {
                TextAsset asset = assets[i];
                if (!datas.ContainsKey(asset.name))
                {
                    datas.Add(asset.name, asset.bytes);
                }
            }

            Debug.Log("ConfigBattle==================");
            List<ConfigBattle> list = GetData<ConfigBattle>("ConfigBattle");
            for (int i = 0; i < list.Count; i++)
            {
                Debug.LogFormat("{0}_{1}",list[i].bid,list[i].level);
            }


            Debug.Log("ConfigItem==================");
            List<ConfigItem> listItem = GetData<ConfigItem>("ConfigItem");
            for (int i = 0; i < listItem.Count; i++)
            {
                Debug.LogFormat("{0}__________", listItem[i].bid);
            }

            Debug.Log("ConfigModule==================");
            List<ConfigModule> listModule = GetData<ConfigModule>("ConfigModule");
            for (int i = 0; i < listModule.Count; i++)
            {
                Debug.LogFormat("{0}__{1}", listModule[i].bid, listModule[i].name);
            }
        }

        /// <summary>
        /// 获取配置表数据
        /// </summary>
        public List<T> GetData<T>(string name)
        {
            byte[] bytes;
    
            if (datas.TryGetValue(name, out bytes))
            {
                MemoryStream mStream = new MemoryStream();
                mStream.Write(bytes, 0, bytes.Length);
                mStream.Position = 0;

                BinaryFormatter binFormat = new BinaryFormatter();
                List<T> list = (List<T>)binFormat.Deserialize(mStream);

                mStream.Close();
                mStream.Dispose();
                mStream = null;

                return list;
            }
            return null;
        }
    }
}
