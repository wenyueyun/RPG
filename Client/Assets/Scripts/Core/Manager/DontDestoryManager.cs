using Game.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Core.Manager
{
    /********************************
	* 作者：wenyueyun
	* 时间：2018/9/18 22:04:10
	* 描述：暂未添加描述 
	*********************************/
    public class DontDestoryManager : Singleton<DontDestoryManager>
    {
        private Canvas canvas;
        public Canvas Canvas
        {
            get
            {
                return canvas;
            }
        }

        private Camera uicamera;
        public Camera UICamera
        {
            get
            {
                return uicamera;
            }
        }

        public void Initialize()
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            GameObject.DontDestroyOnLoad(canvas);

            uicamera = GameObject.Find("UICamera").GetComponent<Camera>();
        }
    }
}