using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(GridObject))]
public class SelectableObject : MonoBehaviour, IPointerClickHandler, IScrollHandler
{
    protected GridObject _gridObject;
    protected bool _selected = false;
    [SerializeField]
    protected Renderer[] _renderers;         //serialized in case we need to override for any reason
    [SerializeField]
    protected Transform _model;

    public OutlineEffect OutlineEffect
    {
        get
        {
            return GameManager.Instance.OutlineEffect;
        }
    }

    private void Awake()
    {
        if (_gridObject == null)
        {
            initialize();
        }
    }

    protected virtual void initialize()
    {
        _gridObject = GetComponent<GridObject>();

        if (_renderers == null || _renderers.Length == 0)
        {
			_renderers = _model.GetComponentsInChildren<Renderer>(true);
        }

        _gridObject.OnSelected += OnSelected;
        _gridObject.OnDeselected += OnDeselected;
    }

    //Called by unity event system when a click is detected on the object
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (FallbackEventReceiver.Instance.IsCameraMoving)
            {
                return;
            }

            //when placing an object for the first time, ignore other clicks.
            if (GridManager.Instance.ActiveObject != null && !GridManager.Instance.ActiveObject.HasBeenPlaced)
            {
                return;
            }

            if (GridManager.Instance.ActiveObject == _gridObject)
            {
                GridManager.Instance.DeselectObject();
            }
            else
            {
                GridManager.Instance.SelectObject(_gridObject);
            }
        }
    }

    protected virtual void OnSelected()
    {
        if (_gridObject == null)
        {
            initialize();
        }

		if(!_selected)
		{
			_selected = true;
			setHighlight();
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = gameObject;
            UnityEditor.EditorGUIUtility.PingObject(UnityEditor.Selection.activeObject);
#endif
        }
    }

    protected virtual void OnDeselected()
    {
        if (_selected)
        {
            _selected = false;
			setHighlight();
        }
    }

    protected virtual void setHighlight()
    {
        if (_selected)
        {
            setOutline(1);
            OutlineEffect.fillAmount = 0;
        }
        else
        {
            setOutline(-1);
        }
    }

    protected void setOutline(int lineColor)
    {
        if (_renderers == null)
        {
            return;
        }

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null)
            {
                if (lineColor >= 0)
                {
                    Outline outline = Toolbox.GetOrCreateComponent<Outline>(_renderers[i].gameObject);
                    outline.color = lineColor;
                    OutlineEffect.AddOutline(outline);
                }
                else
                {
                    OutlineEffect.RemoveOutline(Toolbox.GetOrCreateComponent<Outline>(_renderers[i].gameObject));
                }
            }
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        ExecuteEvents.scrollHandler.Invoke(FallbackEventReceiver.Instance, eventData);
    }
}
