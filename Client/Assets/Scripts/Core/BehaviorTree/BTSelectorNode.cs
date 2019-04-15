using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 10:38:31 
	* 描述： 选择节点（Selector）：组合节点，顺序执行子节点，只要碰到一个子节点返回TRUE，则返回TRUE；否则返回FALSE
	*************************/
    public class BTSelectorNode : BTNode
    {
        public BTSelectorNode() : base()
        {

        }

        public override bool Execute()
        {
            foreach (var node in nodes)
            {
                if (node.Execute()) return true;
            }
            return false;
        }
    }
}
