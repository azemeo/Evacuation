using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour {

    Transform popup;

	// Use this for initialization
	void Start () {
        popup = transform.FindChild("Popup Panel");
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void OnMouseEnter()
    {
        print("Mouseover");
        popup.gameObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        print("Mouse Exit");
        popup.gameObject.SetActive(false);
    }

    public void Buy(string itemName)
    {
        print("Purchased" + itemName);
    }
}
