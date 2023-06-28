using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class TaskRoot : ITaskParent
    {
        private int MAX_CHILDREN = 1;

        #region Properties
        public List<ITask> Children { get; set; } = new List<ITask>(1);
        public BehaviourTree RootTree { get; set; }
        public GameObject Owner { get; set; }
        public string Name { get; set; }
        #endregion

        public TaskRoot() { }
        public TaskRoot(string name) : base()
        {
            Name = name;
        }

        public void AddChild(ITask child)
        {
            if (Children.Count > MAX_CHILDREN)
            {
                Debug.LogError("TaskRoot:: You cannot add child more than one.");
                return;
            }
                

            Children.Add(child);
        }

        public eTaskState Evaluate()
        {
            if(Children.Count == 0)
            {
                return eTaskState.SUCCESS;
            }

            return Children[0].Evaluate();
        }
    }
}