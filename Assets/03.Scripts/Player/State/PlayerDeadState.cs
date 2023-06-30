using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : State<PlayerController>
{
    private Animator _animator;
    private int _animHash;
    private int _animIndexHash;
    private int _animIndex;

    public override void OnInitialized()
    {
        _animator = _owner.Animator;
        _animHash = Animator.StringToHash(ePlayerAnimState.DEAD.ToString());
        _animIndexHash = Animator.StringToHash("DeadIndex");
        _animIndex = 1;
    }

    public override void OnStart()
    {
        _animIndex = GetRandomDeadIndex();
        _animator.SetInteger(_animIndexHash, _animIndex);
        _animator.SetTrigger(_animHash);
    }

    private int GetRandomDeadIndex()
    {
        return Random.Range(1, 3);
    }
}
