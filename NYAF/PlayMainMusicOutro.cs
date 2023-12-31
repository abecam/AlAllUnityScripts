using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMainMusicOutro : MonoBehaviour
{
    public OutroScript mainScript;
    bool mainMusicNotStarted = true;

    // Start is called before the first frame update
    void Start()
    {
        
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
