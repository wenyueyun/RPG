using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.Util
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/5/23 14:12:52
	* 描述： HtmlUtil 
	*************************/
    public class HtmlUtil
    {
        public static string Color(string msg, string color)
        {
            return string.Format("<color={0}>{1}</color>", color, msg);
        }

        public static string ColorSize(string msg, string color, int size)
        {
            return string.Format("<color={0}><size={1}>{2}</size></color>", color, size, msg);
        }
    }
}
