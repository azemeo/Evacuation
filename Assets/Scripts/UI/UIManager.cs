using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : SingletonBehavior<UIManager>
{
    public enum PanelID
    {
        NONE = 0,
        PlacementPanel = 1,
        BuildTimersPanel = 2,
        DamageBarsPanel = 3,
        GameMessagePanel = 4
    }

    public Text CountdownDisplay;
    public Text CashDisplay;
    public Text AvailableBuildersDisplay;
    public Text AvailableMarshalsDisplay;

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

    void Update()
    {
        CashDisplay.text = "$" + PlayerProfileManager.Instance.GetResourceBalance(Helpers.ResourceType.Cash);
        if (WaveManager.Instance != null)
        {

            if (TimerManager.Instance.IsTimerRunning("big_wave_arrive"))
            {
                CountdownDisplay.text = string.Format("{0} until wave {1}", TimerManager.Instance.GetTimerLeftFormatted("big_wave_arrive"), WaveManager.Instance.NextWave);
                //WaveStatsDisplay.text = string.Format("Height: {0}m   Danger: {1}", WaveManager.Instance.GetWaveHeight(), WaveManager.Instance.GetWaveDanger());
            }
            else
            {
                CountdownDisplay.text = "Wave Arrived!";
            }
            System.TimeSpan timeToWave = System.TimeSpan.FromSeconds(WaveManager.Instance.NextWaveTime - Time.time);
            CountdownDisplay.text = string.Format("{0}:{1} until wave {2}", timeToWave.Minutes, timeToWave.Seconds, WaveManager.Instance.NextWave);
            CashDisplay.text = "$" + PlayerProfileManager.Instance.GetResourceBalance(Helpers.ResourceType.Cash);
         }

        if (AvailableBuildersDisplay != null && AvailableMarshalsDisplay != null)
        {
            AvailableBuildersDisplay.text = string.Format("{0}/{1}", GameManager.Instance.AvailableBuilders, GameManager.Instance.TotalBuilders);
            AvailableMarshalsDisplay.text = string.Format("{0}/{1}", GameManager.Instance.AvailableMarshals, GameManager.Instance.TotalMarshals);
        }
    }

}
