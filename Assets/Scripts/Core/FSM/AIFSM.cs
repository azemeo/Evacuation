using UnityEngine;
using System.Collections;

public class AIFSM : FSM {

    public AIAgent Agent
    {
        get
        {
            return _agent;
        }
    }

	private AIAgent _agent;

    public AIFSM(AIState initialState, AIAgent agent)
    {
        _agent = agent;
		SetState(initialState);
    }
}
