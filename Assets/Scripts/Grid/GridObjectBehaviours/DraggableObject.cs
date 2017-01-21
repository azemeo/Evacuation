using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class DraggableObject : SelectableObject, IDragHandler, IEndDragHandler, IBeginDragHandler {

    private Camera _gameCamera;
    private Plane _dragPlane;

    private bool _dragging = false;
    private bool _isPlacing = false;
    private Vector3 _dragOffset;

    void Start()
    {
        if (_gridObject == null)
        {
            initialize();
        }
    }

    protected override void initialize()
    {
        base.initialize();

        _gridObject.OnPlaced = new GridObject.GridEvent(OnPlaced);

        _dragPlane = new Plane(Vector3.up, Vector3.zero);
		_gameCamera = FallbackEventReceiver.Instance.MainCam;
    }


    #region Unity Events
    //Called by unity event system when starting a drag motion on the object
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_selected)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _dragging = true;
                float distance = 0f;
                Ray ray = _gameCamera.ScreenPointToRay(eventData.position);
                if (_dragPlane.Raycast(ray, out distance))
                {
                    _dragOffset = ray.GetPoint(distance) - transform.position;
                }

                DragStarted();
            }
        }
        else
        {
            ExecuteEvents.beginDragHandler.Invoke(FallbackEventReceiver.Instance, eventData);
        }
    }

    protected virtual void DragStarted()
    {
        _isPlacing = true;
        if (_gridObject.HasBeenPlaced)
        {
            GridManager.Instance.DetachObject(_gridObject, false);
        }
    }

    //Called by unity event system every frame that the pointer moves after a drag has begun
    public void OnDrag(PointerEventData eventData)
    {
        if (_selected)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                float distance = 0f;
                Ray ray = _gameCamera.ScreenPointToRay(eventData.position);
                if (_dragPlane.Raycast(ray, out distance))
                {
                    snapToGrid(ray.GetPoint(distance) - _dragOffset);
                }
            }
        }
        else
        {
            ExecuteEvents.dragHandler.Invoke(FallbackEventReceiver.Instance, eventData);
        }
    }

    //Called by unity event system upon releasing a drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_selected)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (GridManager.Instance.CanBePlaced(_gridObject) && _gridObject.HasBeenPlaced)
                {
                    placeObject();
                }
                _dragging = false;
                DragComplete();
            }
        }
        else
        {
            ExecuteEvents.endDragHandler.Invoke(FallbackEventReceiver.Instance, eventData);
        }

    }

    protected virtual void DragComplete()
    {

    }
    #endregion


    /// <summary>
    /// Snaps the object to a grid location that is closest to a given point, and visualizes the object on the grid.
    ///  Does not actually update the objects grid coordinates. Just the transform of it.
    /// </summary>
    /// <param name="point"></param>
    private void snapToGrid(Vector3 point)
    {
        if (_selected)
        {
			GridCell currentCell = GridManager.Instance.GetCell(transform.position);
			GridCell newCell = GridManager.Instance.GetCell(point);

            if (newCell != null)    //if newCell is null it's outside of the grid bounds.
            {
                if (newCell != currentCell)     //If we are still in the same cell, no need to update
                {
                    //finally, make sure the objects size doesn't make it stretch out of bounds
                    if (GridManager.Instance.IsRectWithinBounds(GridManager.Instance.GetGridCoordinates(point), _gridObject.Size))
                    {
                        ClearVisualization();
                        SetPosition(newCell.transform.position);
                        ShowVisualization();
                        if (_gridObject.PlacementPanel != null)
                        {
                            _gridObject.PlacementPanel.UpdatePlacementButtons();
                        }
                    }
                }
            }
        }
    }

    protected virtual void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    protected virtual void ClearVisualization()
    {
        GridManager.Instance.ClearVisualization(_gridObject);
    }
    protected virtual void ShowVisualization()
    {
        GridManager.Instance.VisualizePlacement(_gridObject);
        setHighlight();
    }

    /// <summary>
    /// Attempts to place the object on the grid at it's current transform location.
    ///  If it can't be placed, the object is reset.
    ///  This updates the coordinates of the gridObject if successful.
    /// </summary>
    protected virtual void placeObject()
    {
        if (GridManager.Instance.CanBePlaced(_gridObject))
        {
            GridManager.Instance.ClearVisualization(_gridObject);

            //First update the coordinates of the object to its new location...
            _gridObject.SetCoordinates(GridManager.Instance.GetGridCoordinates(transform.position));
            //and then notify the GridManager of its new home.
            GridManager.Instance.PlaceObject(_gridObject);
        }
        else
        {
            resetObject();
        }
    }

    /// <summary>
    /// Resets the object to its last valid coordinates, and clears the visualization for it.
    /// </summary>
	protected void resetObject()
    {
        if (_isPlacing)
        {
            GridManager.Instance.ClearVisualization(_gridObject);
            _gridObject.transform.position = GridManager.Instance.GetCell(_gridObject.Coordinates).transform.position;
            if (GridManager.Instance.CanBePlaced(_gridObject))
            {
                placeObject();
            }
            else
            {
                Debug.LogWarning("Could not find a valid placement to return object '" + _gridObject.name + "' to. Destroying.");
                PoolBoss.Despawn(_gridObject.transform);
            }
            _dragging = false;
            _isPlacing = false;
        }
        setHighlight();
    }

    protected override void setHighlight()
    {
        if (_isPlacing || !_gridObject.HasBeenPlaced)
        {
            if (GridManager.Instance.CanBePlaced(_gridObject))
            {
                setOutline(1);
                OutlineEffect.fillAmount = 0;
                //setMaterial(GameManager.Instance.HighlightMaterial);
            }
            else
            {
                setOutline(0);
                OutlineEffect.fillAmount = 0.4f;
                //setMaterial(GameManager.Instance.BadPlacementMaterial);
            }
        }
        else
        {
            base.setHighlight();
        }
    }

    protected override void OnDeselected()
    {
        if (_gridObject.HasBeenPlaced)
        {
            resetObject();
            base.OnDeselected();
        }
        else
        {
            PoolBoss.Despawn(_gridObject.transform);
        }
    }

    protected virtual void OnPlaced()
    {
        _isPlacing = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!_dragging)
        {
            base.OnPointerClick(eventData);
        }
    }
}
