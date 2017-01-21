using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class GridObject : ConfigurableObject
{
    public delegate void GridEvent();

    public abstract int GridObjectType { get; }

    [SerializeField] protected Vector2Int _size = new Vector2Int(2,2);
	[SerializeField] protected int _movementCost = 2;
    [SerializeField] private Transform _uiAnchor;
	[SerializeField] protected BoxCollider _collider;

	protected Vector2Int _coordinates;

    [SerializeField]
    private bool _isSelectable = false;
    private bool _hasBeenPlaced = false;
    private bool _isPlaceable = true;
    private bool _isSelected = false;

    private string _uid = "";
	private string _name;

    private UIPlacementPanel _placementPanel;

    public GridEvent OnSelected;
    public GridEvent OnDeselected;
    public GridEvent OnPlaced;
    public GridEvent OnDetached;
    public GridEvent OnConfirmPlacement;

	protected bool _isDirty;

	protected AIAgent _assignedBuilder;

	public void SetCoordinates(Vector2Int coords, bool updatePosition = true)
	{
		_coordinates = coords;
		if (updatePosition)
		{
			Vector3? updatedPosition = GridManager.Instance.GetWorldCoordinates(Coordinates);

			if(updatedPosition.HasValue)
				transform.position = updatedPosition.Value;
		}
	}

	#region Public Accessors
	public string Name
	{
		get
		{
			return _name;
		}

		protected set
		{
			_name = value;
		}
	}

	public Vector2Int Size
	{
		get { return _size; }
		set { _size = value; }
	}

	public Vector2Int Coordinates
	{
		get
        {
            return _coordinates;
        }
	}

    public string UID
    {
        get
        {
            return _uid;
        }
        set
        {
            if (string.IsNullOrEmpty(_uid))
            {
                _uid = value;
            }
            else
            {
                Debug.LogWarning("You may only set the UID on a GridObject once.");
            }
        }
    }

	public int MovementCost
	{
		get { return _movementCost; }
		set { _movementCost = value; }
	}

    public bool HasBeenPlaced
    {
        get { return _hasBeenPlaced; }
    }


    public Transform UIAnchor
    {
        get
        {
            if (_uiAnchor == null)
            {
                _uiAnchor = transform.FindChild("UIAnchor");
                if (_uiAnchor == null)
                {
                    _uiAnchor = new GameObject("UIAnchor").transform;
                    _uiAnchor.SetParent(transform, false);
                }
            }
            return _uiAnchor;
        }
    }

    public UIPlacementPanel PlacementPanel
    {
        get
        {
            if (_placementPanel == null && _isPlaceable)
            {
                if (GetComponent<DraggableObject>() == null)
                {
                    _isPlaceable = false;
                    return null;
                }
                _placementPanel = UIManager.Instance.Get<UIPlacementPanel>(UIManager.PanelID.PlacementPanel);
            }
            return _placementPanel;
        }
    }

	public Collider Collider
	{
		get
		{
			return _collider;
		}
	}

    #endregion

	public override void OnSpawned()
    {
		Vector3 localPosition = new Vector3((_size.x * 0.5f) - 0.5f, 0.5f, (_size.y * 0.5f) - 0.5f);

		_collider.transform.localPosition = localPosition;
		_collider.transform.localScale = Vector3.one;
		_collider.center = Vector3.zero;
		_collider.size = new Vector3(_size.x, 1, _size.y);
    }

	public override void OnDespawned()
    {
        GridManager.Instance.ObjectDespawned(this);
        _uid = "";
        _hasBeenPlaced = false;
    }

    protected virtual void OnEnable()
    {
        GridManager.OnGridCreated += GridCreated;
    }

    protected virtual void OnDisable()
    {
        GridManager.OnGridCreated -= GridCreated;
    }

    protected virtual void GridCreated()
    {
        SetCoordinates(GridManager.Instance.GetCoordinatesFromWorldPosition(transform.position));
        GridManager.Instance.PlaceObject(this);
    }

    #region GridEvents
    public virtual void Placed()
    {
        if (!_hasBeenPlaced)
        {
            _hasBeenPlaced = true;
        }
        if (OnPlaced != null)
        {
            OnPlaced();
        }

		_isDirty = true;
    }

    public virtual void ConfirmPlacement()
    {
        if (PlacementPanel != null)
        {
            PlacementPanel.Hide();
        }

        if (OnConfirmPlacement != null)
        {
            OnConfirmPlacement();
        }
    }

    public virtual void DetachedFromGrid()
    {
        if (OnDetached != null)
        {
            OnDetached();
        }

		_isDirty = false;
    }

    public virtual void Selected()
    {
        if (!_isSelected)
        {
            _isSelected = true;
            if (!_hasBeenPlaced && PlacementPanel != null)
            {
                PlacementPanel.Show(this);
            }

            if (OnSelected != null)
            {
                OnSelected();
            }

			_collider.size += Vector3.up * 0.5f;
        }
    }

    public virtual void Deselected()
    {
        if (_isSelected)
        {
            _isSelected = false;

            if (OnDeselected != null)
            {
                OnDeselected();
            }

			_collider.size -= Vector3.up * 0.5f;
        }
    }
    #endregion


    public virtual void AssignBuilder(AIAgent builder = null)
    {
        _assignedBuilder = builder;

        if (_assignedBuilder == null)
        {
            _assignedBuilder = GameManager.Instance.AssignBuilder();
        }

        if (_assignedBuilder != null)
        {
            //_assignedBuilder.SetBuilderTarget(this, builder != null);
        }
    }

    public virtual void ReturnBuilder()
    {
        if (_assignedBuilder != null)
        {
            //_assignedBuilder.SetBehavior(Troop.TroopStatus.Idle);
            GameManager.Instance.ReturnBuilder(_assignedBuilder);
            //_assignedBuilder.ReturnHome();
            _assignedBuilder = null;
        }
    }

    /// <summary>
    /// Get a random position around the perimiter of the building;
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomPerimiterPosition()
	{
		return GetClosestPerimiterPosition(GetRandomPositionInArea());
	}

	public Vector3 GetClosestPerimiterPosition(Vector3 pos)
	{
        pos = Collider.ClosestPointOnBounds(pos);
        pos.y = 0;
        return pos;
	}

	/// <summary>
	/// Get a random position inside the perimiter of the building.
	/// </summary>
	/// <returns></returns>
	public Vector3 GetRandomPositionInArea()
	{
		Vector3 max = transform.position;
		max.x += (Size.x - 1) * GridManager.Instance.GridCellSize;
		max.z += (Size.y - 1) * GridManager.Instance.GridCellSize;
		return new Vector3(Random.Range(transform.position.x, max.x), transform.position.y, Random.Range(transform.position.z, max.z));
	}

    public abstract float ConstructionTimeRemaining { get; }
}
