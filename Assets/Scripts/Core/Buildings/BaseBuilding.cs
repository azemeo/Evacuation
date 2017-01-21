using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Helpers;

public abstract class BaseBuilding : PurchasableObject {
    #region Class Data Structures
    public struct BuildingLevel
    {
        public int MaxHitPoints;      // maximum number of HP for this structure at this level
        public BuildRequirement BuildCost;    // resource and other structure requirements
    }
    #endregion

    [Header("Building Setup")]
    [SerializeField] private GameObject _constructionGraphic;

    [SerializeField]
    protected BuildingLevel _buildingLevel;

    [SerializeField]
    private bool _requiresBuilder = false;

    [SerializeField]
    protected GameObject _model;

    [SerializeField]
    protected float _fillRate = 0.1f;

    [SerializeField]
    protected float _drainRate = 0.05f;

    protected bool _isAlive = true;
    [SerializeField]
    protected float _currentFill = 0;
    protected float _height = 0;
    protected bool _isFlooded = false;

    private bool _filledThisFrame = false;

    [SerializeField]
    protected WaterPlane _waterPlane;


    #region MonoBehaviour Methods

    protected override void OnEnable()
    {
        base.OnEnable();
        TimerManager.onTimerExpired += new TimerManager.TimerExpiredDelegate(onTimerExpired);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TimerManager.onTimerExpired -= new TimerManager.TimerExpiredDelegate(onTimerExpired);
    }

    protected virtual void onTimerExpired(TimerManager.TimerEventData eventData)
    {
        if (eventData.id == UID + "_build")
        {
            BuildComplete(eventData.EndTime);
        }
    }

    #endregion

    #region Core Methods

    protected override void StartBuild()
    {
        if (BuildingCost.BuildTime > 0)
        {
            if (_requiresBuilder) AssignBuilder();

            TimerManager.Instance.StartTimerNow(UID + "_build", BuildingCost.BuildTime);
            UIManager.Instance.Get<UIBuildTimersPanel>(UIManager.PanelID.BuildTimersPanel).AddTimer(UID + "_build", UIAnchor, Vector3.up * 2);
            if (_constructionGraphic != null)
            {
                _constructionGraphic.SetActive(true);
                Model.gameObject.SetActive(false);
            }

            BuildStarted();
        }
        else
        {
            BuildComplete(DateTime.Now);
        }
    }

    public override void OnSpawned()
    {
        base.OnSpawned();

        Vector3 modelLocalPosition = _collider.transform.localPosition;
        modelLocalPosition.y = 0;
        _model.transform.localPosition = modelLocalPosition;
    }

    protected virtual void BuildStarted() { }

    public virtual void BuildComplete(System.DateTime timeCompleted)
    {
        if (_constructionGraphic != null)
        {
            _constructionGraphic.SetActive(false);
        }

        Model.gameObject.SetActive(true);

        //if this was called because of skipping rather than waiting, cancel the exisiting timer.
        if (TimerManager.Instance.IsTimerRunning(UID + "_build"))
        {
            TimerManager.Instance.CancelTimer(UID + "_build");
        }

        ReturnBuilder();

        UIManager.Instance.Get<UIBuildTimersPanel>(UIManager.PanelID.BuildTimersPanel).RemoveTimer(UID + "_build");

        //AudioManager.Instance.Play(AudioSFXDatabase.Instance.BuildSFX, AudioManager.AudioGroup.Building_Created);
    }

    public virtual void FillFromNeighbour(BaseBuilding neighbour)
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

            if(_currentFill > 0)
            {
                if(_waterPlane != null)
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
    }

    public virtual void Drain(float amount)
    {
        if(CurrentFillAmount > 0)
        {
            _currentFill = Mathf.Clamp01(_currentFill - amount);

            if (IsFlooded && CurrentFillAmount <= 0)
            {
                Reclaim();
            }
        }
    }

    protected virtual void Reclaim()
    {
        _currentFill = 0;
        _isFlooded = false;
        if (_waterPlane != null)
        {
            _waterPlane.SetFill(0f);
        }
    }

    protected virtual void Update()
    {
        if (!HasBeenPlaced) return;
        if (CurrentFillAmount > 0)
        {
            GridCell[] neighbours = GridManager.Instance.GetNeighbors(Coordinates);
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i].IsOccupied && neighbours[i].Occupant is BaseBuilding)
                {
                    BaseBuilding bb = neighbours[i].Occupant as BaseBuilding;
                    bb.FillFromNeighbour(this);
                }
            }

            Drain(_drainRate * Time.deltaTime);
        }
    }

    #endregion

    #region Public Accessors

    public bool IsBuilding
    {
        get { return TimerManager.Instance.IsTimerRunning(UID + "_build"); }
    }

    public override float ConstructionTimeRemaining
    {
        get
        {
            float timeRemaining = 0;
            if (IsBuilding)
            {
                timeRemaining = (float)TimerManager.Instance.GetTimerLeftDateTime(UID + "_build").TotalSeconds;
            }

            return timeRemaining;
        }
    }

	public override BuildRequirement BuildingCost
    {
        get { return _buildingLevel.BuildCost; }
    }

    public bool RequiresBuilder
    {
        get { return _requiresBuilder; }
    }

    public GameObject Model
    {
        get
        {
            return _model;
        }
    }

    public float FillRate
    {
        get { return _fillRate; }
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

    public float DrainRate
    {
        get { return _drainRate; }
        set { _drainRate = value; }
    }
    #endregion
}

