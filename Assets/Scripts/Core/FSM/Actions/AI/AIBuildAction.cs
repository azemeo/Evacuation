using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBuildAction : AIWaitAction {

    public override int StateID
    {
        get
        {
            return FSMStateTypes.AI.BUILD;
        }
    }

    public AIBuildAction(float duration) : base(duration)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        Agent.SetAnimationBool("Building", true);
    }

    public override void ExitState()
    {
        base.ExitState();

        Agent.SetAnimationBool("Building", false);
    }
}
