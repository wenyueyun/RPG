using Game.Code.Core.Res;
using Game.Core.Common;
using Game.Core.Config;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Core.Util;
using Object = UnityEngine.Object;

namespace Game.Core.Manager
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/8/27 16:19:27 
	* 描述： AssetManager 
	*************************/
    public class AssetManager : Singleton<AssetManager>
    {
        public AssetBundleManifest manifest;
        private Dictionary<string, string[]> dicDependencie = new Dictionary<string, string[]>();
        private Dictionary<string, uint> dicAssetBundleCreateRequestCount = new Dictionary<string, uint>();
        private Dictionary<string, AssetBundleData> dicAssetBundle = new Dictionary<string, AssetBundleData>();
        private Dictionary<string, AssetBundleCreateRequest> dicAssetBundleCreateRequest = new Dictionary<string, AssetBundleCreateRequest>();
        private List<AssetBundleLoadOperation> listProgressOperations = new List<AssetBundleLoadOperation>();

        public void Initialize()
        {
            LogUtil.LogGreen(GameConfig.AssetPath("StreamingAssets"));
            AssetBundle assetBundle = AssetBundle.LoadFromFile(GameConfig.AssetPath("StreamingAssets"));
            manifest = (AssetBundleManifest)assetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest));

            LoadSprite(ResType.Atlas,"common");
        }

        public void UnInitialize()
        {
            dicAssetBundle.Clear();
            dicAssetBundleCreateRequest.Clear();
            dicAssetBundleCreateRequestCount.Clear();
            listProgressOperations.Clear();
            dicDependencie.Clear();
        }

        public void Update()
        {
            if (manifest == null)
            {
                return;
            }

            List<string> loaded = new List<string>();
            foreach (var item in dicAssetBundleCreateRequest)
            {
                AssetBundleCreateRequest request = item.Value;
                if (request.isDone)
                {
                    AssetBundle bundle = request.assetBundle;
                    AssetBundleData loadedAssetBundle = null;
                    if (bundle != null && !dicAssetBundle.TryGetValue(item.Key, out loadedAssetBundle))
                    {
                        loadedAssetBundle = new AssetBundleData(bundle, dicAssetBundleCreateRequestCount[item.Key]);
                        dicAssetBundleCreateRequestCount.Remove(item.Key);

                        dicAssetBundle.Add(item.Key, loadedAssetBundle);
                    }
                    loaded.Add(item.Key);
                }
            }

            int len = loaded.Count;
            for (int i = 0; i < len; i++)
            {
                dicAssetBundleCreateRequest.Remove(loaded[i]);
            }

            // 检测加载进度
            for (int i = 0; i < listProgressOperations.Count;)
            {
                if (!listProgressOperations[i].Update() && listProgressOperations[i].IsDone())
                {
                    if (listProgressOperations[i] is AssetBundleLoadAssetOperation)
                    {
                        AssetBundleLoadAssetOperation assetOP = (listProgressOperations[i] as AssetBundleLoadAssetOperation);
                        if (assetOP.LoadComplete != null)
                        {
                            try
                            {
                                assetOP.LoadComplete(assetOP.GetABData());
                                assetOP.LoadComplete = null;
                            }
                            catch (Exception e)
                            {
                                LogUtil.LogError(e.Message);
                            }
                        }
                        assetOP.Dispose();
                        assetOP = null;
                    }
                    listProgressOperations.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        #region LoadAssetBundle
        private void LoadAssetBundle(string abPath)
        {
            if (manifest == null)
            {
                LogUtil.LogRed("AssetBundleManifest文件没加载");
                return;
            }

            bool isLoading = LoadAssetBundleInternal(abPath);
            if (!isLoading)
            {
                LoadDependencies(abPath);
            }
        }

        private void LoadDependencies(string abPath)
        {
            if (manifest == null)
            {
                LogUtil.LogRed("AssetBundleManifest文件没加载");
                return;
            }

            string[] dependencies = manifest.GetAllDependencies(abPath);
            if (dependencies.Length == 0)
            {
                return;
            }

            dicDependencie.Add(abPath, dependencies);

            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundleInternal(dependencies[i]);
            }
        }

        private bool LoadAssetBundleInternal(string abPath)
        {
            AssetBundleData bundle = null;
            if (dicAssetBundle.TryGetValue(abPath, out bundle))
            {
                bundle.ReferencedCount++;
                return true;
            }

            if (dicAssetBundleCreateRequest.ContainsKey(abPath))
            {
                dicAssetBundleCreateRequestCount[abPath] += 1;
                return true;
            }

            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(GameConfig.AssetPath(abPath));
            dicAssetBundleCreateRequest.Add(abPath, req);
            dicAssetBundleCreateRequestCount.Add(abPath, 1);
            LogUtil.LogYellow("加载资源-----------> " + abPath);
            return false;
        }
        #endregion

        #region UnLoadAssetBundle
        public void UnLoadAssetBundle(ResType type, string abName)
        {
            //这里可以过滤一下公共资源

            //卸载资源
            string abPath = ResConfig.GetABPath(type, abName);
            UnloadAssetBundleInternal(abPath);
            UnloadDependencies(abPath);
        }

        private void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!this.dicDependencie.TryGetValue(assetBundleName, out dependencies))
            {
                return;
            }

            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            this.dicDependencie.Remove(assetBundleName);
        }

        private void UnloadAssetBundleInternal(string abPath)
        {
            AssetBundleData bundle = GetAssetBundle(abPath);
            if (bundle == null)
            {
                return;
            }

            if (--bundle.ReferencedCount == 0)
            {
                bundle.Dispose();
                bundle = null;
                dicAssetBundle.Remove(abPath);
                LogUtil.LogGreen("卸载资源-----------> " + abPath);
            }
        }
        #endregion

        /// <summary>
        /// 获取已加载的ab
        /// </summary>
        public AssetBundleData GetAssetBundle(string abPath)
        {
            AssetBundleData bundle = null;

            //检测LoadedAsset有没有被加载
            this.dicAssetBundle.TryGetValue(abPath, out bundle);
            if (bundle == null)
            {
                return null;
            }

            //检测有没有依赖资源需要加载，如果没有依赖直接返回LoadedAsset
            string[] dependencies = null;
            if (!this.dicDependencie.TryGetValue(abPath, out dependencies))
            {
                return bundle;
            }

            //检测是否所有依赖资源都已经加载
            foreach (var dependency in dependencies)
            {
                AssetBundleData dependentBundle;
                this.dicAssetBundle.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    //如果有依赖资源没加载完，则继续等待
                    return null;
                }
            }
            return bundle;
        }

        /// <summary>
        /// 获取已加载的资源
        /// </summary>
        public GameObject GetPrefab(ResType type, string abName)
        {
            GameObject obj = null;
            string abPath = ResConfig.GetABPath(type, abName);
            AssetBundleData assetBundle = GetAssetBundle(abPath);
            if (assetBundle == null)
            {
                LoadAssetBundle(abPath);
                LogUtil.LogError("资源获取失败------abName：" + abPath);
                return null;
            }
            else
            {
                dicAssetBundle.TryGetValue(abPath, out assetBundle);
                if (assetBundle != null)
                {
                    obj = assetBundle.ab.LoadAsset(abName, typeof(GameObject)) as GameObject;
                }
            }

            if (obj == null)
            {
                LogUtil.LogError("资源获取失败------abName：" + abName);
                return null;
            }
            return UnityEngine.Object.Instantiate<GameObject>(obj);
        }

        /// <summary>
        /// 加载预制体
        /// </summary>
        public void LoadPrefab(ResType type, string abName, Action<AssetBundleData> loadComplete)
        {
            AssetBundleLoadOperation operation = null;
            string abPath = ResConfig.GetABPath(type, abName);
            AssetBundleData loadedAssetBundle = GetAssetBundle(abPath);
            if (loadedAssetBundle == null)
            {
                LoadAssetBundle(abPath);
            }
            operation = new AssetBundleLoadAssetOperation(abPath, abName, typeof(GameObject), loadComplete);
            listProgressOperations.Add(operation);
        }

        /// <summary>
        /// 加载音频
        /// </summary>
        public void LoadAudio(ResType type, string abName, Action<AssetBundleData> loadComplete)
        {
            AssetBundleLoadOperation operation = null;
            string abPath = ResConfig.GetABPath(type, abName);
            AssetBundleData loadedAssetBundle = GetAssetBundle(abPath);
            if (loadedAssetBundle == null)
            {
                LoadAssetBundle(abPath);
            }
            operation = new AssetBundleLoadAssetOperation(abPath, abName, typeof(AudioClip), loadComplete);
            listProgressOperations.Add(operation);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        public void LoadSprite(ResType type, string abName, string assetName = "", Action<AssetBundleData> loadComplete = null)
        {
            AssetBundleLoadOperation operation = null;
            string abPath = ResConfig.GetABPath(type, abName);
            AssetBundleData loadedAssetBundle = GetAssetBundle(abPath);
            if (loadedAssetBundle == null)
            {
                LoadAssetBundle(abPath);
            }
            operation = new AssetBundleLoadAssetOperation(abPath, assetName, typeof(Sprite), loadComplete);
            listProgressOperations.Add(operation);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        public void LoadConfig(ResType type, string abName, string assetName = "", Action<AssetBundleData> loadComplete = null)
        {
            AssetBundleLoadOperation operation = null;
            string abPath = ResConfig.GetABPath(type, abName);
            AssetBundleData loadedAssetBundle = GetAssetBundle(abPath);
            if (loadedAssetBundle == null)
            {
                LoadAssetBundle(abPath);
            }
            operation = new AssetBundleLoadAssetOperation(abPath, assetName, typeof(TextAsset), loadComplete);
            listProgressOperations.Add(operation);
        }
    }
}
