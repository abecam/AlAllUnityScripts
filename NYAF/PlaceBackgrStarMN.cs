using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBackgrStarMN : MonoBehaviour
{
    public GameObject charactersStatics;
    public Transform parentStatics;

    private List<GameObject> allStaticChars;
    private int nbOfStaticChars;

    public bool areAsteroid = false;
    public float shiftZ = 0;

    // Start is called before the first frame update
    void Start()
    {
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

    private void placeCharactersOnACurve()
    {
        GameObject newChar;

        for (int iStar = 0; iStar < 300; iStar++)
        {
            //float amplitude = AmplitudeFunction(z);
            {
                float x = 60 * Random.value - 30;
                float y = 60 * Random.value - 30;
                float z = 40 * Random.value - 20;

                Quaternion direction = Quaternion.AngleAxis(Random.value * 180, Vector3.forward);

                int nbOfCharHere = 1; //  ((int)(10 * Random.value));
                for (int iChar = 0; iChar < nbOfCharHere; iChar++)
                {
                    int whichChar = (int)(nbOfStaticChars * Random.value);

                    newChar = Instantiate(allStaticChars[whichChar], new Vector3(x, y, z+ shiftZ), direction, parentStatics);
                    if (areAsteroid)
                    {
                        // Add the script moving the asteroid around.
                        newChar.AddComponent<MoveAsteroids>();
                    }

                    bool flip = Random.value > 0.5f;
                    float scale = 0.3f * Random.value;

                    if (flip)
                    {
                        newChar.transform.localScale = new Vector3(-newChar.transform.localScale.x, newChar.transform.localScale.y, newChar.transform.localScale.z);
                    }
                    newChar.transform.localScale = new Vector3((0.02f + scale) * newChar.transform.localScale.x, (0.02f + scale) * newChar.transform.localScale.y, (0.02f + scale) * newChar.transform.localScale.z);
                    //Debug.Log("Added one object!" + iChar);
                }
            }
        }
    }
}
