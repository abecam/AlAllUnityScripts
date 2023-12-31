using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCharsToFindMN : MonoBehaviour
{
    public GameObject charactersToFind;
    public GameObject bombsRouge;
    public GameObject bombsNoir;
    public GameObject traps;
    public Transform parentTraps;
    public Transform parentBlackBombs;
    public Transform parentRedBombs;

    private List<GameObject> allStaticChars;
    private int nbOfStaticChars;
    private List<Vector2> placesTaken;

    private List<GameObject> allTraps;
    private int nbOfTraps;

    private int currentLevel = 0;
    private int currentDifficulty = 2;
    bool pieceTransparences = true;

    void Awake()
    {
        if (LocalSave.HasIntKey("MovingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("MovingNYAF_currentLevel");
        }

        bool haskey = LocalSave.HasIntKey("MovingNYAF_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("MovingNYAF_difficulty");
        }

        haskey = LocalSave.HasIntKey("MovingNYAF_transparentChars");
        if (haskey)
        {
            pieceTransparences = LocalSave.GetInt("MovingNYAF_transparentChars") == 1;
        }

        prepareStaticChars();

        placeCharactersOnACurve();

        if (currentDifficulty > 2)
        {
            PlaceBombsAndTraps();
        }
    }

    private void PlaceBombsAndTraps()
    {
        float z = 0;
        GameObject newPenalty;

        int nbTraps = 50;
        int nbBlackBombs = 0;
        int nbRedBombs = 0;

        switch (currentDifficulty)
        {
            case 3:
                nbTraps = 0;
                nbBlackBombs = 5;
                nbRedBombs = 0;
                break;
            case 4:
                nbTraps = 0;
                nbBlackBombs = 10;
                nbRedBombs = 5;
                break;
            case 5:
                nbTraps = 0;
                nbBlackBombs = 20;
                nbRedBombs = 10;
                break;
            case 6:
                nbTraps = 25;
                nbBlackBombs = 40;
                nbRedBombs = 20;
                break;
            case 7:
                nbTraps = 50;
                nbBlackBombs = 80;
                nbRedBombs = 40;
                break;
        }

        for (int iBlackBomb = 0; iBlackBomb < nbBlackBombs; iBlackBomb++)
        {
            float radius = (Random.value * 8 * Mathf.PI) - (4 - Mathf.PI);
            z = Random.value * 8;
            {
                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                            //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);

                bool success = true;

                Vector2 ourPlaceIn2D = new Vector2(radius, z);
                foreach (Vector2 onePlaced in placesTaken)
                {
                    ///Debug.Log("Distance to oneBefore " + (ourPlaceIn2D - onePlaced).magnitude);

                    if ((ourPlaceIn2D - onePlaced).magnitude < 1)
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                {
                    PlaceBackgrCharMN.applyFunction(radius, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
                    PlaceBackgrCharMN.applyFunction(radius + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

                    Vector2 from = new Vector2(xCurr, yCurr);
                    Vector2 to = new Vector2(xNext, yNext);

                    Vector2 normal = Vector2.Perpendicular(to - from);
                    Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                    {
                        newPenalty = Instantiate(bombsNoir, new Vector3(xCurr, yCurr, zOut), direction, parentBlackBombs);

                        bool flip = Random.value > 0.5f;
                        float scale = 0.7f * Random.value;

                        if (flip)
                        {
                            newPenalty.transform.localScale = new Vector3(-newPenalty.transform.localScale.x, newPenalty.transform.localScale.y, newPenalty.transform.localScale.z);
                        }
                        newPenalty.transform.localScale = new Vector3((0.5f + scale) * newPenalty.transform.localScale.x, (0.5f + scale) * newPenalty.transform.localScale.y, (0.5f + scale) * newPenalty.transform.localScale.z);

                        if (pieceTransparences)
                        {
                            newPenalty.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.8f);
                        }
                        else
                        {
                            newPenalty.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f);
                        }
                        newPenalty.gameObject.AddComponent<MoveAroundInScene>();
                        newPenalty.gameObject.GetComponent<MoveAroundInScene>().areFindable = true;
                        newPenalty.gameObject.GetComponent<MoveAroundInScene>().setInitialPos(radius / 8);
                    }
                }
            }
        }

        for (int iBlackBomb = 0; iBlackBomb < nbRedBombs; iBlackBomb++)
        {
            float radius = (Random.value * 8 * Mathf.PI) - (4 - Mathf.PI);
            z = Random.value * 8;
            {
                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                            //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);

                bool success = true;

                Vector2 ourPlaceIn2D = new Vector2(radius, z);
                foreach (Vector2 onePlaced in placesTaken)
                {
                    ///Debug.Log("Distance to oneBefore " + (ourPlaceIn2D - onePlaced).magnitude);

                    if ((ourPlaceIn2D - onePlaced).magnitude < 1)
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                {
                    PlaceBackgrCharMN.applyFunction(radius, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
                    PlaceBackgrCharMN.applyFunction(radius + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

                    Vector2 from = new Vector2(xCurr, yCurr);
                    Vector2 to = new Vector2(xNext, yNext);

                    Vector2 normal = Vector2.Perpendicular(to - from);
                    Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                    {
                        newPenalty = Instantiate(bombsRouge, new Vector3(xCurr, yCurr, zOut), direction, parentRedBombs);

                        bool flip = Random.value > 0.5f;
                        float scale = 0.7f * Random.value;

                        if (flip)
                        {
                            newPenalty.transform.localScale = new Vector3(-newPenalty.transform.localScale.x, newPenalty.transform.localScale.y, newPenalty.transform.localScale.z);
                        }
                        newPenalty.transform.localScale = new Vector3((0.5f + scale) * newPenalty.transform.localScale.x, (0.5f + scale) * newPenalty.transform.localScale.y, (0.5f + scale) * newPenalty.transform.localScale.z);

                        if (pieceTransparences)
                        {
                            newPenalty.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.8f);
                        }
                        else
                        {
                            newPenalty.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f);
                        }
                        newPenalty.gameObject.AddComponent<MoveAroundInScene>();
                        newPenalty.gameObject.GetComponent<MoveAroundInScene>().areFindable = true;
                        newPenalty.gameObject.GetComponent<MoveAroundInScene>().setInitialPos(radius / 8);
                    }
                }
            }
        }

        for (int iTraps = 0; iTraps < nbTraps; iTraps++)
        {
            //float amplitude = AmplitudeFunction(z);
            float shiftX = Random.value;

            float radius = (Random.value * 8 * Mathf.PI) - (4 - Mathf.PI);
            z = Random.value * 8;
            {
                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                            //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);

                bool success = true;

                Vector2 ourPlaceIn2D = new Vector2(radius, z);
                foreach (Vector2 onePlaced in placesTaken)
                {
                    ///Debug.Log("Distance to oneBefore " + (ourPlaceIn2D - onePlaced).magnitude);

                    if ((ourPlaceIn2D - onePlaced).magnitude < 1)
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                {
                    PlaceBackgrCharMN.applyFunction(radius, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
                    PlaceBackgrCharMN.applyFunction(radius + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

                    Vector2 from = new Vector2(xCurr, yCurr);
                    Vector2 to = new Vector2(xNext, yNext);

                    Vector2 normal = Vector2.Perpendicular(to - from);
                    Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                    int whichChar = (int)(nbOfTraps * Random.value);

                    newPenalty = Instantiate(allTraps[whichChar], new Vector3(xCurr, yCurr, zOut), direction, parentTraps);

                    bool flip = Random.value > 0.5f;
                    float scale = 0.7f * Random.value;

                    if (flip)
                    {
                        newPenalty.transform.localScale = new Vector3(-newPenalty.transform.localScale.x, newPenalty.transform.localScale.y, newPenalty.transform.localScale.z);
                    }
                    newPenalty.transform.localScale = new Vector3((0.5f + scale) * newPenalty.transform.localScale.x, (0.5f + scale) * newPenalty.transform.localScale.y, (0.5f + scale) * newPenalty.transform.localScale.z);

                    if (pieceTransparences)
                    {
                        newPenalty.GetComponent<Renderer>().material.color = new Color(1, 0.6f, 0.6f, 0.8f);
                    }
                    else
                    {
                        newPenalty.GetComponent<Renderer>().material.color = new Color(1, 0.6f, 0.6f, 1f);
                    }
                    newPenalty.gameObject.AddComponent<MoveAroundInScene>();
                    newPenalty.gameObject.GetComponent<MoveAroundInScene>().areFindable = true;
                    newPenalty.gameObject.GetComponent<MoveAroundInScene>().setInitialPos(radius / 8);

                    //Debug.Log("Added one object!" + iChar);
                }
            }
        }
    }

    private void prepareStaticChars()
    {
        nbOfStaticChars = 0;
        allStaticChars = new List<GameObject>();

        foreach (Transform oneChar in charactersToFind.transform)
        {
            allStaticChars.Add(oneChar.gameObject);
            nbOfStaticChars++;
        }

        //Debug.Log("Got " + nbOfStaticChars + " objects");

        nbOfTraps = 0;
        allTraps = new List<GameObject>();

        foreach (Transform oneChar in traps.transform)
        {
            allTraps.Add(oneChar.gameObject);
            nbOfTraps++;
        }
    }

    private void placeCharactersOnACurve()
    {
        placesTaken = new List<Vector2>();

        float sizeFromDiff = 1;

        // Change character size depending on the difficulty
        switch (currentDifficulty)
        {
            case 0:
                sizeFromDiff = 2;
                break;
            case 1:
                sizeFromDiff = 1.5f;
                break;
            case 2:
                sizeFromDiff = 1;
                break;
            case 3:
                sizeFromDiff = 0.9f;
                break;
            case 4:
                sizeFromDiff = 0.8f;
                break;
            case 5:
                sizeFromDiff = 0.7f;
                break;
            case 6:
                sizeFromDiff = 0.7f;
                break;
            case 7:
                sizeFromDiff = 0.7f;
                break;
        }

        foreach (Transform oneCharToFind in charactersToFind.transform)
        {
            //float amplitude = AmplitudeFunction(z);
            {
                bool success = false;
                int iTrials = 0;

                float area;
                float z = 0;
                float x = 0;

                // z might not be always on the full area, as some parts are hidden!
                while (!success && iTrials++ < 100)
                {
                    area = Random.value;
                    z = 8*Random.value;

                    x = 16 * Random.value - 8;

                    Vector2 ourPlaceIn2D = new Vector2(x, z);

                    success = true;

                    // now check the distance to all other already placed
                    foreach (Vector2 onePlaced in placesTaken)
                    {
                        ///Debug.Log("Distance to oneBefore " + (ourPlaceIn2D - onePlaced).magnitude);

                        if ((ourPlaceIn2D - onePlaced).magnitude < 1)
                        {
                            success = false;
                            break;
                        }
                    }
                }

                if (iTrials >= 100)
                {
                    Debug.Log("ZZZZZZZZZZZZZZZZZZ Did not find a free place!!!!");
                }

                {

                    float xCurr, yCurr, xNext, yNext;

                    float zOut; // Ignored here
                                //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);


                    PlaceBackgrCharMN.applyFunction(x, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
                    PlaceBackgrCharMN.applyFunction(x + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

                    Vector2 from = new Vector2(xCurr, yCurr);
                    Vector2 to = new Vector2(xNext, yNext);

                    Vector2 normal = Vector2.Perpendicular(to - from);
                    Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                    oneCharToFind.localPosition = new Vector3(xCurr, yCurr, zOut); //  - 4 + z);
                    oneCharToFind.localRotation = direction;
                    oneCharToFind.localScale = new Vector3(oneCharToFind.localScale.x * sizeFromDiff, oneCharToFind.localScale.y * sizeFromDiff, oneCharToFind.localScale.z * sizeFromDiff);
                    oneCharToFind.gameObject.AddComponent<MoveAroundInScene>();
                    oneCharToFind.gameObject.GetComponent<MoveAroundInScene>().areFindable = true;
                    oneCharToFind.gameObject.GetComponent<MoveAroundInScene>().setInitialPos(x/8);

                    if (pieceTransparences)
                    {
                        oneCharToFind.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.9f);
                    }

                    placesTaken.Add(new Vector2(x, z));
                }
            }
        }
    }
}
