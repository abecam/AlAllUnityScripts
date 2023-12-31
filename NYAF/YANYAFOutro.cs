using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class YANYAFOutro : MonoBehaviour
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.
    public bool gameIsInPause = false; // Allow to not take any more selection, like when a menu is displayeds

    private String rootNameOfLevel = "YANYAF";

    public GameObject backgroundImage; // The current image

    public GameObject title;

    public GameObject characters;

    public GameObject textOutro1;
    public GameObject textOutro2;

    private float minImageX;
    private float maxImageX;
    private float minImageY;
    private float maxImageY;

    private int musicVolume = 100;
    private int soundVolume = 100;

    Vector3 targetPos;
    private const string GAMEMODEUNLOCKED = "GAMEMODEUNLOCKED";

    String gameModeUnlocked;
    String secretFigureFound;

    public int dimingTime = 100; // 200 frame

    List<string> allLinesOfText = new List<string>()
        {
           "Well Done!\nThank you for playing",
           GAMEMODEUNLOCKED,
           "YANYAF\nYet Another NYAF\nA Game by\nAlain Becam",
           "Graphics by\nSébastien Lesage",
           "Testing, gameplay ideas, drums\nWilliam",
           "Music by\nAlexander Nakarada\nChris Huelsbeck\nChan Redfield\nLiQWYD",
           "Font\nGlametrixBold\nSudegnakNo3\nby Gluk\nhttp://www.glukfonts.pl",
           "Special Thanks to\nAlexander Zolotov\nhttps://www.warmplace.ru",
           "Thank you for playing"
        };

    int currentText = 0;

    // Achievements will be defined by an interface and free numbers of achievements

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
        /////////////////////////////////////////////////////////////////////// DEBUG REMOVE AFTER /////////////////
        //resetSmallFound();
        // See which string we need to show
        completeStrings();

        // Get the kind of game from the menu
        initIntroductionBox();

        // Find background boundaries
        minImageX = backgroundImage.GetComponent<Renderer>().bounds.min.x;
        maxImageX = backgroundImage.GetComponent<Renderer>().bounds.max.x;
        minImageY = backgroundImage.GetComponent<Renderer>().bounds.min.y;
        maxImageY = backgroundImage.GetComponent<Renderer>().bounds.max.y;

        bool haskey = LocalSave.HasIntKey("musicVolume");
        if (haskey)
        {
            musicVolume = LocalSave.GetInt("musicVolume");
        }

        haskey = LocalSave.HasIntKey("soundVolume");
        if (haskey)
        {
            soundVolume = LocalSave.GetInt("soundVolume");
        }
        setupNextText();

        // Position the objects to find
        setupObjectsInSphere();

        createTexture();
    }

    private void setupNextText()
    {
        if (currentText >= allLinesOfText.Count)
        {
            currentText = 0;
            initIntroductionBox();
        }
        String lineOfText = allLinesOfText[currentText++];
        if (lineOfText.Equals(GAMEMODEUNLOCKED))
        {
            lineOfText = gameModeUnlocked;
        }

        ((TextMesh)textOutro1.GetComponent("TextMesh")).text = lineOfText;
        ((TextMesh)textOutro2.GetComponent("TextMesh")).text = lineOfText;

        float newRed = ((Random.value / 4) + 0.75f);
        float newGreen = ((Random.value / 1.5f) + 0.25f);
        float newBlue = ((Random.value / 1.5f) + 0.25f);
        //float newAlpha = ((Random.value / 4) + 0.75f);

        textOutro2.GetComponent<Renderer>().material.color = new Color(newRed, newGreen, newBlue, 1);
    }

    private void initIntroductionBox()
    {
        currentZoomForIntroBox = 0;
        // Z = -9
        title.SetActive(true);
        //backgroundImage.transform.localScale = new Vector3(globalZoom, globalZoom, 1);

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom);

        stepBetweenZoom = (normalZoom - globalZoom) / ((float)dimingTime);
    }

    private void completeStrings()
    {
        int currentGameMode = 0;

        // And which mode we are in
        bool haskey = LocalSave.HasIntKey("yanyaf_lastModeFinished");
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("yanyaf_lastModeFinished");
        }

        LoadTimeSpent(currentGameMode);

        long timeSpentLongMode = (long)allTimeSpent;

        long hoursSpentMode = timeSpentLongMode / 3600;
        if (hoursSpentMode > 0)
        {
            timeSpentLongMode -= hoursSpentMode * 3600;
        }

        long minutesSpentMode = timeSpentLongMode / 60;
        if (minutesSpentMode > 0)
        {
            timeSpentLongMode -= minutesSpentMode * 60;
        }

        gameModeUnlocked = "You finished the mode in " + ((hoursSpentMode > 0) ? hoursSpentMode + " hours " : "") +
             ((hoursSpentMode > 0) || (minutesSpentMode > 0) ? minutesSpentMode + " minutes " : "") +
             ((timeSpentLongMode > 0) ? timeSpentLongMode + " seconds " : "0 seconds");

        if (isBestTime)
        {
            gameModeUnlocked += "\nNew best time!";
        }
        else
        {
            long bestTimeSpentLongMode = (long)bestTimeSpent;

            long hoursBestSpentMode = bestTimeSpentLongMode / 3600;
            if (hoursBestSpentMode > 0)
            {
                bestTimeSpentLongMode -= hoursBestSpentMode * 3600;
            }

            long minutesBestSpentMode = bestTimeSpentLongMode / 60;
            if (minutesBestSpentMode > 0)
            {
                bestTimeSpentLongMode -= minutesBestSpentMode * 60;
            }

            gameModeUnlocked += "\nBest time is " + ((hoursBestSpentMode > 0) ? hoursBestSpentMode + " hours " : "") +
            ((hoursBestSpentMode > 0) || (minutesBestSpentMode > 0) ? minutesBestSpentMode + " minutes " : "") +
            ((bestTimeSpentLongMode > 0) ? bestTimeSpentLongMode + " seconds " : "0 seconds");
        }
    }

    float allTimeSpent = 0;
    float absAllTimeSpent = 0;
    float bestTimeSpent = 100000000000;
    bool isBestTime = false;

    private void LoadTimeSpent(int currentGameMode)
    {
        bool haskey = LocalSave.HasFloatKey("yanyaf_allTimeSpent" + currentGameMode);

        if (haskey)
        {
            allTimeSpent = LocalSave.GetFloat("yanyaf_allTimeSpent" + currentGameMode);
        }

 		// Best time
        haskey = LocalSave.HasFloatKey("yanyaf_bestTimeSpent" + currentGameMode);

        if (haskey)
        {
            bestTimeSpent = LocalSave.GetFloat("yanyaf_bestTimeSpent" + currentGameMode);
        }

		if (allTimeSpent < bestTimeSpent)
        {
            bestTimeSpent = allTimeSpent;
            isBestTime = true;
        }
        // Save the absolute total for this mode

        haskey = LocalSave.HasFloatKey("yanyaf_absAllTimeSpent" + currentGameMode);

        if (haskey)
        {
            absAllTimeSpent = LocalSave.GetFloat("yanyaf_absAllTimeSpent" + currentGameMode);
        }

		LocalSave.SetFloat("yanyaf_bestTimeSpent" + currentGameMode, bestTimeSpent);

        LocalSave.SetFloat("yanyaf_absAllTimeSpent" + currentGameMode, absAllTimeSpent + allTimeSpent);

        // Now reset the values!
        LocalSave.SetFloat("yanyaf_allTimeSpent" + currentGameMode, 0);
        LocalSave.Save();
    }

    /**
     * Push on a shape
     */
    private void setupObjectsOnSphere()
    {
        // Place all characters available, ensure they are not at the same place as others.
        int nbOfElements = characters.transform.childCount;

        float extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        float halfX = extendX / 2;

        float extendY = maxImageY - minImageY - 0.2f;
        float halfY = extendY / 2;

        float minOfHalf = halfX;
        if (halfY < halfX)
        {
            minOfHalf = halfY;
        }

        foreach (Transform childCharacter in characters.transform)
        {
            Debug.Log("Child: " + childCharacter);

            // TODO: - Check that we are on the picture
            // - Avoid pref icon and ad block

            float posZ = 2 * Random.value - 1;   // uniform in -1, 1
            float t = 2 * ((float)Math.PI) * Random.value;   // uniform in 0, 2*pi
            float posX = (float)(Math.Sqrt(1 - Math.Pow(posZ, 2)) * Math.Cos(t));
            float posY = (float)(Math.Sqrt(1 - Math.Pow(posZ, 2)) * Math.Sin(t));

            childCharacter.position = new Vector3(minOfHalf * posX, minOfHalf * posY, minOfHalf * posZ);

            // Make them a tiny bit transparent
            float newAlpha = 1;

            Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
            childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
        }
    }

    /**
     * Push on a shape
     */
    private void setupObjectsInSphere()
    {
        // Place all characters available, ensure they are not at the same place as others.
        int nbOfElements = characters.transform.childCount;

        float extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        float halfX = extendX / 2;

        float extendY = maxImageY - minImageY - 0.2f;
        float halfY = extendY / 2;

        float minOfHalf = halfX;
        if (halfY < halfX)
        {
            minOfHalf = halfY;
        }

        foreach (Transform childCharacter in characters.transform)
        {
            Debug.Log("Child: " + childCharacter);

            // TODO: - Check that we are on the picture
            // - Avoid pref icon and ad block

            float phi = (Random.value) * 2f * (float)Math.PI;
            float costheta = Random.value * 2 - 1;
            float u = Random.value;

            float theta = (float)Math.Acos(costheta);
            float r = minOfHalf * cubeRoot(u);

            float posX = (float)(r * Math.Sin(theta) * Math.Cos(phi));
            float posY = (float)(r * Math.Sin(theta) * Math.Sin(phi));
            float posZ = (float)(r * Math.Cos(theta));

            childCharacter.position = new Vector3(posX, posY, posZ);

            // Make them a tiny bit transparent
            float newAlpha = 1;

            Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
            childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
        }
    }

    private float cubeRoot(float number)
    {
        return (float)(Math.Pow(number, (1.0 / 3.0)));
    }

    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    private bool startingGame = true;

    // Update is called once per frame
    void Update()
    {
        if (startingGame)
        {
            // Manage the introduction box
            showIntroductionBox(true);

            return;
        }

        updateText();

        zoomBackgrounds();
    }

    bool needToReplaceChar = true;

    float currentZoomForIntroBox;

    //private float globalZoom = 0.04f;
    //private float normalZoom = 0.1f;
    //private float stepBetweenZoom = 0.1f;

    private static readonly float globalZoom = -18f;
    private static readonly float normalZoom = 4f;
    private static readonly float ratio = globalZoom / normalZoom;

    private float stepBetweenZoom = 0.1f;

    /**
     * Zoom the background and move the characters when needed. 
     */
    private void zoomBackgrounds()
    {
        currentZoomForIntroBox += stepBetweenZoom;

        // backgroundImage.transform.localScale = new Vector3(globalZoom + currentZoomForIntroBox, globalZoom + currentZoomForIntroBox, 1);
        //Camera.main.orthographicSize = globalZoom + currentZoomForIntroBox;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom + currentZoomForIntroBox);

        if (needToReplaceChar && Camera.main.transform.position.z > 9)
        {
            setupObjectsInSphere();
            needToReplaceChar = false;
        }
        if (Camera.main.transform.position.z > 9)
        {
            // Create a new texture
            createTexture();

            currentZoomForIntroBox = 0;
            needToReplaceChar = true;

            // Load the next text
            setupNextText();

            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom + currentZoomForIntroBox);
        }
    }

    private void updateText()
    {
        //
    }

    float xSphereRot = 0;
    float ySphereRot = 0;
    float zSphereRot = 0;

    Vector3 speedRotation = new Vector3(0.2f, 0.1f, 0.05f);

    float dir = 0.0f;

    private void rotateSphere()
    {
        dir += Random.value * 0.2f - 0.1f;

        //float newAccX = 4*(float )Math.Sin(dir);
        //float newAccY = 4*(float )Math.Cos(dir);
        //float newAccZ = Random.value * 0.2f - 0.1f;

        //speedRotation = new Vector3(speedRotation.x + newAccX, speedRotation.y + newAccY, 0);

        xSphereRot = xSphereRot + speedRotation.x;
        ySphereRot = ySphereRot + speedRotation.y;
        zSphereRot = zSphereRot + speedRotation.z;


        characters.transform.Rotate(Vector3.right, xSphereRot);
        characters.transform.Rotate(Vector3.up, ySphereRot);
        characters.transform.Rotate(Vector3.forward, zSphereRot);

        foreach (Transform childCharacter in characters.transform)
        {
            childCharacter.transform.rotation = new Quaternion(0, 0, 0, 1);
        }
    }
    
    public static float GetScreenToWorldWidth()
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        float width = edgeVector.x * 2;

        width = 1;

        return width;
    }

    // After the drum roll, start the music
    internal void playMusic()
    {
        if (GetComponent<AudioSource>() != null)
        {
            ((AudioSource)GetComponent<AudioSource>()).Play();
        }
    }

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;

    private void showIntroductionBox(bool isIntro)
    {
        if (currentStep == 0)
        {
            // Zoom the box into view
            if (zoomingTime <= totalZoomingTime)
            {
                float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
                foreach (Transform childIntrobox in title.transform)
                {
                    Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, fading);
                    }
                }

                zoomingTime++;
            }
            else
            {
                currentStep = 2;
                zoomingTime = 0;
            }
        }
        else if (currentStep == 2)
        {
            if (zoomingTime <= dimingTime)
            {
                // Fading out of the box
                float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

                foreach (Transform childIntrobox in title.transform)
                {
                    Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                    }
                }
                currentZoomForIntroBox += stepBetweenZoom;

                // backgroundImage.transform.localScale = new Vector3(globalZoom + currentZoomForIntroBox, globalZoom + currentZoomForIntroBox, 1);
                //Camera.main.orthographicSize = globalZoom + currentZoomForIntroBox;
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom + currentZoomForIntroBox);

                zoomingTime++;
            }
            else
            {
                currentStep = 3;
            }
        }
        else if (currentStep == 3)
        {
            // All done
            if (isIntro)
            {
                startingGame = false;

                // Remove the title
                title.SetActive(false);
            }
            else
            {
                startingGame = true;
            }
        }
    }

    private void createTexture()
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;

        float value = Random.value * 900;

        //value = 130;

        if (value > 800)
        {
            doCollidingPlasma2(texture);
        }
        else if (value > 700)
        {
            doCollidingPlasma(texture);
        }
        else if (value > 600)
        {
            doPlasma(texture);
        }
        else if (value > 500)
        {
            doCollidingPlasmaBW(texture);
        }
        else if (value > 400)
        {
            // set the pixel values
            sinCos1(texture);
        }
        else if (value > 300)
        {
            // set the pixel values
            sinCos2(texture);
        }
        else if (value > 200)
        {
            // set the pixel values
            sinCos3(texture);
        }
        else if (value > 100)
        {
            // set the pixel values
            sinCosBW(texture);
        }   
        else
        {
            // set the pixel values
            sinCosBW3(texture);
        }
     
        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        Renderer imageRenderer = backgroundImage.GetComponent<Renderer>();
        imageRenderer.material.mainTexture = texture;
    }

    private void doPlasma(Texture2D texture)
    {
        Debug.Log("doPlasma");

        float ratio = Random.value * 100;
        float choice = Random.value * 20;

        float doFullRandom = Random.value;
        bool inFullRandom = false;

        if (doFullRandom > 0.5f)
        {
            inFullRandom = true;
        }
        float isBW = 2 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;

                float perlinNoise = Mathf.PerlinNoise(xFloat / ratio, yFloat / ratio);

                float r = Mathf.Cos(2 * Mathf.PI * perlinNoise);
                float g = Mathf.Sin(2 * Mathf.PI * perlinNoise);
                float b = perlinNoise; // Mathf.Cos(2 * Mathf.PI * perlinNoise) * Mathf.Sin(2 * Mathf.PI * perlinNoise);

                if (choice > 18)
                {
                    b = Mathf.Cos(2 * Mathf.PI * perlinNoise) * Mathf.Sin(2 * Mathf.PI * perlinNoise);
                }
                else if (choice > 16)
                {
                    g = perlinNoise;
                }
                else if (choice > 14)
                {
                    r = Mathf.PerlinNoise(xFloat / (ratio * choice), yFloat / (ratio * choice));
                }
                else if (choice > 12)
                {
                    r = Mathf.Cos(2 * Mathf.PI * perlinNoise);
                    g = r * Mathf.PerlinNoise(xFloat / (ratio * choice), yFloat / (ratio * choice));
                    float newRand = Random.value;
                    b = g * Mathf.PerlinNoise(xFloat / (ratio * newRand), yFloat / (ratio * newRand));
                }
                else if (choice > 10)
                {
                    r = Mathf.Cos(2 * Mathf.PI * perlinNoise);
                    g = r;
                    b = r;
                }
                else if (choice > 8)
                {
                    r = 0.4f + 0.6f * Mathf.Cos(2 * Mathf.PI * perlinNoise);
                    g = 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
                    b = 0.6f + 0.4f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
                }
                else if (choice > 6)
                {
                    r = 0.5f + 0.5f * Mathf.Cos(2 * Mathf.PI * perlinNoise);
                    g = 0.4f + 0.6f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
                    b = 0.6f + 0.4f * perlinNoise;
                }
                else if (choice > 4)
                {
                    r = 0.5f + 0.5f * perlinNoise;
                    g = 0.4f + 0.6f * perlinNoise;
                    b = 0.6f + 0.4f * perlinNoise;
                }
                else if (choice > 2)
                {
                    r = 0.2f + 0.8f * perlinNoise;
                    g = 0.3f + 0.7f * perlinNoise;
                    b = 0.4f + 0.6f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
                }

                if (inFullRandom)
                {
                    isBW = 2 * Random.value;
                }

                Color currentColor = new Color(r, g, b);
                if (isBW > 1f)
                {
                    currentColor = Color.white;

                    if (r > g)
                    {
                        currentColor = Color.black;
                    }
                    else if (g > b)
                    {
                        currentColor = new Color(b, b, b);
                    }
                    else if (b > r)
                    {
                        currentColor = new Color(g, g, g);
                    }
                }
                else if (isBW > 0.5f)
                {
                    if (r > g)
                    {
                        currentColor = Color.black;
                    }
                    else if (g > b)
                    {
                        currentColor = new Color(b, b, b);
                    }
                    else
                    {
                        currentColor = new Color(g, g, g);
                    }
                }
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private void doCollidingPlasma(Texture2D texture)
    {
        Debug.Log("doPlasma");

        float ratio = Random.value * 100;
        float choice = Random.value * 20;

        float ratio2 = Random.value * 100;
        float choice2 = Random.value * 20;

        float doFullRandom = Random.value;
        bool inFullRandom = false;

        if (doFullRandom > 0.5f)
        {
            inFullRandom = true;
        }
        float isBW = 2 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                selectColorFromPerlinAndChoice(ratio, choice, yFloat, xFloat, out float r, out float g, out float b);
                selectColorFromPerlinAndChoice(ratio2, choice2, yFloat, xFloat, out float r2, out float g2, out float b2);
                float r3, g3, b3;

                if (r > r2)
                {
                    r3 = r;
                }
                else
                {
                    r3 = r2;
                }
                if (b > b2)
                {
                    b3 = b;
                }
                else
                {
                    b3 = b2;
                }
                if (g > g2)
                {
                    g3 = g;
                }
                else
                {
                    g3 = g2;
                }

                if (inFullRandom)
                {
                    isBW = 2 * Random.value;
                }

                Color currentColor = new Color(r3, g3, b3);

                if (isBW > 1f)
                {
                    currentColor = Color.white;

                    if (r3 > g3)
                    {
                        currentColor = Color.black;
                    }
                    else if (g3 > b3)
                    {
                        currentColor = new Color(b3, b3, b3);
                    }
                    else if (b3 > r3)
                    {
                        currentColor = new Color(g3, g3, g3);
                    }
                }
                else if (isBW > 0.5f)
                {
                    if (r3 > g3)
                    {
                        currentColor = Color.black;
                    }
                    else if (g3 > b3)
                    {
                        currentColor = new Color(b3, b3, b3);
                    }
                    else
                    {
                        currentColor = new Color(g3, g3, g3);
                    }
                }
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private void doCollidingPlasma2(Texture2D texture)
    {
        Debug.Log("doPlasma");

        float ratio = 10 + Random.value * 20;
        float choice = Random.value * 20;

        float ratio2 = 10 + Random.value * 20;
        float choice2 = Random.value * 20;

        float ratio3 = 10 + Random.value * 20;
        float ratio4 = 10 + Random.value * 20;

        float ratio5 = Random.value * 100;
        float ratio6 = Random.value * 100;

        float doFullRandom = Random.value;
        bool inFullRandom = false;

        if (doFullRandom > 0.5f)
        {
            inFullRandom = true;
        }
        float isBW = 2 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                selectColorFromPerlinAndChoice(ratio5, choice, yFloat + 100 * Mathf.Cos(yFloat / ratio3), xFloat + 100 * Mathf.Sin(xFloat / ratio), out float r, out float g, out float b);
                selectColorFromPerlinAndChoice(ratio6, choice2, yFloat + 100 * Mathf.Cos(yFloat / ratio4), xFloat + 100 * Mathf.Sin(xFloat / ratio2), out float r2, out float g2, out float b2);
                float r3, g3, b3;

                if (r > r2)
                {
                    r3 = r;
                }
                else
                {
                    r3 = r2;
                }
                if (b > b2)
                {
                    b3 = b;
                }
                else
                {
                    b3 = b2;
                }
                if (g > g2)
                {
                    g3 = g;
                }
                else
                {
                    g3 = g2;
                }

                if (inFullRandom)
                {
                    isBW = 2 * Random.value;
                }

                Color currentColor = new Color(r3, g3, b3);

                if (isBW > 1f)
                {
                    currentColor = Color.white;

                    if (r3 > g3)
                    {
                        currentColor = Color.black;
                    }
                    else if (g3 > b3)
                    {
                        currentColor = new Color(b3, b3, b3);
                    }
                    else if (b3 > r3)
                    {
                        currentColor = new Color(g3, g3, g3);
                    }
                }
                else if (isBW > 0.5f)
                {
                    if (r3 > g3)
                    {
                        currentColor = Color.black;
                    }
                    else if (g3 > b3)
                    {
                        currentColor = new Color(b3, b3, b3);
                    }
                    else
                    {
                        currentColor = new Color(g3, g3, g3);
                    }
                }
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private void doCollidingPlasmaBW(Texture2D texture)
    {
        Debug.Log("doCollidingPlasmaBW");

        float ratio = 10 + Random.value * 20;
        float choice = Random.value * 20;

        float ratio2 = 10 + Random.value * 20;
        float choice2 = Random.value * 20;

        float ratio3 = 10 + Random.value * 20;
        float ratio4 = 10 + Random.value * 20;

        float ratio5 = Random.value * 100;
        float ratio6 = Random.value * 100;

        float isBW = 2 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                selectColorFromPerlinAndChoice(ratio5, choice, yFloat + 100 * Mathf.Cos(yFloat / ratio3), xFloat + 100 * Mathf.Sin(xFloat / ratio), out float r, out float g, out float b);
                selectColorFromPerlinAndChoice(ratio6, choice2, yFloat + 100 * Mathf.Cos(yFloat / ratio4), xFloat + 100 * Mathf.Sin(xFloat / ratio2), out float r2, out float g2, out float b2);
                float r3, g3, b3;

                if (r > r2)
                {
                    r3 = r;
                }
                else
                {
                    r3 = r2;
                }
                if (b > b2)
                {
                    b3 = b;
                }
                else
                {
                    b3 = b2;
                }
                if (g > g2)
                {
                    g3 = g;
                }
                else
                {
                    g3 = g2;
                }

                Color currentColor = new Color(r3, g3, b3);

                if (isBW > 1f)
                {
                    currentColor = Color.white;

                    if (r3 > g3)
                    {
                        currentColor = Color.black;
                    }
                    else if (g3 > b3)
                    {
                        currentColor = new Color(b3, b3, b3);
                    }
                    else if (b3 > r3)
                    {
                        currentColor = new Color(g3, g3, g3);
                    }
                }
                else
                {
                    if (r3 > g3)
                    {
                        currentColor = Color.black;
                    }
                    else if (g3 > b3)
                    {
                        currentColor = new Color(b3, b3, b3);
                    }
                    else
                    {
                        currentColor = new Color(g3, g3, g3);
                    }
                }
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private static void selectColorFromPerlinAndChoice(float ratio, float choice, float yFloat, float xFloat, out float r, out float g, out float b)
    {
        float perlinNoise = Mathf.PerlinNoise(xFloat / ratio, yFloat / ratio);

        r = Mathf.Cos(2 * Mathf.PI * perlinNoise);
        g = Mathf.Sin(2 * Mathf.PI * perlinNoise);
        b = perlinNoise;
        if (choice > 18)
        {
            b = Mathf.Cos(2 * Mathf.PI * perlinNoise) * Mathf.Sin(2 * Mathf.PI * perlinNoise);
        }
        else if (choice > 16)
        {
            g = perlinNoise;
        }
        else if (choice > 14)
        {
            r = Mathf.PerlinNoise(xFloat / (ratio * choice), yFloat / (ratio * choice));
        }
        else if (choice > 12)
        {
            r = Mathf.Cos(2 * Mathf.PI * perlinNoise);
            g = r * Mathf.PerlinNoise(xFloat / (ratio * choice), yFloat / (ratio * choice));
            float newRand = Random.value;
            b = g * Mathf.PerlinNoise(xFloat / (ratio * newRand), yFloat / (ratio * newRand));
        }
        else if (choice > 10)
        {
            r = Mathf.Cos(2 * Mathf.PI * perlinNoise);
            g = r;
            b = r;
        }
        else if (choice > 8)
        {
            r = 0.4f + 0.6f * Mathf.Cos(2 * Mathf.PI * perlinNoise);
            g = 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
            b = 0.6f + 0.4f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
        }
        else if (choice > 6)
        {
            r = 0.5f + 0.5f * Mathf.Cos(2 * Mathf.PI * perlinNoise);
            g = 0.4f + 0.6f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
            b = 0.6f + 0.4f * perlinNoise;
        }
        else if (choice > 4)
        {
            r = 0.5f + 0.5f * perlinNoise;
            g = 0.4f + 0.6f * perlinNoise;
            b = 0.6f + 0.4f * perlinNoise;
        }
        else if (choice > 2)
        {
            r = 0.2f + 0.8f * perlinNoise;
            g = 0.3f + 0.7f * perlinNoise;
            b = 0.4f + 0.6f * Mathf.Sin(2 * Mathf.PI * perlinNoise);
        }
    }

    private static void sinCos1(Texture2D texture)
    {
        Debug.Log("sinCos1");

        float aValue = 30 * Random.value;
        float aValue2 = 30 * Random.value;
        float aValue3 = 30 * Random.value;
        float aValue4 = 30 * Random.value;
        float aValue5 = 30 * Random.value;
        float aValue6 = 30 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                float r = Mathf.Cos(xFloat / aValue + yFloat / aValue2);
                float g = Mathf.Sin(yFloat * xFloat / aValue3 + yFloat / aValue4);
                float b = Mathf.Cos(xFloat / aValue5 * yFloat / aValue6);

                Color currentColor = new Color(r, g, b);
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private static void sinCos2(Texture2D texture)
    {
        Debug.Log("sinCos2");

        float aValue = 30 * Random.value;
        float aValue2 = 30 * Random.value;
        float aValue3 = 30 * Random.value;
        float aValue4 = 30 * Random.value;
        float aValue5 = 30 * Random.value;
        float aValue6 = 30 * Random.value;
        float aValue7 = 30 * Random.value;
        float aValue8 = 30 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                float r = Mathf.Cos(xFloat / aValue + aValue2 * Mathf.Cos(yFloat / aValue3));
                float g = Mathf.Sin(aValue4 * Mathf.Cos(xFloat / aValue5) + yFloat / aValue6);
                float b = Mathf.Cos(xFloat / aValue7 * yFloat / aValue8);

                Color currentColor = new Color(r, g, b);
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private static void sinCos3(Texture2D texture)
    {
        Debug.Log("sinCos3");

        float aValue = 30 * Random.value;
        float aValue2 = 50 * Random.value;
        float aValue3 = 40 * Random.value;
        float aValue4 = 40 * Random.value;
        float aValue5 = 30 * Random.value;
        float aValue6 = 30 * Random.value;
        float aValue7 = 30 * Random.value;
        float aValue8 = 30 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                float r = (xFloat / aValue + yFloat / aValue2) / (1024 / aValue3 + 1024 / aValue4);
                float g = Mathf.Sin(aValue5 * Mathf.Cos(xFloat / aValue6) + yFloat / 20);
                float b = Mathf.Cos(xFloat / aValue7 * yFloat / aValue8);

                Color currentColor = new Color(r, g, b);
                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private static void sinCosBW(Texture2D texture)
    {
        Debug.Log("sinCosBW");

        float aValue = 30 * Random.value;
        float aValue2 = 50 * Random.value;
        float aValue3 = 40 * Random.value;
        float aValue4 = 40 * Random.value;
        float aValue5 = 30 * Random.value;
        float aValue6 = 30 * Random.value;

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
            {
                float xFloat = (float)x;
                float r = Mathf.Cos(xFloat / aValue + yFloat / aValue2);
                float g = Mathf.Sin(yFloat * xFloat / aValue3 + yFloat / aValue4);
                float b = Mathf.Cos(xFloat / aValue5 * yFloat / aValue6);

                Color currentColor = Color.white;

                if (r > g)
                {
                    currentColor = Color.black;
                }
                else if (g > b)
                {
                    currentColor = new Color(b, b, b);
                }
                else
                {
                    currentColor = new Color(g, g, g);
                }

                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private static void sinCosBW3(Texture2D texture)
    {
        Debug.Log("sinCosBW3");

        int[] lastLine = new int[1024];
        int[] nextLastLine = new int[1024];

        for (int x = 0; x < 1024; x++)
        {
            lastLine[x] = 0;
            nextLastLine[x] = 0;
        }

        for (int y = 0; y < 1024; y++)
        {
            int yMod = y % 10;

            for (int x = 0; x < 1024; x++)
            {

                int xMod = x % 10;
                int value = 0;

                if (x == 0)
                {
                    if ((lastLine[x] + lastLine[x + 1]) == 0)
                    {
                        value = 1;
                    }
                }
                else if (x == 1023)
                {
                    if ((nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1]) % 2 == 0)
                    {
                        value = 1;
                    }
                }
                else
                {
                    if ((nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1] + lastLine[x + 1]) % 2 == 0)
                    {
                        value = 1;
                    }
                }
                Color currentColor = Color.white;

                if (value == 1)
                {
                    currentColor = Color.black;
                }

                texture.SetPixel(x, y, currentColor);

                nextLastLine[x] = value;
            }
            for (int x = 0; x < 1024; x++)
            {
                lastLine[x] = nextLastLine[x];
            }
        }
    }

    public void loadYANYAF()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
    }
}
