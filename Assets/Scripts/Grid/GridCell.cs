using UnityEngine;
using System.Collections;

public class GridCell : MonoBehaviour {

	public bool IsOccupied
	{
		get { return _gridObject != null; }
	}

	public SpriteRenderer TileRenderer
	{
		get
		{
			return _tileRenderer;
		}
	}

	public Vector2Int GridPosition
	{
		get
		{
			return _gridPosition;
		}
	}

	public int Cost
	{
		get
		{
			if (_gridObject == null)
			{
				return _baseMovementCost;
			}
			else
			{
				return _gridObject.MovementCost;
			}
		}
	}

	public GridObject Occupant
	{
		get
		{
			return _gridObject;
		}
	}

	public string OccupantTemplateID
	{
		get
		{
			return Occupant != null ? Occupant.TemplateID : "";
		}
	}

	//Inspector Editable
	[Tooltip("color of a cell that is available to be placed within")]
	[SerializeField]
	private Color _unoccupiedColor = Color.green;

	[Tooltip("color of a cell that has an existing object placed in it")]
	[SerializeField]
	private Color _occupiedColor = Color.red;

	[Tooltip("color of a cell when showing the grid mask preview")]
	[SerializeField]
	private Color _maskColor = new Color(1f, 1f, 1f, 0.5f);

	[Tooltip("the base pathfinding cost of an empty cell")]
	[SerializeField]
	private int _baseMovementCost = 1;

	[SerializeField]
	private SpriteRenderer _tileRenderer;

	[SerializeField]
	private SpriteRenderer _maskRenderer;

	private Vector2Int _gridPosition;
	private GridObject _gridObject = null;
	private bool _isSelected = false;

	public void Select()
	{
		_isSelected = true;
		Repaint();
	}

	public void Deselect()
	{
		_isSelected = false;
		Repaint();
	}

	public void ShowMask()
	{
		_maskRenderer.enabled = true;
		_maskRenderer.color = _maskColor;
	}

	public void HideMask()
	{
		_maskRenderer.enabled = false;
	}

	private void Repaint()
	{
		if (_isSelected)
		{
			TileRenderer.enabled = true;
			TileRenderer.color = (IsOccupied ? _occupiedColor : _unoccupiedColor);
		}
		else
		{
			TileRenderer.enabled = false;
		}
	}

	public virtual void SetOccupant(GridObject newOccupant)
	{
		if (newOccupant == null)
		{
			Reset();
		}
		else
		{
			if (_gridObject != null)
			{
				//This warning is so that we don't accidentally overwrite cells without properly accounting for what was in them.
				// Could lead to lost data during saves.
				Debug.LogWarning("Overwriting contents of occupied cell '" + name + "'. You should call Reset before doing this. Current occupant = " + _gridObject.name);
			}
			_gridObject = newOccupant;
		}
	}

	public virtual void Init(int x, int y)
	{
		_gridPosition = new Vector2Int(x, y);
		Reset();
	}

	public void Reset()
	{
		TileRenderer.enabled = false;
		_isSelected = false;
		_gridObject = null;
	}
}
