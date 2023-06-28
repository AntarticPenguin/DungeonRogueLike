using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class BehaviourTree
    {
        private GameObject _owner;
        //Blackboard _dataContext;

        public TaskRoot Root { get; } = new TaskRoot();

        public BehaviourTree() {}

        public BehaviourTree(GameObject owner)
        {
            _owner = owner;
        }

        public void Tick()
        {
            Root.Evaluate();
        }

        /// <summary>
        /// Setup Root Tree
        /// </summary>
        public void SyncNode(ITaskParent taskParent)
        {
            taskParent.Owner = _owner;
            taskParent.RootTree = this;

            foreach(ITask child in taskParent.Children)
            {
                child.Owner = _owner;
                child.RootTree = this;

                var parent = child as ITaskParent;
                if(null != parent)
                {
                    SyncNode(parent);
                }
            }
        }
    }
}
