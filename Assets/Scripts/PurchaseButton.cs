using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour {

    Transform popup;

    [SerializeField]
    private string _templateID;

	// Use this for initialization
	void Start () {
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

    public void Buy()
    {


        if (BuildingCreator.Instance.CreateBuilding(_templateID))
        {
            print("Purchased" + _templateID);
        }
        else
        {
            print("Purchase Failed");
            GameManager.Instance.ShowMessage("Not enough Cash!");
        }
    }
}
