using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 10:36:21 
	* 描述： 并行节点（Parallel）：顺序执行所有子节点，无论如何都返回true 
	*************************/
    public class BTParallelNode : BTNode
    {
        public BTParallelNode() : base()
        {

        }

        public override bool Execute()
        {
            foreach (var node in nodes)
            {
                node.Execute();
            }
            return true;
        }
    }
}
