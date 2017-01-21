using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSFXDatabase : ScriptableObject {

	public static AudioSFXDatabase Instance
	{
		get
		{
			if(_instance == null)
				_instance = Resources.Load<AudioSFXDatabase>(Path);

			return _instance;
		}
	}

	public AudioClip BuildSFX
	{
		get
		{
			return _buildSFX;
		}
	}

	public AudioClip BuildingSelectedSFX
	{
		get
		{
			return _buildingSelectedSFX;
		}
	}

	public AudioClip BuildingDestroyedSFX
	{
		get
		{
			return _buildingDestroyedSFXs[Random.Range(0, _buildingDestroyedSFXs.Length)];
		}
	}

	public AudioClip TroopDeathSFX
	{
		get
		{
			return _troopDeathSFX;
		}
	}

	public AudioClip BattleWonMusic
	{
		get
		{
			return _battleWonMusic;
		}
	}

	public AudioClip BattleLostMusic
	{
		get
		{
			return _battleLostMusic;
		}
	}

	private static AudioSFXDatabase _instance = null;
	public const string Path = "Audio/AudioSFXDatabase";

	[SerializeField]
	private AudioClip _buildSFX;

	[SerializeField]
	private AudioClip _buildingSelectedSFX;

	[SerializeField]
	private AudioClip[] _buildingDestroyedSFXs;

	[SerializeField]
	private AudioClip _battleWonMusic;

	[SerializeField]
	private AudioClip _battleLostMusic;

	[SerializeField]
	private AudioClip _troopDeathSFX;
}
