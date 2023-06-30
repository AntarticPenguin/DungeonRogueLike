using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : State<PlayerController>
{
    private Animator _animator;
    private AnimationCallback _animationCallback;
    
    private int _attackAnimHash;
    private int _attackIndexHash;
    private int _animIndex;

    public override void OnInitialized()
    {
        _animator = _owner.Animator;
        _animationCallback = _owner.AnimationCallback;
        _attackAnimHash = Animator.StringToHash(ePlayerAnimState.ATTACK.ToString());
        _attackIndexHash = Animator.StringToHash("AttackIndex");
        _animIndex = 1;

        _animationCallback.InitCallback(
            null, 
            null,
            () =>
            {
                _stateMachine.ChangeState<PlayerIdleState>();
            });
    }

    public override void OnStart()
    {
        _animIndex = GetRandomAttackIndex();

        _animator.SetInteger(_attackIndexHash, _animIndex);
        _animator.SetBool(_attackAnimHash, true);

    }

    public override void OnExit()
    {
        _animator.SetBool(_attackAnimHash, false);
    }

    private int GetRandomAttackIndex()
    {
        //TODO: 플레이어 애니메이션 인포 관련 클래스 만들기
        return Random.Range(1, 4);
    }
}
