using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to configure a Timed Despawner
/// </summary>
[AddComponentMenu("Dark Tonic/Pool Boss/Timed Despawner")]
// ReSharper disable once CheckNamespace
public class TimedDespawner : MonoBehaviour, ISpawnableObject {
    public float LifeSeconds = 5;
    public bool StartTimerOnSpawn = true;

    private Transform _trans;
    private YieldInstruction _timerDelay;

    // ReSharper disable once UnusedMember.Local
    void Awake() {
        _trans = transform;
        AwakeOrSpawn();
    }

    // ReSharper disable once UnusedMember.Local
    public void OnSpawned() { // used by Core GameKit Pooling & also Pool Manager Pooling!
        AwakeOrSpawn();
    }

	public void OnDespawned()
	{
	}

    void AwakeOrSpawn() {
        if (StartTimerOnSpawn) {
            StartTimer();
        }
    }

    /// <summary>
    /// Call this method to start the Timer if it's not set to start automatically.
    /// </summary>
    public void StartTimer() {
        _timerDelay = new WaitForSeconds(LifeSeconds);
        StartCoroutine(WaitUntilTimeUp());
    }

    private IEnumerator WaitUntilTimeUp() {
        yield return _timerDelay;

        PoolBoss.Despawn(_trans);
    }
}