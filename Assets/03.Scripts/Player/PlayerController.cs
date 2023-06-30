using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
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

    #region Properties
    public Animator Animator => _animator;
    public AnimationCallback AnimationCallback { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    public float MoveSpeed => _moveSpeed;
    public float TurnSmoothTime => _turnSmoothTime;
    #endregion

    private void Awake()
	{
		_controller = GetComponent<CharacterController>();
        _animator = _modelRoot.GetComponentInChildren<Animator>();
        AnimationCallback = _modelRoot.GetComponentInChildren<AnimationCallback>();
        PlayerInput = GetComponent<PlayerInput>();
        _transform = transform;

        InitState();
	}

	private void Update()
	{
        _stateMachine.Update(Time.deltaTime);
	}

    private void InitState()
    {
        _stateMachine = new StateMachine<PlayerController>(this, new PlayerIdleState());
        _stateMachine.AddState(new PlayerAttackState());
        _stateMachine.AddState(new PlayerMoveState());
    }

    private void OnGUI()
    {
        if(null != _stateMachine)
        {
            GUI.Label(new Rect(10, 10, 150, 50), _stateMachine.CurrentState.GetType().Name);
        }
    }
}
