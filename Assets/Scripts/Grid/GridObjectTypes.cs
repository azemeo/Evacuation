using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GridObjectTypes
{
    public static class Building
    {
        public const int NONE = -1;
        public const int BUILDING = 0;
        public const int BLOCKADE = 1;
        public const int PUMP = 2;
        public const int WALL = 3;
        public const int ROAD = 4;
    }

    public static class Other
    {
        public const int OCEAN = 100;
    }
}
