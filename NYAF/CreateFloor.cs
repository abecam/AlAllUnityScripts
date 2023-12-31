using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFloor : MonoBehaviour
{
    public GameObject backgroundImage;
    // Start is called before the first frame update
    void Start()
    {
        createTexture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static int sizeTexture = 2048;

    private void createTexture()
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(sizeTexture, sizeTexture, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;

        doCollidingPlasmaBW(texture);

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to
        Renderer imageRenderer = backgroundImage.GetComponent<Renderer>();
        imageRenderer.material.mainTexture = texture;
    }
    private void doCollidingPlasmaBW(Texture2D texture)
    {
        Debug.Log("doCollidingPlasmaBW");

        //float ratio = 10 + Random.value * 20;
        //float choice = Random.value * 20;

        //float ratio2 = 10 + Random.value * 20;
        //float choice2 = Random.value * 20;

        //float ratio3 = 10 + Random.value * 20;
        //float ratio4 = 10 + Random.value * 20;

        //float ratio5 = Random.value * 100;
        //float ratio6 = Random.value * 100;
        //float ratio = 60; //  20 + Random.value * 20;
        //float choice = 20 + Random.value * 20;

        //float ratio2 = 60; // 20 + Random.value * 20;
        //float choice2 = 40 + Random.value * 20;

        //float ratio3 = 60; // 20 + Random.value * 20;
        //float ratio4 = 60; //  20 + Random.value * 20;

        //float ratio5 = 50; //  Random.value * 50;
        //float ratio6 = 50; //  Random.value * 50;
        float ratio = 50 + Random.value * 100;
        float choice = Random.value * 20;

        float ratio2 = 50 + Random.value * 100;
        float choice2 = Random.value * 20;

        float isBW = 2 * Random.value;

        Debug.Log("UUUU - ratio " + ratio + " choice " + choice + " ratio2 " + ratio2 + "choice2 " + choice2 + "isBW " + isBW);
        for (int y = 0; y < sizeTexture; y++)
        {
            float yFloat = (float)y;

            for (int x = 0; x < sizeTexture; x++)
            {
                float xFloat = (float)x;
                //selectColorFromPerlinAndChoice(ratio5, choice, yFloat + 5 * Mathf.Cos(yFloat / ratio3), xFloat + 5 * Mathf.Sin(xFloat / ratio), out float r, out float g, out float b);
                //selectColorFromPerlinAndChoice(ratio6, choice2, yFloat + 5 * Mathf.Cos(yFloat / ratio4), xFloat + 5 * Mathf.Sin(xFloat / ratio2), out float r2, out float g2, out float b2);

                selectColorFromPerlinAndChoice(ratio, choice, yFloat, xFloat, out float r, out float g, out float b);
                selectColorFromPerlinAndChoice(ratio2, choice2, yFloat, xFloat, out float r2, out float g2, out float b2);

                float r3, g3, b3;

                r3 = r;
                g3 = g;
                b3 = b;

                //if (r > r2)
                //{
                //    r3 = r;
                //}
                //else
                //{
                //    r3 = r2;
                //}
                //if (b > b2)
                //{
                //    b3 = b;
                //}
                //else
                //{
                //    b3 = b2;
                //}
                //if (g > g2)
                //{
                //    g3 = g;
                //}
                //else
                //{
                //    g3 = g2;
                //}

                Color currentColor = new Color(r3, r3, r3);

                //if (isBW > 1f)
                //{
                //    currentColor = Color.white;

                //    if (r3 > g3)
                //    {
                //        currentColor = new Color(r3, r3, r3);
                //    }
                //    else if (g3 > b3)
                //    {
                //        currentColor = new Color(b3, b3, b3);
                //    }
                //    else if (b3 > r3)
                //    {
                //        currentColor = new Color(g3, g3, g3);
                //    }
                //}
                //else
                //{
                //    if (r3 > g3)
                //    {
                //        currentColor = new Color(r3, r3, r3);
                //    }
                //    else if (g3 > b3)
                //    {
                //        currentColor = new Color(b3, b3, b3);
                //    }
                //    else
                //    {
                //        currentColor = new Color(g3, g3, g3);
                //    }
                //}
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
}
