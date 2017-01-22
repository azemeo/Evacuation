using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Helpers : MonoBehaviour {

    [MenuItem("Tools/Snap Selection To Grid")]
    public static void SnapSelectionToGrid()
    {
        Object[] selection = Selection.GetFiltered(typeof(GridObject), SelectionMode.TopLevel);
        for (int i = 0; i < selection.Length; i++)
        {
            GridObject go = selection[i] as GridObject;
            Vector3 pos = go.transform.position;
            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);
            pos.z = Mathf.RoundToInt(pos.z);
            go.transform.position = pos;
        }
    }
}
