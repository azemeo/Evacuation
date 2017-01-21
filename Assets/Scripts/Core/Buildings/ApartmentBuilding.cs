using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentBuilding : BaseBuilding
{
    public override int GridObjectType
    {
        get
        {
            return GridObjectTypes.Building.BUILDING;
        }
    }
}
