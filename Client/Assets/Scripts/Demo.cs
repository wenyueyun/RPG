using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2019/4/16 17:00:25 
	* 描述： Demo 
	*************************/
    public class Demo
    {
        private Text text;

        private void Test()
        {
            text.DOText("aa", 1);
        }
    }
}
