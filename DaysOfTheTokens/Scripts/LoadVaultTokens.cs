using Common.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadVaultTokens
{
    static string ourPass = "IGIGIIBi969689";

    internal static VaultTokensSave loadAll()
    {
        VaultTokensSave newSaveInList = new VaultTokensSave();

        Debug.Log("Trying to load seeds " + giveSavePath());
        if (File.Exists(giveSavePath()))
        {
            // 2
            String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

            newSaveInList = JsonUtility.FromJson<VaultTokensSave>(json);

            Debug.Log("All data Loaded");
        }

        return newSaveInList;
    }

    internal static void Save(VaultTokensSave boxToSave)
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

            Debug.Log("All vault saved");
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
        return Application.persistentDataPath + "/vaultSeeds.save";
    }

    internal static bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    internal static void codeEncryptAndSaveAllSeed(List<long> seeds)
    {
        long saltNb = LocalSave.GetLong(ManageBox.DateOfFirstPlay) * 3;
        string saltTxt = CompressString.StringCompressor.longToString(saltNb);

        VaultTokensSave newSaveInList = new VaultTokensSave();

        for (int iSeed = 0; iSeed < seeds.Count; ++iSeed)
        {
            string seedTxt = CompressString.StringCompressor.longToString(seeds[iSeed]);
            string seedEncoded = CryptFacility.Encrypt(seedTxt, ourPass, saltTxt);

            newSaveInList.seeds.Add(seedEncoded);
        }

        Save(newSaveInList);
    }

    internal static List<long> loadDecodeDecryptAllSeeds()
    {
        VaultTokensSave newSaveInList = loadAll();
        List<long> allSeedFound = new List<long>();

        long saltNb = LocalSave.GetLong(ManageBox.DateOfFirstPlay) * 3;
        string saltTxt = CompressString.StringCompressor.longToString(saltNb);

        for (int iSeed = 0; iSeed < newSaveInList.seeds.Count; ++iSeed)
        {
            string seedDecoded = CryptFacility.Decrypt(newSaveInList.seeds[iSeed], ourPass, saltTxt);
            long seedNb = CompressString.StringCompressor.GetLong(seedDecoded);

            allSeedFound.Add(seedNb);
        }

        return allSeedFound;
    }
}
