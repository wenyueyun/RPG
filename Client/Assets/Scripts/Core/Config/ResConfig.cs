using Game.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.Config
{
    /********************************
	* 作者：wenyueyun
	* 时间：2018/9/17 23:22:19
	* 描述：暂未添加描述 
	*********************************/

    public enum ResType
    {
        None = 0,
        Font = 1,
        Config = 2,
        Audio = 3,
        Atlas = 4,
        UI = 5,
        Model = 6,
        Map = 7,
    }

    public class ResConfig
    {
        public static string GetABPath(ResType type,string abName)
        {
            switch(type)
            {
                case ResType.Font:
                case ResType.Audio:
                    return abName;
                case ResType.Config:
                    return "config/data";
                case ResType.Atlas:
                    return string.Format("atlas/{0}", abName).ToLower();
                case ResType.UI:
                    return string.Format("ui/{0}", abName).ToLower();
                case ResType.Map:
                    return string.Format("map/{0}", abName).ToLower();
                case ResType.Model:
                    return string.Format("model/{0}", abName).ToLower();
                default:
                    LogUtil.LogRed("资源类型错误------------->"+(ResType)type);
                    return "";
            }
        }
    }
}
