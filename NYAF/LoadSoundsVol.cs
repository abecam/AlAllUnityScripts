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
        float floatMusicValue = ((float)musicVolume) / 100;

        float floatMusicMixerValue = Mathf.Log(floatMusicValue) * 20;
        if (floatMusicMixerValue < -80)
        {
            floatMusicMixerValue = -80;
        }
        masterMixer.SetFloat("MusicVolume", floatMusicMixerValue);

        haskey = LocalSave.HasIntKey("soundVolume");
        if (haskey)
        {
            soundVolume = LocalSave.GetInt("soundVolume");
        }
        float floatSoundValue = ((float)soundVolume) / 100;

        float floatSoundMixerValue = Mathf.Log(floatSoundValue) * 20;
        if (floatSoundMixerValue < -80)
        {
            floatSoundMixerValue = -80;
        }
        masterMixer.SetFloat("SoundVolume", floatSoundMixerValue);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
