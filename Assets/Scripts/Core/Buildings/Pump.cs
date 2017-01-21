using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pump : BaseBuilding
{

    public override int GridObjectType
    {
        get
        {
            return GridObjectTypes.Building.PUMP;
        }
    }

    protected override void Update()
    {
        if (!HasBeenPlaced || ParentObject == null) return;

        GridCell[] neighbours = GridManager.Instance.GetNeighbors(Coordinates);
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].IsOccupied)
            {
                neighbours[i].Occupant.Drain(DrainRate * Time.deltaTime);
            }
        }
        ParentObject.Drain(DrainRate * Time.deltaTime);
    }
}
