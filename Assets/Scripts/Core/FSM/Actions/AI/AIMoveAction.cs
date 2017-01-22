using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class AIMoveAction : AIState {

	public override int StateID
	{
		get
		{
			return FSMStateTypes.AI.MOVE;
		}
	}

    private Vector3 _dest;
	protected Vector3[] _waypoints;
	protected int _currentWaypointIndex;

    protected float _normalizedSpeed = 0;

    protected AIAgent _cachedAgent;
    protected Vector3 _facingDirection = Vector3.forward;
    protected Quaternion _lookRotation = Quaternion.LookRotation(Vector3.forward);
    private bool _usePathfinder = true;

    protected AIMoveAction() {}

	public AIMoveAction(Vector3 destination, float normalizedSpeed, string tag = "", bool usePathfinder = true)
    {
        _dest = destination;
        _usePathfinder = usePathfinder;
        _normalizedSpeed = normalizedSpeed;
        _tag = tag;
    }

	public AIMoveAction(Vector3[] waypoints, float normalizedSpeed, string tag = "")
	{
		_waypoints = waypoints;
		_normalizedSpeed = normalizedSpeed;
        _tag = tag;
	}

	public override void EnterState ()
	{
		base.EnterState ();
        _cachedAgent = Agent;
		_cachedAgent.CurrentSpeed = _cachedAgent.MovementSpeed * _normalizedSpeed; // Mathf.Lerp(_currentSpeed, _cachedAgent.MovementSpeed * _normalizedSpeed, 0.1f);
        Agent.SetAnimationFloat("Velocity", 1f);
        Agent.SetAnimationSpeed(_normalizedSpeed);
        if(_usePathfinder)
        {
            Pathfinder.Instance.FindPath(Agent.GetHashCode(), Agent.transform.position, _dest, PathFound);
        }
    }

    private void PathFound(bool success, PathData pathData)
    {
        if (success)
        {
            _waypoints = pathData.waypoints;
        }
        else
        {
            _waypoints = new Vector3[] { _dest };
        }
    }

    public override void ExitState ()
	{
		base.ExitState ();
		_cachedAgent.CurrentSpeed = 0;
	}

    public override void Run()
    {
        base.Run();

        if (_waypoints == null) return;

        if (_cachedAgent.MovementSpeed == 0)
        {
            if (_errorHandler != null)
            {
                _errorHandler("Can't move because MovementSpeed is 0");
                return;
            }
        }

		if(!WaypointReached())
		{
            //_currentSpeed = _cachedAgent.MovementSpeed * _normalizedSpeed; // Mathf.Lerp(_currentSpeed, _cachedAgent.MovementSpeed * _normalizedSpeed, 0.1f);
            //SetAnimationFloat("Velocity", _normalizedSpeed);
            Move();
		}
		else
		{
			if(HasArrived())
			{
                //lerp down to 0
				_cachedAgent.CurrentSpeed = 0;
                _cachedAgent.SetAnimationFloat("Velocity", 0);
                _cachedAgent.RigidBody.MovePosition(_waypoints[_currentWaypointIndex]);
                stateComplete();

                //if (_currentSpeed <= 0.1f)
                //{
                //	_currentSpeed = 0;
                //	stateComplete();
                //}
            }
			else
			{
				NextWaypoint();
			}
		}

    }

    protected virtual bool HasArrived()
    {
		return _currentWaypointIndex == (_waypoints.Length - 1) && WaypointReached();
    }

	protected virtual bool WaypointReached()
	{
		return Vector3.SqrMagnitude(_waypoints[_currentWaypointIndex] - _cachedAgent.transform.position) < Mathf.Pow(_cachedAgent.CurrentSpeed * Time.deltaTime, 2);
	}

	protected void NextWaypoint()
	{
		if(_currentWaypointIndex <= _waypoints.Length - 2)
			_currentWaypointIndex++;
	}

    protected virtual void Move()
    {
		Vector3 facingDirection = _cachedAgent.LookAt(_waypoints[_currentWaypointIndex]);
		_cachedAgent.RigidBody.MovePosition(_cachedAgent.transform.position + facingDirection * _cachedAgent.CurrentSpeed * Time.deltaTime);
    }

#if UNITY_EDITOR
    public override void DrawDebug()
    {
        base.DrawDebug();

        Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);

        if (_waypoints != null && _waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = _waypoints.Length - 1; i > _currentWaypointIndex; i--)
            {
                Gizmos.DrawCube(_waypoints[i], size);
                Gizmos.DrawLine(_waypoints[i], _waypoints[i - 1]);
            }
            Gizmos.DrawCube(_waypoints[_currentWaypointIndex], size);
            Gizmos.DrawLine(_waypoints[_currentWaypointIndex], _cachedAgent.transform.position);
        }
    }
#endif
}
