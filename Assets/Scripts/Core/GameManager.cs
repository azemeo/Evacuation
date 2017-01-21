using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Helpers;

public class GameManager : SingletonBehavior<GameManager>
{
	public static event Action onGameReady;

    private Queue<AIAgent> _availableBuilders = new Queue<AIAgent>();
    private List<AIAgent> _workingBuilders = new List<AIAgent>();

    private long _spawnIndex;

	private bool _isGameReady = false;

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

	public static int LineOfSightMask
	{
		get
		{
			if(_lineOfSightMask == -1)
			{
				_lineOfSightMask = 1 << LayerMask.NameToLayer("Building");
				_lineOfSightMask |= 1 << LayerMask.NameToLayer("Obstacle");
			}

			return _lineOfSightMask;
		}
	}

	private static int _lineOfSightMask = -1;

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
                Debug.Log("Not enough " + buildingData.BuildingCost.ResourceCost.Type.ToString() + ".");
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
    public void AddBuilder(AIAgent newBuilder, bool assigned = false)
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

    public AIAgent AssignBuilder()
    {
        if (_availableBuilders.Count > 0)
        {
            AIAgent builder = _availableBuilders.Dequeue();
            _workingBuilders.Add(builder);
            return builder;
        }
        return null;
    }

    public void ReturnBuilder(AIAgent builder)
    {
        if (_workingBuilders.Contains(builder))
        {
            _workingBuilders.Remove(builder);
            _availableBuilders.Enqueue(builder);
        }
    }

#endregion

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
	}

    public void ShowMessage(string message)
    {
        UIManager.Instance.Get<UIMessagePanel>(UIManager.PanelID.GameMessagePanel).Show(message);
    }
}
