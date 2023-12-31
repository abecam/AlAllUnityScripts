using Common.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ReceiveTokens : MonoBehaviour
{
    CodeEncodeToString ourEncoder = null;

    public ManageBox ourBox;

    string keyFromOurToken = "";

    public InputField ourInputField = null;

    public void getTokens()
    {
        string receivedContent = ourInputField.text;

        if (receivedContent.Length == 0)
        {
            return;
        }

        // Remove the pading
        receivedContent = receivedContent.Replace("*", "");

        string decompressedString = CompressString.StringCompressor.DecompressString(receivedContent);

        //string globalTxt = ourSeed + separator + ourKey + boxSeeds;

        // seed-> playerTokenEncrypted + separator + explodeAfterEncrypted + separator + nbPassEncrypted;
        string[] allStrings = decompressedString.Split(CreateCodeToSend.separator[0]);

        if (allStrings.Length > 3)
        {
            string ourDecodedKey = getKey(allStrings[3]);

            VisitorTokensAndInfo mainToken = getMainToken(allStrings[0], allStrings[1], allStrings[2], ourDecodedKey);

            // And a new token
            bool isLocalLoop =ourBox.addANewToken(mainToken);

            if (isLocalLoop)
            {
                Debug.Log("Copy/Pasted from the same player");
                return;
            }
            if (allStrings.Length > 5)
            {
                // See if there is anything left
                List<VisitorTokensAndInfo> allOthersTokens = getVisitorsToken(allStrings, ourDecodedKey);

                Debug.Log("Nb of pass : " + allOthersTokens[0].NbBeforeExplode);
                // Check if good or bad :)
                if (allOthersTokens[0].NbBeforeExplode >= CreateCodeToSend.blueTokenNb)
                {
                    // Worst box :)
                    if (Random.value > 0.7f )
                    {
                        // Blue token!
                        ourBox.addABlueToken();
                    }
                    else
                    {
                        // Red tokens
                        ourBox.addRedToken(allOthersTokens.Count);
                    }
                    // And maybe something good
                    if (Random.value > 0.7f)
                    {
                        // Super token!
                        ourBox.addSuperToken(2);
                    }
                    if (Random.value > 0.9f)
                    {
                        // Mega token!
                        ourBox.addSuperToken(5);
                    }
                }
                else if(allOthersTokens[0].NbBeforeExplode >= CreateCodeToSend.redTokenNb)
                {
                    if (Random.value > 0.5f)
                    {
                        // Red token!
                        ourBox.addRedToken(allOthersTokens.Count);
                    }
                    else
                    {
                        // Green tokens
                        ourBox.addGreenToken(allOthersTokens.Count);
                    }
                    // And maybe something good
                    if (Random.value > 0.8f)
                    {
                        // Super token!
                        ourBox.addSuperToken(1);
                    }
                }
                else if (allOthersTokens[0].NbBeforeExplode >= CreateCodeToSend.greenTokenNb)
                { 
                    {
                        // Green tokens
                        ourBox.addGreenToken(allOthersTokens.Count);
                    }
                    // And maybe something good
                    if (Random.value > 0.9f)
                    {
                        // Super token!
                        ourBox.addSuperToken(0);
                    }
                }
                else
                {
                    ourBox.addVisitors(allOthersTokens);
                }
            }
        }
        else
        {
            Debug.Log("Too few argument");
            return;
        }   
    }

    private string getKey(string receivedKey)
    {
        Debug.Log("Reading key " + receivedKey);
        return CryptFacility.Decrypt(receivedKey, CreateCodeToSend.ourPass, CreateCodeToSend.id);
    }

    private VisitorTokensAndInfo getMainToken(string playerTokenEncrypted, string explodeAfterEncrypted, string nbPassEncrypted, string ourDecodedKey)
    {
        VisitorTokensAndInfo aNewToken = new VisitorTokensAndInfo();

        // seed-> playerTokenEncrypted + separator + explodeAfterEncrypted + separator + nbPassEncrypted;
        String playerTokenDecrypted = CryptFacility.Decrypt(playerTokenEncrypted, CreateCodeToSend.ourPass, ourDecodedKey);
        aNewToken.tokenId = CompressString.StringCompressor.GetLong(playerTokenDecrypted);

        String explodeAfterDecrypted = CryptFacility.Decrypt(explodeAfterEncrypted, CreateCodeToSend.ourPass + playerTokenDecrypted, ourDecodedKey);
        aNewToken.NbBeforeExplode = (int )CompressString.StringCompressor.GetLong(explodeAfterDecrypted);

        String nbPassDecrypted = CryptFacility.Decrypt(nbPassEncrypted, CreateCodeToSend.ourPass + playerTokenDecrypted, ourDecodedKey);
        aNewToken.NbSoFar = CompressString.StringCompressor.GetLong(nbPassDecrypted);

        keyFromOurToken = explodeAfterDecrypted + playerTokenDecrypted;

        return aNewToken;
    }


    private List<VisitorTokensAndInfo> getVisitorsToken(string[] allStrings, string ourDecodedKey)
    {
        int iVisitorToken = 4; // We start at 4

        List<VisitorTokensAndInfo> newTokens = new List<VisitorTokensAndInfo>();

        for (; iVisitorToken < allStrings.Length; iVisitorToken+=3)
        {
            VisitorTokensAndInfo aNewToken = new VisitorTokensAndInfo();

            String visitorTokenDecrypted = CryptFacility.Decrypt(allStrings[iVisitorToken], CreateCodeToSend.ourPass + keyFromOurToken, ourDecodedKey);
            aNewToken.tokenId = CompressString.StringCompressor.GetLong(visitorTokenDecrypted);

            String explodeAfterDecrypted = CryptFacility.Decrypt(allStrings[iVisitorToken+1], CreateCodeToSend.ourPass + keyFromOurToken, ourDecodedKey);
            aNewToken.NbBeforeExplode = (int)CompressString.StringCompressor.GetLong(explodeAfterDecrypted);

            String nbPassDecrypted = CryptFacility.Decrypt(allStrings[iVisitorToken+2], CreateCodeToSend.ourPass + keyFromOurToken, ourDecodedKey);
            aNewToken.NbSoFar = CompressString.StringCompressor.GetLong(nbPassDecrypted);

            newTokens.Add(aNewToken);
        }
        return newTokens;
    }

    public void getPastedText()
    {
        TextEditor _textEditor = new TextEditor();

        _textEditor.Paste();

        ourInputField.text = _textEditor.text;
    }
}
