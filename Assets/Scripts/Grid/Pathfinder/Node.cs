using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {

	public float FCost
	{
		get
		{
			return GCost + HCost;
		}
	}

	public int HeapIndex
	{
		get
		{
			return _heapIndex;
		}

		set
		{
			_heapIndex = value;
		}
	}
	
	public int GCost;
	public int HCost;
	public Node Parent;

	public GridCell CellObj;

	private int _heapIndex;

	public int CompareTo(Node node)
	{
		int compare = FCost.CompareTo(node.FCost);

		if(compare.Equals(0))
		{
			compare = HCost.CompareTo(node.HCost);
		}
			
		return -compare;
	}
}
