using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Code.Core.Res
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/8/27 16:55:20 
	* 描述： AssetBundleData 
	*************************/
    public class AssetBundleData
    {
        public AssetBundle ab;
        public int ReferencedCount;
        public AssetBundleData(AssetBundle assetBundle, uint referencedCount)
        {
            ab = assetBundle;
            ReferencedCount = (int)referencedCount;
        }

        public void Dispose()
        {
            if(ab != null)
            {
                ab.Unload(true);
                ab = null;
            }
        }
    }
}
