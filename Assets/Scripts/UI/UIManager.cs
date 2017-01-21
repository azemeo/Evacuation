using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : SingletonBehavior<UIManager>
{
    public enum PanelID
    {
        NONE = 0,
        SelectionPanel = 1,
        TrainableInfoPanel = 2,
        PlacementPanel = 3,
        BuildTimersPanel = 4,
        CalloutsPanel = 5,
		BarracksSelectionPanel = 6,
        InfoPanel = 7,
        UpgradePanel = 8,
        ResearchPanel = 9,
        TrainingPanel = 10,
		GameMessagePanel = 11,
        ResearchUpgradePanel = 12,
        ShopPanel = 13,
        DebugPanel = 14,
        EditorPanel = 15,
        EditorSelectionPanel = 16,
        SpellTrainingPanel = 17,
        ScenarioPanel = 18,
        BattleUI = 19,
        GameUI = 20,
        BattleCompletePanel = 21,
        DamageBarsPanel = 22,
		RaidInProgressPanel = 23,
        GroupUpgradePanel = 24,
        EmpirePanel = 25,
		DialogPanel = 26,
		DeleteTroopPanel = 27,
		SettingsPanel = 28,
		PlayerProfilePanel = 29,
		PlayerProfileTownHallPanel = 30,
		TrainableUpgradePanel = 31,
        MessagePanel = 32,
		ChatPanel = 33,
		RunningBattlePanel = 34,
		PlayerProfileInfoPanel = 35,
        ReplayUI = 36
	}

    [System.Serializable]
    public class UIStateData
    {
        public int State;
        public GameObject[] ActiveObjects;

        public void SetActive(bool value)
        {
            for(int i = 0; i < ActiveObjects.Length; i++)
            {
                ActiveObjects[i].SetActive(value);
            }
        }
    }

    [SerializeField]
    private UIStateData[] GameStates;

	[SerializeField]
	private Material _disabledItemMaterial;

    private Dictionary<PanelID, UIPanel> _registeredPanels = new Dictionary<PanelID, UIPanel>();
	private Dictionary<int, UIStateData> _states = new Dictionary<int, UIStateData>();
	int _currentUIState = FSMStateTypes.Game.EMPTY;

    protected override void Init()
    {
        base.Init();

        //auto register everything under the UI manager
        foreach(UIPanel panel in GetComponentsInChildren<UIPanel>(true))
        {
            if (!IsRegistered(panel))
            {
                Register(panel);
            }
        }

        for (int i = 0; i < GameStates.Length; i++)
        {
            _states[GameStates[i].State] = GameStates[i];
            GameStates[i].SetActive(false);
        }

        GameStateManager.onStateChanged += gameStateChanged;

        if (_states.ContainsKey(GameStateManager.Instance.CurrentStateType))
        {
            //kick off the initial update, since the state change has already happened
			gameStateChanged(GameStateManager.Instance.PreviousStateType, GameStateManager.Instance.CurrentStateType);
        }
    }

    private void OnDestroy()
    {
        GameStateManager.onStateChanged -= gameStateChanged;
    }

    private void gameStateChanged(int previousStateType, int newStateType)
    {
        //only change UI state if the state is actually defined
        if (_states.ContainsKey(newStateType))
        {
            //clear the old state if it's set
            if (_states.ContainsKey(_currentUIState))
            {
                _states[_currentUIState].SetActive(false);
            }

			_currentUIState = newStateType;
            _states[_currentUIState].SetActive(true);
        }
    }

    public void Register(UIPanel panel)
    {
        if (!_registeredPanels.ContainsKey(panel.PanelID))
        {
            _registeredPanels.Add(panel.PanelID, panel);
        }
        else
        {
            Debug.LogError("Panel ID '" + panel.PanelID.ToString() + "' is already registered with UIManager. Please create a new ID first");
        }
    }

    public bool IsRegistered(UIPanel panel)
    {
        return _registeredPanels.ContainsKey(panel.PanelID);
    }

    public T Get<T>(PanelID id) where T : UIPanel
    {
        if (_registeredPanels.ContainsKey(id))
        {
            if (_registeredPanels[id] is T)
            {
                return _registeredPanels[id] as T;
            }
            else
            {
                Debug.LogError("UIPanel mathcing ID '" + id.ToString() + "' is not of type '" + typeof(T).ToString() + "'.");
            }
        }
        else
        {
            Debug.LogError("No UIPanel with id '" + id.ToString() + "' registered with UIManager.");
        }
        return null;
    }

	public void SetItemEnabled(GameObject itemObject, bool isEnabled)
	{
		Graphic[] graphics = itemObject.GetComponentsInChildren<Graphic>();

		for(int i = 0; i < graphics.Length; ++i)
		{
			graphics[i].material = isEnabled ? graphics[i].defaultMaterial : _disabledItemMaterial;
		}
	}
}
