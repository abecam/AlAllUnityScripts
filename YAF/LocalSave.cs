using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalSave : MonoBehaviour
{
    static bool firstAccess = true;

    static SaveGeneric lastSave = new SaveGeneric();

    private static void loadAll()
    {
        firstAccess = false;

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

    public static void Save()
    {
        Debug.Log(Application.persistentDataPath);

        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }

        if (!File.Exists(giveSavePath()))
        {
            SaveGenericList newSaveInList = lastSave.toSaveInList();

            string json = JsonUtility.ToJson(newSaveInList);

            File.WriteAllText(giveSavePath(), json, System.Text.UTF8Encoding.UTF8);

            Debug.Log("All data saved");
        }
        else
        {
            Debug.LogError("PROBLEM: Could not delete save file");
        }
    }

    public static void deleteSave()
    {
        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }
    }

    public static void deleteMemorySave()
    {
        lastSave = new SaveGeneric();
    }

    private static string giveSavePath()
    {
        return Application.persistentDataPath + "/prefsAndAll.save";
    }

    public static bool existSave()
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

    public static bool HasDoubleKey(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.doubleValues.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public static bool HasStringKey(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.stringValues.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public static bool HasLongKey(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.longValues.ContainsKey(key))
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

    public static double GetDouble(String key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.doubleValues.ContainsKey(key))
        {
            return lastSave.doubleValues[key];
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

	   internal static long GetLong(string key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.longValues.ContainsKey(key))
        {
            return lastSave.longValues[key];
        }
        throw new KeyNotFoundException();
    }

    internal static string GetString(string key)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.stringValues.ContainsKey(key))
        {
            return lastSave.stringValues[key];
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
        if (lastSave.doubleValues.ContainsKey(key))
        {
            lastSave.doubleValues.Remove(key);
            return;
        }
        if (lastSave.longValues.ContainsKey(key))
        {
            lastSave.longValues.Remove(key);
            return;
        }
        if (lastSave.stringValues.ContainsKey(key))
        {
            lastSave.stringValues.Remove(key);
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

    public static void SetDouble(String key, double value)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.doubleValues.ContainsKey(key))
        {
            lastSave.doubleValues.Remove(key);
        }
        lastSave.doubleValues.Add(key, value);
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

	public static void SetLong(String key, long value)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.longValues.ContainsKey(key))
        {
            lastSave.longValues.Remove(key);
        }
        lastSave.longValues.Add(key, value);
    }

    public static void SetString(String key, string value)
    {
        if (firstAccess)
        {
            loadAll();
        }
        if (lastSave.stringValues.ContainsKey(key))
        {
            lastSave.stringValues.Remove(key);
        }
        lastSave.stringValues.Add(key, value);
    }
}
