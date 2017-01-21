using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : SingletonBehavior<WaveManager> {

    public int NextWave = 1;
    public float NextWaveTime;

    public int GetWaveHeight () { return NextWave * 5; }
    public int GetWaveHeight (int waveNumber) { return waveNumber* 5; }

    public int GetWaveDanger () { return NextWave * 2; }
    public int GetWaveDanger (int waveNumber) { return waveNumber * 2; }

}
