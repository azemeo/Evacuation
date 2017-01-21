using UnityEngine;
using System.Collections;
using Pathfinding;

public class AIWanderState : AIState {

	public override int StateID
	{
		get
		{
			return FSMStateTypes.AI.WANDER;
		}
	}

    private Rect _wanderArea;

    private Vector3 _wanderTarget;

	public AIWanderState(Rect area)
    {
        _wanderArea = area;
    }

    public override void EnterState()
    {
        _childFSM = new AIFSM(new AIIdleState(), Agent);
		_childFSM.onStateComplete += StateComplete;

        if (_wanderArea.Contains(Agent.transform.position))
        {
            DestinationReached();
        }
        else
        {
            Pathfinder.Instance.FindPath(Agent.GetHashCode(), Agent.transform.position, GetRandomPositionInArea(), PathFound);
        }
    }

	public override void ExitState ()
	{
        _childFSM.onStateComplete -= StateComplete;
	}

	private void PathFound(bool success, PathData pathData)
	{
		AIMoveAction moveAction = null;

		if(success)
		{
			moveAction = new AIMoveAction(pathData.waypoints, 1f);
		}

		_childFSM.SetState(moveAction);
	}

    public void StateComplete(FSMState completedState)
    {
        if (completedState.StateID == FSMStateTypes.AI.MOVE)
        {
            DestinationReached();
        }
        if (completedState.StateID == FSMStateTypes.AI.WAIT)
        {
            WaitComplete();
        }
    }

    public void DestinationReached()
    {
        _childFSM.SetState(new AIWaitAction(Random.Range(2, 5)));
    }

    public void WaitComplete()
    {
		_childFSM.SetState(new AIMoveAction(GetRandomPositionInArea(), 1f));
    }

	private Vector3 GetRandomPositionInArea()
	{
		return new Vector3(Random.Range(_wanderArea.min.x, _wanderArea.max.x), 0, Random.Range(_wanderArea.min.y, _wanderArea.max.y));
	}
}
