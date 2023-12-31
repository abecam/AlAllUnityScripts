using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharIntroScript : MonoBehaviour
{
    private const string rootNameOfLevel = "MainBoard";

    public GameObject charactersStatics;
    public Transform parentStatics;

    private List<GameObject> allStaticChars;
    private int nbOfStaticChars;

    public GameObject title;
    public GameObject canvasExplications;

    // Start is called before the first frame update
    void Start()
    {
        // Check if it is the first time or not, if not, go to the last level played.
        checkIfContinueOrFirstStart();

        // Otherwise place and animate the characters.
        prepareStaticChars();

        placeCharactersOnACurve();
    }

    private void checkIfContinueOrFirstStart()
    {
        // See where to send the player
        if (LocalSave.HasIntKey("currentLevel"))
        {
            int lastLevel = LocalSave.GetInt("currentLevel");

            // And load the level
            if (lastLevel == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + lastLevel);
            }
        }
        else
        {
            title.SetActive(true);
            canvasExplications.SetActive(true);

            return; // No level saved, so the game was never started
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        float z = 0;

        for (int iLayer = 0; iLayer < 30; iLayer++)
        {
            //Debug.Log("Layer  " + iLayer);

            float amplitude = 3 + 1 * Mathf.Cos(z) * Mathf.Sin(0.2f * z);

            for (float radius = 0; radius < 2 * Mathf.PI; radius += 0.02f * Mathf.PI)
            {
                float xCurr = amplitude * Mathf.Cos(radius);
                float yCurr = amplitude * Mathf.Sin(radius);

                float xNext = amplitude * Mathf.Cos(radius + 0.02f * Mathf.PI);
                float yNext = amplitude * Mathf.Sin(radius + 0.02f * Mathf.PI);
                Vector2 from = new Vector2(xCurr, yCurr);
                Vector2 to = new Vector2(xNext, yNext);

                Vector2 normal = Vector2.Perpendicular(to - from);
                Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

                int nbOfCharHere = 1; //  ((int)(10 * Random.value));
                for (int iChar = 0; iChar < nbOfCharHere; iChar++)
                {
                    float xShift = 0; // (0.2f * Random.value) - 0.1f;
                    float yShift = 0; // (0.2f * Random.value) - 0.1f;

                    int whichChar = (int)(nbOfStaticChars * Random.value);

                    Instantiate(allStaticChars[whichChar], new Vector3(xShift + xCurr, yShift + yCurr, 20 + z), direction, parentStatics);

                    //Debug.Log("Added one object!" + iChar);
                }
            }
            z += 0.08f;
        }
    }
}
