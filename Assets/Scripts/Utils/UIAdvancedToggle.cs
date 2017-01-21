using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAdvancedToggle : Toggle {

	public GameObject onGameObject;
	public GameObject offGameObject;

	public void SetOnOffState(bool isOn)
	{
		this.isOn = isOn;
		UpdateToggleGraphics();
	}

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerClick (eventData);
		UpdateToggleGraphics();
	}

	private void UpdateToggleGraphics()
	{
		onGameObject.SetActive(isOn);
		offGameObject.SetActive(!isOn);
	}
}
