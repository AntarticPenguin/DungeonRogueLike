using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class DecoratorBase : ITaskParent
    {
        private int MAX_CHILDREN = 1;

        #region Properties
        public List<ITask> Children { get; set; } = new List<ITask>(1);
        public BehaviourTree RootTree { get; set; }
        public GameObject Owner { get; set; }
        public string Name { get; set; }
        #endregion

        public void AddChild(ITask child)
        {
            if (Children.Count > MAX_CHILDREN)
            {
                Debug.LogError("Decorator:: You cannot add child more than one.");
                return;
            }

            Children.Add(child);
        }

        public eTaskState Evaluate()
        {
            return UpdateTask();
        }

        protected virtual eTaskState UpdateTask()
        {
            return eTaskState.FAILURE;
        }
    }
}