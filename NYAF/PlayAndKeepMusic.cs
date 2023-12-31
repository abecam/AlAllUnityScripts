using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAndKeepMusic : MonoBehaviour
{
    private static PlayAndKeepMusic instance = null;
    private bool activated = false;

    public static PlayAndKeepMusic Instance
    {
        get { return instance; }
    }

    public bool Activated { get => activated; set => activated = value; }

    void Awake()
    {
        Debug.Log("PlayAndKeepMusic - Awakening "+ instance+" - "+activated);

        if (instance != null && instance != this) 
        {
            Debug.Log("PlayAndKeepMusic - Destroyed! " + instance + " - " + activated);
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            Debug.Log("PlayAndKeepMusic - Instantiated! " + instance + " - " + activated);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (activated)
        { 
            startOrContinueMusic();
        }
    }

    private GameObject currentMusic = null;
    private int iMusic = 0;

    public GameObject allMusics;

    public void startOrContinueMusic()
    {
        if (currentMusic == null)
        {
            iMusic = (int)(Random.value * (float)allMusics.transform.childCount);

            if (iMusic == allMusics.transform.childCount)
            {
                iMusic = 0;
            }

            currentMusic = allMusics.transform.GetChild(iMusic).gameObject;

            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Play();

            return;
        }

        if (!((AudioSource)currentMusic.GetComponent<AudioSource>()).isPlaying)
        {
            // Get the next one
            iMusic++;

            if (iMusic == allMusics.transform.childCount)
            {
                iMusic = 0;
            }
            currentMusic = allMusics.transform.GetChild(iMusic).gameObject;

            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Play();
        }
    }

    public void stopMusic()
    {
        if (currentMusic == null)
        {
            return;
        }

        if (((AudioSource)currentMusic.GetComponent<AudioSource>()).isPlaying)
        {
            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Stop();
        }
    }
}
