using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour {

    [SerializeField]
    private Transform popup;

    [SerializeField]
    private string _templateID;

    [SerializeField]
    private BaseBuilding buildingTemplate;

    public KeyCode hotkey = KeyCode.None;

	// Use this for initialization
	protected void Start () {
        if (popup == null)
        {
            popup = transform.FindChild("Popup Panel");
        }

        DisplayCost();

	}

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(hotkey))
        {
            Buy();
        }
	}

    public void OnMouseEnter()
    {
        popup.gameObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        popup.gameObject.SetActive(false);
    }

    public virtual void Buy()
    {
        Buy(_templateID);
    }

    public virtual void Buy(string itemName)
    {
        BuildingCreator.Instance.CreateBuilding(_templateID);
    }

    protected virtual void DisplayCost()
    {
        Text costDisplay = transform.FindChild("Cost Display").GetComponent<Text>();
        if (costDisplay != null && buildingTemplate != null)
        {
            costDisplay.text = "$" + buildingTemplate.BuildingCost.ResourceCost.Amount;
        }
    }
}
