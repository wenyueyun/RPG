using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 10:43:23 
	* 描述： // 动作节点（Action）：叶节点，执行动作过程，一般返回TRUE 
	*************************/
    public class BTActionNode:BTNode
    {
        public BTActionNode():base()
        {

        }

        public override bool Execute()
        {
            Running = DoAction();
            return Running;
        }
        public virtual bool DoAction()
        {
            return true;
        }
    }
}
