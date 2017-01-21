using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using Pathfinding;
using Helpers;

public class GridManager : SingletonBehavior<GridManager>
{
    public static Action OnGridCreated;

	[Header("Grid Options")]
	[Tooltip("The number of tiles per axis.")]
	[SerializeField]
	private int _gridSize = 32;

	[SerializeField]
	private GridCell _gridCellPrefab;

	[SerializeField]
	private float _cellSize = 1;

    [SerializeField]
    private int _borderWidth = 2;

    [SerializeField]
    private GridCell _borderCellPrefab;

    private GameObject _gridRoot;
    private GridObject _activeObject;
    private Bounds _gridBounds;

    [SerializeField]
    private TextAsset _defaultMap;

    //----------------------------------------
    // The grid data structure that contains all cell.
	private GridCell[,] _grid;
	private bool[,] _gridMask;

    private Dictionary<string,GridObject> _allObjects = new Dictionary<string, GridObject>();

	private List<GridCell> _emptyCellsList;

	private bool _isDirty = false;

    #region Public Accessors
    public int GridSize
	{
		get
		{
			return _gridSize;
		}
	}

	public GridCell[,] Grid
	{
		get
		{
			return _grid;
		}
	}

    public GridObject ActiveObject
    {
        get { return _activeObject; }
    }

	public float GridCellSize
    {
        get
        {
            return _cellSize;
        }
    }

    public Bounds GridBounds
    {
        get
        {
            if (_gridBounds.size == Vector3.zero)
            {
				_gridBounds = new Bounds(Vector3.zero, new Vector3(_grid.GetLength(0) * GridCellSize, 0, _grid.GetLength(1) * GridCellSize));
            }
            return _gridBounds;
        }
    }
    #endregion

    #region Grid Creation

	public void CreateGrid()
	{
		initialize(_gridSize, _gridSize);
        if(OnGridCreated != null)
        {
            OnGridCreated();
        }
	}

    /// <summary>
    /// Creates a blank grid of a given size.
    /// </summary>
    /// <param name="sizeX">The x-axis size.</param>
    /// <param name="sizeY">The y-axis size.</param>
    private void initialize(int sizeX, int sizeY)
    {
		if (_gridCellPrefab == null)
        {
            Debug.LogError("CellPrefab not assigned to GridManager.", gameObject);
            return;
        }

        _gridRoot = new GameObject("Root");
        _gridRoot.transform.SetParent(transform, false);

		_grid = GridBuilder.Create(_gridRoot.transform, _gridCellPrefab, sizeX, sizeY, _cellSize, _borderWidth, _borderCellPrefab);
		_gridMask = new bool[sizeX, sizeY];
		_emptyCellsList = new List<GridCell>(sizeX * sizeY);

		for (int x = 0; x < _gridSize; ++x)
		{
			for (int y = 0; y < _gridSize; ++y)
			{
				_gridMask[x, y] = false;
				_emptyCellsList.Add(_grid[x, y]);
			}
		}

		Pathfinder.Instance.Initialize();
    }

    #endregion

    #region Grid Interface
    /// <summary>
    /// Checks if the given object can be placed at it's current location based on its size
    /// </summary>
    /// <returns><c>true</c>, if bounds were clear for placement, <c>false</c> otherwise.</returns>
    /// <param name="gridObject">The GridObject to check.</param>
    public bool CanBePlaced(GridObject gridObject)
	{
        Vector2Int coords = GetGridCoordinates(gridObject.transform.position);

        if (IsPointWithinBounds(coords.x, coords.y))
        {
            if (_grid[coords.x, coords.y].IsOccupied)
            {
                if(gridObject.IsAttachable)
                {
                    if(_grid[coords.x, coords.y].Occupant.Attachment == null || _grid[coords.x, coords.y].Occupant.Attachment == gridObject)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        return false;
	}

    /// <summary>
    /// Visualizes the bounds of an object at its current transform position by showing conflicts against existing grid objects.
    /// </summary>
    /// <param name="gridObject">The GridObject to check to show bounds for.</param>
	public void VisualizePlacement(GridObject gridObject)
	{
        Vector2Int coords = GetGridCoordinates(gridObject.transform.position);

		if (IsPointWithinBounds(coords.x, coords.y))
        {
			_grid[coords.x, coords.y].Select();
        }
	}

    /// <summary>
    /// Reset the visualization state of an object
    /// </summary>
    /// <param name="gridObject"></param>
	public void ClearVisualization(GridObject gridObject)
	{
        Vector2Int coords = GetGridCoordinates(gridObject.transform.position);

		if (IsPointWithinBounds(coords.x, coords.y))
        {
			_grid[coords.x, coords.y].Deselect();
        }
    }

    /// <summary>
    /// Places an object on the grid and sets occupied cells to reference it
    /// </summary>
    /// <param name="gridObject"></param>
	public void PlaceObject(GridObject gridObject)
	{
        Vector2Int coords = gridObject.Coordinates;

        if (!IsPointWithinBounds(coords.x, coords.y))
        {
            return;
        }

        if (_grid[coords.x, coords.y].IsOccupied)
        {
            _grid[coords.x, coords.y].Occupant.AttachObject(gridObject);
            return;
        }

		gridObject.transform.parent = _grid[coords.x, coords.y].transform;

		_grid[coords.x, coords.y].SetOccupant(gridObject);
		_emptyCellsList.Remove(_grid[coords.x, coords.y]);

        gridObject.Placed();

		if (!_allObjects.ContainsKey(gridObject.UID))
        {
            _allObjects.Add(gridObject.UID, gridObject);
        }

		if(GameManager.Instance.IsGameReady)
			RecalculateMask();
	}

    /// <summary>
    /// Detaches an object from the grid, removing all references to it from the cells.
    /// </summary>
    /// <param name="gridObject"></param>
	public void DetachObject(GridObject gridObject, bool updateParent = true)
    {
        Vector2Int coords = gridObject.Coordinates;

        if (updateParent)
        {
            gridObject.transform.parent = transform;
        }

        if (_grid[coords.x, coords.y].IsOccupied && _grid[coords.x, coords.y].Occupant.Attachment == gridObject)
        {
            _grid[coords.x, coords.y].Occupant.DetachObject();
            return;
        }

        gridObject.DetachedFromGrid();

		if (IsPointWithinBounds(coords.x, coords.y))
        {
			_grid[coords.x, coords.y].Reset();
			_emptyCellsList.Add(_grid[coords.x, coords.y]);
        }
    }

    /// <summary>
    /// Call from the OnDespawn() of a GridObject to remove it from gridManager entirely
    /// </summary>
    /// <param name="gridObject"></param>
    public void ObjectDespawned(GridObject gridObject)
    {
        ClearVisualization(gridObject);
        if (gridObject == _activeObject)
        {
            DeselectObject();
        }
        if (gridObject.HasBeenPlaced)
        {
            DetachObject(gridObject);
			if (_allObjects.ContainsKey(gridObject.UID))
			{
				_allObjects.Remove(gridObject.UID);
			}
        }
		_isDirty = true;
    }

    public void SelectObject(GridObject gridObject)
    {
        if (_activeObject != gridObject)
        {
            if (_activeObject != null)
            {
                DeselectObject();
            }
            _activeObject = gridObject;
            _activeObject.Selected();
        }
    }

    public void DeselectObject()
    {
        if (_activeObject != null)
        {
            GridObject tmp = _activeObject;
            _activeObject = null;
            tmp.Deselected();
        }
    }

    public void ResetGrid()
    {
        _allObjects.Clear();

        Destroy(_gridRoot);
        CreateGrid();
    }

	/// <summary>
	/// Clears the mask entirely.
	/// </summary>
	public void ClearMask()
	{
		for (int x = 0; x < _gridSize; x++)
		{
			for (int y = 0; y < _gridSize; y++)
			{
				_gridMask[x, y] = false;
				_grid[x, y].HideMask();
			}
		}
	}

	/// <summary>
	/// Shows the mask at given coordinates and size.
	/// </summary>
	/// <param name="coords">Top left coordinates.</param>
	/// <param name="size">Size.</param>
	public void ShowMaskAtCoords(Vector2Int coords)
	{
		if (IsPointWithinBounds(coords.x, coords.y))
		{
			_gridMask[coords.x, coords.y] = true;
			_grid[coords.x, coords.y].ShowMask();
		}
	}

	/// <summary>
	/// Hides the mask at given coordinates and size.
	/// </summary>
	/// <param name="coords">Top left coordinates.</param>
	/// <param name="size">Size.</param>
	public void HideMaskAtCoords(Vector2Int coords, Vector2Int size)
	{
		for (int x = coords.x - 1; x < coords.x + size.x + 1; x++)
		{
			for (int y = coords.y - 1; y < coords.y + size.y + 1; y++)
			{
				if (IsPointWithinBounds(x, y))
				{
					_gridMask[x, y] = false;
					_grid[x, y].HideMask();
				}
			}
		}
	}

	public void RecalculateMask()
	{
		ClearMask();

		foreach(KeyValuePair<string, GridObject> kvp in _allObjects)
		{
			GridObject gridObject = kvp.Value;
			int goType = gridObject.GridObjectType;

			//if(goType == GridObjectTypes.Building.BUILDING || goType == GridObjectTypes.Other.OBSTACLE || goType == GridObjectTypes.Other.DECORATION)
			//	continue;

			Vector2Int coords = gridObject.Coordinates;

			ShowMaskAtCoords(coords);
		}
	}
    #endregion

    #region GridQueries

	public bool GridMaskTest(Vector2Int coords)
	{
		return _gridMask[coords.x, coords.y];
	}

	public GridCell GetCell(Vector2Int coordinates)
    {
		return _grid[coordinates.x, coordinates.y];
    }

	public GridCell GetCell(Vector3 worldPosition)
    {
		Vector2Int coords = GetCoordinatesFromWorldPosition(worldPosition);

		if(IsPointWithinBounds(coords.x, coords.y))
        {
			return _grid[coords.x, coords.y];
        }
        return null;
    }

	public Vector2Int GetCoordinatesFromWorldPosition(Vector3 worldPosition)
	{
		int x = Mathf.FloorToInt(worldPosition.x / _cellSize) + _gridSize / 2;
		int y = Mathf.FloorToInt(worldPosition.z / _cellSize) + _gridSize / 2;

		return new Vector2Int(x, y);
	}

    /// <summary>
    /// Returns the GridCells in cardinal directions around the cell in the specified position
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
	public GridCell[] GetNeighbors(Vector2Int cellCoords)
    {
		List<GridCell> cells = new List<GridCell>(4);

        if (IsPointWithinBounds(cellCoords.x, cellCoords.y + 1)) cells.Add(_grid[cellCoords.x, cellCoords.y + 1]);
        if (IsPointWithinBounds(cellCoords.x + 1, cellCoords.y)) cells.Add(_grid[cellCoords.x + 1, cellCoords.y]);
        if (IsPointWithinBounds(cellCoords.x, cellCoords.y - 1)) cells.Add(_grid[cellCoords.x, cellCoords.y - 1]);
        if (IsPointWithinBounds(cellCoords.x - 1, cellCoords.y)) cells.Add(_grid[cellCoords.x - 1, cellCoords.y]);

        return cells.ToArray();
    }

	public List<GridCell> GetRandomEmptyCells()
	{
		_emptyCellsList.Shuffle();
		return _emptyCellsList;
	}

	public GridCell GetRandomEmptyCell()
	{
		var emptyCells = GetRandomEmptyCells();
		GridCell randomCell = emptyCells.Count > 0 ? emptyCells[0] : null;
		return randomCell;
	}

	public GridCell GetClosestEmptyCell(Vector3 worldPos, params string[] ignoreTemplateIDs)
	{
		GridCell currentCell = GetCell(worldPos);
		return GetClosestEmptyCell(currentCell, ignoreTemplateIDs);
	}

	public GridCell GetClosestEmptyCell(GridCell currentCell, params string[] ignoreTemplateIDs)
	{
		if(!currentCell.IsOccupied)
			return currentCell;

		Queue<GridCell> queue = new Queue<GridCell>();
		HashSet<GridCell> visited = new HashSet<GridCell>();
		queue.Enqueue(currentCell);
		List<string> ignoreList = new List<string>(ignoreTemplateIDs);

		while(queue.Count > 0){
			GridCell current = queue.Dequeue();
			Vector2Int pos = current.GridPosition;
			for(int x = pos.x - 1; x <= pos.x + 1; ++x)
			{
				for(int y = pos.y - 1; y <= pos.y + 1; ++y)
				{
					if(IsPointWithinBounds(x, y))
					{
						GridCell neighbour = _grid[x, y];
						if(!visited.Contains(neighbour))
						{
							visited.Add(neighbour);
							if(!neighbour.IsOccupied || ignoreList.Contains(neighbour.Occupant.TemplateID))
								return neighbour;
							else
								queue.Enqueue(neighbour);
						}
					}
				}
			}
		}

		return null;
	}

    public Vector2Int GetGridCoordinates(Vector3 worldPosition)
    {
		return GetCoordinatesFromWorldPosition(worldPosition);
    }

    public Vector3? GetWorldCoordinates(Vector2Int gridPosition)
    {
		Vector3? worldPos = null;

		if(IsPointWithinBounds(gridPosition.x, gridPosition.y))
			worldPos = _grid[gridPosition.x, gridPosition.y].transform.position;

		return worldPos;
    }

    public GridObject GetGridObject(string UID)
    {
        if (_allObjects.ContainsKey(UID))
        {
            return _allObjects[UID];
        }

        return null;
    }

	public bool IsPointWithinBounds(int x, int y)
    {
		return x >= 0 && x < _grid.GetLength(0) && y >= 0 && y < _grid.GetLength(1);
    }

    public bool IsRectWithinBounds(Vector2Int origin, Vector2Int size)
    {
		if (!IsPointWithinBounds(origin.x, origin.y))
        {
            return false;
        }
        if (size.x > 1 && size.y > 1)
        {
			if (!IsPointWithinBounds(origin.x + size.x - 1, origin.y + size.y - 1))
            {
                return false;
            }
        }

        return true;
    }

    public GridObject[] GetAllGridObjects()
    {
        return new List<GridObject>(_allObjects.Values).ToArray();
    }

	public List<T> GetAllGridObjectsOfType<T>(int gridObjectType) where T : GridObject
	{
		List<T> objectsList = new List<T>();

		foreach(KeyValuePair<string,GridObject> gridObject in _allObjects)
		{
			if(gridObject.Value.GridObjectType == gridObjectType)
			{
				T obj = gridObject.Value as T;

				if(obj != null)
					objectsList.Add(obj);
			}
		}

		return objectsList;
	}
    #endregion
}
