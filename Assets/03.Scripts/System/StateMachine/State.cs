using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 호출 순서
/// OnInit(): 스테이트머신에 추가될 때, 최초로 호출.
/// OnStart(): 스테이트가 시작할 때 호출
/// OnUpdate(): 스테이트 업데이트(실행) 중
/// OnExit(): 스테이트가 바뀔 때, 호출 후 스테이트 변경
/// </summary>
public abstract class State<T>
{
	#region Variables
	protected StateMachine<T> _stateMachine;
	protected T _owner;
	#endregion

	#region Methods
	public void SetMachineAndOwner(StateMachine<T> stateMachine, T owner)
	{
		_stateMachine = stateMachine;
		_owner = owner;
		OnInitialized();
	}

	/// <summary>
	/// 스테이트 초기화 함수
	/// 스테이트머신에 추가될 때, SetStateMachineAndOwer함수를 통해 호출
	/// </summary>
	public virtual void OnInitialized()
	{ }

	public virtual void OnStart()
	{ }

	public virtual void Update(float deltaTime)
	{ }
	
	public virtual void FixedUpdate(float fixedDeltaTime)
	{ }

	public virtual void OnExit()
	{ }
	#endregion
}
