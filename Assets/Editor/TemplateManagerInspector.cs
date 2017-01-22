using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TemplateManager))]
public class TemplateManagerInspector : Editor
{
    private ReorderableList list;

    private SerializedProperty _templateList;

    private void OnEnable()
    {
        _templateList = serializedObject.FindProperty("_templateList");
        list = new ReorderableList(serializedObject,
                _templateList,
                true, true, true, true);

        //draw header
        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Templates");
        };

        //draw list elements
        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 30f, EditorGUIUtility.singleLineHeight), index.ToString());
            EditorGUI.PropertyField(
                new Rect(rect.x + 30f, rect.y, rect.width - 30f, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
        };


        //highlight the prefab in the inspector
        list.onSelectCallback = (ReorderableList l) => {
            var prefab = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as GameObject;
            if (prefab)
                EditorGUIUtility.PingObject(prefab.gameObject);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        DropAreaGUI();
    }

    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drop Items Here");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        if (dragged_object is GameObject)
                        {
                            ConfigurableObject configObj = ((GameObject)dragged_object).GetComponent<ConfigurableObject>();
                            if (configObj != null)
                            {
                                _templateList.arraySize++;
                               // serializedObject.ApplyModifiedProperties();
                                _templateList.GetArrayElementAtIndex(_templateList.arraySize - 1).objectReferenceValue = dragged_object;
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
                break;
        }
    }
}

