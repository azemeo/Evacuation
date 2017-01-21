using UnityEngine;
using System.Collections;

public class UIWorldPositioner : MonoBehaviour {

    [SerializeField]
    private Transform _objectToFollow;

	[SerializeField]
	private Vector3 _offset;

    public Transform ObjectToFollow
    {
        get
        {
            if (_objectToFollow == null)
            {
                if (GridManager.Instance.ActiveObject != null)
                {
                    return GridManager.Instance.ActiveObject.transform;
                }
            }
            return _objectToFollow;
        }

        set
        {
            _objectToFollow = value;
            updatePosition();
        }
    }

	public Vector3 Offset
	{
		get
		{
			return _offset;
		}

		set
		{
			_offset = value;
		}
	}

	// Update is called once per frame
	private void Update ()
    {
        updatePosition();
    }

    private void updatePosition()
    {
        if (ObjectToFollow != null)
        {
			transform.position = ObjectToFollow.position + _offset;
        }
    }
}
