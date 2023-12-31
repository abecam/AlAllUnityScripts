using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNbCoins : MonoBehaviour
{
    private TextMesh ourText;

    public ManageOneShopLine.Money usedMoney;

    private static readonly Dictionary<ManageOneShopLine.Money, string> moneyInSave = new Dictionary<ManageOneShopLine.Money, string>
     {
         { ManageOneShopLine.Money.Regular, "Coins" },
            { ManageOneShopLine.Money.Actual, "Euros" },
            { ManageOneShopLine.Money.Hero, "Hero" },
            { ManageOneShopLine.Money.Diamonds, "Junk" }
     };

    private static readonly Dictionary<ManageOneShopLine.Money, string> moneyAbbr = new Dictionary<ManageOneShopLine.Money, string>
     {
         { ManageOneShopLine.Money.Regular, "Coins" },
            { ManageOneShopLine.Money.Actual, "€" },
            { ManageOneShopLine.Money.Hero, "SCoins" },
            { ManageOneShopLine.Money.Diamonds, "Diamonds" }
     };

    private double nbOfCoins = 0;
    // Start is called before the first frame update
    void Start()
    {
        ourText = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        string Key = moneyInSave[usedMoney];

        bool hasKey = LocalSave.HasDoubleKey(Key);
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetDouble(Key);
        }
        string abbrvCurrency = moneyAbbr[usedMoney];

        string priceAbbr = YAFFacility.AbbreviateNumber(nbOfCoins);

        ourText.text = priceAbbr + " " + abbrvCurrency;
    }
}
