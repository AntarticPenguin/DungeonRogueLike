using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 첫 실패한 노드를 만날 때까지 실행
    /// </summary>
    public class SequenceNode : ITaskParent
    {
        public List<ITask> Children { get; set; } = new List<ITask>();
        public BehaviourTree RootTree { get; set; }
        public GameObject Owner { get; set; }
        public string Name { get; set; }

        public SequenceNode() { }
        public SequenceNode(string name)
        {
            Name = name;
        }

        public eTaskState Evaluate()
        {
            foreach (ITask node in Children)
            {
                eTaskState childState = node.Evaluate();
                if (eTaskState.SUCCESS != childState)
                {
                    return childState;
                }
            }

            return eTaskState.SUCCESS;
        }

        public void AddChild(ITask child)
        {
            Children.Add(child);
        }
    }
}
