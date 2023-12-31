using Common.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadSaveTokens
{
    static string ourPass = "IGIUGIusig876t8";

    internal static BoxTokensSave loadAll()
    {
        BoxTokensSave newSaveInList = new BoxTokensSave();

        Debug.Log("Trying to load seeds " + giveSavePath());
        if (File.Exists(giveSavePath()))
        {
            // 2
            String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

            newSaveInList = JsonUtility.FromJson<BoxTokensSave>(json);

            Debug.Log("All data Loaded");
        }

        return newSaveInList;
    }

    internal static void Save(BoxTokensSave boxToSave)
    {
        Debug.Log(Application.persistentDataPath);

        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }

        if (!File.Exists(giveSavePath()))
        {
            string json = JsonUtility.ToJson(boxToSave);

            File.WriteAllText(giveSavePath(), json, System.Text.UTF8Encoding.UTF8);

            Debug.Log("All box saved");
        }
        else
        {
            Debug.Log("PROBLEM: Could not delete save file");
        }
    }

    internal static void deleteSave()
    {
        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }
    }

    internal static string giveSavePath()
    {
        return Application.persistentDataPath + "/boxSeeds.save";
    }

    internal static bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    internal static void codeEncryptAndSaveAllTokens(List<VisitorTokensAndInfo> seeds)
    {
        long saltNb = LocalSave.GetLong(ManageBox.DateOfFirstPlay) * 4;
        string saltTxt = CompressString.StringCompressor.longToString(saltNb);

        BoxTokensSave newSaveInList = new BoxTokensSave();

        for (int iSeed = 0; iSeed < seeds.Count; ++iSeed)
        {
            string seedTxt = CompressString.StringCompressor.longToString(seeds[iSeed].tokenId);
            string seedEncoded = CryptFacility.Encrypt(seedTxt, ourPass, saltTxt);

            newSaveInList.seeds.Add(seedEncoded);

            string nbBeforeExplodeTxt = CompressString.StringCompressor.longToString(seeds[iSeed].NbBeforeExplode);
            string nbBeforeExplodeEncoded = CryptFacility.Encrypt(nbBeforeExplodeTxt, ourPass + seedTxt, saltTxt);

            newSaveInList.nbBeforeExplode.Add(nbBeforeExplodeEncoded);

            Debug.Log("AAAAAAA- Nb so far " + seeds[iSeed].NbSoFar);
            string nbSoFarTxt = CompressString.StringCompressor.longToString(seeds[iSeed].NbSoFar);
            string nbSoFarEncoded = CryptFacility.Encrypt(nbSoFarTxt, ourPass + seedTxt, saltTxt);

            newSaveInList.nbSoFar.Add(nbSoFarEncoded);
        }

        Save(newSaveInList);
    }

    internal static List<VisitorTokensAndInfo> loadDecodeDecryptAllSeeds()
    {
        BoxTokensSave newSaveInList = loadAll();
        List<VisitorTokensAndInfo> allSeedFound = new List<VisitorTokensAndInfo>();

        long saltNb = LocalSave.GetLong(ManageBox.DateOfFirstPlay) * 4;
        string saltTxt = CompressString.StringCompressor.longToString(saltNb);

        for (int iSeed = 0; iSeed < newSaveInList.seeds.Count; ++iSeed)
        {
            VisitorTokensAndInfo aNewVisitor = new VisitorTokensAndInfo();

            string seedDecoded = CryptFacility.Decrypt(newSaveInList.seeds[iSeed], ourPass, saltTxt);
            long seedNb = CompressString.StringCompressor.GetLong(seedDecoded);
            aNewVisitor.tokenId = seedNb;

            string nbBeforeExplodeDecoded = CryptFacility.Decrypt(newSaveInList.nbBeforeExplode[iSeed], ourPass + seedDecoded, saltTxt);
            long nbBeforeExplodeNb = CompressString.StringCompressor.GetLong(seedDecoded);
            aNewVisitor.NbBeforeExplode = (int ) nbBeforeExplodeNb;

            string nbSoFarDecoded = CryptFacility.Decrypt(newSaveInList.nbSoFar[iSeed], ourPass + seedDecoded, saltTxt);
            long nbSoFarNb = CompressString.StringCompressor.GetLong(nbSoFarDecoded);
            aNewVisitor.NbSoFar = nbSoFarNb;

            allSeedFound.Add(aNewVisitor);
        }

        return allSeedFound;
    }
}
