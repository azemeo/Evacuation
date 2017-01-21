using UnityEngine;
using System;
using System.Collections;

public class Heap<T> where T : IHeapItem<T>
{
	private T[] _items;
	private int _itemCount;

	public Heap(int maxSize)
	{
		_items = new T[maxSize];
	}

	public void Add(T item)
	{
		item.HeapIndex = _itemCount;
		_items[_itemCount] = item;
		SortUp(item);
		_itemCount++;
	}

	public T RemoveFirst()
	{
		T firstItem = _items[0];
		_itemCount--;

		_items[0] = _items[_itemCount];
		_items[0].HeapIndex = 0;

		SortDown(_items[0]);
		return firstItem;
	}

	public void UpdateItem(T item)
	{
		SortUp(item);
	}

	public int Count
	{
		get
		{
			return _itemCount;
		}
	}

	public bool Contains(T item)
	{
		return Equals(_items[item.HeapIndex], item);
	}

	void SortDown(T item)
	{
		while(true)
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;

			int swapIndex = 0;

			if(childIndexLeft < _itemCount)
			{
				swapIndex = childIndexLeft;

				if(childIndexRight < _itemCount)
				{
					if(_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)
					{
						swapIndex = childIndexRight;
					}
				}

				if(item.CompareTo(_items[swapIndex]) < 0)
					Swap(item, _items[swapIndex]);
				else
					return;
			}
			else
				return;

		}
	}

	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;

		while(true)
		{
			T parentItem = _items[parentIndex];

			if(item.CompareTo(parentItem) > 0)
			{
				Swap(item, parentItem);
			}
			else
			{
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void Swap(T itemA, T itemB)
	{
		_items[itemA.HeapIndex] = itemB;
		_items[itemB.HeapIndex] = itemA;

		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex
	{
		get;
		set;
	}
}
