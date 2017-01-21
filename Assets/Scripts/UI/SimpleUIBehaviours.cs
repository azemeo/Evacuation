using UnityEngine;
using System.Collections;

/// <summary>
/// This script can be applied to any UI element to enable certain controls from Button Events
/// </summary>
public class SimpleUIBehaviours : MonoBehaviour {

    public void SetEnabled(bool value)
    {
        gameObject.SetActive(value);
    }

    public void ToggleEnabled()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
