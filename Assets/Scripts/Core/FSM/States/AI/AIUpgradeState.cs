using UnityEngine;
using System.Collections;

public class AIUpgradeState : AIState
{
    public override int StateID
    {
        get
        {
            return FSMStateTypes.AI.UPGRADE;
        }
    }

	private GridObject _target;

    public AIUpgradeState(GridObject target)
    {
        _target = target;
    }

    public override void EnterState()
    {
        _childFSM = new AIFSM(new AIIdleState(), Agent);
        _childFSM.onStateComplete += substateComplete;
        playBuildAnim();
    }

	public override void ExitState ()
	{
		_childFSM.onStateComplete -= substateComplete;
        _childFSM.SetState(new AIIdleState());
	}

    private void playBuildAnim()
    {
        _childFSM.SetState(new AIBuildAction(Random.Range(3f, 6f)));
    }

    private void substateComplete(FSMState completedState)
    {
        switch (completedState.StateID)
        {
            case FSMStateTypes.AI.MOVE:
                Agent.LookAt(_target.UIAnchor.position);
                playBuildAnim();
                break;

            case FSMStateTypes.AI.BUILD:
                _childFSM.SetState(new AIMoveAction(_target.GetRandomPerimiterPosition(), 1f));
                break;
            default:
                break;
        }
    }
}
