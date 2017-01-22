using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonBehavior<AudioManager> {

	public enum AudioGroup
	{
		BGM_Music,
		UI,
        Other
    }

	public bool IsMusicMute
	{
		get
		{
			return _isMusicMute;
		}
	}

	public bool IsSFXMute
	{
		get
		{
			return _isSFXMute;
		}
	}

	public bool IsAllMute
	{
		get
		{
			return IsMusicMute && IsSFXMute;
		}
	}

	[SerializeField]
	private AudioClip _buildMusic;

	public AudioClip _siren;

    [SerializeField]
    private AudioClip _waveMusic;

    [SerializeField]
    private AudioClip _waveSound;

    [SerializeField]
    private AudioClip _waveapproach;

    [SerializeField]
	private AudioClip _buttonSFX;

    [SerializeField]
	private AudioSFX _audioSFXPrefab;

	private Dictionary<AudioGroup, List<AudioSFX>> _sfxsDictionary = new Dictionary<AudioGroup, List<AudioSFX>>();
	private Dictionary<AudioGroup, int> _sfxsLimit = new Dictionary<AudioGroup, int>();

	private Job _crossFadeMusicJob = null;

	private bool _isMusicMute;
	private bool _isSFXMute;

    int _waveindex;
    int _bgmIndex;

    private float _musicVolume = 1.0f;
	private AudioSource _audiosource;
	private Transform _sfxPoolTransform;

	protected override void Init()
	{
		_audiosource = GetComponent<AudioSource>();

		_sfxPoolTransform = new GameObject("SFXPool").transform;
		_sfxPoolTransform.parent = transform;

	    ConfigureAudio();
        WaveManager.onWaveArrival += WaveArrived;
        WaveManager.onWaveApproach += waveApproach;
        WaveManager.onWaveRecede += WaveRecede;
	}

	void ConfigureAudio ()
	{
		_sfxsLimit.Add(AudioGroup.BGM_Music, 2);
		_sfxsDictionary.Add(AudioGroup.BGM_Music, new List<AudioSFX>());

        _sfxsLimit.Add(AudioGroup.UI, 2);
        _sfxsDictionary.Add(AudioGroup.UI, new List<AudioSFX>());

        _sfxsLimit.Add(AudioGroup.Other, 10);
        _sfxsDictionary.Add(AudioGroup.Other, new List<AudioSFX>());
    }

    void WaveArrived()
    {
        Stop(AudioGroup.BGM_Music, _bgmIndex, 2f);
        Play(_waveSound, AudioGroup.Other);
        PlayWaveMusic();
    }

    void WaveRecede()
    {
        Job.Create(recede());
    }

    IEnumerator recede()
    {
        fadeOutWave();
        yield return new WaitForSeconds(5f);
        PlayBuildMusic(fadeInTime: 5f);
    }

    void waveApproach()
    {
        Play(_waveapproach, AudioGroup.Other);
    }

	public void Play(AudioSource source, AudioClip clip, AudioGroup audioGroup, float fadeInTime = 0, float volume = 1, float pitch = 1, bool loop = false, string tag = "")
	{
		if(_isSFXMute)
			return;

		source.clip = clip;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;

		Play(source, audioGroup, fadeInTime);
	}

	public int Play(AudioClip clip, AudioGroup audioGroup, float fadeInTime = 0, float volume = 1, float pitch = 1, bool loop = false, string tag = "")
	{
		if(_isSFXMute)
			return -1;

		var sfxList = _sfxsDictionary[audioGroup];

		if(sfxList.Count < _sfxsLimit[audioGroup])
		{
			AudioSFX sfx = Instantiate<AudioSFX>(_audioSFXPrefab, Vector3.zero, Quaternion.identity, _sfxPoolTransform);
			sfx.Setup(clip.name, tag);
			sfxList.Insert(0, sfx);

			Play(sfx.AudioSource, clip, audioGroup, fadeInTime, volume, pitch, loop, tag);

			return sfx.ID;
		}
		else
		{
			for(int i = (sfxList.Count - 1); i >= 0; --i)
			{
				if(!sfxList[i].IsActiveAndPlaying)
				{
					sfxList[i].Setup(clip.name, tag);
					Play(sfxList[i].AudioSource, clip, audioGroup, fadeInTime, volume, pitch, loop, tag);

					return sfxList[i].ID;
				}
			}

			return -1;
		}
	}

	public void Play(AudioSource source, AudioGroup audioGroup, float fadeInTime = 0)
	{
		if(fadeInTime > 0)
			Job.Create(FadeIn(source, fadeInTime));
		else
			source.Play();
	}

	public void Stop(AudioSource source, AudioGroup audioGroup, float fadeOutTime = 0)
	{
		if(fadeOutTime > 0)
			Job.Create(FadeOut(source, fadeOutTime));
		else
			source.Stop();
	}

	public void Stop(AudioGroup audioGroup, string name, float fadeOutTime = 0)
	{
		int index = _sfxsDictionary[audioGroup].FindIndex(s => s.name.Equals("SFX_"+name));

		if(index != -1)
			Stop(_sfxsDictionary[audioGroup][index].AudioSource, audioGroup, fadeOutTime);
	}

	public void Stop(AudioGroup audioGroup, int sfxId, float fadeOutTime = 0)
	{
		int index = _sfxsDictionary[audioGroup].FindIndex(sfx => sfx.ID.Equals(sfxId));

		if(index != -1)
			Stop(_sfxsDictionary[audioGroup][index].AudioSource, audioGroup, fadeOutTime);
	}

	public void StopAllByTag(AudioGroup audioGroup, string tag)
	{
		List<AudioSFX> sfxs = _sfxsDictionary[audioGroup].FindAll(sfx => sfx.Tag.Equals(tag));

		foreach(AudioSFX sfx in sfxs)
		{
			Stop(audioGroup, sfx.ID);
		}
	}

	public void PlayButtonSFX()
	{
		if(_isSFXMute)
			return;

		Play(_buttonSFX, AudioGroup.UI, 0f, 0.5f, 1f, false, "");
	}

	public void SetMusicVolume(float volume, float fadeTime = 0.0f)
	{
		_musicVolume = volume;
		Job.Create(ChangeVolume(fadeTime));
	}

	public void PlayBuildMusic(float fadeInTime = 1, float fadeOutTime = 1)
	{
        _bgmIndex = Play(_buildMusic, AudioGroup.UI, 2f);
	}

	public void PlayWaveMusic(float fadeInTime = 1, float fadeOutTime = 1)
	{
        _waveindex = Play(_waveMusic, AudioGroup.BGM_Music, 2f);
	}

    public void fadeOutWave()
    {
        Stop(AudioGroup.BGM_Music, _waveindex, 5f);
    }

	public void StopCurrentMusic(float fadeOutTime = 1)
	{
		if(_audiosource.clip == null)
			return;

		if(_crossFadeMusicJob != null)
			_crossFadeMusicJob.Kill();

		_crossFadeMusicJob = Job.Create(FadeOut(_audiosource, fadeOutTime));
	}

	public void SetGeneralVolume(float volume)
	{
		AudioListener.volume = volume;
	}

	IEnumerator ChangeVolume(float fadeTime)
	{
		float deltaVolume = _musicVolume - _audiosource.volume;

		if(fadeTime > 0.0f)
		{
			while(Mathf.Abs(_audiosource.volume - _musicVolume) > 0.1f)
			{
				_audiosource.volume += (deltaVolume / fadeTime) * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}

		_audiosource.volume = _musicVolume;
	}

	IEnumerator CrossFadeMusic(AudioClip newClip, float fadeInTime = 1, float fadeOutTime = 1)
	{
		_audiosource.loop = true;

		yield return Job.Create(FadeOut(_audiosource, fadeOutTime)).StartAsRoutine();

		_audiosource.clip = newClip;
		Play(_audiosource, AudioGroup.BGM_Music);

		Job.Create(FadeIn(_audiosource, fadeInTime));
	}

	IEnumerator FadeIn(AudioSource source, float time)
	{
		float timer = Time.realtimeSinceStartup;
		float lastTime = timer;
		source.volume = 0;

		Play(source, AudioGroup.BGM_Music);

		while((Time.realtimeSinceStartup - timer) < time)
		{
			if(source == null)
				yield break;

			float deltaTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;

			source.volume += (_musicVolume / time) * deltaTime;

			if(source.volume >= 1)
				break;

			yield return new WaitForEndOfFrame();
		}

		if(source == null)
			yield break;

		source.volume = _musicVolume;

		if(!source.isPlaying)
			Play(source, AudioGroup.BGM_Music);
	}

	IEnumerator FadeOut(AudioSource source, float time)
	{
		float timer = Time.realtimeSinceStartup;
		float lastTime = timer;
		while((Time.realtimeSinceStartup - timer) < time)
		{
			if(source == null)
				yield break;

			float deltaTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;

			source.volume -= (_musicVolume / time) * deltaTime;

			if(source.volume <= 0)
				break;

			yield return new WaitForEndOfFrame();
		}

		if(source == null)
			yield break;

		source.volume = 0;
		Stop(source, AudioGroup.BGM_Music);
	}

	public void MuteMusic(bool mute)
	{
		_isMusicMute = mute;

		if (mute)
		{
			StopMusic();
		}
		else
		{
			//PlayCampMusic();
		}
	}

	public void MuteSFX(bool mute)
	{
		_isSFXMute = mute;

		if (mute)
		{
			foreach(KeyValuePair<AudioGroup, List<AudioSFX>> sfxList in _sfxsDictionary)
			{
				for(int i = 0; i < sfxList.Value.Count; ++i)
				{
					sfxList.Value[i].AudioSource.Stop();
				}
			}
		}
	}

	public void MuteAll(bool mute)
	{
		MuteMusic(mute);
		MuteSFX(mute);
	}

	void StopMusic()
	{
		Stop(_audiosource, AudioGroup.BGM_Music);
		_audiosource.clip = null;

		if(_crossFadeMusicJob != null)
			_crossFadeMusicJob.Kill();
	}
}
