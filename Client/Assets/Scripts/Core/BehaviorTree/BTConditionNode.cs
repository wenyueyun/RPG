using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 10:41:13 
	* 描述： 条件节点（Condition）：叶节点，执行条件判断，返回判断结果
	*************************/
    public class BTConditionNode:BTNode
    {
        public BTConditionNode():base()
        {

        }

        public override bool Execute()
        {
            return DoCheck(); 
        }
        public virtual bool DoCheck()
        {
            return true;
        }
    }
}
