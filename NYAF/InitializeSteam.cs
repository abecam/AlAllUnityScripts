using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InitializeSteam : MonoBehaviour
{
    public static bool steamStarted = false;
    public static bool inDemo = true;

    public const uint nyafID = 1378660;
    public const uint nyafDemoID = 1510750;

    public enum achievementKey
    {
        BJS_1,
        BJS_2,
        BJS_3,
        BJS_4,
        BJS_END,
        BJS_END_SUN,
        MMPG_END,
        NYAF_MODE_A_FINISHED,
        NYAF_MODE_B_FINISHED,
        NYAF_MODE_C_FINISHED,
        NYAF_MODE_D_FINISHED,
        NYAF_MODE_E_FINISHED,
        SMALL_SECRET_1,
        SMALL_SECRET_10,
        SMALL_SECRET_11,
        SMALL_SECRET_2,
        SMALL_SECRET_3,
        SMALL_SECRET_4,
        SMALL_SECRET_5,
        SMALL_SECRET_6,
        SMALL_SECRET_7,
        SMALL_SECRET_8,
        SMALL_SECRET_9,
        NYAF_SHOP_ALL_COINS
    }

    public static Dictionary<achievementKey, string> Achievements = new Dictionary<achievementKey, string>
    {
        {achievementKey.BJS_1, "BJS_1"},
{achievementKey.BJS_2, "BJS_2"},
{achievementKey.BJS_3, "BJS_3"},
{achievementKey.BJS_4, "BJS_4"},
{achievementKey.BJS_END, "BJS_END"},
{achievementKey.BJS_END_SUN, "BJS_END_SUN"},
{achievementKey.MMPG_END, "MMPG_END"},
{achievementKey.NYAF_MODE_A_FINISHED, "NYAF_MODE_A_FINISHED"},
{achievementKey.NYAF_MODE_B_FINISHED, "NYAF_MODE_B_FINISHED"},
{achievementKey.NYAF_MODE_C_FINISHED, "NYAF_MODE_C_FINISHED"},
{achievementKey.NYAF_MODE_D_FINISHED, "NYAF_MODE_D_FINISHED"},
{achievementKey.NYAF_MODE_E_FINISHED, "NYAF_MODE_E_FINISHED"},
{achievementKey.SMALL_SECRET_1, "SMALL_SECRET_1"},
{achievementKey.SMALL_SECRET_10, "SMALL_SECRET_10"},
{achievementKey.SMALL_SECRET_11, "SMALL_SECRET_11"},
{achievementKey.SMALL_SECRET_2, "SMALL_SECRET_2"},
{achievementKey.SMALL_SECRET_3, "SMALL_SECRET_3"},
{achievementKey.SMALL_SECRET_4, "SMALL_SECRET_4"},
{achievementKey.SMALL_SECRET_5, "SMALL_SECRET_5"},
{achievementKey.SMALL_SECRET_6, "SMALL_SECRET_6"},
{achievementKey.SMALL_SECRET_7, "SMALL_SECRET_7"},
{achievementKey.SMALL_SECRET_8, "SMALL_SECRET_8"},
{achievementKey.SMALL_SECRET_9, "SMALL_SECRET_9"},
{achievementKey.NYAF_SHOP_ALL_COINS, "NYAF_SHOP_ALL_COINS"},
    };
    // Start is called before the first frame update

    private void Awake()
    {
        startSteam();
    }

    public static void startSteam()
    {
        if (!steamStarted)
        {
            try
            {
                //uint ourID = nyafDemoID;
                Steamworks.SteamClient.Init(nyafID);

                if (Steamworks.SteamApps.IsSubscribedToApp(nyafID))
                {
                    inDemo = false;
                    //ourID = nyafID;
                }
                else
                {
                    Steamworks.SteamClient.Init(nyafDemoID);

                    inDemo = true;
                }
                

                Debug.Log("SteamWorks initialized!");
                steamStarted = true;

                // Check if any achievement has been unlocked but not yet on Steam!
                checkUnlockedAchievements();
            }
            catch (System.Exception e)
            {
                Debug.Log("Could not initialize SteamWorks for main app, checking if demo: " + e);

                try
                {
                    //uint ourID = nyafDemoID;
                    Steamworks.SteamClient.Init(nyafDemoID);

                    Debug.Log("SteamWorks initialized!");
                    steamStarted = true;

                    inDemo = true;
                }
                catch (System.Exception eDemo)
                {
                    Debug.Log("Could not initialize SteamWorks for demo too: " + eDemo);
                }
            }
            // For dev and tests:
            //DeleteAchievements();
            //SaveAchievements();
        }
    }

    private static void checkUnlockedAchievements()
    {
        foreach (string oneAchievementsName in Achievements.Values)
        {
            if (LocalSave.HasBoolKey(oneAchievementsName))
            {
                // If it's there it should be true, but we better plan for idiot-proofing
                bool isUnlocked = LocalSave.GetBool(oneAchievementsName);

                if (isUnlocked)
                {
                    var achievement = new Achievement(oneAchievementsName);
                    if (!achievement.State)
                    {
                        achievement.Trigger();
                    }
                }
            }
        }
    }

    private static string giveSavePath()
    {
        return Application.persistentDataPath + "/allAchievements.json";
    }

    public static void DeleteAchievements()
    {
        foreach (var a in SteamUserStats.Achievements)
        {
            a.Clear();
        }
    }

    public static void SaveAchievements()
    {
        string allAchievements = "";

        string allAchievementsEnum = "";

        foreach (var a in SteamUserStats.Achievements)
        {
            Debug.Log($"{a.Name} ({a.State})");

            allAchievements += "{"+$"achievementKey.{a.Identifier}, \"{a.Identifier}\""+"},\n";
            allAchievementsEnum += $"{a.Identifier},\n";
        }

        Debug.Log(Application.persistentDataPath);

        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }

        if (!File.Exists(giveSavePath()))
        {
            File.WriteAllText(giveSavePath(), allAchievements+"\n\n\n"+ allAchievementsEnum, System.Text.UTF8Encoding.UTF8);

            Debug.Log("All data saved");
        }
        else
        {
            Debug.Log("PROBLEM: Could not delete save file");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }

    public static void unlockAchievement(achievementKey byKey)
    {
        if (!DemoManager.isDemo)
        {
            string steamID = Achievements[byKey];
            Debug.Log("Unlocking " + steamID);
            var achievement = new Achievement(steamID);

            achievement.Trigger();
        }

        {
            // Save the achievement unlocked so we can unlock it later if not in the demo anymore or if something went wrong before
            LocalSave.SetBool(Achievements[byKey], true);  
        }
    }
}
