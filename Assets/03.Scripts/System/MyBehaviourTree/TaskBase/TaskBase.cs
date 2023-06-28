using MyBehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBase : ITask
{
    public BehaviourTree RootTree { get; set; }
    public GameObject Owner { get; set; }
    public string Name { get; set; }

    public TaskBase() { }
    public TaskBase(string name)
    {
        Name = name;
    }

    public eTaskState Evaluate()
    {
        if(null == RootTree)
        {
            return eTaskState.FAILURE;
        }

        return UpdateTask();
    }

    protected virtual eTaskState UpdateTask()
    {
        return eTaskState.FAILURE;
    }
}
