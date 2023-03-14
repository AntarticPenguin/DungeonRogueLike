using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : State<PlayerController>
{
    private Animator _animator;
    private int _idleAnimHash;

    public override void OnInitialized()
    {
        _animator = _owner.Animator;
        _idleAnimHash = Animator.StringToHash(ePlayerAnimState.IDLE.ToString());
    }

    public override void OnStart()
    {
        _animator.SetBool(_idleAnimHash, true);
    }

    public override void Update(float deltaTime)
    {
        if (_owner.InputDirection != Vector3.zero)
        {
            _stateMachine.ChangeState<PlayerMoveState>();
        }
        else if(_owner.IsPressAttack)
        {
            _stateMachine.ChangeState<PlayerAttackState>();
        }
    }

    public override void OnExit()
    {
        _animator.SetBool(_idleAnimHash, false);
    }
}
