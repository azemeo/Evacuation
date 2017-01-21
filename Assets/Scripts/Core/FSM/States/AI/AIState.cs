using UnityEngine;
using System.Collections;

public abstract class AIState : FSMState{

	private AIAgent _agent = null;

    public AIAgent Agent
    {
        get
        {
			if(!_agent)
				_agent = ((AIFSM)ParentFSM).Agent;

			return _agent;
        }
    }

    public virtual void OnTriggerEnter(Collider c)
    {

    }

    public virtual void OnTriggerExit(Collider c)
    {

    }
}
