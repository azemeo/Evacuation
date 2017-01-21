using UnityEngine;
using System.Collections;

public class UITimer : MonoBehaviour {

    [SerializeField]
    private UIFillBar _fillBar;

    private string _timerID;

    public void SetTimer(string timerID)
    {
        _timerID = timerID;
        if (!string.IsNullOrEmpty(timerID))
        {
            UpdateTimer();
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(_timerID))
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        if (TimerManager.Instance.IsTimerRunning(_timerID))
        {
            _fillBar.Set(TimerManager.Instance.GetTimerElapsedTimeNormalized(_timerID), TimerManager.Instance.GetTimerLeftFormatted(_timerID));
        }
    }
}
