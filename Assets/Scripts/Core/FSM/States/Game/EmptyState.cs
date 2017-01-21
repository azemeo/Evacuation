using UnityEngine;
using System.Collections;

// this is an empty state meant to be used when the FSM shouldn't do nothing but run an empty Run() loop
public class EmptyState : GameState {

	public override int StateID 
	{
		get 
		{
			return FSMStateTypes.Game.EMPTY;
		}
	}
}
