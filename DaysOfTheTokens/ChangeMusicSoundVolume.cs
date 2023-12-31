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
        bool haskey = LocalSave.HasIntKey("musicVolume");
        if (haskey)
        {
            musicVolume = LocalSave.GetInt("musicVolume");
        }
        musicSlider.value = ((float)musicVolume) / 100;
        masterMixer.SetFloat("MusicVolume", Mathf.Log(musicSlider.value) * 20);

        haskey = LocalSave.HasIntKey("soundVolume");
        if (haskey)
        {
            soundVolume = LocalSave.GetInt("soundVolume");
        }
        soundSlider.value = ((float)soundVolume) / 100;
        masterMixer.SetFloat("SoundVolume", Mathf.Log(soundSlider.value) * 20);
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
