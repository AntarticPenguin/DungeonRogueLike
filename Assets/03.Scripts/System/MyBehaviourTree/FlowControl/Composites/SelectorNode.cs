using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 첫번째로 성공한 노드를 만날 때까지 실행
    /// </summary>
    public class SelectorNode : ITaskParent
    {
        public List<ITask> Children { get; set; } = new List<ITask>();
        public BehaviourTree RootTree { get; set; }
        public GameObject Owner { get; set; }
        public string Name { get; set; }

        public SelectorNode() { }
        public SelectorNode(string name)
        {
            Name = name;
        }

        public eTaskState Evaluate()
        {
            foreach(ITask node in Children)
            {
                eTaskState childState = node.Evaluate();
                if(eTaskState.FAILURE != childState)
                {
                    return childState;
                }
            }

            return eTaskState.FAILURE;
        }

        public void AddChild(ITask child)
        {
            Children.Add(child);
        }
    }
}

