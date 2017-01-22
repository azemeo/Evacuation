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

	}

    public void OnMouseEnter()
    {
        popup.gameObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        popup.gameObject.SetActive(false);
    }

    public virtual void Buy(string itemName)
    {
        if (BuildingCreator.Instance.CreateBuilding(_templateID))
        {
            print("Purchased" + itemName);
        }
        else
        {
            GameManager.Instance.ShowMessage("Not enough Cash!");
        }
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
