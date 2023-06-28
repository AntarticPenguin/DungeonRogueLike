using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class ActionNode : TaskBase
    {
        private Func<eTaskState> _func;

        public ActionNode() { }
        public ActionNode(string name, Func<eTaskState> func) : base(name)
        {
            _func = func;
        }

        protected override eTaskState UpdateTask()
        {
            return _func();
        }
    }
}