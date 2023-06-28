using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;

public class TestBehaviour : MonoBehaviour
{
    private void Start()
    {
        SetupTree();
    }

    private void SetupTree()
    {
        BehaviourTree tree = new BehaviourTreeBuilder(gameObject)
            .Sequence()
                .Do("Action1", () => { 
                    Debug.Log("Called Action1");
                    return eTaskState.SUCCESS;
                })
                .Do("Action2", () => {
                    Debug.Log("Called Action2");
                    return eTaskState.SUCCESS;
                })
            .End()
            .Build();

        BehaviourTree tree2 = new BehaviourTreeBuilder(gameObject)
            .Sequence()
                .Selector()
                    .Do("Action1", () => {
                        Debug.Log("Called Action1");
                        return eTaskState.SUCCESS;
                    })
                    .Do("Action2", () => {
                        Debug.Log("Called Action2");
                        return eTaskState.SUCCESS;
                    })
                .End()
            .End()
            .Build();
    }
}
