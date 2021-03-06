﻿using System;
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

    [SerializeField]
    private float _drainMultiplier;
    protected override void Update()
    {
        if (!HasBeenPlaced || ParentObject == null || IsBuilding) return;

        GridCell[] neighbours = GridManager.Instance.GetNeighbors(Coordinates);
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].IsOccupied)
            {
                neighbours[i].Occupant.Drain(neighbours[i].Occupant.DrainRate * (_drainMultiplier - 1) * Time.deltaTime);
            }
        }
        ParentObject.Drain(DrainRate * (_drainMultiplier - 1) * Time.deltaTime);
    }

    public override float DrainRate
    {
        get
        {
            if(IsBuilding && ParentObject != null)
            {
                return ParentObject.LocalDrainRate;
            }
            return base.DrainRate;
        }
    }
    public override float FillRate
    {
        get
        {
            if (IsBuilding && ParentObject != null)
            {
                return ParentObject.LocalFillRate;
            }
            return base.FillRate;
        }
    }
}
