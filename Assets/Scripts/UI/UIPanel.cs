using UnityEngine;
using System.Collections.Generic;

public class UIPanel : MonoBehaviour {

    [SerializeField]
    private UIManager.PanelID _id;
    [SerializeField]
    private bool _startsActive = false;

    /// <summary>
    /// Use this to do component initialization instead of Awake
    /// </summary>
    protected virtual void init()
    {

    }

    protected virtual void Start()
    {
        if (_id != UIManager.PanelID.NONE && !UIManager.Instance.IsRegistered(this))
        {
            UIManager.Instance.Register(this);
        }

        init();

        if (!_startsActive)
        {
            gameObject.SetActive(false);
        }
    }

    public UIManager.PanelID PanelID
    {
        get
        {
            return _id;
        }
    }
}
