using UnityEngine;
using System.Collections.Generic;

public class UIBuildTimersPanel : UIPanel
{
    [SerializeField]
    private UITimer _timerPrefab;

    private Dictionary<string, UITimer> _timersDict = new Dictionary<string, UITimer>();

	public void AddTimer(string timerID, Transform objectToFollow, Vector3? offset = null)
    {
        UITimer timer = null;
        if (!_timersDict.ContainsKey(timerID))
        {
            timer = Instantiate(_timerPrefab).GetComponent<UITimer>();
            timer.transform.SetParent(transform, true);
            timer.transform.localScale = Vector3.one;
            timer.transform.localRotation = Quaternion.identity;
            _timersDict.Add(timerID, timer);
        }
        else
        {
            timer = _timersDict[timerID];
        }

        if (timer != null)
        {
			if(offset.HasValue)
				timer.transform.localPosition = offset.Value;
			
            timer.SetTimer(timerID);
            UIWorldPositioner worldPos = timer.GetComponent<UIWorldPositioner>();
            if (worldPos != null)
            {
                worldPos.ObjectToFollow = objectToFollow;

				if(offset.HasValue)
					worldPos.Offset = offset.Value;
            }
        }
    }

    public void RemoveTimer(string timerID)
    {
        if (_timersDict.ContainsKey(timerID))
        {
            Destroy(_timersDict[timerID].gameObject);
            _timersDict.Remove(timerID);
        }
    }

}
