using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MyBehaviourTree
{
    public class BehaviourTreeBuilder
    {
        private BehaviourTree _tree;
        private Stack<ITaskParent> _parentNodeStack = new Stack<ITaskParent>();

        public BehaviourTreeBuilder(GameObject owner)
        {
            _tree = new BehaviourTree(owner);
            _parentNodeStack.Push(_tree.Root);
        }

        public BehaviourTreeBuilder Sequence(string name = "Sequence")
        {
            SequenceNode sequenceNode = new SequenceNode(name);
            
            if(_parentNodeStack.Count > 0)
            {
                _parentNodeStack.Peek().AddChild(sequenceNode);
            }

            _parentNodeStack.Push(sequenceNode);
            return this;
        }

        public BehaviourTreeBuilder Selector(string name = "Selector")
        {
            SelectorNode selectorNode = new SelectorNode(name);

            if(_parentNodeStack.Count > 0)
            {
                _parentNodeStack.Peek().AddChild(selectorNode);
            }

            _parentNodeStack.Push(selectorNode);
            return this;
        }

        public BehaviourTreeBuilder Do(string name, Func<eTaskState> func)
        {
            if(_parentNodeStack.Count <= 0)
            {
                throw new Exception("Tree has no root node. Check your script");
            }

            ActionNode actionNode = new ActionNode(name, func);
            _parentNodeStack.Peek().AddChild(actionNode);

            return this;
        }

        public BehaviourTreeBuilder Wait(float waitTime)
        {
            if (_parentNodeStack.Count <= 0)
            {
                throw new Exception("Tree has no root node. Check your script");
            }

            WaitNode waitNode = new WaitNode("Wait", waitTime);
            _parentNodeStack.Peek().AddChild(waitNode);

            return this;
        }

        public BehaviourTreeBuilder Splice(BehaviourTree subTree)
        {
            if(null == subTree)
            {
                Debug.LogError("Cannot add subtree as null");
                return this;
            }

            if (_parentNodeStack.Count > 0)
            {
                _parentNodeStack.Peek().AddChild(subTree.Root);
            }

            return this;
        }

        public BehaviourTreeBuilder End()
        {
            _parentNodeStack.Pop();
            return this;
        }

        public BehaviourTree Build()
        {
            _tree.SyncNode(_tree.Root);
            return _tree;
        }
    }
}