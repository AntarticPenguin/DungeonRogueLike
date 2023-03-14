using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Inspector
    [SerializeField] Transform _modelRoot;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _turnSmoothTime = 0.1f;
    #endregion

    private CharacterController _controller;
	private Transform _transform;
    private Animator _animator;
    private StateMachine<PlayerController> _stateMachine;

    private Vector3 _inputDirection;
    private bool _isPressAttack;

    #region Properties
    public Animator Animator => _animator;
    public Vector3 InputDirection => _inputDirection;
    public bool IsPressAttack => _isPressAttack;
    public float MoveSpeed => _moveSpeed;
    public float TurnSmoothTime => _turnSmoothTime;
    #endregion

    private void Awake()
	{
		_controller = GetComponent<CharacterController>();
        _animator = _modelRoot.GetComponentInChildren<Animator>();
		_transform = transform;

        _inputDirection = Vector3.zero;

        InitState();
	}

	private void Update()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
        _isPressAttack = Input.GetMouseButtonDown(0);
        _inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        _stateMachine.Update(Time.deltaTime);
	}

    private void InitState()
    {
        _stateMachine = new StateMachine<PlayerController>(this, new PlayerIdleState());
        _stateMachine.AddState(new PlayerAttackState());
        _stateMachine.AddState(new PlayerMoveState());
    }
}
