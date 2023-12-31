using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageToken : MonoBehaviour
{
    public GameObject plasticToken;
    public GameObject bronzeToken;
    public GameObject silverToken;
    public GameObject goldToken;
    public GameObject platiniumToken;
    public GameObject emeraldToken;
    public GameObject rubyToken;
    public GameObject saphireToken;
    public GameObject diamondToken;
    public GameObject plasmaToken;
    public GameObject transcendentalToken;

    public GameObject tokenContainer;

    private GameObject currentToken;

    public TextMesh ourNbSoFarText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool dragMode = false;
    bool ourTokenSelected = false;

    // Update is called once per frame
    void Update()
    {
       
    }

    public void setNewNbSoFar(long nbSoFar)
    {
        foreach (Transform child in tokenContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (nbSoFar < 0)
        {
            if (currentToken != null)
            {
                Destroy(currentToken);
            }
        }
        else if (nbSoFar < 10)
        {
            currentToken = Instantiate(plasticToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_token);
        }
        else if (nbSoFar < 20)
        {
            currentToken = Instantiate(bronzeToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_bronze_token);
        }
        else if (nbSoFar < 50)
        {
            currentToken = Instantiate(silverToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_silver_token);
        }
        else if (nbSoFar < 100)
        {
            currentToken = Instantiate(goldToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_golden_token);
        }
        else if (nbSoFar < 500)
        {
            currentToken = Instantiate(platiniumToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_platinium_token);
        }
        else if (nbSoFar < 1000)
        {
            currentToken = Instantiate(emeraldToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_emerald_token);
        }
        else if (nbSoFar < 2000)
        {
            currentToken = Instantiate(rubyToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_ruby_token);
        }
        else if (nbSoFar < 4000)
        {
            currentToken = Instantiate(saphireToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_sapphire_token);
        }
        else if (nbSoFar < 8000)
        {
            currentToken = Instantiate(diamondToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_diamond_token);
        }
        else if (nbSoFar < 150000)
        {
            currentToken = Instantiate(plasmaToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_first_plasma_token);
        }
        else
        {
            currentToken = Instantiate(transcendentalToken);
            THGE.GameLogic.GameManager.Instance.UnlockAchievement(GPGSIds.achievement_what_is_that);
        }

        currentToken.transform.SetParent(tokenContainer.transform);

        currentToken.transform.localScale = new Vector3(1, 1, 1);
        currentToken.transform.localPosition = new Vector3(0, 0, 0);

        ourNbSoFarText.text = nbSoFar+" jumps";
    }

    internal bool checkIfSelected(Vector2 touchPos)
    {
        if (currentToken.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
        {
            return true;
        }
        return false;
    }
}
