using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 10:34:12 
	* 描述： 顺序节点（Sequence）：组合节点，顺序执行子节点，只要碰到一个子节点返回FALSE，则返回FALSE；否则返回TRUE 
	*************************/
    public class BTSequenceNode : BTNode
    {
        public BTSequenceNode() : base()
        {

        }

        public override bool Execute()
        {
            foreach (var node in nodes)
            {
                if (!node.Execute()) return false;
            }
            return true;
        }
    }
}
