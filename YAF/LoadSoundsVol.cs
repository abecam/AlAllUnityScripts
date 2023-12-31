using UnityEngine;
using UnityEngine.Audio;

public class LoadSoundsVol : MonoBehaviour
{
   
    public AudioMixer masterMixer;

    private int musicVolume = 100;
    private int soundVolume = 100;


    // Start is called before the first frame update
    void Start()
    {
        bool haskey = LocalSave.HasIntKey("musicVolume");
        if (haskey)
        {
            musicVolume = LocalSave.GetInt("musicVolume");
        }
        float value = ((float)musicVolume) / 100;

        masterMixer.SetFloat("MusicVolume", Mathf.Log(value) * 20);

        haskey = LocalSave.HasIntKey("soundVolume");
        if (haskey)
        {
            soundVolume = LocalSave.GetInt("soundVolume");
        }
        value = ((float)soundVolume) / 100;

        masterMixer.SetFloat("SoundVolume", Mathf.Log(value) * 20);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
