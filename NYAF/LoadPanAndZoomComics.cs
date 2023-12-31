using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadPanAndZoomComics : MonoBehaviour
{
    private String rootNameOfLevel = "MainBoard";

    public const string KeyComicsLevel = "ComicsFromLevel";
    public const string KeyComicsMode = "ComicsForMode";

    private int fromLevel = 1;
    private int forMode = 0;

    private bool inTransition = false;

    public GameObject listOfExtra;

    public TextAsset savedPresets;

    // Start is called before the first frame update
    void Start()
    {
        bool haskey = LocalSave.HasIntKey(KeyComicsLevel);
        if (haskey)
        {
            fromLevel = LocalSave.GetInt(KeyComicsLevel);
        }

        haskey = LocalSave.HasIntKey(KeyComicsMode);
        if (haskey)
        {
            forMode = LocalSave.GetInt(KeyComicsMode);
        }

        Debug.Log("Coming from " + fromLevel +" in "+forMode);

        loadCorrespondingExtra();

        loadAllFromAsset();

        // Find the right comic from a file!
        //loadAll();

        skipToCorrectLevelAndMode();

        GetNextScene(true);
    }

    private void skipToCorrectLevelAndMode()
    {
        while (currentPos < ourPosList.inLevel.Count)
        {

            if (ourPosList.inLevel[currentPos] == fromLevel && ourPosList.inMode[currentPos] == forMode)
            {
                break;
            }
            currentPos++;
            Debug.Log("Looking at " + currentPos);
        }
        if (currentPos == ourPosList.inLevel.Count)
        {
            Debug.Log("No image for level " + fromLevel + " and mode "+forMode);
            // Nothing found, skip to next level
            LoadNextLevel();
        }
    }

    private void loadCorrespondingExtra()
    {
        foreach (Transform extra in listOfExtra.transform)
        {
            ExtraForLevelAndMode itsInfos = extra.gameObject.GetComponent<ExtraForLevelAndMode>();

            if (itsInfos.forLevel == fromLevel && itsInfos.forMode == forMode)
            {
                extra.gameObject.SetActive(true);
            }
            else
            {
                extra.gameObject.SetActive(false);
            }
        }
    }
    //private String rootNameOfLevel = "MainBoard";

    static ZoomRotPosForExtras ourPosList = new ZoomRotPosForExtras();

    //void Start()
    //{
    //    loadAll();

    //    currentLevel = mainScript.currentLevel;
    //}

    // Update is called once per frame
    void Update()
    {
        if ( Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
           // load next level
        }

        if (Input.anyKeyDown)
        {
            // Go to next scene
            GetNextScene();
        } 
        else
        {
            Vector3 touchPos = Mouse.current.position.ReadValue();

            // use the input stuff
            if (Mouse.current.leftButton.isPressed)
            {
                // Go to next scene
                GetNextScene();
            }
        }

        if (inTransition)
        {
            UpdatePosZoomAndRot();
        }
    }

    float initTime = 0;
    float speed = 0.5f;

    Vector3 initPos;
    Quaternion initRot;
    float initZoom;

    bool UpdatePosZoomAndRot()
    {
        initTime += Time.deltaTime;

        transform.localPosition = Vector3.Lerp(initPos, targetPos, initTime * speed);
        transform.rotation = Quaternion.Lerp(initRot, targetRot, initTime * speed);
        Camera.main.orthographicSize = Mathf.Lerp(initZoom, targetZoom, initTime * speed);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            initTime = 0;

            inTransition = false;

            return true;
        }

        initTime += Time.deltaTime;

        return false;
    }

    private void loadAllFromAsset()
    {
        if (savedPresets != null)
        {
            ourPosList = JsonUtility.FromJson<ZoomRotPosForExtras>(savedPresets.text);

            Debug.Log("All data Loaded");
        }
    }

    private static void loadAll()
    {
        Debug.Log("Trying to load list of pos/rot " + giveSavePath());
        if (File.Exists(giveSavePath()))
        {
            // 2
            String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

            ourPosList = JsonUtility.FromJson<ZoomRotPosForExtras>(json);

            Debug.Log("All data Loaded");
        }
    }

    private static string giveSavePath()
    {
        return Application.persistentDataPath + "/panZoomComics.json";
    }

    private static bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    int currentPos = 0;

    Vector3 targetPos;
    float targetZoom;
    Quaternion targetRot;

    private void GetNextScene(bool atStart = false)
    {
        if (currentPos >= ourPosList.listX.Count)
        {
            LoadNextLevel();
        }
        else if (ourPosList.inLevel[currentPos] != fromLevel)
        {
            LoadNextLevel();
        }
        else if (ourPosList.inMode[currentPos] != forMode)
        {
            LoadNextLevel();
        }
        else
        {

            float currentZoom = ourPosList.listZoom[currentPos];
            float currentRot = ourPosList.listRot[currentPos];
            float currentX = ourPosList.listX[currentPos];
            float currentY = ourPosList.listY[currentPos];

            if (atStart)
            {
                initZoom = currentZoom;
                //currentZoomCamera = currentZoom;
                initRot = Quaternion.Euler(0, 0, currentRot);
                initPos = new Vector3(currentX, currentY, -7);

                targetZoom = currentZoom;
                //currentZoomCamera = currentZoom;
                targetRot = Quaternion.Euler(0, 0, currentRot);
                targetPos = new Vector3(currentX, currentY, -7);

                Camera.main.orthographicSize = currentZoom;
                //currentZoomCamera = currentZoom;
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, currentRot);
                Camera.main.transform.position = new Vector3(currentX, currentY, -7);
            }
            else
            {
                initPos = targetPos;
                initRot = targetRot;
                initZoom = targetZoom;

                targetZoom = currentZoom;
                //currentZoomCamera = currentZoom;
                targetRot = Quaternion.Euler(0, 0, currentRot);
                targetPos = new Vector3(currentX, currentY, -7);

                inTransition = true;
            }

            currentPos++;
        }
    }

    private void LoadNextLevel()
    {
        fromLevel++;

        if (fromLevel < 12)
        {
            // Same mode, different level
            if (fromLevel <= 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + fromLevel);
            }
        }
        else
        {
            Debug.LogError("Mode should be set only for loading the image, not to go to the next one (outro is there for that)!");
        }
    }
}
