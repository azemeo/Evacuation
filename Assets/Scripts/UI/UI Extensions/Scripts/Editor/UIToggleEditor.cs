using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(UIAdvancedToggle))]

public class UIToggleEditor : UnityEditor.UI.ToggleEditor {

	public override void OnInspectorGUI() 
	{
		UIAdvancedToggle component = (UIAdvancedToggle)target;
		base.OnInspectorGUI();
		component.onGameObject = (GameObject)EditorGUILayout.ObjectField("On GameObject", component.onGameObject, typeof(GameObject), true);
		component.offGameObject = (GameObject)EditorGUILayout.ObjectField("Off GameObject", component.offGameObject, typeof(GameObject), true);
	}

}