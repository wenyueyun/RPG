using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Core.Common;
using Game.Core.Interface;
using Game.Core.Script;
using System.Collections;
using System.IO;
using UnityEngine;
using Game.Core.Util;

namespace Game.Core.Manager
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/6/26 15:22:05 
	* 描述： ScriptManager 
	*************************/
    public class ScriptManager : Singleton<ScriptManager>
    {
        private IScriptHandler script = null;
        private readonly string assemblyName = "HotFix.dll";

        public void Initialize()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            script = new ReflectionHandler();
#else
            script = new ILRuntimeHandler();
#endif

            if (script != null)
            {
                script.Initialize();
                Client.Instance.StartCoroutine(LoadDll());
            }
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        private IEnumerator LoadDll()
        {
            LogUtil.Log("HotFix.开始加载Dll");
            string scriptPath = Path.Combine(Application.persistentDataPath, assemblyName);
            if(File.Exists(scriptPath))
            {
                script.LoadAssembly(scriptPath, () =>
                 {
                     LogUtil.LogYellow("热更文件加载完成");
                 });
            }
            else
            {
                string url;
                if (Application.platform == RuntimePlatform.Android)
                    url =Path.Combine(Application.streamingAssetsPath, assemblyName);
                else
                    url ="file:///" + Path.Combine(Application.streamingAssetsPath, assemblyName);

                LogUtil.Log("DllUrl------>"+url);
                WWW www = new WWW(url);
                yield return www;

                while(www.isDone)
                {
                    if(string.IsNullOrEmpty(www.error))
                    {
                        byte[] bytes = new byte[www.bytes.Length];
                        System.Buffer.BlockCopy(www.bytes, 0, bytes, 0, bytes.Length);
                        www.Dispose();
                        script.LoadAssembly(bytes, () =>
                        {
                            LogUtil.LogYellow("热更文件加载完成");
                        });
                    }
                    else
                    {
                        LogUtil.LogRed(www.error);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 实例化某个类
        /// </summary>
        public T Instantiate<T>(string type, object[] args = null) where T : class
        {
            return script.Instantiate<T>(type,args);
        }

        /// <summary>
        /// 执行某个方法
        /// </summary>

        public object Invoke(string type, string method, object instance, params object[] args)
        {
            return script.Invoke(type,method,instance,args);
        }

        public void UnInitialize()
        {
            if (script != null)
            {
                script.Uninitialize();
            }
        }

        public void Update()
        {
            if(script != null)
            {
                script.Update();
            }
        }

        public void LateUpdate()
        {
            if(script != null)
            {
                script.LateUpdate();
            }
        }

        public void FixedUpdate()
        {
            if (script != null)
            {
                script.FixedUpdate();
            }
        }
    }
}
