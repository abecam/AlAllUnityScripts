using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ShowCreditsYANYAF : MonoBehaviour
{
    public GameObject thisCamera;
    private GameObject mainCamera; // When activated, we deactivate the mainCamera, then start showing the credits
    private SwitchPrefsOnOff prefGui;
    public GameObject fullGUI;

    public GameObject titleLine;
    public GameObject nameLine;

    public GameObject ourSprite;

    public GameObject closeCreditObject; // For mobile, provide a button to close the credits

    //bool showCredit;

    List<string> allTitlesAndNames = new List<string>()
        {
           "YANYAF","Yet Another\nNYAF",
           "A Game by","Alain Becam\n(Dev & gameplay)",
           "Graphics by","Nobody ;)",
           "Testing, gameplay ideas", "William",
           "Music by","Alexander Nakarada",
           "Music by","Chris Huelsbeck",
           "Music by","Chan Redfield",
           "Music by","LiQWYD",
           "Font", "SudegnakNo3\nGlametrixBold by\nGluk\nhttp://www.glukfonts.pl",
           "Special Thanks to",
           "My family and friends"
        };

    // Start is called before the first frame update
    void Start()
    {
    }

    const int nbOfFramesForTitle = 250;
    const int nbOfFramesForText = 350;
    const int offsetForText = 100;
    const int offsetForSprite = 100;
    const int nbOfFramesForSprite = 400;
    private readonly float stepZoomTitle = 0.015f / ((float)nbOfFramesForText);
    private readonly float stepZoomName = 0.015f / ((float)(nbOfFramesForText));

    int currentFrameForText = 0;
    int currentFrameForSprite = 0;

    private readonly float stepZoomSprite = 1f / ((float)nbOfFramesForSprite);

    float currentZoomTitle = 0.015f;
    float currentZoomName = 0.015f;
    float currentZoomSprite = 0;

    int currentLine = 0;
    int currentSprite = 0;

    private void setAllElements()
    {
        Debug.Log("Setting up elements");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject prefImage = GameObject.FindGameObjectWithTag("PrefsSwitch");
        prefGui = prefImage.GetComponent<SwitchPrefsOnOff>();
    }
    // Update is called once per frame
    void Update()
    {
        checkForExit();

        // For each 2 line:
        // Zoom the first one, then the 2nd one
        if (currentFrameForText < nbOfFramesForTitle)
        {
            currentZoomTitle -= stepZoomTitle;
            titleLine.transform.localScale = new Vector3(currentZoomTitle, currentZoomTitle);
        }
        if (currentFrameForText < nbOfFramesForText && currentFrameForText > offsetForText)
        {
            currentZoomName -= stepZoomName;
            nameLine.transform.localScale = new Vector3(currentZoomName, currentZoomName);
        }

        currentFrameForText++;

        if (currentFrameForText > offsetForSprite && currentFrameForSprite < nbOfFramesForSprite)
        {
            currentZoomSprite += stepZoomSprite;
            ourSprite.transform.localScale = new Vector3(currentZoomSprite, currentZoomSprite, currentZoomSprite);

            currentFrameForSprite++;
        }
        if (currentFrameForSprite >= nbOfFramesForSprite)
        {
            // Start again
            setUpLinesAndSprite();
        }
        // Then zoom the next sprite until it covers everything
    }

    private void setUpLinesAndSprite()
    {
        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0.015f;
        currentZoomName = 0.015f;
        currentZoomSprite = 0;

        if (allTitlesAndNames.Count <= currentLine)
        {
            // Back to start
            currentLine = 0;
        }
        String title = allTitlesAndNames[currentLine++];
        String name = allTitlesAndNames[currentLine++];

        ((TextMesh)titleLine.GetComponent("TextMesh")).text = title;
        ((TextMesh)nameLine.GetComponent("TextMesh")).text = name;
        titleLine.transform.localScale = new Vector3(0.0f, 0.0f);
        nameLine.transform.localScale = new Vector3(0.0f, 0.0f);

        if (currentSprite >= ourSprite.transform.childCount)
        {
            currentSprite = 0;
        }
        // Get a new background
        createTexture();

        ourSprite.transform.localPosition = new Vector3(0.0f, 0.0f, 4);
        ourSprite.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void checkForExit()
    {
        if (thisCamera.activeSelf)
        {
            if ( Keyboard.current.escapeKey.wasReleasedThisFrame )
            {
                hideCredits();
            }
            else 
            {
                // use the input stuff
                if (Mouse.current.leftButton.isPressed)
                {
                    hideCredits();
                }
                // Check if the user has clicked
                bool aTouch = Mouse.current.leftButton.isPressed;

                if (aTouch)
                {
                    hideCredits();
                }

                if (Keyboard.current.escapeKey.wasReleasedThisFrame)
                {
                    hideCredits();
                }
            }
        }
    }

    public void showCredits()
    {
        setAllElements();

        mainCamera.SetActive(false);
        thisCamera.SetActive(true);
        prefGui.hideMe();
        fullGUI.SetActive(false);

        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0;
        currentZoomName = 0;
        currentZoomSprite = 0;

        currentLine = 0;

        currentSprite = 0;

        closeCreditObject.SetActive(true);

        setUpLinesAndSprite();

        createTexture();
    }

    public void hideCredits()
    {

        closeCreditObject.SetActive(false);

        mainCamera.SetActive(true);
        thisCamera.SetActive(false);
        if (mainCamera.GetComponent<AudioSource>() != null)
        {
            ((AudioSource)mainCamera.GetComponent<AudioSource>()).Play();
        }
        prefGui.showMe();
        fullGUI.SetActive(true);
    }

    private void createTexture()
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
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
        else if (value > 150)
        {
            doPlasma(texture);
        }
        else if (value > 125)
        {
            doCollidingPlasmaRule(texture, false, new Rules(1, 0, 1, 0, 1));
        }
        else if (value > 100)
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
        Renderer imageRenderer = ourSprite.GetComponent<Renderer>();
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

    private void doCollidingPlasmaRule(Texture2D texture, bool firstLineRandom, Rules newRule)
    {
        Debug.Log("doCollidingPlasmaRule with rules " + newRule.valueForZeroNeighbour + "," + newRule.valueForOneNeighbour + "," +
            newRule.valueForTwoNeighbour + "," + newRule.valueForThreeNeighbour + "," + newRule.valueForFourNeighbour);

        Debug.Log("doPlasma");

        int[] lastLine = new int[1024];
        int[] nextLastLine = new int[1024];

        for (int x = 0; x < 1024; x++)
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

        for (int y = 0; y < 1024; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < 1024; x++)
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
                else if (x == 1023)
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
            for (int x = 0; x < 1024; x++)
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

        int[] lastLine = new int[1024];
        int[] nextLastLine = new int[1024];

        for (int x = 0; x < 1024; x++)
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

        for (int y = 0; y < 1024; y++)
        {
            int yMod = y % 10;

            for (int x = 0; x < 1024; x++)
            {

                int xMod = x % 10;
                int value = 0;
                int nbNeigbours = 0;

                if (x == 0)
                {
                    nbNeigbours = (lastLine[x] + lastLine[x + 1]);
                }
                else if (x == 1023)
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
            for (int x = 0; x < 1024; x++)
            {
                lastLine[x] = nextLastLine[x];
            }
        }
    }

    private static void rulesBWBlended(Texture2D texture, Rules newRule, Rules newRule2, bool firstLineRandom)
    {
        Debug.Log("rulesBWBlended with rules " + newRule.valueForZeroNeighbour + "," + newRule.valueForOneNeighbour + "," +
            newRule.valueForTwoNeighbour + "," + newRule.valueForThreeNeighbour + "," + newRule.valueForFourNeighbour + " - " +
            newRule2.valueForZeroNeighbour + "," + newRule2.valueForOneNeighbour + "," + newRule2.valueForTwoNeighbour + "," +
            newRule2.valueForThreeNeighbour + "," + newRule2.valueForFourNeighbour);

        int[] lastLine = new int[1024];
        int[] nextLastLine = new int[1024];

        int[] lastLine2 = new int[1024];
        int[] nextLastLine2 = new int[1024];

        for (int x = 0; x < 1024; x++)
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

        for (int y = 0; y < 1024; y++)
        {
            int yMod = y % 10;

            for (int x = 0; x < 1024; x++)
            {

                int xMod = x % 10;

                int nbNeigbours = 0;

                if (x == 0)
                {
                    nbNeigbours = (lastLine[x] + lastLine[x + 1]);
                }
                else if (x == 1023)
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
            for (int x = 0; x < 1024; x++)
            {
                lastLine[x] = nextLastLine[x];
                lastLine2[x] = nextLastLine2[x];
            }
        }
    }
}
