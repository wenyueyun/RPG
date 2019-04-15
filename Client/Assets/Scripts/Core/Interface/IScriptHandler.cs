using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Game.Core.Interface
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/6/26 15:24:33 
	* 描述： IScriptHandler 
	*************************/
    public interface IScriptHandler
    {
        T Instantiate<T>(string type, object[] args = null) where T : class;
        void LoadAssembly(string path, UnityAction assemblyLoad);
        void LoadAssembly(byte[] data, UnityAction assemblyLoad);
        object Invoke(string type, string method, object instance, params object[] args);
        void Initialize();
        void Uninitialize();
        void Update();
        void LateUpdate();
        void FixedUpdate();
    }
}
