using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : SingletonBehavior<WaveManager> {

    public static event Action onWaveApproach;
    public static event Action onWaveArrival;
    public static event Action onWaveRecede;

    public int NextWave = 1;
    public int NextWaveTime = 120;
    public int WaveWarningTme = 15;

    public int GetWaveHeight () { return NextWave * 5; }
    public int GetWaveHeight (int waveNumber) { return waveNumber* 5; }

    public int GetWaveDanger () { return NextWave * 2; }
    public int GetWaveDanger (int waveNumber) { return waveNumber * 2; }

    void OnEnable()
    {
        TimerManager.onTimerExpired += OnTimerExpired;
    }

    public void ResetRecedeTimer()
    {
        TimerManager.Instance.StartTimerNow("big_wave_recede", 3);
    }

    private void OnTimerExpired(TimerManager.TimerEventData data)
    {
        if(data.id == "big_wave_arrive")
        {
            NextWave++;
            onWaveArrival();
        }
        if(data.id == "big_wave_recede")
        {
            StartNextWaveCountdown();
            onWaveRecede();
        }
        if(data.id == "big_wave_warning")
        {
            onWaveApproach();
            GameManager.Instance.ShowMessage("WARNING! Huge Wave Approaching!");
        }
    }

    public void StartNextWaveCountdown()
    {
        TimerManager.Instance.StartTimerNow("big_wave_arrive", NextWaveTime);
        TimerManager.Instance.StartTimerNow("big_wave_warning", NextWaveTime-WaveWarningTme);
    }
}
