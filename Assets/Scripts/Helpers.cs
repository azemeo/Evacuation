namespace Helpers
{
	public enum ResourceType
	{
        NONE = 0,
		Cash = 1
	}

	[System.Serializable]
	public struct Resource
	{
		public ResourceType Type;
		public int Amount;

        public Resource(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
	}

}