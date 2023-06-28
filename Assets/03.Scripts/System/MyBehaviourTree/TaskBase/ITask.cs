using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public interface ITask
    {
        BehaviourTree RootTree { get; set; }
        GameObject Owner { get; set; }
        string Name { get; set; }
        eTaskState Evaluate();
    }

}