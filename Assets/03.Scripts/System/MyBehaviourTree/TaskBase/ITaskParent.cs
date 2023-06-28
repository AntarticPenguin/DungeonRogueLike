using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public interface ITaskParent : ITask
    {
        List<ITask> Children { get; set; }
        void AddChild(ITask child);
    }
}


