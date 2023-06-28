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
            SyncNode(Root);
        }

        public void Tick()
        {
            Root.Evaluate();
        }

        private void SyncNode(ITaskParent parent)
        {
            parent.Owner = _owner;
            parent.RootTree = this;

            foreach(ITask child in parent.Children)
            {
                child.Owner = _owner;
                child.RootTree = this;

                var parentTask = child as ITaskParent;
                if(null != parentTask)
                {
                    SyncNode(parentTask);
                }
            }
        }
    }
}
