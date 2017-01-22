using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : SingletonBehavior<WaveManager> {

    public static event Action onWaveApproach;
    public static event Action onWaveArrival;
    public static event Action onWaveRecede;

    public int NextWave = 1;
    public float NextWaveTime;

    public int GetWaveHeight () { return NextWave * 5; }
    public int GetWaveHeight (int waveNumber) { return waveNumber* 5; }

    public int GetWaveDanger () { return NextWave * 2; }
    public int GetWaveDanger (int waveNumber) { return waveNumber * 2; }

    void Update()
    {

    }

}
