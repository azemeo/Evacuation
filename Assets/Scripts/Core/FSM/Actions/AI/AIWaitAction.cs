using UnityEngine;
using System.Collections;

public class AIWaitAction : AIState {

	public override int StateID
	{
		get
		{
			return FSMStateTypes.AI.WAIT;
		}
	}

    private float _waitDuration;

    public AIWaitAction(float duration)
    {
        _waitDuration = duration;
    }

    public void Interrupt()
    {
        _waitDuration = 0;
        stateComplete();
    }

    public override void Run()
    {
        base.Run();
        if (_waitDuration > 0)
        {
            _waitDuration = Mathf.Clamp(_waitDuration - Time.deltaTime, 0f, float.MaxValue);
            if (_waitDuration == 0)
            {
                stateComplete();
            }
        }
    }


}
