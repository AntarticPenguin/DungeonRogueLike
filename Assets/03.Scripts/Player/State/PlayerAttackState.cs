using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : State<PlayerController>
{
    private Animator _animator;
    private int _attackAnimHash;

    //임시 코드
    private float _duration;

    public override void OnInitialized()
    {
        _animator = _owner.Animator;
        _attackAnimHash = Animator.StringToHash(ePlayerAnimState.ATTACK.ToString());
    }

    public override void OnStart()
    {
        _animator.SetBool(_attackAnimHash, true);
        _duration = 0.0f;
    }

    public override void Update(float deltaTime)
    {
        //임시 코드
        _duration += deltaTime;
        if(_duration >= 2.5f)
        {
            _stateMachine.ChangeState<PlayerIdleState>();
        }
    }

    public override void OnExit()
    {
        _animator.SetBool(_attackAnimHash, false);
    }
}
