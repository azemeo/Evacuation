using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class PoolBossInspectorResources {
	public const string PoolBossFolderPath = "PoolBoss";

	//public static Texture logoTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/inspector_header_killer_waves.png", CoreGameKitFolderPath)) as Texture;
	public static Texture DeleteTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/deleteIcon.png", PoolBossFolderPath)) as Texture;
	public static Texture LeftArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_left.png", PoolBossFolderPath)) as Texture;
	public static Texture RightArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_right.png", PoolBossFolderPath)) as Texture;
	public static Texture UpArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_up.png", PoolBossFolderPath)) as Texture;
	public static Texture DownArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_down.png", PoolBossFolderPath)) as Texture;
}
