using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : BaseBuilding
{
    public override int GridObjectType
    {
        get
        {
            return GridObjectTypes.Building.ROAD;
        }
    }
}
