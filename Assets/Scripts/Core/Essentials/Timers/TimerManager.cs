using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : SingletonBehavior<TimerManager> {

	public delegate void TimerStartedDelegate(TimerEventData eventData);
	public delegate void TimerExpiredDelegate(TimerEventData eventData);

	public static event TimerStartedDelegate onTimerStarted;
	public static event TimerExpiredDelegate onTimerExpired;

	/// <summary>
	/// Timer data struct to hold timer's informations
	/// </summary>
	public struct Timer
	{
		public readonly DateTime startTime;
		public readonly int duration;
		public readonly bool temporary;

		public Timer(DateTime _startTime, int _duration, bool _temporary)
		{
			startTime = _startTime;
			duration = _duration;
			temporary = _temporary;
		}

		public override string ToString ()
		{
			return string.Format ("{0}#{1}", startTime.ToBinary().ToString(), duration.ToString());
		}
	}

	public struct TimerEventData
	{
		public string id;
		public DateTime StartTime;
		public DateTime EndTime;
	}

	private void OnEnable()
	{
		GameManager.onGameReady += OnGameReady;
	}

	private void OnDisable()
	{
        GameManager.onGameReady -= OnGameReady;
	}

	void OnGameReady ()
	{
        StartUpdate();
	}

	void OnDestroy()
	{
        StopUpdate();
	}

	public void OnTimerStarted(TimerEventData eventData)
	{
		if(onTimerStarted != null)
			onTimerStarted(eventData);
	}

	public void OnTimerExpired(TimerEventData eventData)
	{
		if(onTimerExpired != null)
			onTimerExpired(eventData);
	}

	private Job _updateTimersJob = null;

	private Dictionary<string, TimerManager.Timer> _timersDictionary = new Dictionary<string, TimerManager.Timer>();
	private const int _kForeverDuration = 3000000;

	public void StartUpdate()
	{
		StopUpdate();
		_updateTimersJob = Job.Create(UpdateTimersRoutine());
	}

	public void StopUpdate()
	{
		if(_updateTimersJob != null)
			_updateTimersJob.Kill();
	}

	// Coroutine to update timers
	private IEnumerator UpdateTimersRoutine()
	{
		var itemsToRemove = new List<string>();

		while(true)
		{
			foreach(KeyValuePair<string, TimerManager.Timer> timer in _timersDictionary)
			{
				if(GetTimerElapsedTime(timer.Key) >= timer.Value.duration)
				{
					itemsToRemove.Add(timer.Key);
				}
			}

			if(itemsToRemove.Count > 0)
			{
				for(int i = 0; i < itemsToRemove.Count; ++i)
				{
					removeTimer(itemsToRemove[i]);
				}

				itemsToRemove.Clear();
			}

			yield return new WaitForSeconds(0.1f);
		}
	}

	private void addTimer(string id, int duration, DateTime startTime, string message = "", bool temporary = false)
	{
		TimerManager.TimerEventData eventData = new TimerManager.TimerEventData();
		eventData.id = id;
		eventData.StartTime = GetTimerStart(id);
		eventData.EndTime = GetTimerEnd(id);

		if (_timersDictionary.ContainsKey(id))
		{
			_timersDictionary[id] = new TimerManager.Timer(startTime, duration, temporary);
		}
		else
		{
			_timersDictionary.Add(id, new TimerManager.Timer(startTime, duration, temporary));
		}

		OnTimerStarted(eventData);
	}

	private void removeTimer(string id)
	{
		TimerManager.TimerEventData eventData = new TimerManager.TimerEventData();
		eventData.id = id;
		eventData.StartTime = GetTimerStart(id);
		eventData.EndTime = GetTimerEnd(id);
		_timersDictionary.Remove(id);

        OnTimerExpired(eventData);
	}

	#region START / STOP TIMERS

	/// <summary>
	/// Starts a new timer.
	/// </summary>
	/// <param name="id">Unique identifier.</param>
	/// <param name="duration">Duration of timer.</param>
	/// <param name="startTime">The starting time of the timer</param>
	public void StartTimer(string id, int duration, DateTime startTime, bool checkExpiry = false)
	{
		addTimer(id, duration, startTime);

		if (checkExpiry)
		{
			if (GetTimerElapsedTime(id) >= duration)
			{
				removeTimer(id);
			}
		}
	}

	/// <summary>
	/// Starts a new timer that runs until stopped calling StopTimerImmediately()
	/// </summary>
	/// <param name="id">Unique identifier.</param>
	public void StartTimer(string id, DateTime startTime)
	{
		StartTimer(id, _kForeverDuration, startTime);
	}

	/// <summary>
	/// Starts a new timer using Now as the start time.
	/// </summary>
	/// <param name="id">Unique identifier.</param>
	/// <param name="duration">Duration of timer.</param>
	/// <param name="message">An optional message that can be used with local notifications.</param>
	public void StartTimerNow(string id, int duration)
	{
		StartTimer(id, duration, DateTime.Now);
	}

	/// <summary>
	/// Starts a new timer that runs until stopped calling StopTimerImmediately()
	/// </summary>
	/// <param name="id">Unique identifier.</param>
	public void StartTimerNow(string id)
	{
		StartTimerNow(id, _kForeverDuration);
	}

	public void StartTemporaryTimer(string id, DateTime startTime, int duration = _kForeverDuration)
	{
		StartTimer(id, duration, startTime);
	}

	/// <summary>
	/// Stops the timer immediately.
	/// </summary>
	/// <param name="id">The unique identifier of the timer.</param>
	public void StopTimerImmediately(string id)
	{
		StartTimerNow(id, -1);
	}

	public void StopAllTimers()
	{
		var timersID = new List<string>();

		foreach(KeyValuePair<string, TimerManager.Timer> timer in _timersDictionary)
		{
			timersID.Add(timer.Key);
		}

		for(int i = 0; i < timersID.Count; ++i)
		{
			StartTimerNow(timersID[i], -1);
		}
	}

	/// <summary>
	/// Cancels the timer. Does not trigger timerExpired event.
	/// </summary>
	/// <param name="id">The unique identifier of the timer.</param>
	public void CancelTimer(string id)
	{
		if (_timersDictionary.ContainsKey(id))
		{
			_timersDictionary.Remove(id);
		}
	}

	/// <summary>
	/// Determines whether the timer with the specified id is running.
	/// </summary>
	/// <returns><c>true</c> if the is timer running with the specified id; otherwise, <c>false</c>.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public bool IsTimerRunning(string id)
	{
		return _timersDictionary.ContainsKey(id);
	}
	#endregion

	#region GETTERS
	/// <summary>
	/// Gets the timer start time.
	/// </summary>
	/// <returns>The timer start time.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public DateTime GetTimerStart(string id)
	{
		if(!_timersDictionary.ContainsKey(id))
			return DateTime.Now;

		return _timersDictionary[id].startTime;
	}

	/// <summary>
	/// Gets the timer end time.
	/// </summary>
	/// <returns>The timer end time.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public DateTime GetTimerEnd(string id)
	{
		if (!_timersDictionary.ContainsKey(id))
			return DateTime.Now;

		return _timersDictionary[id].startTime.AddSeconds(_timersDictionary[id].duration);
	}

	/// <summary>
	/// Gets the timer's time elapsed.
	/// </summary>
	/// <returns>The time passed since it was started in seconds.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public int GetTimerElapsedTime(string id)
	{
		if(!_timersDictionary.ContainsKey(id))
			return -1;

		return (int)GetTimerElapsedDateTime(id).TotalSeconds;
	}

	/// <summary>
	/// Gets the timer's time elapsed.
	/// </summary>
	/// <returns>The time passed since it was started as TimeSpan.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public TimeSpan GetTimerElapsedDateTime(string id)
	{
		if(!_timersDictionary.ContainsKey(id))
			return new TimeSpan(0);

		return DateTime.Now - GetTimerStart(id);
	}

	/// <summary>
	/// Gets the timer's time elapsed normalized.
	/// </summary>
	/// <returns>The time passed since it was started between 0.0 and 1.0.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public float GetTimerElapsedTimeNormalized(string id)
	{
		if(!_timersDictionary.ContainsKey(id))
			return -1;

		return Mathf.Clamp((float) ((DateTime.Now - GetTimerStart(id)).TotalMilliseconds / 1000.0f) / _timersDictionary[id].duration, 0.0f, 1.0f);
	}

	/// <summary>
	/// Gets the timer's time left.
	/// </summary>
	/// <returns>The timer's time left as TimeSpan.</returns>
	/// <param name="id">The unique identifier of the timer.</param>
	public TimeSpan GetTimerLeftDateTime(string id)
	{
		if(!_timersDictionary.ContainsKey(id))
			return new TimeSpan(0);

		return GetTimerStart(id).AddSeconds(_timersDictionary[id].duration + 1) - DateTime.Now;
	}

	/// <summary>
	/// Gets a formatted string representing the time left.
	/// </summary>
	/// <param name="id">The unique identifier of the timer.</param>
	/// <returns>The timer's time left as a human readable string</returns>
	public string GetTimerLeftFormatted(string id)
	{
		if (!_timersDictionary.ContainsKey(id))
		{
			return "0s";
		}

		return FormatTime((int)(GetTimerLeftDateTime(id).TotalSeconds));
	}

	/// <summary>
	/// Gets a formatted string representing the time elapsed.
	/// </summary>
	/// <param name="id">The unique identifier of the timer.</param>
	/// <returns>The timer's time elapsed as a human readable string</returns>
	public string GetTimerElapsedFormatted(string id)
	{
		if (!_timersDictionary.ContainsKey(id))
		{
			return "0s";
		}

		return FormatTime(GetTimerElapsedTime(id));
	}

	public string FormatTime(int seconds)
	{
        return Toolbox.FormatTime(seconds);
	}

	#endregion
}
