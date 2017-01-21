using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class FallbackEventReceiver : SingletonBehavior<FallbackEventReceiver>, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    #region Camera control
    [Header("Camera")]
    [SerializeField]
    private Transform _cameraTrack;
    [SerializeField]
    private Transform _cameraPivot;
    [SerializeField]
    private Transform _cameraAngle;

	[SerializeField]
	private Camera _mainCam;

    [SerializeField]
    private Camera _uiCam;

    [SerializeField]
    private float _zoomSpeed = 5f;
    [SerializeField]
    private float _maxZoom = -15f;
    [SerializeField]
    private float _minZoom = -25f;
	[SerializeField]
	private float _zoomingThreshold = 10;

    [SerializeField]
    private float _cameraClampEdgeDistance = 3;

    private bool _isDragging = false;
    private bool _isZooming = false;

	private float _thresholdZoomingAmount = 0.0f;
    #endregion

    #region Spawning control
    [SerializeField]
    private float _holdDelay;
    [SerializeField]
    private float _holdStartDelay;

	[SerializeField]
	private PhysicsRaycaster _physicsRaycaster;

    private bool _isPressed = false;
    private bool _isHeld = false;
    private float _holdTimer;

    private Plane _dragPlane;

    private PointerEventData _heldEventData;

    #endregion

    public Camera MainCam
    {
        get
        {
            return _mainCam;
        }
    }

    public bool IsCameraMoving
    {
        get
        {
            return _isDragging;
        }
    }

    protected override void Init()
    {
        base.Init();
        _dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    private float unitsPerPixel(Camera cam, float z)
    {
        Vector3 p1 = cam.ScreenToWorldPoint(new Vector3(0,0, z));
        Vector3 p2 = cam.ScreenToWorldPoint(new Vector3(1f, 0f, z));
        return Vector3.Distance(p1, p2);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDragging && !_isHeld)
        {
            _isPressed = false;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _isDragging = true;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _heldEventData = eventData;
        if (!_isZooming)
        {
            if (_isDragging)
            {
                float units = unitsPerPixel(MainCam, -_cameraTrack.localPosition.z);

                //translate it in local space so that it moves on the correct axis with respect to its current rotation
                _cameraPivot.Translate(new Vector3(-eventData.delta.x * units, 0, -eventData.delta.y * units), Space.Self);

                clampCameraToGridBounds();
            }
        }
    }

    private void clampCameraToGridBounds()
    {
        //clamp it to the Grid Bounds
        Vector3 localPos = _cameraPivot.transform.localPosition;
        localPos.x = Mathf.Clamp(localPos.x, GridManager.Instance.GridBounds.min.x + _mainCam.orthographicSize - _cameraClampEdgeDistance, GridManager.Instance.GridBounds.max.x - _mainCam.orthographicSize + _cameraClampEdgeDistance);
        localPos.z = Mathf.Clamp(localPos.z, GridManager.Instance.GridBounds.min.z + _mainCam.orthographicSize - _cameraClampEdgeDistance, GridManager.Instance.GridBounds.max.z - _mainCam.orthographicSize + _cameraClampEdgeDistance);
        _cameraPivot.transform.localPosition = localPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }


	// this is just a temporary method to reset the camera to its default values
	public void ResetCamera()
	{
		_cameraPivot.rotation = Quaternion.Euler(0, 225, 0);
		_cameraPivot.position = Vector3.zero;

		_cameraTrack.localPosition = new Vector3(0, 0, -25);
	}

    public void OnPointerClick(PointerEventData eventData)
    {
		if (!_isDragging && eventData.button == PointerEventData.InputButton.Left)
        {
            if (GridManager.Instance.ActiveObject != null && GridManager.Instance.ActiveObject.HasBeenPlaced)
            {
                GridManager.Instance.DeselectObject();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
		if (_isPressed)
        {
            _isPressed = false;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        _isPressed = false;
        Zoom(eventData.scrollDelta.y);
    }

    public void Zoom(float delta)
    {
        float newZ = Mathf.Clamp(MainCam.transform.localPosition.z + (delta * Time.deltaTime * _zoomSpeed), _minZoom, _maxZoom);
        _mainCam.transform.localPosition = new Vector3(0,0,newZ);
        //_uiCam.orthographicSize = newZ;

        clampCameraToGridBounds();
    }
}
