using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StateMachine<T>
{
	#region Variables
	private T _owner;
	private State<T> _currentState;
	private State<T> _previousState;
	private float _elapsedTime = 0.0f;
	private Dictionary<System.Type, State<T>> _states = new Dictionary<System.Type, State<T>>();
	#endregion

	#region Properties
	public State<T> CurrentState => _currentState;
	public State<T> PreviousState => _previousState;
    public float ElapsedTime => _elapsedTime;
	#endregion

	#region Methods
	public StateMachine(T owner, State<T> initialState)
	{
		_owner = owner;

		AddState(initialState);
		_currentState = initialState;

		_currentState.OnStart();
	}

	public void AddState(State<T> state)
	{
		state.SetMachineAndOwner(this, _owner);
		_states.Add(state.GetType(), state);
	}

	public void Update(float deltaTime)
	{
		_elapsedTime += deltaTime;
		_currentState.Update(deltaTime);
	}

	public void FixedUpdate(float fixedDeltaTime)
	{
		_currentState.FixedUpdate(fixedDeltaTime);
	}

	public void ChangeState<R>() where R : State<T>
	{
		var newType = typeof(R);
		if (_currentState.GetType() == newType)
			return;

		if(_currentState != null)
		{
			_currentState.OnExit();
		}

		#if UNITY_EDITOR
		if(!_states.ContainsKey(newType))
		{
			var error = newType + ": state does not exist.";
			throw new System.Exception(error);
		}
		#endif

		_previousState = _currentState;
		_currentState = _states[newType];
		_currentState.OnStart();
		_elapsedTime = 0.0f;
	}

	public void ChangeToPreviousState()
	{
		if(_currentState != null)
		{
			_currentState.OnExit();
		}

		var targetState = _states[_previousState.GetType()];
		_previousState = _currentState;
		_currentState = _states[targetState.GetType()];

		_currentState.OnStart();
		_elapsedTime = 0.0f;
	}
	#endregion
}
