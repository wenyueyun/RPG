using Game.Core.Manager;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Code.Core.Res
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/8/27 16:37:18 
	* 描述： AssetBundleLoadAssetOperation 
	*************************/
    public class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
    {
        protected AssetBundleRequest abRequest = null;
      
        public AssetBundleLoadAssetOperation(string abName,string assetName,Type type, Action<AssetBundleData> loadComplete= null, Action<string> loadError = null)
        {
            this.abName = abName;
            this.assetName = assetName;
            this.assetType = type;
            this.loadComplete = loadComplete;
            this.loadError = loadError;
        }


        public override float Progress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private AssetBundleData abData;
        public override AssetBundleData GetABData()
        {
            return abData;
        }

        public override Object GetAsset()
        {
            if (abRequest != null && abRequest.isDone)
            {
                return abRequest.asset;
            }
            else
            {
                return null;
            }
        }


        public override bool IsDone()
        {
            if (abRequest == null)
            {
                return true;
            }
            return abRequest != null && abRequest.isDone;
        }

        public override bool Update()
        {
            if (abRequest != null)
                return false;

            abData = AssetManager.Instance.GetAssetBundle(abName);
            if (abData != null)
            {
                if (string.IsNullOrEmpty(AssetName))
                {
                    return false;
                }
                else
                {
                    abRequest = abData.ab.LoadAssetAsync(assetName, assetType);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void Dispose()
        {
            abRequest = null;
        }
    }
}
