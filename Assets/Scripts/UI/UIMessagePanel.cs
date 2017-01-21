using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMessagePanel : UIPanel {

	[SerializeField]
	private Text _messageLabelPrefab;

	private List<Text> _messages = new List<Text>();
	private List<Job> _animations = new List<Job>();

	public void Show(string message)
	{
		Text messageLabel = PoolBoss.Spawn(_messageLabelPrefab.transform, transform.position, Quaternion.identity, transform, false).GetComponent<Text>();
		messageLabel.rectTransform.localPosition = Vector3.zero;
		messageLabel.transform.localScale = Vector3.one;

		messageLabel.text = message;
		_messages.Add(messageLabel);
		_animations.Add(Job.Create(ShowRoutine(messageLabel)));
	}

	void OnDisable()
	{
		FlushMessages();
	}

	private IEnumerator ShowRoutine(Text messageLabel)
	{
		messageLabel.transform.localScale = Vector3.zero;

		Hashtable ht = new Hashtable();
		ht.Add("time", 0.25f);
		ht.Add("scale", Vector3.one);
		ht.Add("easetype", iTween.EaseType.easeOutBack);

		iTween.ScaleTo(messageLabel.gameObject, ht);

		yield return new WaitForSeconds(2.0f);

		ht["time"] = 0.1f;
		ht["scale"] = Vector3.zero;
		ht["easetype"] = iTween.EaseType.easeInQuad;

		iTween.ScaleTo(messageLabel.gameObject, ht);

		yield return new WaitForSeconds(0.3f);
		_messages.Remove(messageLabel);

		PoolBoss.Despawn(messageLabel.transform);
	}

	public void FlushMessages()
	{
		for(int i = 0; i < _animations.Count; ++i)
		{
			_animations[i].Kill();
		}

		for(int i = 0; i < _messages.Count; ++i)
		{
			PoolBoss.Despawn(_messages[i].transform);
		}
	}
}
