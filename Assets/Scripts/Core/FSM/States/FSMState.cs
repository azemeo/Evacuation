using UnityEngine;
using System.Collections;

public abstract class FSMState
{
    public delegate void StateErrorHandler(string error);

	public abstract int StateID { get; }

    protected FSM _parentFSM;
    public FSM ParentFSM
    {
        get
        {
            return _parentFSM;
        }
    }

    protected string _tag;
    public string Tag
    {
        get
        {
            return _tag;
        }
    }

    protected FSM _childFSM;
    protected StateErrorHandler _errorHandler;
    protected bool _isPaused;
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
    }
    protected bool _isComplete;
    public bool IsComplete
    {
        get
        {
            return _isComplete;
        }
    }



    public virtual void SetControllingFSM(FSM parent)
    {
        _parentFSM = parent;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }

    public virtual void Run()
    {
        if (_childFSM != null)
            _childFSM.Update();
    }

    public virtual void Restart()
    {
        _isComplete = false;
        _isPaused = false;
    }

    public virtual void SetPaused(bool value)
    {
        _isPaused = value;
    }

    protected virtual void stateComplete()
    {
		if(_isComplete)
			return;
		
        _isComplete = true;
        ParentFSM.StateComplete(this);
    }

    public virtual void OnApplicationFocus(bool focus)
    {
        if (_childFSM != null)
            _childFSM.OnApplicationFocus(focus);
    }

    public virtual void OnApplicationQuit()
    {
        if (_childFSM != null)
            _childFSM.OnApplicationQuit();
    }

    public void RegisterErrorHandler(StateErrorHandler errorHandler)
    {
        _errorHandler = errorHandler;
    }

    public void DeregisterErrorHandler()
    {
        _errorHandler = null;
    }

    public virtual void DrawDebug()
    {
        if (_childFSM != null && _childFSM.CurrentState != null)
        {
            _childFSM.CurrentState.DrawDebug();
        }
    }
}
