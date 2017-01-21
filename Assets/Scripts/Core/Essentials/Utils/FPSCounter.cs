using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

	#if IS_DEBUG_BUILD
    public float updateInterval = 0.5f;
    private float lastInterval;
    private int frames = 0;
    private float fps;
    void Start() {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }
    void OnGUI() {
		GUI.Label(new Rect(Screen.width * 0.5f, Screen.height - 20, 100, 25), fps.ToString("f2"));
    }
    void Update() {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval) {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }
	#endif
}