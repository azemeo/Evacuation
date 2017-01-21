using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonPlaySFX : MonoBehaviour {

	void Start ()
	{
		GetComponent<Button>().onClick.AddListener(ButtonPressed);
	}

	void ButtonPressed()
	{
		//AudioManager.Instance.PlayButtonSFX();
	}
}
