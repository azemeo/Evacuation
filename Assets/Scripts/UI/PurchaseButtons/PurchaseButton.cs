using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour {

    [SerializeField]
    private Transform popup;

    [SerializeField]
    private string _templateID;

	// Use this for initialization
	protected void Start () {
        if(popup == null)
            popup = transform.FindChild("Popup Panel");
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
}
