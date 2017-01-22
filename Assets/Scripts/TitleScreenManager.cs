using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour {

    public AudioSource music, ambiance, transition;
    public Animation fadeToBlack;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) && !IsInvoking("LoadNext"))
        {
            Invoke("LoadNext", 5.25f);
            fadeToBlack.Play();
            music.Stop();
            ambiance.Stop();
            transition.Play();
        }
	}

    void LoadNext()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
