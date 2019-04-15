using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.Util
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/5/8 16:26:04 
	* 描述： PathUtil 
	*************************/
    public class PathUtil
    {
        public  static string CutFilePath(string path,int value)
        {
            StringBuilder sb = new StringBuilder();
            string[] temps = path.Split('/');
            for (int i = 0; i < temps.Length - value; i++)
            {
                sb.Append(temps[i]+"/");
            }
            return sb.ToString();
        }
    }
}
