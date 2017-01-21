using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Waits for a time while playing the teleport animation
/// </summary>
public class AITeleportAction : AIWaitAction
{

    public AITeleportAction(float duration) : base(duration)
    {

    }

    public override int StateID
    {
        get
        {
            return FSMStateTypes.AI.TELEPORT;
        }
    }

    public override void EnterState()
    {
        base.EnterState();
        Agent.SetAnimationTrigger("Teleport");
    }
}
