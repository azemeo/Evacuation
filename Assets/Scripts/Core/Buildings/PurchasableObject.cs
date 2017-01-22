using UnityEngine;
using System.Collections;

using Helpers;

public abstract class PurchasableObject : GridObject {

	public abstract BuildRequirement BuildingCost {get;}

	[System.Serializable]
	public struct BuildRequirement
	{
		public Resource ResourceCost;
		public int BuildTime;
	}

	public override void Placed()
	{
		if (!HasBeenPlaced)
		{
			//The check for whether the player has enough should be done before creating the building from the build menu.
			// If they are already in the process of placing it, they should have enough.
			// Still doing a check here just in case.
			if (GameManager.Instance.SpendResource(BuildingCost.ResourceCost.Type, BuildingCost.ResourceCost.Amount))
			{
                if (_requiresBuilder)
                {
                    AssignBuilder();
                }
                else
                {
                    StartBuild();
                }
			}
			else
			{
				Debug.LogError("Not enough " + BuildingCost.ResourceCost.Type.ToString() + " to complete purchase. Please check that there are enough resources before building creation. Destroying building.");
				Destroy(gameObject);
			}
		}

		base.Placed();
	}
}
