using UnityEngine;
using System;

namespace Game.Core.Util
{
    /************************* 
    * 作者： wenyueyun 
    * 时间： 2018/4/25 10:29:01 
    * 描述： ColorUtil 
    *************************/
    public static class ColorUtil
    {
        /// <summary>
        /// color 转换hex
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }

        /// <summary>
        /// hex转换到color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string format)
        {
            if (format.StartsWith("#"))
            {
                format = format.Replace("#", "").Trim();
            }
            Color color = Color.white;
            color.r = Convert.ToInt16("0x" + format.Substring(0, 2).ToString(), 16) / 255f;
            color.g = Convert.ToInt16("0x" + format.Substring(2, 2).ToString(), 16) / 255f;
            color.b = Convert.ToInt16("0x" + format.Substring(4, 2).ToString(), 16) / 255f;
            if (format.Length == 8)
            {
                color.a = Convert.ToInt16("0x" + format.Substring(6, 2).ToString(), 16) / 255f;
            }
            return color;
        }
    }
}
