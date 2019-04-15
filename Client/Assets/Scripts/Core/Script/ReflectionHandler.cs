using System;
using UnityEngine.Events;
using Game.Core.Interface;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace Game.Core.Script
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/6/26 15:29:52 
	* 描述： ReflectionHandler 
	*************************/
    public class ReflectionHandler : IScriptHandler
    {
        private Assembly assembly;
        private object gameMain;
        //加载本地程序集
        public void LoadAssembly(string path, UnityAction assemblyLoad)
        {
            Debug.Log("ReflectionHandler LoadAssembly-->" + path);
            //-----------加载程序集-------------
            byte[] bytes = null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
            }

            //-----------开始执行程序进入游戏-------------
            assembly = Assembly.Load(bytes);

            gameMain = Invoke("Game.HotFix.GameMain", "Instance", null, null);
            Invoke("Game.HotFix.GameMain", "Initialize", gameMain, null);

            //-----------执行回调方法-------------
            if (assemblyLoad != null)
            {
                assemblyLoad();
            }
        }

        //加载服务器更新下来的程序集
        public void LoadAssembly(byte[] data, UnityAction assemblyLoad)
        {
            Debug.Log("ReflectionHandlerLoadAssembly-->" + "data");
            //-----------开始执行程序进入游戏-------------
            assembly = Assembly.Load(data);

            gameMain = Invoke("Game.HotFix.GameMain", "Instance", null, null);
            Invoke("Game.HotFix.GameMain", "Initialize", gameMain, null);

            //-----------执行回调方法-------------
            if (assemblyLoad != null)
            {
                assemblyLoad();
            }
        }

        //实例化某个类
        public T Instantiate<T>(string type, object[] args = null) where T : class
        {
            Type t = assembly.GetType(type, false, true);
            if(t!= null)
            {
                return Activator.CreateInstance(t, args) as T;
            }
            return default(T);
        }

        // 执行某个方法
        public object Invoke(string type, string method, object instance, params object[] args)
        {
            Type t = assembly.GetType(type, false, true);
            MethodInfo m = t.GetMethod(method);
            return m.Invoke(instance, args);
        }

        public void Initialize()
        {
        }

        //update
        public void Update()
        {
            if(gameMain != null)
            {
                Invoke("Game.HotFix.GameMain", "Update", gameMain, null);
            }
        }

        //LateUpdate
        public void LateUpdate()
        {
            if (gameMain != null)
            {
                Invoke("Game.HotFix.GameMain", "LateUpdate", gameMain, null);
            }
        }

        //FixedUpdate
        public void FixedUpdate()
        {
            if (gameMain != null)
            {
                Invoke("Game.HotFix.GameMain", "FixedUpdate", gameMain, null);
            }
        }

        public void Uninitialize()
        {
            if(assembly != null)
            {
                assembly = null;
            }
        }
    }
}
