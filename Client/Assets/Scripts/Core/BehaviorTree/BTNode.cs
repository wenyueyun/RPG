using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Core.BehaviorTree
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/18 9:19:18 
	* 描述： 行为树
	*************************/
    public class BTNode
    {
        public string name;
        public BTNode root;
        public BTNode parent;
        public BTNode current;
        public List<BTNode> nodes = new List<BTNode>();
        public List<BTNode> runningNodes = new List<BTNode>();

        protected bool _running;
        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                if (value)
                {
                    if (!_running) root.runningNodes.Add(this);
                }
                else
                {
                    if (_running) root.runningNodes.Remove(this);
                }
                _running = value;
            }
        }

        public BTNode()
        {

        }

        public virtual void Init()
        {
            foreach (var node in nodes)
            {
                node.Init();
            }
        }

        public virtual void UnInit()
        {
            foreach (var node in nodes)
            {
                node.UnInit();
            }
        }

        public void Add(BTNode node)
        {
            nodes.Add(node);
            SetRootAndParent(root, this);
        }

        public void SetRootAndParent(BTNode root, BTNode parent)
        {
            this.root = root;
            this.parent = parent;
            foreach (var node in nodes)
            {
                node.SetRootAndParent(root, this);
            }
        }

        public BTNode Find(string name)
        {
            foreach (var node in nodes)
            {
                if (node.name == name) return node;
                var next = node.Find(name);
                if (next != null) return next;
            }
            return null;
        }

        public virtual bool Execute()
        {
            return true;
        }

        public void Call(Action<BTNode> action)
        {
            action(this);
            foreach (var node in nodes)
            {
                node.Call(action);
            }
        }

        public string ToTreeString()
        {
            string s = "Node: " + GetType().ToString() + "\n";
            foreach (var node in nodes)
            {
                s += node.ToTreeString().Replace(@"Node:", @"    Node:");
            }
            return s;
        }
    }
}
