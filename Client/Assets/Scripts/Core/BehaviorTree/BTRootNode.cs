using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 10:45:38 
	* 描述： 根节点 
	*************************/
    public class BTRootNode:BTSelectorNode
    {
        public BTRootNode():base()
        {
            this.name = "Root";
            this.root = this;
        }

        public override bool Execute()
        {
            if(runningNodes.Count > 0)
            {
                return runningNodes[runningNodes.Count -1].Execute();
            }
            return base.Execute();
        }
    }
}
