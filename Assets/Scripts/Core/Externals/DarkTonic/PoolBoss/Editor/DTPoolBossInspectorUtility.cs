using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class DTPoolBossInspectorUtility {
    private const string FoldOutTooltip = "Click to expand or collapse";
    private const string AlertTitle = "Pool Boss Alert";
    private const string AlertOkText = "Ok";

    // ReSharper disable InconsistentNaming
    // COLORS FOR DARK SCHEME
    private static readonly Color DarkSkin_DragAreaColor = Color.yellow;
    private static readonly Color DarkSkin_InactiveHeaderColor = new Color(.6f, .6f, .6f);
    private static readonly Color DarkSkin_ActiveHeaderColor = new Color(.3f, .8f, 1f);
    private static readonly Color DarkSkin_OuterGroupBoxColor = new Color(.7f, 1f, 1f);
    private static readonly Color DarkSkin_GroupBoxColor = new Color(.6f, .6f, .6f);
    private static readonly Color DarkSkin_SecondaryGroupBoxColor = new Color(.5f, .8f, 1f);
    private static readonly Color DarkSkin_SecondaryHeaderColor = new Color(.8f, .8f, .8f);

    // COLORS FOR LIGHT SCHEME
    private static readonly Color LightSkin_OuterGroupBoxColor = Color.white;
    private static readonly Color LightSkin_DragAreaColor = new Color(1f, 1f, .3f);
    private static readonly Color LightSkin_InactiveHeaderColor = new Color(.6f, .6f, .6f);
    private static readonly Color LightSkin_ActiveHeaderColor = new Color(.3f, .8f, 1f);
    private static readonly Color LightSkin_GroupBoxColor = new Color(.7f, .7f, .8f);
    private static readonly Color LightSkin_SecondaryGroupBoxColor = new Color(.6f, 1f, 1f);
    private static readonly Color LightSkin_SecondaryHeaderColor = Color.white;
    // ReSharper restore InconsistentNaming

    public enum FunctionButtons { None, Add, Remove, ShiftUp, ShiftDown, Edit, DespawnAll, Rename }

    public static FunctionButtons AddFoldOutListItemButtons(int position, int totalPositions, string itemName, bool showAddButton, bool showMoveButtons = false) {
        if (Application.isPlaying) {
            return FunctionButtons.None;
        }

        if (showMoveButtons) {
            if (position > 0) {
                // the up arrow.
                var upArrow = PoolBossInspectorResources.UpArrowTexture;
                if (GUILayout.Button(new GUIContent(upArrow, "Click to shift " + itemName + " up"),
                                          EditorStyles.toolbarButton, GUILayout.Width(24))) {
                    return FunctionButtons.ShiftUp;
                }
            } else {
                GUILayout.Space(24);
            }

            if (position < totalPositions - 1) {
                // The down arrow will move things towards the end of the List
                var dnArrow = PoolBossInspectorResources.DownArrowTexture;
                if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift " + itemName + " down"),
                    EditorStyles.toolbarButton, GUILayout.Width(24))) {

                    return FunctionButtons.ShiftDown;
                }
            } else {
                GUILayout.Space(24);
            }
        }

        if (showAddButton) {
            GUI.contentColor = Color.yellow;

            var addPress = GUILayout.Button(new GUIContent("Add", "Click to insert " + itemName),
                EditorStyles.toolbarButton, GUILayout.Width(32));

            GUI.contentColor = Color.white;

            if (addPress) {
                return FunctionButtons.Add;
            }
        }

        // Remove Button - Process presses later
        var removeContent = PoolBossInspectorResources.DeleteTexture == null ? new GUIContent("-", "Click to remove " + itemName) : new GUIContent(PoolBossInspectorResources.DeleteTexture, "Click to remove " + itemName);

        if (GUILayout.Button(removeContent, EditorStyles.toolbarButton, GUILayout.Width(32))) {
            return FunctionButtons.Remove;
        }

        return FunctionButtons.None;
    }

    public static void ResetColors() {
        GUI.color = Color.white;
        GUI.contentColor = Color.white;
        GUI.backgroundColor = Color.white;
    }

    private static Color GroupBoxColor {
        get {
            return IsDarkSkin ? DarkSkin_GroupBoxColor : LightSkin_GroupBoxColor;
        }
    }

    private static Color SecondaryGroupBoxColor {
        get {
            return IsDarkSkin ? DarkSkin_SecondaryGroupBoxColor : LightSkin_SecondaryGroupBoxColor;
        }
    }

    public static Color InactiveHeaderColor {
        get {
            return IsDarkSkin ? DarkSkin_InactiveHeaderColor : LightSkin_InactiveHeaderColor;
        }
    }

    public static Color ActiveHeaderColor {
        get {
            return IsDarkSkin ? DarkSkin_ActiveHeaderColor : LightSkin_ActiveHeaderColor;
        }
    }

    private static Color SecondaryHeaderColor {
        get {
            return IsDarkSkin ? DarkSkin_SecondaryHeaderColor : LightSkin_SecondaryHeaderColor;
        }
    }

    private static Color OuterGroupBoxColor {
        get {
            return IsDarkSkin ? DarkSkin_OuterGroupBoxColor : LightSkin_OuterGroupBoxColor;
        }
    }

    public static Color DragAreaColor {
        get {
            return IsDarkSkin ? DarkSkin_DragAreaColor : LightSkin_DragAreaColor;
        }
    }

    private static bool IsDarkSkin {
        get {
            return EditorPrefs.GetInt("UserSkin") == 1;
        }
    }

    public static void AddSpaceForNonU5(int height = 2) {
#if UNITY_5_0
        //
#else
        GUILayout.Space(height);
#endif
    }

    public static void BeginGroupedControls() {
        GUI.backgroundColor = OuterGroupBoxColor;
        GUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    public static void EndGroupedControls() {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3f);
        GUILayout.EndHorizontal();

        GUILayout.Space(3f);
    }

    public static void StartGroupHeader(int level = 0, bool showBoth = true) {
        switch (level) {
            case 0:
                GUI.backgroundColor = GroupBoxColor;
                break;
            case 1:
                GUI.backgroundColor = SecondaryGroupBoxColor;
                break;
        }

        EditorGUILayout.BeginVertical(CornerGUIStyle);

        if (!showBoth) {
            return;
        }

        switch (level) {
            case 0:
                GUI.backgroundColor = SecondaryHeaderColor;
                break;
        }

        EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
    }

    public static void EndGroupHeader() {
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private static GUIStyle CornerGUIStyle {
        get {
#if UNITY_5_0
            return EditorStyles.helpBox;
#else
#if UNITY_3_5_7 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4
            return EditorStyles.numberField;
#else
            return EditorStyles.textArea;
#endif
#endif
        }

    }

    public static bool Foldout(bool expanded, string label) {
        var content = new GUIContent(label, FoldOutTooltip);
        expanded = EditorGUILayout.Foldout(expanded, content);

        return expanded;
    }

    public static void DrawTexture(Texture tex) {
        if (tex == null) {
            Debug.Log("Logo texture missing");
            return;
        }

        var rect = GUILayoutUtility.GetRect(0f, 0f);
        rect.width = tex.width;
        rect.height = tex.height;
        GUILayout.Space(rect.height);
        GUI.DrawTexture(rect, tex);
    }

    public static void ShowColorWarning(string warningText) {
        GUI.color = Color.green;
        EditorGUILayout.LabelField(warningText, EditorStyles.miniLabel);
        GUI.color = Color.white;
    }

    public static void ShowRedError(string errorText) {
        GUI.color = Color.red;
        EditorGUILayout.LabelField(errorText, EditorStyles.toolbarButton);
        GUI.color = Color.white;
    }

    public static void ShowLargeBarAlert(string errorText) {
        GUI.color = Color.yellow;
        EditorGUILayout.LabelField(errorText, EditorStyles.toolbarButton);
        GUI.color = Color.white;
    }

    public static void ShowAlert(string text) {
        if (Application.isPlaying) {
            Debug.LogWarning(text);
        } else {
            EditorUtility.DisplayDialog(AlertTitle, text, AlertOkText);
        }
    }

    private static PrefabType GetPrefabType(Object gObject) {
        return PrefabUtility.GetPrefabType(gObject);
    }

    public static bool IsPrefabInProjectView(Object gObject) {
        return GetPrefabType(gObject) == PrefabType.Prefab;
    }
}
