using UnityEngine;
using System.Collections;

public class GridBuilder : MonoBehaviour {

	public static GridCell[,] Create(Transform gridRoot, GridCell tilePrefab, int width, int height, int tileSize, int borderWidth, GridCell borderTilePrefab)
	{
		if(gridRoot.childCount > 0)
		{
			Destroy(gridRoot.GetChild(0));
		}

		GridCell[,] grid = new GridCell[width, height];

		for(int x = 0; x < width; ++x)
		{
			for(int y = 0; y < height; ++y)
			{
				Vector3 tilePos = new Vector3(-width / 2f + tileSize * x + tileSize / 2f, 0, -height / 2f + tileSize * y + tileSize / 2f);
                GridCell gridCell;
                if (x >= borderWidth && x < width - borderWidth && y >= borderWidth && y < width - borderWidth)
                {
                    gridCell = Instantiate(tilePrefab, tilePos, tilePrefab.transform.rotation) as GridCell;
                }
                else
                {
                    gridCell = Instantiate(borderTilePrefab, tilePos, borderTilePrefab.transform.rotation) as GridCell;
                }
				gridCell.name = string.Format("Tile ({0}, {1})", x, y);
				gridCell.transform.SetParent(gridRoot);
				gridCell.Init(x, y);

				grid[x, y] = gridCell;
			}
		}

		return grid;
	}
}
