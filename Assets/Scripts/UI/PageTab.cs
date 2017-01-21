using UnityEngine;
using System.Collections;

public class PageTab : MonoBehaviour {

    public virtual void SelectionChanged(bool value)
    {
        gameObject.SetActive(value);
    }
}
