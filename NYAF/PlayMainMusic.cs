using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMainMusic : MonoBehaviour
{
    private CreateBoard mainScript;
    bool mainMusicNotStarted = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainMusicNotStarted)
        {
            if (GetComponent<AudioSource>() != null)
            {
                if (!((AudioSource)GetComponent<AudioSource>()).isPlaying)
                {
                    mainScript.playMusic();
                    mainMusicNotStarted = false;
                }
            }
        }
    }
}
