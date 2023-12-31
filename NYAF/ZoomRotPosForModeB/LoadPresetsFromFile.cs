using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPresetsFromFile : MonoBehaviour
{
    private static LoadPresetsFromFile instance = null;
    public TextAsset savedPresets;
    private ZoomRotPosForSave ourPosList;

    public ZoomRotPosForSave OurPosList { get => ourPosList; }

    public static LoadPresetsFromFile Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (savedPresets != null)
        {
            // 2
            String json = savedPresets.text;

            ourPosList = JsonUtility.FromJson<ZoomRotPosForSave>(json);

            Debug.Log("All data Loaded");
        }
    }
}
