using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class FindAndShowExtra : MonoBehaviour
{
    private String rootNameOfLevel = "MainBoard";

    public const string KeyComicsLevel = "ComicsFromLevel";
    public const string KeyComicsMode = "ComicsForMode";

    private int fromLevel = 1;
    private int forMode = 0;

    public GameObject listOfExtra;

    private bool dragMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    private static readonly float globalZoom = 20f; // Was 4.
    private static readonly float normalZoom = 0.8f;

    private Renderer extraRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

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

        Debug.Log("Coming from " + fromLevel + " in " + forMode);

        loadCorrespondingExtra();

        currentZoomCamera = Camera.main.orthographicSize;
    }

    private void loadCorrespondingExtra()
    {
        bool wasFound = false;

        foreach (Transform extra in listOfExtra.transform)
        {
            ExtraForLevelAndMode itsInfos = extra.gameObject.GetComponent<ExtraForLevelAndMode>();

            if (itsInfos.forLevel == fromLevel && itsInfos.forMode == forMode)
            {
                extra.gameObject.SetActive(true);

                extraRenderer = extra.gameObject.GetComponent<Renderer>();

                extraRenderer.material.color = new Color(extraRenderer.material.color.r, extraRenderer.material.color.g, extraRenderer.material.color.b, 0);

                wasFound = true;
            }
            else
            {
                extra.gameObject.SetActive(false);
            }
        }

        if (!wasFound)
        {
            int levelToShow = (int )(Random.value * ((float )listOfExtra.transform.childCount));

            Transform extra = listOfExtra.transform.GetChild(levelToShow);

            extra.gameObject.SetActive(true);

            extraRenderer = extra.gameObject.GetComponent<Renderer>();

            extraRenderer.material.color = new Color(extraRenderer.material.color.r, extraRenderer.material.color.g, extraRenderer.material.color.b, 0);
        }
    }
    //private String rootNameOfLevel = "MainBoard";

    static ZoomRotPosForExtras ourPosList = new ZoomRotPosForExtras();


    bool starting = true;
    bool ending = false;

    int iFade = 0;
    const int timeToFade = 120;
    const float stepFade = 1 / ((float)timeToFade);

    // Update is called once per frame
    void Update()
    {
        if (starting && iFade < timeToFade)
        {
            float lastAlpha = extraRenderer.material.color.a;

            lastAlpha += stepFade;

            if (lastAlpha > 1)
            {
                lastAlpha = 1;
            }
            extraRenderer.material.color = new Color(extraRenderer.material.color.r, extraRenderer.material.color.g, extraRenderer.material.color.b, lastAlpha);

            iFade++;
        }
        else if (starting)
        {
            starting = false;
        }
        if (ending && iFade < timeToFade)
        {
            float lastAlpha = extraRenderer.material.color.a;

            lastAlpha -= stepFade;

            if (lastAlpha < 0)
            {
                lastAlpha = 0;
            }
            extraRenderer.material.color = new Color(extraRenderer.material.color.r, extraRenderer.material.color.g, extraRenderer.material.color.b, lastAlpha);

            iFade++;
        }
        else if (ending)
        {
            Debug.Log("Going to next level!");
            // Start the next level
            LoadNextLevel();
        }

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Rotate(Vector3.forward, 2);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Rotate(Vector3.forward, -2);
        }
        if (Keyboard.current.rKey.isPressed)
        {
            transform.rotation = Quaternion.identity;
        }

        if (Application.platform != RuntimePlatform.Android)
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

    public void RequestNextLevel()
    {
        // Simply trigger the transition. At the end of the transition, we load the next level.
        ending = true;

        starting = false;

        iFade = 0;
    }
}
