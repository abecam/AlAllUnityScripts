using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGrass : MonoBehaviour
{
    public GameObject charactersStatics;
    public Transform parentStatics;

    private List<GameObject> allStaticChars;
    private int nbOfStaticChars;

    private int currentLevel = 0;

    public bool isUnderground = false;
    public bool isUnderwater = false;

    public Color colorTo = Color.white; // Ignored if unterground or underwater!

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
            oneChar.gameObject.GetComponent<SpriteRenderer>().color = colorTo;

            allStaticChars.Add(oneChar.gameObject);
            nbOfStaticChars++;
        }

        Debug.Log("Got " + nbOfStaticChars + " objects");
    }

    private void placeCharactersOnACurve()
    {
        GameObject newChar;

        for (float z = 0; z < 8; z += 0.02f)
        {
            //float amplitude = AmplitudeFunction(z);
            {
                float x = 20 * Random.value - 10;

                float xCurr, yCurr, xNext, yNext;

                float zOut; // Ignored here
                //sliceFuntion(amplitude, radius, out xCurr, out yCurr, out xNext, out yNext);


                PlaceCharacters.applyFunction(x, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
                PlaceCharacters.applyFunction(x + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

                Vector2 from = new Vector2(xCurr, yCurr);
                Vector2 to = new Vector2(xNext, yNext);

                Vector2 normal = Vector2.Perpendicular(to - from);
                Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                //int nbOfCharHere = 1; //  ((int)(10 * Random.value));
                //for (int iChar = 0; iChar < nbOfCharHere; iChar++)
                {
                    int whichChar = (int)(nbOfStaticChars * Random.value);

                    newChar = Instantiate(allStaticChars[whichChar], new Vector3(xCurr, yCurr, -4 + z), direction, parentStatics);

                    bool flip = Random.value > 0.5f;
                    float scale = 0.2f * Random.value;

                    if (flip)
                    {
                        newChar.transform.localScale = new Vector3(-newChar.transform.localScale.x, newChar.transform.localScale.y, newChar.transform.localScale.z);
                    }
                    newChar.transform.localScale = new Vector3((0.2f + scale) * newChar.transform.localScale.x, (0.2f + scale) * newChar.transform.localScale.y, (0.2f + scale) * newChar.transform.localScale.z);
                    //Debug.Log("Added one object!" + iChar);
                    if (isUnderground)
                    {
                        newChar.GetComponent<Renderer>().material.color = new Color(0.8f + Random.value * 0.2f, 0.8f + Random.value * 0.2f, 0.5f + Random.value * 0.2f, newChar.GetComponent<Renderer>().material.color.a);
                    }
                    else if (isUnderwater)
                    {
                        newChar.GetComponent<Renderer>().material.color = new Color(0.4f + Random.value * 0.2f, 0.4f + Random.value * 0.2f, 0.8f + Random.value * 0.2f, newChar.GetComponent<Renderer>().material.color.a);
                    }
                }
            }
        }
    }
}
