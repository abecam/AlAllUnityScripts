using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangeMusicSoundVolume : MonoBehaviour
{
    public AudioMixer masterMixer;

    private int musicVolume = 100;
    private int soundVolume = 100;

    public Slider musicSlider;
    public Slider soundSlider;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Setting up volume");
        bool haskey = LocalSave.HasIntKey("musicVolume");
        if (haskey)
        {
            musicVolume = LocalSave.GetInt("musicVolume");
        }
        float floatMusicValue = ((float)musicVolume) / 100;
        musicSlider.value = floatMusicValue;

        float floatMusicMixerValue = Mathf.Log(floatMusicValue) * 20;
        if (floatMusicMixerValue < -80)
        {
            floatMusicMixerValue = -80;
        }
        masterMixer.SetFloat("MusicVolume", floatMusicMixerValue);
        //Debug.Log("MusicVolume: Setting up volume to " + (Mathf.Log(floatMusicValue) * 20));

        haskey = LocalSave.HasIntKey("soundVolume");
        if (haskey)
        {
            soundVolume = LocalSave.GetInt("soundVolume");
        }
        float floatSoundValue = ((float)soundVolume) / 100;
        soundSlider.value = floatSoundValue;

        float floatSoundMixerValue = Mathf.Log(floatSoundValue) * 20;
        if (floatSoundMixerValue < -80)
        {
            floatSoundMixerValue = -80;
        }
        masterMixer.SetFloat("SoundVolume", floatSoundMixerValue);
        //Debug.Log("SoundVolume: Setting up volume to " + (Mathf.Log(soundSlider.value) * 20));
    }

    public void setMusicVolume(float newVolume)
    {
        musicVolume = (int)(100.0f * newVolume);
        LocalSave.SetInt("musicVolume", musicVolume);
        LocalSave.Save();

        masterMixer.SetFloat("MusicVolume", Mathf.Log(newVolume) * 20);
    }

    public void setSoundVolume(float newVolume)
    {
        soundVolume = (int)(100.0f * newVolume);
        LocalSave.SetInt("soundVolume", soundVolume);
        LocalSave.Save();

        masterMixer.SetFloat("SoundVolume", Mathf.Log(newVolume) * 20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
