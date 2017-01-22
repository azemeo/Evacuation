using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class GridObject : ConfigurableObject
{
    public delegate void GridEvent();

    public abstract int GridObjectType { get; }

    protected AudioSource _audioSource;
    [SerializeField] protected AudioClip _destructionSound;
    private bool isDead = false;

    [SerializeField] protected int _movementCost = 2;
    [SerializeField] private Transform _uiAnchor;
    [SerializeField] protected BoxCollider _collider;

	protected Vector2Int _coordinates;

    private bool _hasHadWave = false;

    [SerializeField]
    private bool _isSelectable = false;
    private bool _hasBeenPlaced = false;
    private bool _isPlaceable = true;
    private bool _isSelected = false;

    [SerializeField]
    private GridObject _attachment;
    [SerializeField]
    private GridObject _parentObject;

    [SerializeField]
    private bool _attachable = false;

    private string _uid = "";
	private string _name;

    private UIPlacementPanel _placementPanel;

    public GridEvent OnSelected;
    public GridEvent OnDeselected;
    public GridEvent OnPlaced;
    public GridEvent OnDetached;
    public GridEvent OnConfirmPlacement;

	protected Builder _assignedBuilder;

    [SerializeField]
    protected bool _requiresBuilder = false;

    [SerializeField]
    protected float _fillRate = 0.1f;

    [SerializeField]
    protected float _drainRate = 0.05f;

    [SerializeField]
    protected float _reclaimThreshold = 0.1f;

    [SerializeField]
    protected float _currentFill = 0;
    [SerializeField]
    protected float _height = 0;
    protected bool _isFlooded = false;

    [SerializeField]
    protected bool _blockWater = false;
    [SerializeField]
    protected bool _blockPeople = false;

    [SerializeField]
    protected WaterPlane _waterPlane;

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
		get
        {
            if(Attachment != null)
            {
                return Attachment.MovementCost;
            }
            return _movementCost;
        }
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
        transform.localPosition = Vector3.zero;
		_collider.transform.localScale = Vector3.one;
		_collider.center = Vector3.zero;
		_collider.size = new Vector3(1, 1, 1);

        _audioSource = GetComponent<AudioSource>();
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
        WaveManager.onWaveRecede += ResetTsunami;
    }

    protected virtual void OnDisable()
    {
        GridManager.OnGridCreated -= GridCreated;
        WaveManager.onWaveRecede -= ResetTsunami;
    }

    protected virtual void GridCreated()
    {
        SetCoordinates(GridManager.Instance.GetCoordinatesFromWorldPosition(transform.position));
        UID = GameManager.Instance.GetNewUID(this);
        if (_parentObject != null)
        {
            _parentObject.AttachObject(this);
        }
        else
        {
            GridManager.Instance.PlaceObject(this);
        }
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


    public virtual void AssignBuilder(Builder builder = null)
    {
        _assignedBuilder = builder;

        if (_assignedBuilder == null)
        {
            _assignedBuilder = GameManager.Instance.AssignBuilder();
        }

        if (_assignedBuilder != null)
        {
            _assignedBuilder.SetTarget(this);
        }
    }

    public virtual void StartBuild() { }

    public virtual void ReturnBuilder()
    {
        if (_assignedBuilder != null)
        {
            GameManager.Instance.ReturnBuilder(_assignedBuilder);
            _assignedBuilder.ReturnHome();
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
        Vector3 min = transform.position;
        min.x -= 0.4f;
        min.z -= 0.4f;

		Vector3 max = transform.position;
        max.x += 0.4f;
        max.z += 0.4f;
		return new Vector3(Random.Range(min.x, max.x), transform.position.y, Random.Range(min.z, max.z));
	}

    public abstract float ConstructionTimeRemaining { get; }

    public GridObject Attachment
    {
        get
        {
            return _attachment;
        }
    }

    public bool IsAttachable
    {
        get
        {
            return _attachable;
        }
    }

    public GridObject ParentObject
    {
        get
        {
            return _parentObject;
        }
    }

    public virtual bool AttachObject(GridObject objectToAttach)
    {
        if(Attachment == null && objectToAttach.IsAttachable)
        {
            _attachment = objectToAttach;
            _attachment.AttachToParent(this);
            _attachment.Placed();
            return true;
        }
        return false;
    }

    public virtual void AttachToParent(GridObject parent)
    {
        _parentObject = parent;
        transform.SetParent(parent.transform);
        //transform.localPosition = Vector3.zero;
    }

    public virtual void DetachObject()
    {
        if(Attachment != null)
        {
            _attachment.DetachFromParent();
            _attachment.DetachedFromGrid();
            _attachment = null;
        }
    }

    public virtual void DetachFromParent()
    {
        _parentObject = null;
    }

    public virtual void FillFromNeighbour(GridObject neighbour)
    {
        if (neighbour.Height >= Height || neighbour.IsFlooded)
        {
            AddToFillAmount(neighbour.CurrentFillAmount * FillRate * Time.deltaTime);
        }
    }

    public virtual void AddToFillAmount(float fillPctAdded)
    {
        if (fillPctAdded <= 0) return;
        if (_currentFill < 1f)
        {
            _currentFill = Mathf.Clamp01(_currentFill + fillPctAdded);

            if (_currentFill > 0)
            {
                if (_waterPlane != null)
                {
                    _waterPlane.SetFill(CurrentFillAmount);
                }
            }

            if (_currentFill == 1f)
            {
                Flood();
            }
        }
    }

    protected virtual void Flood()
    {
        _currentFill = 1f;
        _isFlooded = true;
        if (_waterPlane != null)
        {
            _waterPlane.SetFill(CurrentFillAmount);
        }
        if (Attachment != null)
        {
            Attachment.Flood();
            Attachment.Die();
        }

        ReturnBuilder();
    }

    protected virtual void Die()
    {
        if (isDead) return;

        if (_audioSource != null && _destructionSound != null)
        {
            print("Playing DestructionSound");
            AudioManager.Instance.Play(_audioSource, _destructionSound, AudioManager.AudioGroup.Other);
        }

        isDead = true;

        DraggableObject draggable = GetComponent<DraggableObject>();
        if (draggable != null)
        {
            Destroy(draggable);
        }
        Destroy(this);
        
        //gameObject.SetActive(false);
    }

    public virtual void Drain(float amount)
    {
        if (CurrentFillAmount > 0)
        {
            _currentFill = Mathf.Clamp01(_currentFill - amount);

            if (IsFlooded && CurrentFillAmount <= _reclaimThreshold)
            {
                Reclaim();
            }
        }
    }

    protected virtual void Reclaim()
    {
        _currentFill = 0;
        if (_waterPlane != null)
        {
            _waterPlane.SetFill(CurrentFillAmount);
        }

        if (IsFlooded)
        {
            if (Attachment != null)
            {
                Destroy(Attachment.gameObject);
                DetachObject();
            }
            _isFlooded = false;
        }

    }

    public virtual void Tsunami(float force)
    {
        if(IsFlooded)
        {
            if (_hasHadWave) return;

            new Job(PropegateWave(force));
        }
        else
        {
            HitByWave(force);
        }
        _hasHadWave = true;
    }

    private IEnumerator PropegateWave(float force)
    {
        PoolBoss.Spawn(GameManager.Instance.TsunamiFX.transform, transform.position, Quaternion.identity, null, true);
        WaveManager.Instance.ResetRecedeTimer();
        yield return new WaitForSeconds(0.8f);

        GridCell[] neighbours = GridManager.Instance.GetNeighbors(Coordinates);
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].IsOccupied)
            {
                if (neighbours[i].Occupant.Coordinates.y > Coordinates.y)
                {
                    neighbours[i].Occupant.Tsunami(force);
                }
                else
                {
                    if(neighbours[i].Occupant.Attachment == null)
                    {
                        neighbours[i].Occupant.Tsunami(force);
                    }
                    else
                    {
                        neighbours[i].Occupant.Tsunami(0.25f);
                    }
                }
            }
        }
    }

    private void HitByWave(float force)
    {
        FillRate += WaveManager.Instance.WaveDanger * force * 0.1f;
        AddToFillAmount(FillRate * WaveManager.Instance.WaveDanger * force);
    }

    private void ResetTsunami()
    {
        _hasHadWave = false;
    }

    protected virtual void Update()
    {
        if (!HasBeenPlaced) return;
        if (CurrentFillAmount > 0)
        {
            if (!BlocksWater || IsFlooded)
            {
                GridCell[] neighbours = GridManager.Instance.GetNeighbors(Coordinates);
                for (int i = 0; i < neighbours.Length; i++)
                {
                    if (neighbours[i].IsOccupied)
                    {
                        neighbours[i].Occupant.FillFromNeighbour(this);
                    }
                }
            }

            Drain(DrainRate * Time.deltaTime);
        }
    }

    public virtual float FillRate
    {
        get
        {
            if (Attachment != null)
            {
                return Attachment.FillRate;
            }
            return _fillRate;
        }

        set
        {
            if (Attachment != null)
            {
                Attachment.FillRate = value;
            }
            else
            {
                _fillRate = value;
            }
        }
    }

    public float LocalFillRate
    {
        get
        {
            return _fillRate;
        }
    }

    public float CurrentFillAmount
    {
        get { return _currentFill; }
    }

    public bool IsFlooded
    {
        get { return _isFlooded; }
    }

    public float Height
    {
        get { return _height; }
    }

    public virtual float DrainRate
    {
        get
        {
            if (Attachment != null)
            {
                return Attachment.DrainRate;
            }
            return _drainRate;
        }
    }

    public float LocalDrainRate
    {
        get
        {
            return _drainRate;
        }
    }

    public bool BlocksWater
    {
        get
        {
            if (Attachment != null)
            {
                return Attachment.BlocksWater;
            }
            return _blockWater;
        }
    }

    public bool BlocksPeople
    {
        get
        {
            if (Attachment != null)
            {
                return Attachment.BlocksPeople;
            }
            return _blockPeople;
        }
    }
}
