using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOutroText : MonoBehaviour
{
    private string content = "Wow!\n\n\n\n\n\n\n\n\nYou seriously bought\nALL THAT useless STUFF!?\n" +
        "And you actually spent [ACTUAL]€!\n(just jocking,\nbut be careful out there,\nother freemium will really\nget your actual money)\n\n" +
        "[GAME FINISHED]\n\n\n" +
        "Thank you for playing YAF!\nYet Another Freemium!\n" + "A Game by Alain Becam,\nGraphics by Sébastien Lesage\n"+ "Drums by William\n"+
           "Music by Chan Redfield\n and me...\n"+
           "Font GlametrixBold SudegnakNo3\nby Gluk http://www.glukfonts.pl\n"+
        "I hope you got some fun!\nGames shouldn't be\na burden to play\nnor take years to be finished!\n\nIf you liked YAF,\nfeel free to check\nour other games:\n"+
        "No unlimited In-App Purchases\nand reasonable prices.\nYou can always have the full game\nfor a low price :)\nAnd we encourage you\nto filter out games\nwith huge repeated IAP,\n"+
        "they are probably\nnot there for your fun!\n"+
        "You can use the Meta Shop\nto get recommendation\non honest and fun games!"+
        "\n\nThank you again for playing!\n";

    private static readonly Dictionary<ManageOneShopLine.Money, string> moneyInSave = new Dictionary<ManageOneShopLine.Money, string>
     {
         { ManageOneShopLine.Money.Regular, "Coins" },
            { ManageOneShopLine.Money.Actual, "Euros" },
            { ManageOneShopLine.Money.Hero, "Hero" },
            { ManageOneShopLine.Money.Diamonds, "Junk" }
     };

    const float posStart = -5.58f;
    const float posEnd = 7.85f;

    float currPos = posStart;

    Vector3 normalPos;
    // Start is called before the first frame update
    void Start()
    {
        double nbOfCoins = 0;

        string Key = moneyInSave[ManageOneShopLine.Money.Actual];

        bool hasKey = LocalSave.HasDoubleKey(Key);
        if (hasKey)
        {
            nbOfCoins = Math.Floor(100 * (10000 - LocalSave.GetDouble(Key)))/100;
        }

        content = content.Replace("[ACTUAL]", nbOfCoins + "");

        hasKey = LocalSave.HasBoolKey("MMHIWon");
        if (hasKey)
        {
            content = content.Replace("[GAME FINISHED]", "And you finished all YAF!\nCongratulations!");
        }
        else
        {
            content = content.Replace("[GAME FINISHED]", "You finished half of YAF!\nGreat job!");
        }

        GetComponent<TextMesh>().text = content;

        normalPos = transform.localPosition;

        transform.localPosition = new Vector3(normalPos.x, normalPos.y + currPos, normalPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        currPos += 0.005f;

        if (currPos > posEnd)
        {
            currPos = posStart;
        }
        transform.localPosition = new Vector3(normalPos.x, normalPos.y + currPos, normalPos.z);
    }
}
