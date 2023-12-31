using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveZoomRotPos : MonoBehaviour
{
    //private String rootNameOfLevel = "MainBoard";

    static ZoomRotPosForSave ourPosList = new ZoomRotPosForSave();

    public CreateBoard mainScript;

    int currentLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        loadAll();

        currentLevel = mainScript.currentLevel;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Rotate(Vector3.forward, 2);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Rotate(Vector3.forward, -2);
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            transform.rotation = Quaternion.identity;
        }

        /*
        if (Input.GetKey("n"))
        {
            if (currentLevel < 11)
            {
                currentLevel++;

                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + currentLevel);
            }
        }

        if (Input.GetKey("p"))
        {
            if (currentLevel > 1)
            {
                currentLevel--;

                if (currentLevel == 1)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + currentLevel);
                }
            }       
        }

        if (Input.GetKeyDown("space"))
        {
            print("Enter was pressed");

            // Add the current infos, then save the file
            float zoom = transform.localScale.x;

            float rot = transform.rotation.eulerAngles.z;

            float x = transform.position.x;
            float y = transform.position.y;

            ourPosList.inLevel.Add(mainScript.currentLevel);

            ourPosList.listZoom.Add(zoom);

            ourPosList.listRot.Add(rot);

            ourPosList.listX.Add(x);
            ourPosList.listY.Add(y);

            Save();
        }
        */
    }

    private static void loadAll()
    {
        Debug.Log("Trying to load list of pos/rot " + giveSavePath());
        if (File.Exists(giveSavePath()))
        {
            // 2
            String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

            ourPosList = JsonUtility.FromJson<ZoomRotPosForSave>(json);

            Debug.Log("All data Loaded");
        }
    }

    public static void Save()
    {
        Debug.Log(Application.persistentDataPath);

        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }

        if (!File.Exists(giveSavePath()))
        {
            string json = JsonUtility.ToJson(ourPosList);

            File.WriteAllText(giveSavePath(), json, System.Text.UTF8Encoding.UTF8);

            Debug.Log("All data saved");
        }
        else
        {
            Debug.Log("PROBLEM: Could not delete save file");
        }
    }

    private static string giveSavePath()
    {
        return Application.persistentDataPath + "/zoomOnModes.json";
    }

    private static bool existSave()
    {
        return File.Exists(giveSavePath());
    }
}
