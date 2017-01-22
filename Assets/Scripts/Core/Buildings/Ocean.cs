using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : GridObject
{
    public override float ConstructionTimeRemaining
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override int GridObjectType
    {
        get
        {
            return GridObjectTypes.Other.OCEAN;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        WaveManager.onWaveArrival += WaveArrived;
        _isFlooded = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        WaveManager.onWaveArrival -= WaveArrived;
    }

    private void WaveArrived()
    {
        Tsunami(1);
    }

    protected override void Update()
    {
        if (!HasBeenPlaced) return;

        GridCell[] neighbours = GridManager.Instance.GetNeighbors(Coordinates);
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].IsOccupied && neighbours[i].Occupant is BaseBuilding)
            {
                BaseBuilding bb = neighbours[i].Occupant as BaseBuilding;
                bb.AddToFillAmount(bb.FillRate * Time.deltaTime);
            }
        }
    }
}
