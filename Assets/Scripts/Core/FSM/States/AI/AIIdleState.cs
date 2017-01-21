using UnityEngine;
using System.Collections;

public class AIIdleState : AIState {

	public override int StateID
	{
		get
		{
			return FSMStateTypes.AI.IDLE;
		}
	}

	public override void EnterState ()
	{
        Agent.SetAnimationFloat("Velocity", 0);
	}
}
