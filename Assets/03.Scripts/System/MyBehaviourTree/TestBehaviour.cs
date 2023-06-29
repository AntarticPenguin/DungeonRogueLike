using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;

public class TestBehaviour : MonoBehaviour
{
    private BehaviourTree tree1;
    private BehaviourTree tree2;
    private BehaviourTree tree3;
    private BehaviourTree waitTest;

    private void Start()
    {
        SetupTree();
    }

    private void Update()
    {
        //tree1.Tick();
        //tree2.Tick();
        waitTest.Tick();
    }

    private void SetupTree()
    {
        tree1 = new BehaviourTreeBuilder(gameObject)
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

        tree2 = new BehaviourTreeBuilder(gameObject)
            .Sequence()
                .Do("Action1", () => {
                    Debug.Log("Called Action1");
                    return eTaskState.FAILURE;
                })
                .Do("Action2", () => {
                    Debug.Log("Called Action2");
                    return eTaskState.SUCCESS;
                })
            .End()
            .Build();

        tree3 = new BehaviourTreeBuilder(gameObject)
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

        waitTest = new BehaviourTreeBuilder(gameObject)
            .Sequence()
                .Do("Log Action", () =>
                {
                    Debug.Log("Wait Start");
                    return eTaskState.SUCCESS;
                })
                .Wait(5.0f)
                .Do("Log Action", () =>
                {
                    Debug.Log("Wait End");
                    return eTaskState.SUCCESS;
                })
            .End()
            .Build();
    }
}
