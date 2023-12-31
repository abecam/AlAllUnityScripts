using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetNbOfCoins : MonoBehaviour
{
    private Text outText;

    public enum Money
    {
        Regular,
        Actual,
        Hero,
        Diamonds
    }

    private static readonly Dictionary<Money, string> moneyInSave = new Dictionary<Money, string>
     {
         { Money.Regular, "Coins" },
            { Money.Actual, "Euros" },
            { Money.Hero, "Hero" },
            { Money.Diamonds, "Junk" }
     };

    public Money usedMoney;

    // Start is called before the first frame update
    void Start()
    {
        outText = transform.GetComponent<Text>();

        double nbOfCoins = 0;

        string Key = moneyInSave[usedMoney];

        bool hasKey = LocalSave.HasDoubleKey(Key);
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetDouble(Key);
        }

        string nbOfCoinsAbbr = YAFFacility.AbbreviateNumber(nbOfCoins);

        outText.text = "x" + nbOfCoinsAbbr;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNewNbOfCoin(double nbOfCoins)
    {
        string nbOfCoinsAbbr = YAFFacility.AbbreviateNumber(nbOfCoins);

        outText.text = "x" + nbOfCoinsAbbr;
    }
}
