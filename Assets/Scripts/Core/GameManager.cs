using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Helpers;

public class GameManager : SingletonBehavior<GameManager>
{
	public static event Action onGameReady;

    private Queue<Builder> _availableBuilders = new Queue<Builder>();
    private List<Builder> _workingBuilders = new List<Builder>();

    private Queue<Marshal> _availableMarshals = new Queue<Marshal>();
    private List<Marshal> _workingMarshals = new List<Marshal>();

    private GameObject[] _safeZones;

    [SerializeField]
    private int _minSpawns = 10;
    [SerializeField]
    private int _maxSpawns = 20;

    [SerializeField]
    private int _totalSpawns = 100;

    private long _spawnIndex;

	private bool _isGameReady = false;

    [SerializeField]
    private GridObject _headquarters;
    public GridObject HQ
    {
        get { return _headquarters; }
    }

    [SerializeField]
    private GameObject _cameraRig;


    [SerializeField]
    private OutlineEffect _outlineEffect;
    public OutlineEffect OutlineEffect
    {
        get
        {
            return _outlineEffect;
        }
    }

	public bool IsGameReady
	{
		get
		{
			return _isGameReady;
		}
	}

    public int AvailableBuilders
    {
        get { return _availableBuilders.Count; }
    }

    public int TotalBuilders
    {
        get { return _availableBuilders.Count + _workingBuilders.Count; }
    }

    public int AvailableMarshals
    {
        get { return _availableMarshals.Count; }
    }

    public int TotalMarshals
    {
        get { return _availableMarshals.Count + _workingMarshals.Count; }
    }

    #region MONOBEHAVIOR METHODS

    protected override void Init()
    {
        base.Init();

        _cameraRig.SetActive(true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        //Debug keys

#endif
    }

    private void Start()
    {
		Job.Create(InitializeGame());
    }

#endregion

#region GridObject Creation
    public GridObject CreateBuilding(string templateID)
    {
		GridObject template = TemplateManager.Instance.Get<PurchasableObject>(templateID);

        if (template != null)
        {
			PurchasableObject buildingData = (PurchasableObject)template;
            if (buildingData != null)
            {
                if (HasResource(buildingData.BuildingCost.ResourceCost.Type, buildingData.BuildingCost.ResourceCost.Amount))
                {
                    GridObject gridObject = TemplateManager.Instance.Spawn<GridObject>(template);
                    gridObject.UID = GetNewUID(gridObject);
                    return gridObject;
                }
                ShowMessage("Not enough Cash!");
            }
            else
            {
                Debug.LogError("Can not create building '" + templateID + "' because it is not of type 'BaseBuilding'.");
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a new unique identifier for a grid object.
    ///  Calling this will increment the _spawnIndex so don't use it just to check what it's currently at.
    /// </summary>
    /// <returns>A UID</returns>
    public string GetNewUID(GridObject gridObject)
    {
        if (_spawnIndex < long.MaxValue)
        {
            _spawnIndex++;
        }
        else
        {
            _spawnIndex = 0;
            Debug.LogError("_spawnIndex has reached max value. Somethng is seriously messed up!");
        }
        return gridObject.TemplateID + "_" + _spawnIndex.ToString("0000");
    }

#endregion


#region Builders
    public void AddBuilder(Builder newBuilder, bool assigned = false)
    {
        if (assigned)
        {
            _workingBuilders.Add(newBuilder);
        }
        else
        {
            _availableBuilders.Enqueue(newBuilder);
        }
    }

    public Builder AssignBuilder()
    {
        if (_availableBuilders.Count > 0)
        {
            Builder builder = _availableBuilders.Dequeue();
            _workingBuilders.Add(builder);
            return builder;
        }
        return null;
    }

    public void ReturnBuilder(Builder builder)
    {
        if (_workingBuilders.Contains(builder))
        {
            _workingBuilders.Remove(builder);
            _availableBuilders.Enqueue(builder);
        }
    }

    #endregion

    #region Builders
    public void AddMarshal(Marshal newMarshal, bool assigned = false)
    {
        if (assigned)
        {
            _workingMarshals.Add(newMarshal);
        }
        else
        {
            _availableMarshals.Enqueue(newMarshal);
        }
    }

    public Marshal AssignMarshal()
    {
        if (_availableMarshals.Count > 0)
        {
            Marshal marshals = _availableMarshals.Dequeue();
            _workingMarshals.Add(marshals);
            return marshals;
        }
        return null;
    }

    public void ReturnMarshal(Marshal marshal)
    {
        if (_workingMarshals.Contains(marshal))
        {
            _workingMarshals.Remove(marshal);
            _availableMarshals.Enqueue(marshal);
        }
    }

    #endregion

    private List<Builder> _builderTemplates;
    public string GetRandomBuilderTemplate()
    {
        if(_builderTemplates == null)
        {
            _builderTemplates = TemplateManager.Instance.GetAll<Builder>();
        }

        return _builderTemplates[UnityEngine.Random.Range(0, _builderTemplates.Count)].TemplateID;
    }

    private List<Marshal> _marshalTemplates;
    public string GetRandomMarshalTemplate()
    {
        if (_marshalTemplates == null)
        {
            _marshalTemplates = TemplateManager.Instance.GetAll<Marshal>();
        }

        return _marshalTemplates[UnityEngine.Random.Range(0, _marshalTemplates.Count)].TemplateID;
    }

    private List<Civilian> _civilianTemplates;
    public string GetRandomCivilianTemplate()
    {
        if (_civilianTemplates == null)
        {
            _civilianTemplates = TemplateManager.Instance.GetAll<Civilian>();
        }

        return _civilianTemplates[UnityEngine.Random.Range(0, _civilianTemplates.Count)].TemplateID;
    }

    #region Resource Management

    public bool SpendResource(Helpers.ResourceType resourceType, int amount)
    {
        if (HasResource(resourceType, amount))
        {
            //NONE is effectively free.
            if (resourceType == Helpers.ResourceType.NONE)
            {
                return true;
            }
            return PlayerProfileManager.Instance.SpendResources(resourceType, amount);
        }
        else
        {
            Debug.Log("Not enough " + resourceType.ToString() + " to complete purchase.");
            return false;
        }
    }

    public bool AddResource(Helpers.ResourceType resourceType, int amount)
    {
        if (resourceType == Helpers.ResourceType.NONE)
        {
            return true;
        }

        if (amount < 0)
        {
            Debug.LogWarning("Attempted to add negative resource amount. This is not allowed to prevent hacking");
            return false;
        }

        if (amount == 0)
        {
            return true;
        }

        return PlayerProfileManager.Instance.AddResources(resourceType, amount);
    }

    public bool HasResource(Helpers.ResourceType resourceType, int amount)
    {

        if (resourceType == Helpers.ResourceType.NONE)
        {
            return true;
        }

        if (PlayerProfileManager.Instance.GetResourceBalance(resourceType) >= amount)
        {
            return true;
        }
        return false;
    }

#endregion

	// Game initialization routine.
	private IEnumerator InitializeGame()
	{
		// Wait for all the Start() in all components to be called and completed
		yield return new WaitForEndOfFrame();

        GridManager.Instance.CreateGrid();

		// Fire the event so game object know when the game has been initialized and can be played
		if(onGameReady != null)
			onGameReady();

		_isGameReady = true;

        _safeZones = GameObject.FindGameObjectsWithTag("SafeZone");
        List<Road> allRoads = GridManager.Instance.GetAllGridObjectsOfType<Road>(GridObjectTypes.Building.ROAD);
        while(_totalSpawns > 0)
        {
            int randAmount = Mathf.Clamp(UnityEngine.Random.Range(_minSpawns, _maxSpawns + 1), 0, _totalSpawns);
            int randRoad = UnityEngine.Random.Range(0, allRoads.Count);
            allRoads[randRoad].AddCivilians(randAmount);
            _totalSpawns -= randAmount;
            allRoads.Remove(allRoads[randRoad]);
        }
	}

    public GameObject FindNearestSafeZone(Vector3 position)
    {
        GameObject best = null;
        float dist = float.MaxValue;
        for(int i = 0; i < _safeZones.Length; i++)
        {
            float newDist = (_safeZones[i].transform.position - position).magnitude;
            if(newDist < dist)
            {
                best = _safeZones[i];
                dist = newDist;
            }
        }
        return best;
    }

    public void ShowMessage(string message)
    {
        UIManager.Instance.Get<UIMessagePanel>(UIManager.PanelID.GameMessagePanel).Show(message);
    }
}
