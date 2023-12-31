using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalScript : MonoBehaviour
{
    public enum typeOfPortal
    {
        local, global, special
    }

    public enum typeOfScene
    {
        groundNormal, spaceNormal, asteroidField, underground, undersea, oversea, swamp, halloween
    }

    public int nbOfCharsNeeded = 50;
    public TextMesh ourText;
    public typeOfPortal ofType = typeOfPortal.local;
    public typeOfScene nextSceneType = typeOfScene.groundNormal;
    public bool followGround = false;

    public int nbOfFunctionNextScene = 0;
    private int nbOfCharFoundTotal = 0;
    private bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject);
        // Check if we are unlocked 
        bool haskey = LocalSave.HasIntKey("Nyaf3dallCharFound");
        if (haskey)
        {
            nbOfCharFoundTotal = LocalSave.GetInt("Nyaf3dallCharFound");
        }
        if (nbOfCharFoundTotal >= nbOfCharsNeeded)
        {
            isActivated = true;
            ourText.text = "";
        }
        else
        {
            ourText.text = nbOfCharsNeeded - nbOfCharFoundTotal + "";
        }
        if (followGround)
        {
            int currentLevel = 0;
            // Only in FindingNyaf levels
            if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
            {
                currentLevel = LocalSave.GetInt("FindingNYAF_currentLevel");
            }

            float x = transform.position.x;
            float z = transform.position.z;

            float xCurr, yCurr, xNext, yNext;

            float zOut; // Ignored here

            PlaceCharacters.applyFunction(x, 0, z, out xCurr, out yCurr, out zOut, currentLevel);
            PlaceCharacters.applyFunction(x + 0.04f * Mathf.PI, 0, z, out xNext, out yNext, out zOut, currentLevel);

            Vector2 from = new Vector2(xCurr, yCurr);
            Vector2 to = new Vector2(xNext, yNext);

            Vector2 normal = Vector2.Perpendicular(to - from);
            Quaternion direction = Quaternion.FromToRotation(Vector3.up, normal);

            transform.position = new Vector3(xCurr, yCurr, -4 + z);
            transform.rotation = direction;
        }
    }

    int nbCheck = 0;

    // Update is called once per frame
    void Update()
    {
        if (nbCheck++ % 60 == 0)
        {
            bool haskey = LocalSave.HasIntKey("Nyaf3dallCharFound");
            if (haskey)
            {
                nbOfCharFoundTotal = LocalSave.GetInt("Nyaf3dallCharFound");
            }
            if (nbOfCharFoundTotal >= nbOfCharsNeeded)
            {
                isActivated = true;
                ourText.text = "";
            }
            else
            {
                ourText.text = nbOfCharsNeeded - nbOfCharFoundTotal + "";
            }
        }
        if (isActivated)
        {
            bool aTouch = Mouse.current.leftButton.wasPressedThisFrame;
            //bool rightClick = Input.GetMouseButton(1);

            if (aTouch)
            {
                Vector3 touchPos = Mouse.current.position.ReadValue();

                Ray ray = Camera.main.ScreenPointToRay(touchPos);
                RaycastHit hit;

                // Check if we found the object
                if (Physics.Raycast(ray, out hit, 100))
                {
                    //Debug.Log("We hit " + hit.transform.gameObject.name);

                    if (this.transform == hit.transform)
                    {
                        goToNextLevel();
                    }
                }
            }
        }
    }

    private void goToNextLevel()
    {
        switch (nextSceneType)
        {
            case typeOfScene.groundNormal:
                LocalSave.SetInt("FindingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/MainScene");
                break;
            case typeOfScene.spaceNormal:
                LocalSave.SetInt("MovingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/MovingNyafings");
                break;
            case typeOfScene.asteroidField:
                LocalSave.SetInt("MovingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/AsteroidsField");
                break;
            case typeOfScene.underground:
                LocalSave.SetInt("FindingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Underground");
                break;
            case typeOfScene.undersea:
                LocalSave.SetInt("FindingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Undersea2");
                break;
            case typeOfScene.oversea:
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Sea");
                break;
            case typeOfScene.swamp:
                LocalSave.SetInt("FindingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Swamp");
                break;
            case typeOfScene.halloween:
                LocalSave.SetInt("FindingNYAF_currentLevel", nbOfFunctionNextScene);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Halloween");
                break;
        }
    }
}
