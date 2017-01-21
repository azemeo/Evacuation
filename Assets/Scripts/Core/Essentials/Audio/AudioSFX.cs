using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSFX : MonoBehaviour {

	public int ID
	{
		get
		{
			return _id;
		}
	}

	public string Tag
	{
		get
		{
			return _tag;
		}
	}

	public AudioSource AudioSource
	{
		get
		{
			return _audiosource;
		}
	}

	public bool IsActiveAndPlaying
	{
		get
		{
			return _audiosource.isPlaying && gameObject.activeSelf;
		}
	}

	[SerializeField]
	private int _id;

	[SerializeField]
	private string _tag;

	[SerializeField]
	private AudioSource _audiosource;

	public void Setup(string name, string tag, bool startsActive = true)
	{
		_id = GetInstanceID();
		_tag = tag;

		gameObject.name = "SFX_" + name;
		gameObject.SetActive(startsActive);
	}

	void Update()
	{
		if(!_audiosource.isPlaying && _audiosource.time > 0)
			gameObject.SetActive(false);
	}
}
