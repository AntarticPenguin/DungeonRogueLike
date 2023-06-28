using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class WaitNode : TaskBase
    {
        private float _waitTime = 1.0f;
        private float _duration = 0.0f;

        public WaitNode(string name, float waitTime) : base(name)
        {
            Name = name;
            _waitTime = waitTime;
        }

        protected override eTaskState UpdateTask()
        {
            _duration += Time.deltaTime;

            if(_duration > _waitTime)
            {
                _duration = 0.0f;
                return eTaskState.SUCCESS;
            }

            return eTaskState.RUNNING;
        }
    }
}