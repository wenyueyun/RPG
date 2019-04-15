using UnityEngine;

namespace Game.Core.Common
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/7/5 10:49:40 
	* 描述： MonoSingleton 
	*************************/
    public class MonoSingleton<T> : MonoBehaviour
      where T : class
    {
        protected volatile static T _instance;
        public static T Instance
        {
            get { return _instance; }
        }
        protected virtual void Awake()
        {
            _instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}
