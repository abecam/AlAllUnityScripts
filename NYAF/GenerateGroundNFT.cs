using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGroundNFT : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;

    private int currentLevel = 0;

    public static float highestGround = -100;
    public static float lowestGround = 100;

    public GameObject groundTexture;
    public GameObject referenceImage;

    Texture2D referenceTexture;

    static int sizeTexture = 2048;

    private ArrayList allFunctions = new ArrayList();

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

                Color currentColor = new Color(r, r, r);
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

                Color currentColor = new Color(r3, r3, r3);

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

                if (inFullRandom)
                {
                    isBW = 2 * Random.value;
                }

                Color currentColor = new Color(r3, r3, r3);

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

        float ratio5 = 10 + Random.value * 100;
        float ratio6 = 10 + Random.value * 100;

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

                Color currentColor = new Color(r3, r3, r3);

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

    private void doChecked(Texture2D texture, bool firstLineRandom)
    {
        Debug.Log("doChecked");

        float ratio3 = 4 + Random.value * 20;

        float doFullRandom = Random.value;

        float isBW = 2 * Random.value;

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;
            int valueY = ((int)(yFloat / ratio3)) % 2;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;

                int value = ((int)(xFloat / ratio3)) % 2;

                Color currentColor = Color.white;

                if (valueY == 1)
                {
                    if (value == 1)
                    {
                        currentColor = Color.black;
                    }
                }
                else
                {
                    if (value == 0)
                    {
                        currentColor = Color.black;
                    }
                }

                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private void doCheckedFromSin(Texture2D texture, bool firstLineRandom)
    {
        Debug.Log("doCheckedFromSin");

        float ratio = 4 + Random.value * 20;
        float ratio3 = 4 + Random.value * 20;

        float doFullRandom = Random.value;

        float isBW = 2 * Random.value;

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = ratio * Mathf.Cos((float)y / ratio3);
            int valueY = ((int)(yFloat)) % 2;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = ratio * Mathf.Cos((float)x / ratio3);

                int value = ((int)(xFloat)) % 2;

                Color currentColor = Color.white;

                if (valueY == 1)
                {
                    if (value == 1)
                    {
                        currentColor = Color.black;
                    }
                }
                else
                {
                    if (value == 0)
                    {
                        currentColor = Color.black;
                    }
                }

                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    private static void selectColorFromPerlinAndChoice(float ratio, float choice, float yFloat, float xFloat, out float r, out float g, out float b)
    {
        float perlinNoise = Mathf.PingPong((xFloat / ratio) * (yFloat / ratio), 1);

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

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;
                float r = Mathf.Sin(yFloat * xFloat / aValue3 + yFloat / aValue4);
                float g = r;
                float b = r;

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
                float g = r;
                float b = r;

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
                float r = Mathf.Sin(aValue5 * Mathf.Cos(xFloat / aValue6) + yFloat / 20);
                float g = r;
                float b = r;

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
                    currentColor = new Color(r / 3, r / 3, r / 3);
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
            for (int x = 0; x < sizeTexture; x++)
            {
                lastLine[x] = nextLastLine[x];
            }
        }
    }

    private static void sinCosBW4(Texture2D texture, bool firstLineRandom)
    {
        float aValue = 5 + 30 * Random.value;

        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;
            int yMod = ((int)(yFloat / aValue)) % 2;

            for (int x = 0; x < sizeTexture; x++)
            {

                float xFloat = (float)x;
                int xMod = ((int)(xFloat / aValue)) % 2;

                int result = xMod * yMod;

                Color currentColor = Color.white;

                if (result == 1)
                {
                    currentColor = Color.black;
                }

                texture.SetPixel(x, y, currentColor);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        Debug.Log("Working on " + referenceTexture);

        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(sizeTexture + 1) * (sizeTexture + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        referenceTexture = generateTexture(sizeTexture);

        for (int y = 0, i = 0; y <= sizeTexture; y++)
        {
            for (int x = 0; x <= sizeTexture; x++, i++)
            {
                Color whatToPlaceRGBf = referenceTexture.GetPixel(x, y);
                Color32 whatToPlaceRGB = whatToPlaceRGBf;

                float height = 10 * whatToPlaceRGBf.r+ 10 * whatToPlaceRGBf.g + 10 * whatToPlaceRGBf.b;

                vertices[i] = new Vector3(x, height, y);
                //Debug.Log("Height is " + pointInfo.height);

                uv[i] = new Vector2(x / (float)sizeTexture, y / (float)sizeTexture);

                if (height > highestGround)
                {
                    highestGround = height;
                }
                else if (height < lowestGround)
                {
                    lowestGround = height;
                }
            }
        }
        mesh.vertices = vertices;

        int[] triangles = new int[sizeTexture * sizeTexture * 6];

        for (int ti = 0, vi = 0, y = 0; y < sizeTexture; y++, vi++)
        {
            for (int x = 0; x < sizeTexture; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + sizeTexture + 1;
                triangles[ti + 5] = vi + sizeTexture + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.uv = uv;
        //Unwrapping.GenerateSecondaryUVSet(mesh);
        //mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        //mesh.RecalculateTangents();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        // Now the 8k texture

        int sizeGenTexture = 4;

        var texture = new Texture2D(sizeTexture * sizeGenTexture, sizeTexture * sizeGenTexture, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;

        for (int y = 0, i = 0; y <= sizeTexture * sizeGenTexture; y++)
        {
            for (int x = 0; x <= sizeTexture * sizeGenTexture; x++, i++)
            {
                Color whatToPlaceRGBf = referenceTexture.GetPixel(x / sizeGenTexture, y / sizeGenTexture);
                Color32 whatToPlaceRGB = whatToPlaceRGBf;

                //float height = 10 * whatToPlaceRGBf.r + 10 * whatToPlaceRGBf.g + 10 * whatToPlaceRGBf.b;

                //texture.SetPixel(x, y, getColorFromAltitude(x,y,height, highestGround, 20) );
                texture.SetPixel(x, y, whatToPlaceRGBf);
            }
        }

        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        Renderer imageRenderer = groundTexture.GetComponent<Renderer>();
        imageRenderer.material.mainTexture = texture;
    }

    private Texture2D generateTexture(int sizeTexture)
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(sizeTexture, sizeTexture, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;

        float value = Random.value * 210;

        //value = 95;
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
            doCheckedFromSin(texture, false);
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
            doChecked(texture, false);
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
            sinCosBW4(texture, true);
        }
        else if (value > 20)
        {
            Debug.Log("C");
            // set the pixel values
            doChecked(texture, false);
        }
        else if (value > 10)
        {
            Debug.Log("D");
            // set the pixel values
            doCheckedFromSin(texture, false);
        }
        else
        {
            Debug.Log("E");
            // set the pixel values
            doCheckedFromSin(texture, false); // new Rules(1, 0, 0, 0, 1)
        }
        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        return texture;

    }

    public Color getColorFromAltitude(float x, float y, float height, float maxHill, int withSize)
    {
        float xShifted = x;
        float yShifted = y; //  (y + yShift)/sizeHill; // Get cut...

        if (height < 0)
        {
            height = 0;
        }
        //height = 20f * Mathf.Cos(x / 10f) * Mathf.Cos(y / 10f);
        Color newColor = Color.white;

        //if ((height > heightHill - (heightHill/7) + 0.2f*(Mathf.Cos(xShifted * 10) + Mathf.Cos(yShifted * 10))) && (height < heightHill - (heightHill * 10) + 0.2f * (Mathf.Cos(xShifted * 10) + Mathf.Cos(yShifted * 10))))
        ///if ((height > heightHill - (heightHill / 4)) && (height < heightHill - (heightHill / 5)))
        if ((height > maxHill - (maxHill / 4) + 6 * withSize * (Mathf.Cos(xShifted / (2 * withSize)) * Mathf.Cos(yShifted / (2 * withSize)))) && (height < maxHill - (maxHill / 4.5f) + 6 * withSize * (Mathf.Cos(xShifted / (2 * withSize)) * Mathf.Cos(yShifted / (2 * withSize)))))
        //if ((height > 0) && (height < maxHill))
        {
            // Black
            newColor = Color.black;
            //Debug.Log("Green");
        }
        else
        {
            // light green on one side...
        }

        //Debug.Log("Calculated " + ourNewData.height + " and " + ourNewData.color);
        return newColor;
    }
}
