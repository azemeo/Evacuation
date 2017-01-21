using UnityEngine;
using System.Collections;

public static class FSMStateTypes {

	public static class AI
	{
		public const int IDLE = 0;
		public const int WANDER = 1;
		public const int FIND_PATH = 2;
		public const int MOVE = 3;
		public const int WAIT = 4;
        public const int UPGRADE = 5;
        public const int COMBAT = 6;
        public const int ATTACK = 7;
        public const int HEAL = 8;
        public const int SUPPORT = 9;
        public const int TROOP_COMBAT = 10;
        public const int TURRET = 11;
        public const int RANDOM_WANDER = 12;
		public const int DEFEND = 13;
		public const int FOLLOW = 14;
        public const int WALL_COMBAT = 15;
        public const int CAMP = 16;
        public const int TELEPORT = 17;
        public const int BUILD = 18;
	}

	public static class Game
	{
		public const int BATTLE_SCENARIO = 0;
        public const int BATTLE_RAID = 1;
		public const int EMPTY = 2;
		public const int HOME = 3;
		public const int LAYOUT_EDITOR = 4;
		public const int LOAD = 5;
		public const int SAVE = 6;
		public const int SCENARIO_EDITOR = 7;
        public const int STARTUP = 8;
        public const int REPLAY = 9;
	}
}