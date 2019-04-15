using System;
using UnityEngine;

namespace Game.Core.Common
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 9:41:29 
	* 描述： 泛型单例模版
	*************************/
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static readonly object synclock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (synclock)
                    {
                        _instance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                    }
                }
                return _instance;
            }
        }
    }
}
