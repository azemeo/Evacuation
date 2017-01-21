using UnityEngine;
using System.Collections;

public class FSM {

	public delegate void StateChangeEvent(FSMState previousState, FSMState newState);
	public delegate void StateErrorOccurred(FSMState failedState, string error);
    public delegate void StateCompleteEvent(FSMState completedState);

	public event StateChangeEvent onStateChanged;
	public event StateErrorOccurred onStateErrorOccurred;
    public event StateCompleteEvent onStateComplete;

	public FSMState CurrentState
	{
		get
		{
			return _currentState;
		}
	}

	public FSMState PreviousState
	{
		get
		{
			return _previousState;
		}
	}

	private FSMState _currentState;
	private FSMState _previousState;

	private FSMState _nextState;

	protected FSM() {}

	public FSM(FSMState initialState)
	{
        SetState(initialState);
	}

	public void Update()
	{
        if (_currentState != null)
        {
            if (!_currentState.IsPaused && !_currentState.IsComplete)
            {
                _currentState.Run();
            }
        }
	}

	public void SetState(FSMState newState)
	{
		if (newState == _currentState)
		{
			Debug.LogWarning("Already in state " + _currentState.GetType().ToString());
			return;
		}

        if (newState == null)
        {
            Debug.LogWarning("Can not set a null FSMState.");
            return;
        }

		_nextState = newState;
        _nextState.SetControllingFSM(this);
		DoStateChange();
	}

	public void RevertToPreviousState()
	{
		if(_previousState != null)
		{
			SetState(_previousState);
			_previousState = _currentState;
		}
		else
			Debug.LogWarning("FSM: Cannot revert to previous state because it's null!");
	}

	public void OnApplicationFocus(bool focus)
	{
		_currentState.OnApplicationFocus(focus);
	}

	public void OnApplicationQuit()
	{
		_currentState.OnApplicationQuit();
	}

	private void DoStateChange()
	{
		if (_currentState != null)
		{
			_currentState.ExitState();
			_currentState.DeregisterErrorHandler();
		}

		_previousState = _currentState;
		_currentState = _nextState;

		_currentState.RegisterErrorHandler(ErrorOccurred);
		_currentState.EnterState();

		_nextState = null;

		if (onStateChanged != null && _previousState != null)
		{
			onStateChanged(_previousState, _currentState);
		}
	}

    public void StateComplete(FSMState completedState)
    {
        if (onStateComplete != null)
        {
            onStateComplete(completedState);
        }
    }

	private void ErrorOccurred(string error)
	{
		if (onStateErrorOccurred != null)
		{
			onStateErrorOccurred(_currentState, error);
		}
	}

#if UNITY_EDITOR
    public void DrawDebug()
    {
        if (_currentState != null)
        {
            if (!_currentState.IsPaused && !_currentState.IsComplete)
            {
                _currentState.DrawDebug();
            }
        }
    }
#endif
}
