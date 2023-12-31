using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetNbOfCoins : MonoBehaviour
{
    private Text outText;

    // Start is called before the first frame update
    void Start()
    {
        outText = transform.GetComponent<Text>();

        int nbOfCoins = 0;

        bool hasKey = LocalSave.HasIntKey("Coins");
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetInt("Coins");
        }

        outText.text = "x" + nbOfCoins;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNewNbOfCoin(int nbOfCoins)
    {
        // Dogs might find characters, so coins, before the game is started, thus the nb of coins shown.
        if (outText != null)
        { 
            outText.text = "x" + nbOfCoins;
        }
    }
}
