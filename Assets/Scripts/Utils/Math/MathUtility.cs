using UnityEngine;
using System.Collections;


public interface IVectorInt
{
	Vector2 ToVector2();
	Vector3 ToVector3();
}

[System.Serializable]
public struct Vector2Int : IVectorInt
{
	public int x;
	public int y;

	public Vector2Int(int xValue, int yValue)
	{
		x = xValue;
		y = yValue;
	}

	public Vector2Int(Vector2 vector)
	{
		x = Mathf.RoundToInt(vector.x);
		y = Mathf.RoundToInt(vector.y);
	}

	public Vector2Int(Vector3 vector)
	{
		x = Mathf.RoundToInt(vector.x);
		y = Mathf.RoundToInt(vector.y);
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, 0);
	}

	public bool Equals (Vector2Int obj)
	{
		return x == obj.x && y == obj.y;
	}

	public static Vector2Int operator + (Vector2Int vectorA, Vector2Int vectorB)
	{
		return new Vector2Int(vectorA.x + vectorB.x, vectorA.y + vectorB.y);
	}

	public static Vector2Int operator - (Vector2Int vectorA, Vector2Int vectorB)
	{
		return new Vector2Int(vectorA.x - vectorB.x, vectorA.y - vectorB.y);
	}

    public static Vector2Int operator * (Vector2Int vectorA, int value)
    {
        return new Vector2Int(vectorA.x * value, vectorA.y * value);
    }

	public override string ToString()
	{
		return "("+x+", "+y+")";
	}

    public static float Distance(Vector2Int vectorA, Vector2Int vectorB)
    {
        return Mathf.Sqrt(SqrDistance(vectorA, vectorB));
    }

    public static int SqrDistance(Vector2Int vectorA, Vector2Int vectorB)
    {
        return (int)(Mathf.Pow(vectorB.x - vectorA.x, 2) + Mathf.Pow(vectorB.y - vectorA.y, 2));
    }
}

[System.Serializable]
public struct Vector3Int : IVectorInt
{
	public int x;
	public int y;
	public int z;

	public Vector3Int(int xValue, int yValue, int zValue)
	{
		x = xValue;
		y = yValue;
		z = zValue;
	}

	public Vector3Int(Vector2 vector)
	{
		x = Mathf.RoundToInt(vector.x);
		y = Mathf.RoundToInt(vector.y);
		z = 0;
	}

	public Vector3Int(Vector3 vector)
	{
		x = Mathf.RoundToInt(vector.x);
		y = Mathf.RoundToInt(vector.y);
		z = Mathf.RoundToInt(vector.z);
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, z);
	}

	public bool Equals (Vector3Int obj)
	{
		return x == obj.x && y == obj.y && z == obj.z;
	}

	public static Vector3Int operator + (Vector3Int vectorA, Vector3Int vectorB)
	{
		return new Vector3Int(vectorA.x + vectorB.x, vectorA.y + vectorB.y, vectorA.z + vectorB.z);
	}

	public static Vector3Int operator - (Vector3Int vectorA, Vector3Int vectorB)
	{
		return new Vector3Int(vectorA.x - vectorB.x, vectorA.y - vectorB.y, vectorA.z - vectorB.z);
	}

	public override string ToString()
	{
		return "("+x+", "+y+", "+z+")";
	}
}

public class VectorIntUtils
{
	public static int Distance(Vector2Int vectorA, Vector2Int vectorB)
	{
		return Mathf.Abs(vectorA.x - vectorB.x) + Mathf.Abs(vectorA.y - vectorB.y);
	}

	public static int Distance(Vector3Int vectorA, Vector3Int vectorB)
	{
		return Mathf.Abs(vectorA.x - vectorB.x) + Mathf.Abs(vectorA.y - vectorB.y) + Mathf.Abs(vectorA.z - vectorB.z);
	}
}