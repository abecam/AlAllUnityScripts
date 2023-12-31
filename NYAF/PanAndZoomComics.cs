using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanAndZoomComics : MonoBehaviour
{
    public const string KeyComicsLevel = "ComicsFromLevel";
    public const string KeyComicsMode = "ComicsForMode";

    private int fromLevel = 1;
    private int forMode = 0;

    public GameObject listOfExtra;
    // Start is called before the first frame update
    void Start()
    {
        //bool haskey = localsave.hasintkey(keycomicslevel);
        //if (haskey)
        //{
        //    fromlevel = localsave.getint(keycomicslevel);
        //}

        //haskey = localsave.hasintkey(keycomicsmode);
        //if (haskey)
        //{
        //    formode = localsave.getint(keycomicsmode);
        //}

        // Load the previous positions
        loadAll();

        // Find the right comic from a file!
    }

    //private String rootNameOfLevel = "MainBoard";

    static ZoomRotPosForExtras ourPosList = new ZoomRotPosForExtras();

    private bool dragMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    private static readonly float globalZoom = 40f; // Was 4.
    private static readonly float normalZoom = 0.4f;

    //void Start()
    //{
    //    loadAll();

    //    currentLevel = mainScript.currentLevel;
    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("right"))
        {
            transform.Rotate(Vector3.forward, 2);
        }
        if (Input.GetKey("left"))
        {
            transform.Rotate(Vector3.forward, -2);
        }
        if (Input.GetKey("r"))
        {
            transform.rotation = Quaternion.identity;
        }

        if (Input.GetKeyDown("n"))
        {
            if (fromLevel < 11)
            {
                fromLevel++;

                // Load corresponding picture
                loadCorrespondingExtra();
            }
        }

        if (Input.GetKeyDown("m"))
        {
            if (fromLevel > 1)
            {
                fromLevel--;

                loadCorrespondingExtra();
            }       
        }

        if (Input.GetKeyDown("j"))
        {
            if (forMode < 5)
            {
                forMode++;

                loadCorrespondingExtra();
            }
        }

        if (Input.GetKeyDown("k"))
        {
            if (forMode > 0)
            {
                forMode--;

                loadCorrespondingExtra();
            }
        }

        if (Input.GetKeyDown("space"))
        {
            print("Saving step!");

            // Add the current infos, then save the file
            float zoom = Camera.main.orthographicSize;

            float rot = transform.rotation.eulerAngles.z;

            float x = transform.position.x;
            float y = transform.position.y;

            ourPosList.inLevel.Add(fromLevel);
            ourPosList.inMode.Add(forMode);

            ourPosList.listZoom.Add(zoom);

            ourPosList.listRot.Add(rot);

            ourPosList.listX.Add(x);
            ourPosList.listY.Add(y);

            Save();
        }

        {
            Vector3 touchPos = Mouse.current.position.ReadValue();

            // use the input stuff
            if (Mouse.current.leftButton.isPressed)
            {
                moveCamera(touchPos);
            }
            else
            {
                dragMode = false;
            }

            /*
            if (rightClick)
            {
                zoomBoard(true);
            }
            else
            {
                zoomBoard(false);
            }
            */

            if (Mouse.current.scroll.ReadValue().y > 0f) // forward
            {
                zoomWheel(true);
            }
            else if (Mouse.current.scroll.ReadValue().y < 0f) // backwards
            {
                zoomWheel(false);
            }
        }

        // Move the camera if needed
        updateCameraPos();
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
        return Application.persistentDataPath + "/panZoomComics.json";
    }

    private static bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    private void GetNextScene()
    {
        ZoomRotPosForSave listOfPresets = LoadPresetsFromFile.Instance.OurPosList;

        int newPos = 0;


        float currentZoom = listOfPresets.listZoom[newPos];
        float currentRot = listOfPresets.listRot[newPos];
        float currentX = listOfPresets.listX[newPos];
        float currentY = listOfPresets.listY[newPos];

        Camera.main.orthographicSize = currentZoom;
        //currentZoomCamera = currentZoom;
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, currentRot);
        Camera.main.transform.position = new Vector3(currentX, currentY, -7);
    }

    float currentZoomCamera = normalZoom;
    public float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.

    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.

    private void zoomWheel(bool zoomIn)
    {
        if (zoomIn)
        {
            // Zoom in
            if (currentZoomCamera > normalZoom)
            {
                currentZoomCamera -= 0.3f;
            }
        }
        else
        {
            // Zoom out
            if (currentZoomCamera < globalZoom)
            {
                currentZoomCamera += 0.3f;
            }
        }

        Camera.main.orthographicSize = currentZoomCamera;
    }

    private void zoomCamera(Vector2 touchPos1, Vector2 touchPos2)
    {
        if (!zoomMode)
        {
            zoomOldPosition1 = touchPos1;
            zoomOldPosition2 = touchPos2;

            zoomMode = true;
        }
        else
        {
            if (Vector2.Distance(touchPos1, touchPos2) > Vector2.Distance(zoomOldPosition1, zoomOldPosition2))
            {
                // Zoom in
                if (currentZoomCamera > normalZoom)
                {
                    currentZoomCamera -= 0.3f;
                }
                //else
                //{
                //    currentZoomCamera = normalZoom;
                //}
            }
            else
            {
                // Zoom out
                if (currentZoomCamera < globalZoom)
                {
                    currentZoomCamera += 0.3f;
                }
                //else
                //{
                //    currentZoomCamera = globalZoom;
                //}
            }

            Camera.main.orthographicSize = currentZoomCamera;
        }
    }

    Vector3 cameraSpeed = new Vector3(0, 0, 0);

    private void moveCamera(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 positionDelta = worldPos - Camera.main.ScreenToWorldPoint(dragOldPosition);

        if (!dragMode)
        {
            totalDrag += positionDelta;
            if (Vector3.Distance(totalDrag, Vector3.zero) > 0)
            {
                dragMode = true;
            }
        }
        else
        {
            //Vector3 cameraPos = Camera.main.transform.position;

            //cameraPos -= positionDelta;
            cameraSpeed -= positionDelta / DIVIDER_CAMERA_SPEED;

            //Camera.main.transform.position = RestrainCamera(cameraPos);
        }

        dragOldPosition = mousePos;
    }

    private void updateCameraPos()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        cameraPos += cameraSpeed;

        cameraSpeed = cameraSpeed * REDUCTION_OF_SPEED;

        if (cameraSpeed.magnitude < 0)
        {
            cameraSpeed = new Vector3(0, 0, 0);
        }

        Camera.main.transform.position = RestrainCamera(cameraPos);
    }

    private Vector3 RestrainCamera(Vector3 cameraPos)
    {
        // Need to check the background image boundaries.

        //if (camerapos.x > maximagex)
        //{
        //    camerapos.x = maximagex;
        //}
        //else if (camerapos.x < minimagex)
        //{
        //    camerapos.x = minimagex;
        //}
        //if (camerapos.y > maximagey)
        //{
        //    camerapos.y = maximagey;
        //}
        //else if (camerapos.y < minimagey)
        //{
        //    camerapos.y = minimagey;
        //}
        return cameraPos;
    }
}
