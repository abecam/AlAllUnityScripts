using System.Collections;
using Common.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CreateCodeToSend
{

    static public string ourPass = "3whoiuhoi9869t98UFZ";
    static public string id = "SALTEDCHICKEN";

    string keyFromOurToken = "";

    public const string separator = "*"; // Not part of Base64!
    public const int greenTokenNb = 10000;
    public const int redTokenNb = 100000;
    public const int blueTokenNb = 1000000;

    CodeEncodeToString ourEncoder = null;

    private ManageBox ourBox;

    public CreateCodeToSend(ManageBox ourBox)
    {
        this.ourBox = ourBox;
    }

    internal string getFullTokens()
    {
        ourEncoder = new CodeEncodeToString();

        string ourKey = getKey();

        string ourSeed = getSeedTxt(ourKey);

        string boxSeeds = getBoxSeedsTxt(ourKey, false, false, false);

        string ourKeyCrypted = CryptFacility.Encrypt(ourKey, ourPass, id);

        string globalTxt = ourSeed + separator + ourKeyCrypted + boxSeeds;

        Debug.Log("Would send (before compression) " + globalTxt + "(size " + globalTxt.Length + ")");

        string compressTxt = CompressString.StringCompressor.CompressString(globalTxt);
        return "****"+compressTxt+ "****";
    }

    internal string getBadTokens(bool isBlue)
    {
        ourEncoder = new CodeEncodeToString();

        string ourKey = getKey();

        string ourSeed = getSeedTxt(ourKey);

        string boxSeeds = getBoxSeedsTxt(ourKey,true,true, isBlue);

        string ourKeyCrypted = CryptFacility.Encrypt(ourKey, ourPass, id);

        string globalTxt = ourSeed + separator + ourKeyCrypted + boxSeeds;

        Debug.Log("Would send (before compression) " + globalTxt + "(size " + globalTxt.Length + ")");

        string compressTxt = CompressString.StringCompressor.CompressString(globalTxt);
        return "****"+compressTxt+ "****";
    }

    internal string getBothTokens()
    {
        string goodTokens = getFullTokens();

        // Now the bad ones, with more chance of stronger tokens.
        string badTokens = getBadTokens(true);

        string stringToReturn = "Here goes 2 codes: with good tokens or with bad tokens... In which order? Noone knows. Choose wisely!\n\n";

        // Now choose randomly which one goes first
        if (Random.value > 0.5f)
        {
            stringToReturn += badTokens + "\n\n-----------------------------\n" + goodTokens;
        }
        else
        {
            stringToReturn += goodTokens + "\n\n-----------------------------\n" + badTokens;
        }
        return stringToReturn;
    }

    private string getKey()
    {
        long saltNb = LocalSave.GetLong(ManageBox.DateOfFirstPlay) * 4;

        int saltInt = (int)saltNb;

        String ourKey = ourEncoder.ConvertToBase(saltInt) + CompressString.StringCompressor.longToString(saltNb);

        return ourKey;
    }

    private string getSeedTxt(string ourKey)
    {
        string result = "";

        long playerTokenId = ourBox.PlayerToken;
        string playerTokenTxt = CompressString.StringCompressor.longToString(playerTokenId);
        String playerTokenEncrypted = CryptFacility.Encrypt(playerTokenTxt, ourPass, ourKey);

        long explodeAfter = ourBox.ExplodeAfter;
        string explodeAfterTxt = CompressString.StringCompressor.longToString(explodeAfter);
        String explodeAfterEncrypted = CryptFacility.Encrypt(explodeAfterTxt, ourPass + playerTokenTxt, ourKey);

        long nbPassed = 0;
        string nbPassedTxt = CompressString.StringCompressor.longToString(nbPassed);

        String nbPassEncrypted = CryptFacility.Encrypt(nbPassedTxt, ourPass + playerTokenTxt, ourKey);
        String nbPassDecrypted = CryptFacility.Decrypt(nbPassEncrypted, ourPass + playerTokenTxt, ourKey);
        Debug.Log("Nb of passes are" + (int)CompressString.StringCompressor.GetLong(nbPassDecrypted));

        keyFromOurToken = explodeAfterTxt + playerTokenTxt;

        result = playerTokenEncrypted + separator + explodeAfterEncrypted + separator + nbPassEncrypted;

        return result;
    }

    private string getBoxSeedsTxt(string ourKey, bool greenOnes, bool redOnes, bool blueOne)
    {
        string result = "";

        List<VisitorTokensAndInfo> boxTokens = ourBox.AllBoxTokens;

        foreach (VisitorTokensAndInfo oneVisitorToken in boxTokens)
        {
            long visitorTokenId = oneVisitorToken.tokenId;
            string visitorTokenTxt = CompressString.StringCompressor.longToString(visitorTokenId);
            String visitorTokenEncrypted = CryptFacility.Encrypt(visitorTokenTxt, ourPass + keyFromOurToken, ourKey);

            long explodeAfter = oneVisitorToken.NbBeforeExplode;
            Debug.Log("Explode After is " + explodeAfter + " for Token " + oneVisitorToken.tokenId);
            if (greenOnes)
            {
                explodeAfter += greenTokenNb;
            }
            if (redOnes)
            {
                explodeAfter += redTokenNb;
            }
            if (blueOne)
            {
                explodeAfter += blueTokenNb;
            }
            string explodeAfterTxt = CompressString.StringCompressor.longToString(explodeAfter);
            String explodeAfterEncrypted = CryptFacility.Encrypt(explodeAfterTxt, ourPass + keyFromOurToken, ourKey);

            long nbPassed = oneVisitorToken.NbSoFar;
            string nbPassedTxt = CompressString.StringCompressor.longToString(nbPassed);
            String nbPassEncrypted = CryptFacility.Encrypt(nbPassedTxt, ourPass + keyFromOurToken, ourKey);

            result += separator + visitorTokenEncrypted + separator + explodeAfterEncrypted + separator + nbPassEncrypted;
        }

        return result;
    }
}
