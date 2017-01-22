using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPlacementPanel : UIPanel {
    [SerializeField]
    private Button _confirmPlacementButton;

    private GridObject _selectedObject;

    public void Show(GridObject selected)
    {
        if (selected == null)
        {
            Hide();
        }

        _selectedObject = selected;

        //float offset = GridManager.Instance.GridCellSize;
        //Debug.Log(offset);
        //transform.localPosition = new Vector3(0, 0, -offset);


        gameObject.SetActive(true);

        UpdatePlacementButtons();
    }

    public void Hide()
    {
        _selectedObject = null;
        gameObject.SetActive(false);
    }

    public void UpdatePlacementButtons()
    {
        if (_selectedObject != null)
        {
            if (_confirmPlacementButton == null)
                return;

            _confirmPlacementButton.interactable = GridManager.Instance.CanBePlaced(_selectedObject);
			UIManager.Instance.SetItemEnabled(_confirmPlacementButton.gameObject, _confirmPlacementButton.interactable);
        }
    }

    public void ConfirmPlacementPressed()
    {
        if (_selectedObject != null)
        {
            GridManager.Instance.ClearVisualization(_selectedObject);

            //First update the coordinates of the object to its new location...
            _selectedObject.SetCoordinates(GridManager.Instance.GetGridCoordinates(_selectedObject.transform.position));
            //and then notify the GridManager of its new home.
            GridManager.Instance.PlaceObject(_selectedObject);
            GridManager.Instance.DeselectObject();

            _selectedObject.ConfirmPlacement();
        }
    }

    public void CancelPlacementPressed()
    {
        //GridManager.Instance.ClearVisualization(_selectedObject);
        GridManager.Instance.DeselectObject();
        Hide();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmPlacementPressed();
        }
    }
}
