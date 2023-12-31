using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBackgrCharMN : MonoBehaviour
{
    public GameObject charactersStatics;
    public Transform parentStatics;

    private List<GameObject> allStaticChars;
    private int nbOfStaticChars;

    private int currentLevel = 0;

    void Start()
    {
        if (LocalSave.HasIntKey("MovingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("MovingNYAF_currentLevel");
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
    private void placeCharactersOnACurveOnlyOnce()
    {
        foreach (GameObject oneCharToFind in allStaticChars)
        {
            //float amplitude = AmplitudeFunction(z);
            {
                // z might not be always on the full area, as some parts are hidden!
                float area = Random.value;
                float z = Random.value;

                if (area > 0.5f)
                {
                    z = 6.5f + 1.5f * z;
                }
                else
                {
                    z = 3.6f * z;
                }

                float x = 6 * Random.value - 3;

                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);


                applyFunction(x, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
                applyFunction(x + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

                Vector2 from = new Vector2(xCurr, yCurr);
                Vector2 to = new Vector2(xNext, yNext);

                Vector2 normal = Vector2.Perpendicular(to - from);
                Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);
                GameObject newChar = Instantiate(oneCharToFind, new Vector3(xCurr, yCurr, -z), direction, parentStatics);

                newChar.transform.localPosition = new Vector3(xCurr, yCurr, z);
                newChar.transform.localRotation = direction;
            }
        }
    }

    private void placeCharactersOnACurve()
    {
        float z = 0;
        GameObject newChar;

        for (int iLayer = 0; iLayer < 50; iLayer++)
        {
            //float amplitude = AmplitudeFunction(z);

            for (float radius = -4 * Mathf.PI; radius < 4 * Mathf.PI; radius += 0.2f * Mathf.PI)
            {
                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                            //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);

                float xShift = 0; //  (0.2f * Random.value) - 0.1f;
                float zShift = 0; // 0.4f * Mathf.Cos(radius + z); // (0.2f * Random.value) - 0.1f;

                applyFunction(radius + xShift, 0, z + zShift, out xCurr, out yCurr, out zOut, currentLevel);
                applyFunction(radius + xShift + 0.04f * Mathf.PI, 0, z + zShift, out xNext, out yNext, out zOut, currentLevel);

                Vector2 from = new Vector2(xCurr, yCurr);
                Vector2 to = new Vector2(xNext, yNext);

                Vector2 normal = Vector2.Perpendicular(to - from);
                Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                int nbOfCharHere = 1; //  ((int)(10 * Random.value));
                for (int iChar = 0; iChar < nbOfCharHere; iChar++)
                {


                    int whichChar = (int)(nbOfStaticChars * Random.value);

                    newChar = Instantiate(allStaticChars[whichChar], new Vector3(xCurr, yCurr, zOut), direction, parentStatics);
                    newChar.AddComponent<MoveAroundInScene>();
                    newChar.gameObject.GetComponent<MoveAroundInScene>().setInitialPos(radius);

                    bool flip = Random.value > 0.5f;
                    float scale = 0.7f * Random.value;

                    if (flip)
                    {
                        newChar.transform.localScale = new Vector3(-newChar.transform.localScale.x, newChar.transform.localScale.y, newChar.transform.localScale.z);
                    }
                    newChar.transform.localScale = new Vector3((0.5f + scale) * newChar.transform.localScale.x, (0.5f + scale) * newChar.transform.localScale.y, (0.5f + scale) * newChar.transform.localScale.z);
                    //Debug.Log("Added one object!" + iChar);
                }
            }
            z += 0.16f;
        }
    }

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
                    float amplitude2 = 3 + 1 * Mathf.Cos(zIn) * Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = amplitude2 * 0.4f * Mathf.Sin(-1.6f + xIn / 4);

                    zOut = zIn;
                }
                break;
            case 1:
                {
                    float amplitude2 = 2.5f;

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = amplitude2 * 0.5f * Mathf.Sign(Mathf.Sin(xIn - 1.5f));

                    zOut = zIn - 4;
                }
                break;
            case 2:
                {
                    float amplitude2 = 3 + 1 * Mathf.Cos(zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = amplitude2 * 0.4f * Mathf.Sin(-1.6f + xIn / 4);

                    zOut = zIn;
                }
                break;
            case 3:
                {
                    float amplitude2 = 4 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = amplitude2 * 0.4f * Mathf.Cos(xIn / 8) * Mathf.Sin(-1.6f + xIn / 4);

                    zOut = zIn;
                }
                break;
            case 4:
                {
                    float amplitude2 = 5 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = -3 + amplitude2 * 0.2f * Mathf.Sqrt(Mathf.Abs(2f + xIn));

                    zOut = zIn - 2;
                }
                break;
            case 5:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = -3 + amplitude2 * 0.4f * Mathf.Pow(Mathf.Abs(0.5f + 0.2f * xIn), 1.2f);

                    zOut = zIn - 2;
                }
                break;
            case 6:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = -2 + amplitude2 * 0.05f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + 0.1f * Mathf.Sign(Mathf.Sin(0.5f * xIn - 1.5f));

                    zOut = zIn;
                }
                break;
            case 7:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 0.5f));

                    zOut = zIn;
                }
                break;
            case 8:
                {
                    float amplitude2 = 3;

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 2.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 2f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 1f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn - 0.5f)) + amplitude2 * 0.03f * Mathf.Sign(Mathf.Sin(xIn));

                    zOut = zIn;
                }
                break;
            case 9:
                {
                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = -3 + amplitude2 * 0.4f * Mathf.Pow(Mathf.Abs(0.5f + 0.2f * xIn), 1.2f);

                    zOut = zIn - 2;
                }
                break;
            case 10:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = (1*Mathf.Sin(xIn / 4));
                    yOut = (1*Mathf.Cos(xIn / 4));
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = 4;
                }
                break;
            case 11:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = (1 * Mathf.Sin(xIn / 4));
                    yOut = (1 * Mathf.Cos(xIn / 4));
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = 4;
                }
                break;
            case 12:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = (1 * Mathf.Sin(xIn / 4));
                    yOut = (1 * Mathf.Cos(xIn / 4));
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = 4;
                }
                break;
            case 13:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = (1 * Mathf.Sin(xIn / 4));
                    yOut = (1 * Mathf.Cos(xIn / 4));
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = 4;
                }
                break;
            case 14:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = -1 + 3 * Mathf.Sin(0.4f * zIn);

                    float amplitude2 = 3 + Mathf.Sin(0.4f * zIn);

                    xOut = amplitude2 * 0.4f * Mathf.Cos(-1.6f + xIn / 4);
                    yOut = -2 + amplitude2 * 0.4f * Mathf.Pow(Mathf.Abs(0.5f + 0.2f * xIn), 1.2f);
                }
                break;
            case 15:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = zIn - 4;
                    yOut = -1;
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = xIn * 2.1f;
                }
                break;
            case 16:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = (1 * Mathf.Sin(2 * Mathf.PI * zIn / 8));
                    yOut = (1 * Mathf.Cos(2 * Mathf.PI * zIn / 8));
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = xIn * 2.1f;
                }
                break;
            case 17:
                {
                    //float xIn2 = Random.value * 2 * Mathf.PI;

                    xOut = (1 * Mathf.Sin((xIn / 4) + (zIn - 4) / 2));
                    yOut = (1 * Mathf.Cos((xIn / 4) + (zIn - 4) / 2));
                    //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]

                    zOut = xIn * 2.1f;
                }
                break;
        }
    }
}
