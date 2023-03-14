using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : State<PlayerController>
{
    private Animator _animator;
    private int _moveAnimHash;

    private Transform _transform;
    private Vector3 _moveDirection;
    private float _turnSmoothVelocity;

    private CharacterController _controller;

    public override void OnInitialized()
    {
        _transform = _owner.transform;
        _animator = _owner.Animator;
        _controller = _owner.GetComponent<CharacterController>();
        _moveAnimHash = Animator.StringToHash(ePlayerAnimState.MOVE.ToString());
    }

    public override void OnStart()
    {
        _animator.SetBool(_moveAnimHash, true);
    }

    public override void Update(float deltaTime)
    {
        Vector3 inputDirection = _owner.InputDirection;
        if (inputDirection == Vector3.zero)
        {
            _stateMachine.ChangeState<PlayerIdleState>();
            return;
        }

        if(_owner.IsPressAttack)
        {
            _stateMachine.ChangeState<PlayerAttackState>();
            return;
        }

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;        //Unity ClockWise
        float smoothAngle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _owner.TurnSmoothTime);
        _transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        _controller.Move(_moveDirection.normalized * Time.deltaTime * _owner.MoveSpeed);
    }

    public override void OnExit()
    {
        _animator.SetBool(_moveAnimHash, false);
    }
}
