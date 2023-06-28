using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class InverterNode : DecoratorBase
    {
        protected override eTaskState UpdateTask()
        {
            eTaskState taskState = Children[0].Evaluate();

            switch (taskState)
            {
                case eTaskState.SUCCESS:
                    taskState = eTaskState.FAILURE;
                    break;
                case eTaskState.FAILURE:
                    taskState = eTaskState.SUCCESS;
                    break;
            }

            return taskState;
        }
    }
}