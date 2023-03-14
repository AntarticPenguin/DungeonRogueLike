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

    private Vector3 _moveDirection;
    private float _turnSmoothVelocity;

    #region Properties
    public Animator Animator => _animator;
    #endregion

    private void Awake()
	{
		_controller = GetComponent<CharacterController>();
        _animator = _modelRoot.GetComponentInChildren<Animator>();
		_transform = transform;

        InitState();
	}

	private void Update()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

		if(inputDirection != Vector3.zero)
		{
			float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;        //Unity ClockWise
			float smoothAngle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
			_transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

			_moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
			_controller.Move(_moveDirection.normalized * Time.deltaTime * _moveSpeed);
		}
	}

    private void InitState()
    {
        _stateMachine = new StateMachine<PlayerController>(this, new PlayerIdleState());
        //_stateMachine.AddState()
    }
}
