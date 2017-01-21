using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public class SceneLoaderManager : SingletonBehavior<SceneLoaderManager> {
	public delegate void LevelLoaded();
	public static event LevelLoaded onLevelLoadedStarted; 
	public static event LevelLoaded onLevelLoadedCompleted; 

	private bool _firstBoot = true;
	private bool m_isLoading;

	public bool IsLoading
	{
		get
		{
			return m_isLoading;
		}
	}

	public bool FirstBoot
	{
		get
		{
			return _firstBoot;
		}
	}

	public string CurrentSceneName
	{
		get
		{
			return SceneManager.GetActiveScene().name;
		}
	}

	void Load(string levelName, bool showTransition = true)
	{
		if(m_isLoading)
		{
			Debug.LogWarning("WARNING: there is already a level loading in process!");
			return;
		}

		m_isLoading = true;
		Job.Create(LoadInternal(levelName, showTransition));
	}

	public void ReloadCurrentScene()
	{
		Load(CurrentSceneName);
	}

	public void LoadScene(string sceneName)
	{
		Load (sceneName);
	}

	IEnumerator LoadInternal(string levelName, bool showTransition)
	{
		if(onLevelLoadedStarted != null)
			onLevelLoadedStarted();

		AsyncOperation ao = SceneManager.LoadSceneAsync(levelName);
		yield return ao;

		m_isLoading = false;

		if(onLevelLoadedCompleted != null)
			onLevelLoadedCompleted();
	}
}
