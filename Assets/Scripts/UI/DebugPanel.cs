using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this to a game object to make it only active if IS_DEBUG_BUILD is defined
/// </summary>
public class DebugPanel : MonoBehaviour {

    void Awake()
    {
#if !IS_DEBUG_BUILD
        gameObject.SetActive(false);
        Destroy(gameObject);
#endif
    }

    public void SwapActive()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
