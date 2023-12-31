using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ManageYANYAF : MainApp
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.

    private bool inGameOver = false; // Game won or lost, to show the end message

    public int currentLevel;
    private String rootNameOfLevel = "YANYAF";

    public GameObject arrow;

    public GameObject backgroundImage;

    public GameObject title;

    public GameObject calculatingBox; // Message before the creation of the texture.

    public GameObject characters;

    public GameObject hints; // The dynamic hints when less than a certain numbers of characters are left
    public GameObject arrows; // The dynamic arrows pointing at the characters when less than a certain numbers of characters are left

    public GameObject redBomb;
    public GameObject blackBomb;

    private List<GameObject> redBombs = null;
    private List<GameObject> blackBombs = null;
    private List<GameObject> traps = null;

    private AudioSource bombSound;

    public GameObject allMusics;

    int shakeCamera = 0; // Give a visual clue a bomb has been touched.

    public Color red; //= new Color(0.8f, 0.1f,0.0f);
    public Color green; //= new Color(0.1f, 0.9f,0.1f);
    public Color blue; //= new Color(0.1f, 0.2f,1.0f);
    public Color velvet; //= new Color(0.1f, 0.2f,1.0f);

    public Color redLight; //= new Color(1.0f, 0.3f,0.0f);
    public Color greenLight; // = new Color(0.3f, 1.0f,0.2f);
    public Color blueLight; //= new Color(0.3f, 0.3f,1.0f);

    public Color yellow = new Color(1.0f, 0.8f, 0.1f);

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    private float minImageX;
    private float maxImageX;
    private float minImageY;
    private float maxImageY;

    private int nbOfElements = 0;

    private int scoreOfLevel = 0;

    private int coolDown = 0; // Do not immediately do something else after some action

    private bool pieceTransparences = true;
    private bool showHints = true;
    private bool showArrows = true;

    public Toggle pieceTransparencesToggle;
    public Toggle showHintsToggle;
    public Toggle showArrowsToggle;

    Vector3 targetPos;

    float currentTime;
    float lastTime;
    float initialTime;

    float lastTimeSpent = 0; // last saved time for the mode
    float lastTimeSpentLevel = 0; // last saved time for the level

    /// GAME MODE
    int currentGameMode = 0;
    public const int absMaxMode = 5;

    // Secret code trigger
    public GameObject triggerForCode;

    // Mode 1 variables
    // Find X number of characters, no time
    int nbOfPiecesFound = 0;
    private int nbOfPiecesToFind = 1500;

    // Mode 2 variables
    // Find all character, all characters reshuffle after a while
    float currentTimeBeforeReshuffle = 0;
    const float timeBeforeReshuffle = 30; // 30 seconds

    // Mode 3 variables
    // Get the lowest score possible
    // 20 second to find a piece, -20 for each piece found.
    float currentTimeAllocated = 20;
    const float timeForAPiece = 20;

    int currentDifficulty = 2;

    float extendX ; // Not too much at the borders!
    float extendY ;

    float halfX ;
    float halfY ;

    static int sizeTexture = 2048;

    // Achievements will be defined by an interface and free numbers of achievements

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

        if (PlayAndKeepMusic.Instance != null)
        {
            PlayAndKeepMusic.Instance.Activated = false;
            PlayAndKeepMusic.Instance.stopMusic();
        }
        /////////////////////////////////////////////////////////////////////// DEBUG REMOVE AFTER /////////////////
        //resetSmallFound();

        // Get the kind of game from the menu
        initIntroductionBox();

        // Find background boundaries
        minImageX = backgroundImage.GetComponent<Renderer>().bounds.min.x;
        maxImageX = backgroundImage.GetComponent<Renderer>().bounds.max.x;
        minImageY = backgroundImage.GetComponent<Renderer>().bounds.min.y;
        maxImageY = backgroundImage.GetComponent<Renderer>().bounds.max.y;

        extendX = maxImageX - minImageX - 0.3f; // Not too much at the borders!
        extendY = maxImageY - minImageY - 0.3f;

        halfX = extendX / 2;
        halfY = extendY / 2;

        // Check the preferences
        bool haskey = LocalSave.HasIntKey("yanyaf_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("yanyaf_difficulty");
        }

        haskey = LocalSave.HasIntKey("yanyaf_transparentChars");
        if (haskey)
        {
            pieceTransparences = LocalSave.GetInt("yanyaf_transparentChars") == 1;
            pieceTransparencesToggle.isOn = pieceTransparences;

            //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn);
        }

        haskey = LocalSave.HasIntKey("yanyaf_showHints");
        if (haskey)
        {
            showHints = LocalSave.GetInt("yanyaf_showHints") == 1;
        }
        showHintsToggle.isOn = showHints;

        haskey = LocalSave.HasIntKey("yanyaf_showArrows");
        if (haskey)
        {
            showArrows = LocalSave.GetInt("yanyaf_showArrows") == 1;
        }
        showArrowsToggle.isOn = showArrows;

        // And which mode we are in
        haskey = LocalSave.HasIntKey("yanyaf_currentMode");
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("yanyaf_currentMode");
        }

        Debug.Log("#### Current Mode is " + currentGameMode);
        LocalSave.SetInt("YANYAF_lastMode", currentGameMode);
        LocalSave.Save();

        if (currentGameMode == 1)
        {
            LoadNbOfPiecesLeft();
        }

        if (currentGameMode == 3)
        {
            LoadGlobalTime();
        }

        // Load the last time spent
        LoadTimeSpent();
        //prepareTextureAndStart();
    }

    private void prepareTextureAndStart()
    {
        createTexture();

        setupObjects();

        // Create a hidden bomb to get the sound
        GameObject hiddenBlackBomb = Instantiate(blackBomb);
        hiddenBlackBomb.transform.position = new Vector2(1000, 1000);
        bombSound = hiddenBlackBomb.GetComponent<AudioSource>();

        lastTime = Time.time; // Last "frame" time
        initialTime = lastTime; // Level start time
        currentTime = lastTime;
    }

    private void createTexture()
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(sizeTexture, sizeTexture, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;

        float value = Random.value * 210;

        //value = 130;

        if (value > 200)
        {
            doCollidingPlasma2(texture);
        }
        else if (value > 175)
        {
            doCollidingPlasma(texture);
        }
        else if(value > 150)
        {
            doPlasma(texture);
        }
        else if (value > 125)
        {
            doCollidingPlasmaRule(texture,false, new Rules(1, 0, 1, 0, 1));
        }
        else if(value > 100)
        {
            doCollidingPlasmaBW(texture);
        }
        else if (value > 90)
        {
            // set the pixel values
            sinCos1(texture);
        }
        else if (value > 80)
        {
            // set the pixel values
            sinCos2(texture);
        }
        else if (value > 70)
        {
            // set the pixel values
            sinCos3(texture);
        }
        else if (value > 60)
        {
            // set the pixel values
            sinCosBW(texture);
        }
        else if (value > 50)
        {
            // set the pixel values
            Debug.Log("A");
            doCollidingPlasmaRule(texture, false, new Rules(1, 1, 1, 0, 1));
        }
        else if (value > 40)
        {
            // set the pixel values
            sinCosBW3(texture);
        }
        else if (value > 30)
        {
            Debug.Log("B");
            // set the pixel values
            sinCosBW4(texture, new Rules(1, 1, 0, 0, 1), true);
        }
        else if (value > 20)
        {
            Debug.Log("C");
            // set the pixel values
            doCollidingPlasmaRule(texture, false, new Rules(1, 0, 1, 0, 1));
        }
        else if (value > 10)
        {
            Debug.Log("D");
            // set the pixel values
            doCollidingPlasmaRule(texture, false, new Rules(1, 1, 0, 1, 0));
        }
        else
        {
            Debug.Log("E");
            // set the pixel values
            doCollidingPlasmaRule(texture, false, new Rules(1, 0, 1, 0, 1)); // new Rules(1, 0, 0, 0, 1)
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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;

                float perlinNoise = Mathf.PerlinNoise(xFloat/ ratio, yFloat/ ratio);

                float r = Mathf.Cos(2*Mathf.PI*perlinNoise);
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
                    r = Mathf.PerlinNoise(xFloat / (ratio*choice), yFloat / (ratio * choice));
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
                    r = 0.4f+ 0.6f*Mathf.Cos(2 * Mathf.PI * perlinNoise);
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
                    isBW = 2*Random.value;
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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
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

        float ratio3 = 10 + Random.value* 20;
        float ratio4 = 10 + Random.value*20;

        float ratio5 = Random.value * 100;
        float ratio6 = Random.value * 100;

        float doFullRandom = Random.value;
        bool inFullRandom = false;

        if (doFullRandom > 0.5f)
        {
            inFullRandom = true;
        }
        float isBW = 2 * Random.value;

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;
                selectColorFromPerlinAndChoice(ratio5, choice, yFloat+100*Mathf.Cos(yFloat/ ratio3), xFloat+100*Mathf.Sin(xFloat/ ratio), out float r, out float g, out float b);
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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
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

    private void doCollidingPlasmaRule(Texture2D texture, bool firstLineRandom, Rules newRule)
    {
        Debug.Log("doCollidingPlasmaRule with rules " + newRule.valueForZeroNeighbour + "," + newRule.valueForOneNeighbour + "," +
            newRule.valueForTwoNeighbour + "," + newRule.valueForThreeNeighbour + "," + newRule.valueForFourNeighbour);

        Debug.Log("doPlasma");

        int[] lastLine = new int[sizeTexture];
        int[] nextLastLine = new int[sizeTexture];

        for (int x = 0; x < sizeTexture; x++)
        {
            if (firstLineRandom)
            {
                float aValue = Random.value;

                lastLine[x] = (aValue > 0.5f) ? 1 : 0;
            }
            else
            {
                lastLine[x] = 0;
            }
            nextLastLine[x] = 0;
        }

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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;
                selectColorFromPerlinAndChoice(ratio5, choice, yFloat + 100 * Mathf.Cos(yFloat / ratio3), xFloat + 100 * Mathf.Sin(xFloat / ratio), out float r, out float g, out float b);
                selectColorFromPerlinAndChoice(ratio6, choice2, yFloat + 100 * Mathf.Cos(yFloat / ratio4), xFloat + 100 * Mathf.Sin(xFloat / ratio2), out float r2, out float g2, out float b2);

                int value = 0;
                float valueFloat = 0;
                int nbNeigbours = 0;

                if (x == 0)
                {
                    nbNeigbours = (lastLine[x] + lastLine[x + 1]);
                }
                else if (x == sizeTexture - 1)
                {
                    nbNeigbours = (nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1]);
                }
                else
                {
                    nbNeigbours = (nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1] + lastLine[x + 1]);
                }

                if (nbNeigbours == 0)
                {
                    value = newRule.valueForZeroNeighbour;
                    valueFloat = 0.2f;
                }
                else if (nbNeigbours == 1)
                {
                    value = newRule.valueForOneNeighbour;
                    valueFloat = 0.6f;
                }
                else if (nbNeigbours == 2)
                {
                    value = newRule.valueForTwoNeighbour;
                    valueFloat = 0.4f;
                }
                else if (nbNeigbours == 3)
                {
                    value = newRule.valueForThreeNeighbour;
                    valueFloat = 0.4f;
                }
                else if (nbNeigbours == 4)
                {
                    value = newRule.valueForFourNeighbour;
                    valueFloat = 0.2f;
                }

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
                //if ((r3 < ((float )value/3)) && (g3 > ((float)value / 2f)))
                //{
                //    if (value == 1)
                //    {
                //        currentColor = Color.black;
                //    }
                //    else
                //    {
                //        currentColor = Color.white;
                //    }
                //}
                float lastR = currentColor.r;
                float lastG = currentColor.g;
                float lastB = currentColor.b;
                lastR -= lastR * valueFloat;
                lastG -= lastG * valueFloat;
                lastB -= lastB * valueFloat;

                currentColor = new Color(lastR, lastG, lastB);

                texture.SetPixel(x, y, currentColor);

                nextLastLine[x] = value;
            }
            for (int x = 0; x < sizeTexture; x++)
            {
                lastLine[x] = nextLastLine[x];
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

    private static void doRandomBW4(Texture2D texture)
    {
        float aValue = Random.value;
        int zero = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int one = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int two = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int three = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int four = (aValue > 0.5f) ? 1 : 0;

        Rules rules1 = new Rules(zero, one, two, three, four);

        sinCosBW4(texture, rules1, false);
    }

    private static void doRandomBWBlend(Texture2D texture)
    {
        float aValue = Random.value;
        int zero = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int one = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int two = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int three = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        int four = (aValue > 0.5f) ? 1 : 0;

        Rules rules1 = new Rules(zero, one, two, three, four);

        aValue = Random.value;
        zero = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        one = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        two = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        three = (aValue > 0.5f) ? 1 : 0;
        aValue = Random.value;
        four = (aValue > 0.5f) ? 1 : 0;

        Rules rules2 = new Rules(zero, one, two, three, four);
        rulesBWBlended(texture, rules1, rules2, false);
    }

    private static void sinCos1(Texture2D texture)
    {
        Debug.Log("sinCos1");

        float aValue = 30*Random.value;
        float aValue2 = 30*Random.value;
        float aValue3 = 30*Random.value;
        float aValue4 = 30 * Random.value;
        float aValue5 = 30 * Random.value;
        float aValue6 = 30 * Random.value;

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;
                float r = (xFloat / aValue + yFloat / aValue2) /(sizeTexture/ aValue3 + sizeTexture/ aValue4);
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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
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

        int[] lastLine = new int[sizeTexture];
        int[] nextLastLine = new int[sizeTexture];

        for (int x = 0; x < sizeTexture; x++)
        {
            lastLine[x] = 0;
            nextLastLine[x] = 0;
        }

        for (int y = 0; y < sizeTexture; y++)
        {
            int yMod = y % 10;

            for (int x = 0; x < sizeTexture; x++)
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
                else if (x == sizeTexture - 1)
                {
                    if ((nextLastLine[x-1]+lastLine[x] + lastLine[x -1]) % 2 == 0)
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
            for (int x = 0; x < sizeTexture; x++)
            {
                lastLine[x] = nextLastLine[x];
            }
        }
    }

    struct Rules
    {
        public int valueForZeroNeighbour;
        public int valueForOneNeighbour;
        public int valueForTwoNeighbour;
        public int valueForThreeNeighbour;
        public int valueForFourNeighbour;

        public Rules(int valueForZeroNeighbour, int valueForOneNeighbour, int valueForTwoNeighbour, int valueForThreeNeighbour, int valueForFourNeighbour)
        {
            this.valueForZeroNeighbour = valueForZeroNeighbour;
            this.valueForOneNeighbour = valueForOneNeighbour;
            this.valueForTwoNeighbour = valueForTwoNeighbour;
            this.valueForThreeNeighbour = valueForThreeNeighbour;
            this.valueForFourNeighbour = valueForFourNeighbour;
        }
    }
    private static void sinCosBW4(Texture2D texture, Rules newRule, bool firstLineRandom)
    {
        Debug.Log("sinCosBW4 with rules " + newRule.valueForZeroNeighbour + "," + newRule.valueForOneNeighbour + "," +
            newRule.valueForTwoNeighbour + "," + newRule.valueForThreeNeighbour + "," + newRule.valueForFourNeighbour);

        int[] lastLine = new int[sizeTexture];
        int[] nextLastLine = new int[sizeTexture];

        for (int x = 0; x < sizeTexture; x++)
        {
            if (firstLineRandom)
            {
                float aValue = Random.value;

                lastLine[x] = (aValue > 0.5f) ? 1 : 0;
            }
            else
            {
                lastLine[x] = 0;
            }
            nextLastLine[x] = 0;
        }

        for (int y = 0; y < sizeTexture; y++)
        {
            int yMod = y % 10;

            for (int x = 0; x < sizeTexture; x++)
            {

                int xMod = x % 10;
                int value = 0;
                int nbNeigbours = 0;

                if (x == 0)
                {
                    nbNeigbours = (lastLine[x] + lastLine[x + 1]);
                }
                else if (x == sizeTexture - 1)
                {
                    nbNeigbours = (nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1]);
                }
                else
                {
                    nbNeigbours = (nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1] + lastLine[x + 1]);
                }

                if (nbNeigbours == 0)
                {
                    value = newRule.valueForZeroNeighbour;
                }
                else if (nbNeigbours == 1)
                {
                    value = newRule.valueForOneNeighbour;
                }
                else if (nbNeigbours == 2)
                {
                    value = newRule.valueForTwoNeighbour;
                }
                else if (nbNeigbours == 3)
                {
                    value = newRule.valueForThreeNeighbour;
                }
                else if (nbNeigbours == 4)
                {
                    value = newRule.valueForFourNeighbour;
                }
                Color currentColor = Color.white;

                if (value == 1)
                {
                    currentColor = Color.black;
                }

                texture.SetPixel(x, y, currentColor);

                nextLastLine[x] = value;
            }
            for (int x = 0; x < sizeTexture; x++)
            {
                lastLine[x] = nextLastLine[x];
            }
        }
    }

    private static void rulesBWBlended(Texture2D texture, Rules newRule, Rules newRule2, bool firstLineRandom)
    {
        Debug.Log("rulesBWBlended with rules "+ newRule.valueForZeroNeighbour+","+ newRule.valueForOneNeighbour + "," +
            newRule.valueForTwoNeighbour + "," + newRule.valueForThreeNeighbour + "," + newRule.valueForFourNeighbour + " - " +
            newRule2.valueForZeroNeighbour + "," + newRule2.valueForOneNeighbour + "," + newRule2.valueForTwoNeighbour + "," +
            newRule2.valueForThreeNeighbour + "," + newRule2.valueForFourNeighbour);

        int[] lastLine = new int[sizeTexture];
        int[] nextLastLine = new int[sizeTexture];

        int[] lastLine2 = new int[sizeTexture];
        int[] nextLastLine2 = new int[sizeTexture];

        for (int x = 0; x < sizeTexture; x++)
        {
            if (firstLineRandom)
            {
                float aValue = Random.value;

                lastLine[x] = (aValue > 0.5f) ? 1 : 0;
            }
            else
            {
                lastLine[x] = 0;
            }
            nextLastLine[x] = 0;

            if (firstLineRandom)
            {
                float aValue = Random.value;

                lastLine2[x] = (aValue > 0.5f) ? 1 : 0;
            }
            else
            {
                lastLine2[x] = 0;
            }
            nextLastLine2[x] = 0;
        }

        for (int y = 0; y < sizeTexture; y++)
        {
            int yMod = y % 10;

            for (int x = 0; x < sizeTexture; x++)
            {

                int xMod = x % 10;
               
                int nbNeigbours = 0;

                if (x == 0)
                {
                    nbNeigbours = (lastLine[x] + lastLine[x + 1]);
                }
                else if (x == sizeTexture - 1)
                {
                    nbNeigbours = (nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1]);
                }
                else
                {
                    nbNeigbours = (nextLastLine[x - 1] + lastLine[x] + lastLine[x - 1] + lastLine[x + 1]);
                }
                int value = 0;
                int value2 = 0;

                if (nbNeigbours == 0)
                {
                    value = newRule.valueForZeroNeighbour;
                    value2 = newRule2.valueForZeroNeighbour;
                }
                else if (nbNeigbours == 1)
                {
                    value = newRule.valueForOneNeighbour;
                    value2 = newRule2.valueForOneNeighbour;
                }
                else if (nbNeigbours == 2)
                {
                    value = newRule.valueForTwoNeighbour;
                    value2 = newRule2.valueForTwoNeighbour;
                }
                else if (nbNeigbours == 3)
                {
                    value = newRule.valueForThreeNeighbour;
                    value2 = newRule2.valueForThreeNeighbour;
                }
                else if (nbNeigbours == 4)
                {
                    value = newRule.valueForFourNeighbour;
                    value2 = newRule2.valueForFourNeighbour;
                }
                Color currentColor = Color.white;

                if (value == 1 && value2 == 0)
                {
                    currentColor = Color.black;
                }
                else if (value == 0 && value2 == 1)
                {
                    currentColor = new Color(0.4f, 0.4f, 0.4f);
                }
                else if (value == 1 && value2 == 1)
                {
                    currentColor = new Color(0.8f, 0.8f, 0.8f);
                }

                texture.SetPixel(x, y, currentColor);

                nextLastLine[x] = value;
                nextLastLine2[x] = value2;
            }
            for (int x = 0; x < sizeTexture; x++)
            {
                lastLine[x] = nextLastLine[x];
                lastLine2[x] = nextLastLine2[x];
            }
        }
    }

    private void setupObjects()
    {
        // Check if their is a save available, otherwise start anew
        if (existSave())
        {
            loadLevel();

            Debug.Log("Difficulty is " + currentDifficulty+ " and nb of elements is" + nbOfElements);
            if (currentDifficulty == SelectDifficulty.veryEasy)
            {
                if (nbOfElements > 20)
                {
                    nbOfElements = 20;
                }
                nbOfPiecesToFind = 300;
            }
            if (currentDifficulty == SelectDifficulty.easy)
            {
                if (nbOfElements > 40)
                {
                    nbOfElements = 40;
                }
                nbOfPiecesToFind = 600;
            }

            scoreOfLevel = nbOfElements;

            Debug.Log("B-  Difficulty is " + currentDifficulty + " and nb of elements is " + nbOfElements);

            // The bombs are replaced at random at each time
            placeBombsIfNeeded();
        }
        else
        {
            // Place all characters available, ensure they are not at the same place as others.
            nbOfElements = characters.transform.childCount;

            Debug.Log("Difficulty is " + currentDifficulty + " and nb of elements is " + nbOfElements);
            if (currentDifficulty == SelectDifficulty.veryEasy)
            {
                if (nbOfElements > 20)
                {
                    nbOfElements = 20;
                }
                nbOfPiecesToFind = 300;
            }
            if (currentDifficulty == SelectDifficulty.easy)
            {
                if (nbOfElements > 40)
                {
                    nbOfElements = 40;
                }
                nbOfPiecesToFind = 600;
            }

            Debug.Log("B-  Difficulty is " + currentDifficulty + " and nb of elements is" + nbOfElements);

            scoreOfLevel = nbOfElements;

            placeCharactersAtRandom();

            placeBombsIfNeeded();
        }
    }

    private void placeBombsIfNeeded()
    {
        if (currentDifficulty < 3)
        {
            return;
        }
        if (currentDifficulty == SelectDifficulty.hard)
        {
            placeBombs(1);
        }
        if (currentDifficulty == SelectDifficulty.veryHard)
        {
            // 4 of them, random size
            placeBombs(2);
        }
        if (currentDifficulty == SelectDifficulty.insane)
        {
            // 8 of them
            placeBombs(4);
        }
        if (currentDifficulty == SelectDifficulty.purgatory)
        {
            // 16 of them, some trapped pieces
            placeBombs(8);

            placeTraps(8);
        }
        if (currentDifficulty == SelectDifficulty.hell)
        {
            // 16 of them, lot of trapped pieces
            placeBombs(8);

            placeTraps(16);
        }
    }

    private void placeTraps(int nbOfTraps)
    {
        traps = new List<GameObject>();
        // Create fake objects from our pool :)

        for (int iBomb = 0; iBomb < nbOfTraps; iBomb++)
        {
            // Find the object
            int posInCharacters = (int )(((float )nbOfElements) * Random.value);
            GameObject ourTrap = characters.transform.GetChild(posInCharacters).gameObject;

            GameObject aNewTrap = Instantiate(ourTrap);
            // Push it somewhere
            Vector2 newPos = findNewPlace();

            allPosTaken.Add(newPos);

            aNewTrap.transform.position = new Vector3(newPos.x, newPos.y, 2);

            // And change the color :)
            Color previousColor = aNewTrap.GetComponent<Renderer>().material.color;
            aNewTrap.GetComponent<Renderer>().material.color = new Color(1, 0.6f, 0.6f, 1);

            traps.Add(aNewTrap);
        }
    }

    private void placeBombs(int nbOfPairs)
    {
        blackBombs = new List<GameObject>();
        redBombs = new List<GameObject>();

        for (int iBomb = 0; iBomb < nbOfPairs; iBomb++)
        {
            // Just the 2
            GameObject aNewBlackBomb = Instantiate(blackBomb);
            // Push it somewhere
            Vector2 newPos = findNewPlace();

            allPosTaken.Add(newPos);

            aNewBlackBomb.transform.position = new Vector3(newPos.x, newPos.y, 2);
            // Size between 0.2 and 0.5
            float newSize = Random.value * 0.3f + 0.2f;
            Vector2 newBombSize = new Vector2(newSize, newSize);
            aNewBlackBomb.transform.localScale = newBombSize;

            blackBombs.Add(aNewBlackBomb);

            GameObject aNewRedBomb = Instantiate(redBomb);
            // Push it somewhere
            newPos = findNewPlace();

            allPosTaken.Add(newPos);

            aNewRedBomb.transform.position = new Vector3(newPos.x, newPos.y, 2);

            newSize = Random.value * 0.3f + 0.2f;
            Vector2 newRedBombSize = new Vector2(newSize, newSize);
            aNewBlackBomb.transform.localScale = newBombSize;

            redBombs.Add(aNewRedBomb);
        }
    }

    private Vector2 findNewPlace()
    {
        float posX = (extendX * Random.value) - halfX;
        float posY = (extendY * Random.value) - halfY;

        Boolean isOk = false;

        int iTrial = 0;

        while (!isOk && iTrial < 200)
        {
            iTrial++;

            isOk = true; // Default

            Vector2 thisPos = new Vector2(posX, posY);

            foreach (Vector2 onePos in allPosTaken)
            {
                if (Vector2.Distance(onePos, thisPos) < 0.5f)
                {
                    isOk = false;
                    break;
                }
            }
            if (!isOk)
            {
                posX = (extendX * Random.value) - halfX;
                posY = (extendY * Random.value) - halfY;
            }
        }
        if (!isOk)
        {
            Debug.Log("The bomb might be overlapping another object !");

            // Park the bomb or trap...
            return new Vector2(100,100);
        }

        return new Vector2(posX, posY);
    }

    List<Vector2> allPosTaken; // Need to be reused to place the bombs

    private void placeCharactersAtRandom()
    {
        allPosTaken = new List<Vector2>();

        foreach (Transform childCharacter in characters.transform)
        {
            //Debug.Log("Child: " + childCharacter);

            // TODO: - Check that we are on the picture
            // - Avoid pref icon and ad block
            float posX = (extendX * Random.value) - halfX;
            float posY = (extendY * Random.value) - halfY;

            Boolean isOk = false;

            int iTrial = 0;

            while (!isOk && iTrial < 50)
            {
                iTrial++;

                isOk = true; // Default

                Vector2 thisPos = new Vector2(posX, posY);

                foreach (Vector2 onePos in allPosTaken)
                {
                    if (Vector2.Distance(onePos, thisPos) < 1f)
                    {
                        isOk = false;
                        break;
                    }
                }
                if (!isOk)
                {
                    posX = (extendX * Random.value) - halfX;
                    posY = (extendY * Random.value) - halfY;
                }
            }

            if (iTrial <= 50)
            {
                Debug.Log("Found after " + iTrial + "trials");
            }
            else
            {
                Debug.Log("Place not found!!!");
            }
            allPosTaken.Add(new Vector2(posX, posY));

            childCharacter.position = new Vector3(posX, posY, 2);

            // Make them a tiny bit transparent
            float newAlpha = ((Random.value / 4) + 0.75f);
            if (!pieceTransparences)
            {
                newAlpha = 1;
            }

            Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
            float red = previousColor.r;
            float green = previousColor.b;

            if (currentDifficulty == SelectDifficulty.easy)
            {
                childCharacter.localScale = new Vector2(2, 2);
                red = 0.6f;
                green = 0.6f;
            }
            else if (currentDifficulty == SelectDifficulty.veryEasy)
            {
                childCharacter.localScale = new Vector2(4, 4);
                red = 0.6f;
                green = 0.6f;
            }
            childCharacter.GetComponent<Renderer>().material.color = new Color(red, green, previousColor.b, newAlpha);
        }
    }

    /**
     * Same thing but only with active characters, do not alter the score
     */
    private void replaceActiveCharactersAtRandom()
    {
        Vector2[] allPosTaken = new Vector2[nbOfElements];

        float extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        float halfX = extendX / 2;

        float extendY = maxImageY - minImageY - 0.2f;
        float halfY = extendY / 2;

        int iCharacter = 0;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 90)
            {
                float posX = (extendX * Random.value) - halfX;
                float posY = (extendY * Random.value) - halfY;

                Boolean isOk = false;

                int iTrial = 0;

                while (!isOk && iTrial < 50)
                {
                    iTrial++;

                    isOk = true; // Default

                    Vector2 thisPos = new Vector2(posX, posY);

                    foreach (Vector2 onePos in allPosTaken)
                    {
                        if (Vector2.Distance(onePos, thisPos) < 0.4f)
                        {
                            isOk = false;
                            break;
                        }
                    }
                    if (!isOk)
                    {
                        posX = (extendX * Random.value) - halfX;
                        posY = (extendY * Random.value) - halfY;
                    }
                }

                if (iTrial < 50)
                {
                    Debug.Log("Found after " + iTrial + " trials");
                }
                else
                {
                    Debug.Log("Place not found!!!");
                }
                allPosTaken[iCharacter++] = new Vector2(posX, posY);

                childCharacter.position = new Vector3(posX, posY, 2);

                // Make them a tiny bit transparent
                float newAlpha = ((Random.value / 4) + 0.75f);
                if (!pieceTransparences)
                {
                    newAlpha = 1;
                }

                Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
                childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
            }
        }
    }

    private void loadLevel()
    {
        Debug.Log("Trying to load level "+ giveSavePath());
        if (File.Exists(giveSavePath()))
        {
            allPosTaken = new List<Vector2>();

            nbOfElements = characters.transform.childCount;
            scoreOfLevel = nbOfElements;

            // 2
            String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

            Save save = JsonUtility.FromJson<Save>(json);

            // 3
            for (int iChar = 0; iChar < save.charX.Count; iChar++)
            {
                Transform currentObject = characters.transform.GetChild(iChar);

                float posX = save.charX[iChar];
                float posY = save.charY[iChar];

                Boolean wasFound = save.charFound[iChar];
                float posZ = 2;

                if (wasFound)
                {
                    posX = 100;
                    posY = 0;

                    nbOfElements--;
                    scoreOfLevel = nbOfElements;
                }
                else
                {
                    allPosTaken.Add(new Vector2(posX,posY));
                }

                currentObject.transform.position = new Vector3(posX, posY, posZ);

                Color previousColor = currentObject.GetComponent<Renderer>().material.color;
                float alpha = 1;
                if (pieceTransparences)
                {
                    alpha = save.alpha[iChar];
                }
                currentObject.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, alpha);
            }
            int nbLeft = save.nbLeft;

            if (nbLeft != nbOfElements)
            {
                Debug.Log("PROBLEM!!!! Nb of elements left ("+nbOfElements+")not matching with Save ("+nbLeft+") and result from save");
            }

            Debug.Log("Game Loaded");
        }
    }

    private void saveLevel()
    {
        Debug.Log(Application.persistentDataPath);

        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }

        if(!File.Exists(giveSavePath()))
        { 
            Save save = new Save();

            foreach (Transform childCharacter in characters.transform)
            {
                Boolean wasFound = false;

                if (childCharacter.position.x > 90)
                {
                    // Not found
                    wasFound = true;
                }
                
                float posX = childCharacter.transform.position.x;
                float posY = childCharacter.transform.position.y;

                float alpha = childCharacter.GetComponent<Renderer>().material.color.a;

                save.charX.Add(posX);
                save.charY.Add(posY);

                save.alpha.Add(alpha);
                save.charFound.Add(wasFound);
            }
            save.nbLeft = nbOfElements;

            string json = JsonUtility.ToJson(save);

            File.WriteAllText(giveSavePath(), json, System.Text.UTF8Encoding.UTF8);

            Debug.Log("Game saved");
        }
        else
        {
            Debug.Log("PROBLEM: Could not delete save file");
        }
    }

    /**
    * When the level is finished, we delete the current save.
    */
    private void deleteSave()
    {
        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }
    }

    private string giveSavePath()
    {
        return Application.persistentDataPath + "/"+ getCurrentMode() + "yanyaf_gamesave" + currentLevel + ".save";
    }

    private bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    private bool dragMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    bool preStarted = true;
    bool notStarted = false;
    int iFrameCalculating = 0;

    // Update is called once per frame
    void Update()
    {
        if (notStarted)
        {
            if (iFrameCalculating < 10)
            {
                iFrameCalculating++;
                return;
            }
            Debug.Log("Starting calculating");
            prepareTextureAndStart();
            Debug.Log("Hiding calculating message");
            showWaitingBox(false);
            notStarted = false;
        }
        if (preStarted)
        {
            Debug.Log("Showing calculating message");
            showWaitingBox(true);

            preStarted = false;
            notStarted = true;

            return;
        }
        if (startingGame)
        {
            // Manage the introduction box
            showIntroductionBox(true);

            return;
        }

        if (inGameOver)
        {
            // Manage the introduction box
            showIntroductionBox(false);
            return;
        }

        if (gameIsInPause)
        {
            // Nothing to do
            return;
        }

        // Wait for the title to stop playing before removing it
        if (title.activeInHierarchy)
        {
            if (title.GetComponent<AudioSource>() != null)
            {
                // Only if our music is not playing yet.
                if (!((AudioSource)title.GetComponent<AudioSource>()).isPlaying)
                {
                    Debug.Log("Removing title and starting music");
                    // Remove the title
                    title.SetActive(false);
                    playMusic();
                }
            }
        }
        else
        {
            playMusic();
        }

        currentTime = Time.time;

        if (shakeCamera > 0)
        {
            shakeCamera--;
            float xRand = 0.2f*Random.value - 0.1f;
            float yRand = 0.2f * Random.value - 0.1f;
            transform.position = new Vector3(xRand, yRand, -7);
        }
        else if (shakeCamera == 0)
        {
            transform.position = new Vector3(0, 0, -7);
            shakeCamera = -1;
        }

        if (currentGameMode == 2)
        {
            if (currentTimeBeforeReshuffle > timeBeforeReshuffle)
            {
                currentTimeBeforeReshuffle = 0;

                replaceActiveCharactersAtRandom();
            }
            currentTimeBeforeReshuffle += Time.deltaTime;
        }

        if (currentGameMode == 3)
        {
            currentTimeAllocated += Time.deltaTime;
        }

        if (coolDown != 0)
        {

            coolDown--;
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
            // Check if the user has clicked
            bool aTouch = Mouse.current.leftButton.wasReleasedThisFrame;
            bool rightClick = Mouse.current.rightButton.wasReleasedThisFrame;

            if (aTouch)
            {
                // Debug.Log( "Moused moved to point " + touchPos );

                checkBoard(touchPos);
            }
            else
            {
                // No touch reset the cool down
                coolDown = 0;
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

            if (Mouse.current.scroll.ReadValue().y > 0f || Keyboard.current.upArrowKey.isPressed) // forward
            {
                zoomWheel(true);
            }
            else if (Mouse.current.scroll.ReadValue().y < 0f || Keyboard.current.downArrowKey.isPressed) // backwards
            {
                zoomWheel(false);
            }
        }

        // Move the camera if needed
        updateCameraPos();

        // Remove the hints when needed
        updateHints();

        // Keep absolute scale of GUI elements
        //score.transform.localScale = score.transform.localScale;
        lastTime = currentTime; // Last "frame" time
    }

    private void showWaitingBox(bool isVisible)
    {
        calculatingBox.SetActive(isVisible);
    }

    float currentZoomCamera = normalZoom;
    public float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.

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
            if (Vector2.Distance(touchPos1,touchPos2) > Vector2.Distance(zoomOldPosition1,zoomOldPosition2))
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
            float newRatio = currentZoomCamera / normalZoom;
            Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);
            foreach (Transform childCharacter in hints.transform)
            {
                childCharacter.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            }
        }
    }

    private void zoomWheel(bool zoomIn)
    {
        if (zoomIn)
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
        float newRatio = currentZoomCamera / normalZoom;
        Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);
        foreach (Transform childCharacter in hints.transform)
        {
            childCharacter.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        }
    }

    private void zoomBoard(bool isZoom)
    {
        if (isZoom)
        {
            Camera.main.orthographicSize = globalZoom;
            Camera.main.transform.localScale = new Vector3(ratio, ratio, 1);
            foreach (Transform childCharacter in hints.transform)
            {
                childCharacter.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            }
        }
        else
        {
            Camera.main.orthographicSize = normalZoom;
            Camera.main.transform.localScale = new Vector3(1, 1, 1);
            foreach (Transform childCharacter in hints.transform)
            {
                childCharacter.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            }
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
            cameraSpeed -= positionDelta/ DIVIDER_CAMERA_SPEED;

            //Camera.main.transform.position = RestrainCamera(cameraPos);
        }

        dragOldPosition = mousePos;
    }

    private void updateCameraPos()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        cameraPos += cameraSpeed;

        cameraSpeed = cameraSpeed* REDUCTION_OF_SPEED;

        if (cameraSpeed.magnitude < 0)
        {
            cameraSpeed = new Vector3(0,0,0);
        }

        Camera.main.transform.position = RestrainCamera(cameraPos);
    }

    private Vector3 RestrainCamera(Vector3 cameraPos)
    {
        // Need to check the background image boundaries.

        if (cameraPos.x > maxImageX)
        {
            cameraPos.x = maxImageX;
        }
        else if (cameraPos.x < minImageX)
        {
            cameraPos.x = minImageX;
        }
        if (cameraPos.y > maxImageY)
        {
            cameraPos.y = maxImageY;
        }
        else if (cameraPos.y < minImageY)
        {
            cameraPos.y = minImageY;
        }
        return cameraPos;
    }

    // Introduction text
    public GameObject introductionBox;

    private bool startingGame = true;

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;
    public int dimingTime = 2000; // 200 frames
    float zoomFactor;
    float currentZoomForIntroBox;

    //private float globalZoom = 0.04f;
    //private float normalZoom = 0.1f;
    //private float stepBetweenZoom = 0.1f;

    private static readonly float globalZoom = 8f; // Was 4.
    private static readonly float normalZoom = 1.6f;
    private static readonly float ratio = globalZoom / normalZoom;

    private float stepBetweenZoom = 0.1f;

    private void initIntroductionBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        zoomFactor = (0.2f / ((float)dimingTime));
        currentZoomForIntroBox = 0;
        // Z = -9
        
        //backgroundImage.transform.localScale = new Vector3(globalZoom, globalZoom, 1);
        Camera.main.orthographicSize = globalZoom;

        stepBetweenZoom = (normalZoom - globalZoom) / ((float )dimingTime);
    }

    // Using same box, show the game over text
    private void initIOutroBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        zoomFactor = (0.2f / ((float)totalZoomingTime));
        currentZoomForIntroBox = 0;
        // Z = -9
        introductionBox.transform.localPosition = new Vector3(introductionBox.transform.localPosition.x, introductionBox.transform.localPosition.y, 9);
    }

    public static float GetScreenToWorldWidth()
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        float width = edgeVector.x * 2;
        return width;
    }

    bool initialTitleSetup = true;
    private bool inTitle = false;

    private void showIntroductionBox(bool isIntro)
    {
        if (currentStep == 0)
        {
            // Zoom the box into view
            if (zoomingTime <= totalZoomingTime)
            {
                if (initialTitleSetup)
                {
                    float width = GetScreenToWorldWidth();

                    if (width > 7.6f)
                    {
                        width = 7.6f;
                    }
                    title.transform.localScale = Vector2.one * width * 0.5f;

                    // And the scale on z must stay 1, or we can't layer.
                    title.transform.localScale = new Vector3(title.transform.localScale.x, title.transform.localScale.y, 1);

                    // And the title (and its selections) is active
                    inTitle = true;
                }
                float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, fading);
                    }
                }

                // backgroundImage.transform.localScale = new Vector3(0.02f + currentZoomForIntroBox, 0.02f + currentZoomForIntroBox, 1);

                //introductionBox.transform.localScale = new Vector3(0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox);
                zoomingTime++;
            }
            else
            {
                currentStep = 1;
                zoomingTime = 0;
            }
        }
        else if (currentStep == 1)
        {
            // Wait for user input
            if (Application.platform != RuntimePlatform.Android)
            {
                // use the input stuff

                bool aTouch = Mouse.current.leftButton.isPressed;
                if (aTouch)
                {
                    currentStep = 2;
                }
            }
            else
            {
                if (Input.touchCount >= 1)
                {
                    currentStep = 2;
                }
            }
        }
        else if (currentStep == 2)
        {
            inTitle = false;

            if (zoomingTime <= dimingTime)
            {
                // Fading out of the box
                float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                    }
                }
                currentZoomForIntroBox += stepBetweenZoom;

                // backgroundImage.transform.localScale = new Vector3(globalZoom + currentZoomForIntroBox, globalZoom + currentZoomForIntroBox, 1);
                Camera.main.orthographicSize = globalZoom + currentZoomForIntroBox;

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

                // Move the menu away
                // We can't remove it (deactivate it) as we need to know if its sound (drum) is still playing
                // We also need to push it aside, as otherwise we could still select the level (even if they are behind us).
                title.transform.localPosition = new Vector3(600, title.transform.localPosition.y, 20);
            }
            else
            {
                inGameOver = false;
                startingGame = true;
                initialTitleSetup = true;
            }
        }
    }

    private const int nbOfCharsToHint = 6;

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        Boolean foundSomething = false;

        // Check if we found the object
        // We have 2 and a half-> the normal one, the one in the normal ads, the one in the huge ads.
        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
            {
                updateModeVariables();

                // Play the attached sound
                if (childCharacter.GetComponent<AudioSource>() != null)
                {
                    ((AudioSource)childCharacter.GetComponent<AudioSource>()).Play();
                }
                GameObject newFoundParticle = (GameObject)Instantiate(particleFoundChar, childCharacter.transform.position, Quaternion.identity);
                Renderer renderer1 = newFoundParticle.GetComponent<Renderer>();
                if (renderer1 != null)
                {
                    float newRed = ((Random.value / 4) + 0.75f);
                    float newGreen = ((Random.value / 1.5f) + 0.25f);
                    float newBlue = ((Random.value / 1.5f) + 0.25f);
                    float newAlpha = ((Random.value / 4) + 0.75f);

                    renderer1.material.color = new Color(newRed, newGreen, newBlue, newAlpha);
                }
                newFoundParticle.transform.SetParent(particlesContainer.transform);

                //AudioSource.PlayClipAtPoint(childCharacter.GetComponent<AudioSource>(), childCharacter.transform.localPosition);

                // Parc the object
                childCharacter.position = new Vector3(100, 0, -9);

                checkForGameOver();

                foundSomething = true;
            }
        }

        if (redBombs != null)
        {
            foreach (GameObject childRedBomb in redBombs)
            {
                if (childRedBomb.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    // Add a penality
                    doBombPenality(0);
                }
            }
        }
        if (redBombs != null)
        {
            foreach (GameObject childBlackBomb in blackBombs)
            {
                if (childBlackBomb.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    // Add a penality
                    doBombPenality(1);
                }
            }
        }
        if (traps != null)
        {
            foreach (GameObject childTraps in traps)
            {
                if (childTraps.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    // Add a penality
                    doBombPenality(2);
                }
            }
        }

        if (!foundSomething && nbOfElements <= nbOfCharsToHint && !showingHint)
        {
            showLastCharacters();
            showingHint = true;
        }
    }

    private void doBombPenality(int forType)
    {
        bombSound.Play();

        shakeCamera = 10;

        //Debug.Log("Doing the penalty for type " + forType+ " and difficulty " + currentDifficulty);
        if (currentGameMode == 1)
        {
            if (forType == 0)
            {
                // Red bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    decreasePiecesFound(5);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    decreasePiecesFound(10);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    decreasePiecesFound(100);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    decreasePiecesFound(500);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    decreasePiecesFound(1000);
                }

                return;
            }
            if (forType == 1)
            {
                // Black bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    decreasePiecesFound(3);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    decreasePiecesFound(5);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    decreasePiecesFound(50);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    decreasePiecesFound(300);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    decreasePiecesFound(800);
                }

                return;
            }
            if (forType == 2)
            {
                // Traps
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    decreasePiecesFound(200);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    decreasePiecesFound(400);
                }
                return;
            }
        }
        else
        {
            if (forType == 0)
            {
                // Red bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    reawakePieces(5);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    reawakePieces(10);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    reawakePieces(20);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    reawakePieces(1000);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    reawakePieces(1000);
                }

                return;
            }
            if (forType == 1)
            {
                // Black bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    reawakePieces(2);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    reawakePieces(5);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    reawakePieces(10);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    reawakePieces(1000);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    reawakePieces(1000);
                }

                return;
            }
            if (forType == 2)
            {
                // Traps
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    reawakePieces(20);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    reawakePieces(40);
                }
                return;
            }
        }
    }

    private void decreasePiecesFound(int nbOfPiecesToRemove)
    {
        nbOfPiecesFound -= nbOfPiecesToRemove;
        if (nbOfPiecesFound < 0)
        {
            nbOfPiecesFound = 0;
        }

        MMPGAddNewPlayerUnits.nyafBombExploded(nbOfPiecesToRemove, ourShowResult);
    }

    private void reawakePieces(int howMany)
    {
        Debug.Log("reawakePieces " + howMany);

        int iReawaken = 0;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x > 90)
            {
                childCharacter.transform.position = findNewPlace();

                iReawaken++;

                nbOfElements++;
                scoreOfLevel++;

                if (iReawaken > howMany)
                {
                    break;
                }
            }
        }

        MMPGAddNewPlayerUnits.nyafBombExploded(howMany, ourShowResult);
    }

    const int timeToShowHints = 120; // roughly 30s
    private const float xDiffBetweenHints = 0.30f;
    int timeShowingHints = timeToShowHints;
    Boolean showingHint = false;
    float alpha = 1.0f;
    float alphaSteps;

    List<Transform> charForArrows;

    public bool InTitle { get => inTitle; }
    public int ScoreOfLevel { get => scoreOfLevel; }
    public int NbOfPiecesFound { get => nbOfPiecesFound; }
    public int NbOfElements { get => nbOfElements; }
    public float CurrentTimeAllocated { get => currentTimeAllocated; }
    public int NbOfPiecesToFind { get => nbOfPiecesToFind;}

    private void showLastCharacters()
    {
        alpha = 1;
        alphaSteps = alpha / ((float)timeToShowHints);

        float minXHint = -((float )(nbOfElements)) * xDiffBetweenHints / 2;
        float xInitial = minXHint;

        charForArrows = new List<Transform>();

        int iHelps = 0;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 90)
            {
                if (showHints)
                {
                    GameObject aNewHint;

                    aNewHint = Instantiate(childCharacter.gameObject);
                    // Now add as child of hints
                    aNewHint.transform.SetParent(hints.transform);
                    aNewHint.transform.localPosition = new Vector3(xInitial, -1f, 0.0f);
                    aNewHint.SetActive(true);
                    aNewHint.GetComponent<Renderer>().material.color = new Color(aNewHint.GetComponent<Renderer>().material.color.r, aNewHint.GetComponent<Renderer>().material.color.g, aNewHint.GetComponent<Renderer>().material.color.b, alpha);
                }
                if (showArrows)
                {
                    // And add the arrow
                    GameObject aNewArrow;

                    aNewArrow = Instantiate(arrow);
                    // Now add as child of hints
                    aNewArrow.transform.SetParent(arrows.transform);
                    aNewArrow.transform.localPosition = new Vector3(xInitial, -1f, 0.0f);

                    //Quaternion rotation = Quaternion.LookRotation(childCharacter.transform.position - aNewArrow.transform.position, aNewArrow.transform.TransformDirection(Vector3.back));
                    //aNewArrow.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

                    Vector3 dir = childCharacter.position - aNewArrow.transform.position;
                    float distanceToChar = 0.25f * Vector2.Distance(childCharacter.position, aNewArrow.transform.position);
                    if (distanceToChar > 1)
                    {
                        distanceToChar = 1;
                    }

                    charForArrows.Add(childCharacter);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    aNewArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    //Debug.Log("Rotating by " + aNewArrow.transform.rotation);

                    aNewArrow.SetActive(true);
                    aNewArrow.GetComponent<Renderer>().material.color = new Color(aNewArrow.GetComponent<Renderer>().material.color.r, aNewArrow.GetComponent<Renderer>().material.color.g, aNewArrow.GetComponent<Renderer>().material.color.b, alpha * distanceToChar);
                }

                xInitial += xDiffBetweenHints;

                iHelps++;

                if (iHelps >= nbOfCharsToHint)
                {
                    break;
                }
            }
        }
    }

    private void updateHints()
    {
        if (showingHint)
        {
            timeShowingHints--;

            if (timeShowingHints <= 0)
            {
                foreach (Transform childCharacter in hints.transform)
                {
                    //Debug.Log("Destroying hint!");
                    Destroy(childCharacter.gameObject);
                }
                foreach (Transform childCharacter in arrows.transform)
                {
                    //Debug.Log("Destroying arrows!");
                    Destroy(childCharacter.gameObject);
                }
                timeShowingHints = timeToShowHints;
                showingHint = false;
            }
            else
            {
                foreach (Transform childCharacter in hints.transform)
                {
                    childCharacter.GetComponent<Renderer>().material.color = new Color(childCharacter.GetComponent<Renderer>().material.color.r, childCharacter.GetComponent<Renderer>().material.color.g, childCharacter.GetComponent<Renderer>().material.color.b, alpha);
                }
                int iChar = 0;

                foreach (Transform childCharacter in arrows.transform)
                {
                    Transform nextChar = charForArrows[iChar++];
                    float distanceToChar = 0.25f * Vector2.Distance(childCharacter.position, nextChar.transform.position);
                    if (distanceToChar > 1)
                    {
                        distanceToChar = 1;
                    }

                    childCharacter.GetComponent<Renderer>().material.color = new Color(childCharacter.GetComponent<Renderer>().material.color.r, childCharacter.GetComponent<Renderer>().material.color.g, childCharacter.GetComponent<Renderer>().material.color.b, alpha* distanceToChar);

                    //Quaternion rotation = Quaternion.LookRotation(previousObject.transform.position - childCharacter.transform.position, childCharacter.transform.TransformDirection(Vector3.back));
                    //childCharacter.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
                    Vector3 dir = nextChar.position - childCharacter.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    childCharacter.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    //childCharacter.Rotate(new Vector3(0, 0, 1), angle);
                    //Debug.Log("Rotating by " + childCharacter.transform.rotation);
                }
                alpha -= alphaSteps;
            }
        }
    }

    private void updateModeVariables()
    {
        // Do something depending on the current mode
        nbOfElements--;
        scoreOfLevel--;

        // Save the time spent in the mode
        SaveTimeSpent();

        if (currentGameMode == 1)
        {
            nbOfPiecesFound++;
            SaveNbOfPiecesLeft();
        }
        // In game mode 3, remove 20 sec
        if (currentGameMode == 3)
        {
            currentTimeAllocated -= timeForAPiece;

            // Save the time
            SaveGlobalTime();
        }

        // Show the trigger for the secret code every 20 pieces (mode 1 so)

        if (nbOfPiecesFound % 20 == 0 && nbOfPiecesFound != 0)
        {
            triggerForCode.SetActive(true);
        }
        else
        {
            triggerForCode.SetActive(false);
        }
    }

    private void LoadTimeSpent()
    {
        bool haskey = LocalSave.HasFloatKey("yanyaf_allTimeSpent" + currentGameMode);

        if (haskey)
        {
            lastTimeSpent = LocalSave.GetFloat("yanyaf_allTimeSpent" + currentGameMode);
        }

        haskey = LocalSave.HasFloatKey("yanyaf_timeSpentLevel" + currentGameMode + "-" + currentLevel);

        if (haskey)
        {
            lastTimeSpentLevel = LocalSave.GetFloat("yanyaf_timeSpentLevel" + currentGameMode + "-" + currentLevel);
        }
    }

    float savedTimeSpent;
    float savedTimeSpentMode;

    private void SaveTimeSpent()
    {
        savedTimeSpent = currentTime - initialTime + lastTimeSpentLevel;
        savedTimeSpentMode = currentTime - initialTime + lastTimeSpent;

        LocalSave.SetFloat("yanyaf_allTimeSpent" + currentGameMode, savedTimeSpentMode);

        LocalSave.SetFloat("yanyaf_timeSpentLevel" + currentGameMode + "-" + currentLevel, savedTimeSpent);
        LocalSave.Save();
    }

    void checkForGameOver()
    {
        // Do something depending on the current mode
        if (currentGameMode != 1 && nbOfElements == 0)
        {
            LocalSave.SaveCopy();

            deleteSave();
            gameover();
        }
        else if (currentGameMode == 1)
        {       
            if (nbOfPiecesFound >= nbOfPiecesToFind)
            {
                LocalSave.SaveCopy();

                deleteSave();
                // TODO!!!! Reinit the nb of pieces
                gameWonMode1();
            }
            else if (nbOfElements == 0)
            {
                // Replace the pieces in the same level.
                deleteSave();

                setupObjects();
            }
        }
        else
        {
            // Otherwise save the current level
            saveLevel();
        }
    }

    void gameWonMode1()
    {
        LocalSave.SetInt("yanyaf_nbOfPiecesFound", 0);

        long timeSpentLong = (long)savedTimeSpent;

        long hoursSpent = timeSpentLong / 3600;
        if (hoursSpent > 0)
        {
            timeSpentLong -= hoursSpent * 3600;
        }

        long minutesSpent = timeSpentLong / 60;
        if (minutesSpent > 0)
        {
            timeSpentLong -= minutesSpent * 60;
        }

        long timeSpentLongMode = (long)savedTimeSpentMode;

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
        // Reset the time for this level
        LocalSave.SetFloat("yanyaf_timeSpentLevel" + currentGameMode + "-" + currentLevel, 0);

        LocalSave.Save();

        MMPGAddNewPlayerUnits.yanyafLevelEnded(currentDifficulty, ourShowResult);

        ourShowResult.showResultText(hoursSpent, minutesSpent, timeSpentLong, hoursSpentMode, minutesSpentMode, timeSpentLongMode, "OutroYANYAF");
    }

    void gameover()
    {
        loadNext();
    }

    private void SaveGlobalTime()
    {
        LocalSave.SetFloat("yanyaf_currentTimeAllocated", currentTimeAllocated);
        LocalSave.Save();
    }

    private void SaveNbOfPiecesLeft()
    {
        LocalSave.SetInt("yanyaf_nbOfPiecesFound", nbOfPiecesFound);
        LocalSave.Save();
    }

    private void LoadGlobalTime()
    {
        bool haskey = LocalSave.HasFloatKey("yanyaf_currentTimeAllocated");

        if (haskey)
        {
            currentTimeAllocated = LocalSave.GetFloat("yanyaf_currentTimeAllocated");
        }
    }

    private void LoadNbOfPiecesLeft()
    {
        bool haskey = LocalSave.HasIntKey("yanyaf_nbOfPiecesFound");

        if (haskey)
        {
            nbOfPiecesFound = LocalSave.GetInt("yanyaf_nbOfPiecesFound");
        }
    }

    void loadNext()
    {
        MMPGAddNewPlayerUnits.yanyafLevelEnded(currentDifficulty, ourShowResult);

        setNextLevelReached();

        long timeSpentLong = (long)savedTimeSpent;

        long hoursSpent = timeSpentLong / 3600;
        if (hoursSpent > 0)
        {
            timeSpentLong -= hoursSpent * 3600;
        }

        long minutesSpent = timeSpentLong / 60;
        if (minutesSpent > 0)
        {
            timeSpentLong -= minutesSpent * 60;
        }

        long timeSpentLongMode = (long)savedTimeSpentMode;

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
        // Reset the time for this level
        LocalSave.SetFloat("yanyaf_timeSpentLevel" + currentGameMode + "-" + currentLevel, 0);

        LocalSave.Save();

        ourShowResult.showResultText(hoursSpent, minutesSpent, timeSpentLong, hoursSpentMode, minutesSpentMode, timeSpentLongMode, rootNameOfLevel);
    }

    public void setUseTransparence(bool showTrans)
    {
        pieceTransparences = showTrans;
        //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn+ " - " + showTrans);

        LocalSave.SetInt("yanyaf_transparentChars", pieceTransparences? 1 : 0);
        LocalSave.Save();
    }

    public void setShowHints(bool newShowHints)
    {
        showHints = newShowHints;
        LocalSave.SetInt("yanyaf_showHints", newShowHints ? 1 : 0);
        LocalSave.Save();
    }

    public void setShowArrows(bool newShowArrows)
    {
        showArrows = newShowArrows;
        LocalSave.SetInt("yanyaf_showArrows", newShowArrows ? 1 : 0);
        LocalSave.Save();
    }

    public void setNextLevelReached()
    {  
        bool haskey = LocalSave.HasIntKey("yanyaf_levelReached" + getCurrentMode());
        int levelReached = 0;
        if (haskey)
        {
            levelReached = LocalSave.GetInt("yanyaf_levelReached" + getCurrentMode());
        }

        if (currentLevel >= levelReached)
        { 
            LocalSave.SetInt("yanyaf_levelReached" + getCurrentMode(), currentLevel+1);
            LocalSave.Save();
        }
    }

    internal string getCurrentMode()
    {
        String gameMode = "";
        if (currentGameMode > 0)
        {
            gameMode = "" + currentGameMode;
        }

        return gameMode;
    }

    /**
     * When called from another script at start, we might want to look from the save to be sure the data is the latest.
    */
    internal string getCurrentModeFromSave()
    {
        String gameMode = "";

        int currentGameModeFromSave = 0;

        bool haskey = LocalSave.HasIntKey("yanyaf_currentMode");
        if (haskey)
        {
            currentGameModeFromSave = LocalSave.GetInt("yanyaf_currentMode");
        }

        if (currentGameModeFromSave > 0)
        {
            gameMode = "" + currentGameModeFromSave;
        }

        return gameMode;
    }

    public int getCurrentModeNb()
    {
        return currentGameMode;
    }

    public void setCurrentModeNb(int newMode)
    {
        currentGameMode = newMode;

        LocalSave.SetInt("yanyaf_currentMode", newMode);
        LocalSave.Save();
    }

    public void resetLevelReached()
    {
        LocalSave.SetInt("yanyaf_levelReached", 1);
        LocalSave.Save();

        LocalSave.SetInt("yanyaf_levelReached1", 1);
        LocalSave.Save();

        LocalSave.SetInt("yanyaf_levelReached2", 1);
        LocalSave.Save();

        LocalSave.SetInt("yanyaf_levelReached3", 1);
        LocalSave.Save();
    }

    public void resetModeReached()
    {
        LocalSave.SetInt("yanyaf_currentMode", 0);
        LocalSave.Save();
    }

    public void resetTimeSpent()
    {
        for (int iMode = 0; iMode < 4; iMode ++)
        { 
            LocalSave.DeleteKey("yanyaf_bestTimeSpent"+iMode);
            LocalSave.SetFloat("yanyaf_allTimeSpent" + iMode, 0);
        }
        LocalSave.Save();
    }

    public void smallFound()
    {
        bool haskey = LocalSave.HasIntKey("yanyaf_smallFound");
        int allSmallFoundFlag = 0;
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("yanyaf_smallFound");
        }

        int currentOne = 2 << currentLevel;

        allSmallFoundFlag = allSmallFoundFlag | currentOne;

        LocalSave.SetInt("yanyaf_smallFound", allSmallFoundFlag);
        LocalSave.Save();
    }

    public bool wasCurrentSmallFound()
    {
        int currentOne = 2 << currentLevel;

        bool haskey = LocalSave.HasIntKey("yanyaf_smallFound");
        int allSmallFoundFlag = 0;
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("yanyaf_smallFound");

            if ((allSmallFoundFlag & currentOne) > 0)
            {
                return true;
            }
        }
        //Debug.Log("Am here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"+ currentOne+" - "+ levelReached);
        return false;
    }

    public void resetSmallFound()
    {
        LocalSave.SetInt("yanyaf_smallFound", 0);
        LocalSave.Save();
    }

    public void resetSave()
    {
        resetLevelReached();
        resetSmallFound();
        resetModeReached();
        resetTimeSpent();
        deleteLevelSaves();

        loadFirstLevel();
    }

    private void loadFirstLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
    }

    private GameObject currentMusic = null;
    private int iMusic = 0;

    // After the drum roll, start the music
    internal void playMusic()
    {
        // Find the next music from the collection and play it
        if (currentMusic == null)
        {
            iMusic = (int )(Random.value * (float )allMusics.transform.childCount);

            if (iMusic == allMusics.transform.childCount)
            {
                iMusic = 0;
            }

            currentMusic = allMusics.transform.GetChild(iMusic).gameObject;

            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Play();

            return;
        }
        
        if (!((AudioSource)currentMusic.GetComponent<AudioSource>()).isPlaying)
        {
            // Get the next one
            iMusic++;

            if (iMusic == allMusics.transform.childCount)
            {
                iMusic = 0;
            }
            currentMusic = allMusics.transform.GetChild(iMusic).gameObject;

            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Play();
        }
    }

    /**
     * Return to title screen to select another level
     */
    public void goToTitle()
    {
        gameIsInPause = false;

        title.SetActive(true);

        title.transform.localPosition = new Vector3(0, 0, 0.7f);

        Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);

        initIntroductionBox();
    }

    internal static void deleteLevelSaves()
    {
        // It is not very intensive, so we plan for more modes and levels.
        for (int iMode = 0; iMode < 10; iMode++)
        {
            for (int iLevel = 0; iLevel < 30; iLevel++)
            {
                String pathToDelete = Application.persistentDataPath + "/" + iMode + "yanyaf_gamesave" + iLevel + ".save";

                if (iMode == 0)
                {
                    pathToDelete = Application.persistentDataPath + "/yanyaf_gamesave" + iLevel + ".save";
                }             

                if (File.Exists(pathToDelete))
                {
                    File.Delete(pathToDelete);
                }
            }
        }
    }

    void OnDisable()
    {
        if (currentMusic != null)
        {
            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Stop();
        }
    }

    void OnEnable()
    {
        if (currentMusic != null)
        {
            ((AudioSource)currentMusic.GetComponent<AudioSource>()).Play();
        }
    }
}
