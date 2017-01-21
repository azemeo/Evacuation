using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    WaveManager waveManager;
    public Text CountdownDisplay, WaveStatsDisplay;

	// Use this for initialization
	void Start () {
        waveManager = FindObjectOfType<WaveManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (waveManager != null)
        {
            System.TimeSpan timeToWave = System.TimeSpan.FromSeconds(waveManager.NextWaveTime - Time.time);
            CountdownDisplay.text = string.Format("{0}:{1} until wave {2}", timeToWave.Minutes, timeToWave.Seconds, waveManager.NextWave);
            WaveStatsDisplay.text = string.Format("Height: {0}m   Danger: {1}", waveManager.GetWaveHeight(), waveManager.GetWaveDanger());
        }
	}
}
