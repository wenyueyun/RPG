
using System;
using Object = UnityEngine.Object;

namespace Game.Code.Core.Res
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/8/27 16:23:19 
	* 描述： AssetBundleLoadOperation  抽象类
	*************************/
    public abstract class AssetBundleLoadOperation
    {
        /// <summary>
        /// AB包名字
        /// </summary>
        protected string abName;
        public string ABName
        {
            get
            {
                return abName;
            }
        }

        /// <summary>
        /// 资源名字
        /// </summary>
        protected string assetName;
        public string AssetName
        {
            get
            {
                return assetName;
            }
        }

        /// <summary>
        /// 加载成功
        /// </summary>
        protected Action<AssetBundleData> loadComplete;
        public Action<AssetBundleData> LoadComplete
        {
            get
            {
                return loadComplete;
            }
            set
            {
                loadComplete = null;
            }
        }

        /// <summary>
        /// 加载失败
        /// </summary>
        protected Action<string> loadError;
        public Action<string> LoadError
        {
            get
            {
                return loadError;
            }
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        protected Type assetType;
        public Type AssetType
        {
            get
            {
                return assetType;
            }
        }

        /// <summary>
        /// 加载进度
        /// </summary>
        abstract public float Progress
        {
            get;
        }

        /// <summary>
        /// 加载更新
        /// </summary>
        abstract public bool Update();

        /// <summary>
        /// 是否加载完成
        /// </summary>
        abstract public bool IsDone();

        /// <summary>
        /// 加载到的资源
        /// </summary>
        public abstract Object GetAsset();

        public abstract AssetBundleData GetABData();
       
    }
}
