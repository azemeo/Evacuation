using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : BaseBuilding
{
    public override int GridObjectType
    {
        get
        {
            return GridObjectTypes.Building.BUILDING;
        }
    }

    [SerializeField]
    private int _occupants = 0;
}
