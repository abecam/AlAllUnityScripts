using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCharactersLB : MonoBehaviour
{
    public GameObject charactersStatics;
    public Transform parentStatics;

    private List<GameObject> allStaticChars;
    private int nbOfStaticChars;

    private int currentLevel = 0;

    public bool isUnderground = false;
    public bool isUnderwater = false;

    void Start()
    {
        if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("FindingNYAF_currentLevel");
        }

        prepareStaticChars();

        placeCharactersOnACurve();
    }

    private void prepareStaticChars()
    {
        nbOfStaticChars = 0;
        allStaticChars = new List<GameObject>();

        foreach (Transform oneChar in charactersStatics.transform)
        {
            allStaticChars.Add(oneChar.gameObject);
            nbOfStaticChars++;
        }

        Debug.Log("Got " + nbOfStaticChars + " objects");
    }

    //private void placeCharactersOnACurve()
    //{
    //    float z = 0;
    //    GameObject newChar;

    //    for (int iLayer = 0; iLayer < 50; iLayer++)
    //    {
    //        //float amplitude = AmplitudeFunction(z);

    //        for (float radius = -Mathf.PI; radius < Mathf.PI; radius += 0.04f * Mathf.PI)
    //        {
    //            float xCurr, yCurr, xNext, yNext;

    //            float zOut; // Ignored here
    //                        //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);

    //            float xShift = (0.2f * Random.value) - 0.1f;
    //            float zShift = (0.2f * Random.value) - 0.1f;

    //            applyFunction(radius + xShift, 0, z + zShift, out xCurr, out yCurr, out zOut);
    //            applyFunction(radius + xShift + 0.04f * Mathf.PI, 0, z + zShift, out xNext, out yNext, out zOut);

    //            Vector2 from = new Vector2(xCurr, yCurr);
    //            Vector2 to = new Vector2(xNext, yNext);

    //            Vector2 normal = Vector2.Perpendicular(to - from);
    //            Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

    //            int nbOfCharHere = 1; //  ((int)(10 * Random.value));
    //            for (int iChar = 0; iChar < nbOfCharHere; iChar++)
    //            {


    //                int whichChar = (int)(nbOfStaticChars * Random.value);

    //                newChar = Instantiate(allStaticChars[whichChar], new Vector3(xCurr, yCurr, -4 + z), direction, parentStatics);

    //                bool flip = Random.value > 0.5f;
    //                float scale = 0.5f * Random.value;

    //                if (flip)
    //                {
    //                    newChar.transform.localScale = new Vector3(-newChar.transform.localScale.x, newChar.transform.localScale.y, newChar.transform.localScale.z);
    //                }
    //                newChar.transform.localScale = new Vector3((0.5f + scale)*newChar.transform.localScale.x, (0.5f + scale) * newChar.transform.localScale.y, (0.5f + scale) * newChar.transform.localScale.z);
    //                //Debug.Log("Added one object!" + iChar);
    //            }
    //        }
    //        z += 0.16f;
    //    }
    //}
   
    private void placeCharactersOnACurve()
    {
        float z = 0;
        GameObject newChar;

        int totalChar = 0;

        for (int iLayer = 0; iLayer < 80; iLayer++)
        {
            //float amplitude = AmplitudeFunction(z);
            float shiftX = Random.value;

            for (float radius = -6 * Mathf.PI; radius < 6 * Mathf.PI; radius += 0.08f * Mathf.PI)
            {
                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                            //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);

                float xShift = shiftX; //  (0.2f * Random.value) - 0.1f;
                float zShift = 0.4f * Mathf.Cos(radius + z); // (0.2f * Random.value) - 0.1f;

                applyFunction(radius + xShift, 0, z + zShift, out xCurr, out yCurr, out zOut, currentLevel);
                applyFunction(radius + xShift + 0.04f * Mathf.PI, 0, z + zShift, out xNext, out yNext, out zOut, currentLevel);

                Vector2 from = new Vector2(xCurr, yCurr);
                Vector2 to = new Vector2(xNext, yNext);

                Vector2 normal = Vector2.Perpendicular(to - from);
                Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                int nbOfCharHere = ((int)(10 * Random.value));
                int whichChar = (int)(nbOfStaticChars * Random.value);

                for (int iChar = 0; iChar < nbOfCharHere; iChar++)
                {
                   

                    newChar = Instantiate(allStaticChars[whichChar], new Vector3(xCurr, yCurr, -4 + zOut), direction, parentStatics);

                    totalChar++;

                    bool flip = Random.value > 0.5f;
                    float scale = 0.7f * Random.value;

                    if (flip)
                    {
                        newChar.transform.localScale = new Vector3(-newChar.transform.localScale.x, newChar.transform.localScale.y, newChar.transform.localScale.z);
                    }
                    newChar.transform.localScale = new Vector3((0.5f + scale) * newChar.transform.localScale.x, (0.5f + scale) * newChar.transform.localScale.y, (0.5f + scale) * newChar.transform.localScale.z);
                    //Debug.Log("Added one object!" + iChar);
                    if (isUnderground)
                    {
                        newChar.GetComponent<Renderer>().material.color = new Color(0.8f + Random.value * 0.2f, 0.8f + Random.value * 0.2f, 0.5f + Random.value * 0.2f, newChar.GetComponent<Renderer>().material.color.a);
                    }
                    else if (isUnderwater)
                    {
                        newChar.GetComponent<Renderer>().material.color = new Color(0.4f + Random.value * 0.2f, 0.4f + Random.value * 0.2f, 0.8f + Random.value * 0.2f, newChar.GetComponent<Renderer>().material.color.a);
                        newChar.gameObject.AddComponent<MoveInWater>();
                        newChar.gameObject.GetComponent<MoveInWater>().setInitialPos();
                    }
                }
            }
            z += 0.64f;
        }
        Debug.Log("Instantiated " + totalChar +" static characters");
    }

    //private static void sliceFunction(float amplitude, float radius, out float xCurr, out float yCurr, out float xNext, out float yNext)
    //{
    //    xCurr = amplitude * radius*0.4f; // Mathf.Cos(radius);
    //    yCurr = amplitude * Mathf.Sin(radius/2);
    //    xNext = amplitude * (radius + 0.04f * Mathf.PI) * 0.4f; //  Mathf.Cos(radius + 0.02f * Mathf.PI);
    //    yNext = amplitude * Mathf.Sin(radius/2 + 0.04f * Mathf.PI);
    //}

    //private static float AmplitudeFunction(float z)
    //{
    //    //Debug.Log("Layer  " + iLayer);
    //    float result = 3 + 1 * Mathf.Cos(z) * Mathf.Sin(0.4f * z);

    //    return result;
    //}

    public static void applyFunction(float xIn, float yIn, float zIn, out float xOut, out float yOut, out float zOut, int forLevel)
    {
        float amplitude = 3 + 1 * Mathf.Cos(zIn) * Mathf.Sin(0.4f * zIn);

        xOut = amplitude * xIn * 0.4f; // Mathf.Cos(radius);
        yOut = amplitude * 0.4f * Mathf.Sin(-1.6f + xIn / 4);

        zOut = zIn;

        switch (forLevel)
        {
            case 0:
                {
                    float amplitude2 = 2.5f;

                    xOut = amplitude2 * xIn * 0.4f; // Mathf.Cos(radius);
                    yOut = amplitude2 * 0.05f * Mathf.Sign(Mathf.Sin(xIn - 1.5f));

                    zOut = zIn;
                }
                break;
            case 1:
                {
                    float amplitude2 = 3 + 1 * Mathf.Cos(zIn) * Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f; // Mathf.Cos(radius);
                    yOut = amplitude2 * 0.4f * Mathf.Sin(-1.6f + xIn / 4);

                    zOut = zIn;
                }
                break;
            case 2:
                {
                    float amplitude2 = 3 + 1 * Mathf.Cos(zIn) * Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.05f * Mathf.Sign(Mathf.Sin(xIn - 1.5f));

                    zOut = zIn;
                }
                break;
            case 3:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.4f * Mathf.Cos(xIn / 8) * Mathf.Sin(-1.6f + xIn / 4);

                    zOut = zIn;
                }
                break;
            case 4:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.2f * Mathf.Sqrt(Mathf.Abs(2f + xIn));

                    zOut = zIn;
                }
                break;
            case 5:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.4f * Mathf.Pow(Mathf.Abs(0.5f + 0.2f * xIn), 1.2f);

                    zOut = zIn;
                }
                break;
            case 6:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.05f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + 0.1f * Mathf.Sign(Mathf.Sin(0.5f * xIn - 1.5f));

                    zOut = zIn;
                }
                break;
            case 7:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 0.5f));

                    zOut = zIn;
                }
                break;
            case 8:
                {
                    float amplitude2 = 3;

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 2.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 2f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 0.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn));

                    zOut = zIn;
                }
                break;
            case 9:
                {
                    float amplitude2 = 3;

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(2 * xIn - 2.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(4 * xIn - 2f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 0.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn));

                    zOut = zIn;
                }
                break;
            case 10:
                {
                    float amplitude2 = 2.5f;

                    xOut = amplitude2 * xIn * 0.4f; // Mathf.Cos(radius);
                    yOut = amplitude2 * 0.8f * Mathf.Sin(-1.6f + xIn);

                    zOut = zIn;
                }
                break;
            case 11:
                {
                    float amplitude2 = 2.5f;

                    xOut = amplitude2 * xIn * 0.4f; // Mathf.Cos(radius);
                    yOut = amplitude2 * 0.05f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + 0.05f * Mathf.Sign(Mathf.Sin(4 * xIn));

                    zOut = zIn;
                }
                break;
            case 12:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * xIn * 0.4f;
                    yOut = 0.6f + amplitude2 * 0.2f * Mathf.Cos(xIn / 8) * Mathf.Sin(-1.6f + xIn / 4);

                    zOut = zIn;
                }
                break;

        }
    }
}
