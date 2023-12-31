using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalSave : MonoBehaviour
{
    static bool firstAccess = true;

    static SaveGeneric lastSave = new SaveGeneric();

    private void Awake()
    {
        Debug.Log("First loading");
        loadAll();
    }

    private static void loadAll()
    {
        firstAccess = false;
        // First check the cloud
        if (InitializeSteam.steamStarted && SteamRemoteStorage.FileExists("nyafSaves.json"))
        {
            Debug.Log("Trying to load save from cloud.");

            byte[] nyafSaveInByte = SteamRemoteStorage.FileRead("nyafSaves.json");

            String json = Encoding.UTF8.GetString(nyafSaveInByte);

            SaveGenericList newSaveInList = JsonUtility.FromJson<SaveGenericList>(json);

            lastSave.fromSaveInList(newSaveInList);

            Debug.Log("All data Loaded from cloud");
        }
        else
        {
            Debug.Log("Trying to load level " + giveSavePath());
            if (File.Exists(giveSavePath()))
            {
                // 2
                String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

                SaveGenericList newSaveInList = JsonUtility.FromJson<SaveGenericList>(json);

                lastSave.fromSaveInList(newSaveInList);

                Debug.Log("All data Loaded");
            }
        }
    }

    public static void SaveCopy()
    {
        if (File.Exists(giveSavePathCopy()))
        {
            File.Delete(giveSavePathCopy());
        }

        if (!File.Exists(giveSavePathCopy()))
        {
            SaveGenericList newSaveInList = lastSave.toSaveInList();

            string json = JsonUtility.ToJson(newSaveInList);

            File.WriteAllText(giveSavePathCopy(), json, System.Text.UTF8Encoding.UTF8);
            Debug.Log("All data saved");
        }
        else
        {
            Debug.Log("PROBLEM: Could not delete save file");
        }
    }

    public static void Save()
    {
        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }

        if (!File.Exists(giveSavePath()))
        {
            SaveGenericList newSaveInList = lastSave.toSaveInList();

            string json = JsonUtility.ToJson(newSaveInList);

            File.WriteAllText(giveSavePath(), json, System.Text.UTF8Encoding.UTF8);
            if (InitializeSteam.steamStarted)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                SteamRemoteStorage.FileWrite("nyafSaves.json", bytes);

                Debug.Log("All data saved in the cloud");
            }
            Debug.Log("All data saved");
        }
        else
        {
            Debug.Log("PROBLEM: Could not delete save file");
        } 
    }

    private static void deleteSave()
    {
        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }
    }

    private static string giveSavePath()
    {
        return Application.persistentDataPath + "/prefsAndAll.save";
    }

    private static string giveSavePathCopy()
    {
        return Application.persistentDataPath + "/prefsAndAllBak.save";
    }

    private static bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    public static String returnAllAsString()
    {
        SaveGenericList newSaveInList = lastSave.toSaveInList();

        string json = JsonUtility.ToJson(newSaveInList);

        return json;
    }

    public static void restoreAllFromString(String newJson)
    {
        SaveGenericList newSaveInList = JsonUtility.FromJson<SaveGenericList>(newJson);

        lastSave.fromSaveInList(newSaveInList);

        Debug.Log("All data Loaded from String");
    }

    public static bool HasBoolKey(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.boolValues.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public static bool HasIntKey(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.intValues.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public static bool HasFloatKey(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.floatValues.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public static int GetInt(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.intValues.ContainsKey(key))
        {
            return lastSave.intValues[key];
        }
        throw new KeyNotFoundException();
    }

    public static float GetFloat(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.floatValues.ContainsKey(key))
        {
            return lastSave.floatValues[key];
        }
        throw new KeyNotFoundException();
    }

    public static bool GetBool(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.boolValues.ContainsKey(key))
        {
            return lastSave.boolValues[key];
        }
        throw new KeyNotFoundException();
    }

    public static void DeleteKey(String key)
    {
        if (lastSave.boolValues.ContainsKey(key))
        {
            lastSave.boolValues.Remove(key);
            return;
        }
        if (lastSave.intValues.ContainsKey(key))
        {
            lastSave.intValues.Remove(key);
            return;
        }
        if (lastSave.floatValues.ContainsKey(key))
        {
            lastSave.floatValues.Remove(key);
            return;
        }
    }

    public static void SetInt(String key, int value)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.intValues.ContainsKey(key))
        {
            lastSave.intValues.Remove(key);
        }
        lastSave.intValues.Add(key, value);
    }

    public static void SetFloat(String key, float value)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.floatValues.ContainsKey(key))
        {
            lastSave.floatValues.Remove(key);
        }
        lastSave.floatValues.Add(key, value);
    }

    public static void SetBool(String key, bool value)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.boolValues.ContainsKey(key))
        {
            lastSave.boolValues.Remove(key);
        }
        lastSave.boolValues.Add(key, value);
    }

    internal static void restoreFromBackup()
    {
        Debug.Log("Trying to load backup save " + giveSavePathCopy());
        if (File.Exists(giveSavePathCopy()))
        {
            // 2
            String json = File.ReadAllText(giveSavePathCopy(), System.Text.UTF8Encoding.UTF8);

            SaveGenericList newSaveInList = JsonUtility.FromJson<SaveGenericList>(json);

            lastSave.fromSaveInList(newSaveInList);

            Debug.Log("All data Loaded");
        }
    }
}
