using UnityEngine;
using System.Collections;

public class GameStateManager : SingletonBehavior<GameStateManager>
{
	public delegate void StateChangeEvent(int previousStateType, int newStateType);
	public delegate void StateErrorOccurred(int failedStateType, string error);

	public static event StateChangeEvent onStateChanged;
	public static event StateErrorOccurred onStateErrorOccurred;

	private FSM _fsm;

	public int CurrentStateType
    {
        get
        {
			return _fsm.CurrentState.StateID;
        }
    }

	public int PreviousStateType
    {
        get
        {
			return _fsm.PreviousState != null ? _fsm.PreviousState.StateID : FSMStateTypes.Game.EMPTY;
        }
    }

	protected override void Init ()
	{
		base.Init ();

		// create a new FSM and register to its events
		_fsm = new FSM(new EmptyState());

		_fsm.onStateChanged += OnFSMStateChanged;
		_fsm.onStateErrorOccurred += OnFSMStateErrorOccurred;
	}

	void Update()
	{
		if(_fsm != null)
			_fsm.Update();
	}

	void OnDestroy()
	{
		// being a singleton sometimes there might be duplicates that gets destroyed as soon as the scene is loaded, so we deregister from events only if the FSM was created otherwise we get NullRef exceptions.
		if(_fsm != null)
		{
			_fsm.onStateChanged -= OnFSMStateChanged;
			_fsm.onStateErrorOccurred -= OnFSMStateErrorOccurred;
		}
	}

	void OnApplicationFocus(bool focus)
	{
		if(_fsm != null)
			_fsm.OnApplicationFocus(focus);
	}

	void OnApplicationQuit()
	{
		if(_fsm != null)
			_fsm.OnApplicationQuit();
	}

	#region FSM EVENTS // we just propagate the FSM events here

	void OnFSMStateErrorOccurred (FSMState failedState, string error)
	{
        //this should be safe becuase you can not set a FSMState on this manager that is not a GameState
        if (onStateErrorOccurred != null)
			onStateErrorOccurred(failedState.StateID, error);
	}

	void OnFSMStateChanged (FSMState previousState, FSMState currentState)
	{
		if(onStateChanged != null)
			onStateChanged(previousState.StateID, currentState.StateID);
	}

	#endregion

    public void SetState(GameState newState)
    {
		_fsm.SetState(newState);
    }

	public void RevertToPreviousState()
	{
		_fsm.RevertToPreviousState();
	}
}

