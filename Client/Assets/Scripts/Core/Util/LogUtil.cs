using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Core.Util
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/6/5 9:12:33 
	* 描述： LogUtil 
	*************************/
    public class LogUtil
    {
        private static bool debug = true;
        public static void Log(object msg)
        {
            if (!debug) return;
            Debug.Log(msg);
        }

        public static void LogError(object msg)
        {
            if (!debug) return;
            Debug.LogError(msg);
        }

        public static void LogWarning(object msg)
        {
            if (!debug) return;
            Debug.LogWarning(msg);
        }

        public static void LogColor(Color color, object msg)
        {
            if (!debug) return;
            Debug.Log(string.Format("<color=#{0}>{1}</color>", ColorUtil.ColorToHex(color), msg));
        }

        public static void LogRed(object msg)
        {
            LogColor(Color.red, msg);
        }

        public static void LogYellow(object msg)
        {
            LogColor(Color.yellow, msg);
        }

        public static void LogGreen(object msg)
        {
            LogColor(Color.green, msg);
        }
    }
}
