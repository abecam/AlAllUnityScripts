using System;
using UnityEngine;

public class BJSSave
{
    int currentJohn = 1;
    int levelJohn = 1;
    int currentDay = 1;
    float xpJohn = 0;
    float xpNeeded = 400;
    int currentBellForLevel = 0; 
    int maxBellForLevel = 0; // bell (x) for a john (y*100) 
    int maxJohn = 1;

    public BJSSave()
	{
	}

    public int CurrentJohn { get => currentJohn; set => currentJohn = value; }
    public int LevelJohn { get => levelJohn; set => levelJohn = value; }
    public int CurrentDay { get => currentDay; set => currentDay = value; }
    public float XpJohn { get => xpJohn; set => xpJohn = value; }
    public float XpNeeded { get => xpNeeded; set => xpNeeded = value; }
    public int CurrentBellForLevel { get => currentBellForLevel; set => currentBellForLevel = value; }
    public int MaxBellForLevel { get => maxBellForLevel; set => maxBellForLevel = value; }
    public int MaxJohn { get => maxJohn; set => maxJohn = value; }
   
    public bool isSave()
    {
        if (LocalSave.HasFloatKey("BJS_XPJohn"))
        {
            return true;
        }
        return false;
    }

    public void loadAllValues()
    {
        if (LocalSave.HasIntKey("BJS_CurrentJohn"))
        {
            currentJohn = LocalSave.GetInt("BJS_CurrentJohn");
        }
        if (LocalSave.HasIntKey("BJS_JohnLevel"))
        {
            levelJohn = LocalSave.GetInt("BJS_JohnLevel");
        }
        if (LocalSave.HasIntKey("BJS_CurrentDay"))
        {
            currentDay = LocalSave.GetInt("BJS_CurrentDay");
        }
        if (LocalSave.HasFloatKey("BJS_XPJohn"))
        {
            xpJohn = LocalSave.GetFloat("BJS_XPJohn");
        }
        if (LocalSave.HasFloatKey("BJS_XPNeeded"))
        {
            xpNeeded = LocalSave.GetFloat("BJS_XPNeeded");
        }
        if (LocalSave.HasIntKey("BJS_CurrentBellForLevel"))
        {
            currentBellForLevel = LocalSave.GetInt("BJS_CurrentBellForLevel");
        }
        if (LocalSave.HasIntKey("BJS_MaxJohn"))
        {
            maxJohn = LocalSave.GetInt("BJS_MaxJohn");
        }
        if (LocalSave.HasIntKey("BJS_MaxBellForLevel"))
        {
            maxBellForLevel = LocalSave.GetInt("BJS_MaxBellForLevel");
        }
    }

    public void saveAllValues()
    {
        LocalSave.SetInt("BJS_CurrentJohn", currentJohn);

        LocalSave.SetInt("BJS_JohnLevel", levelJohn);

        LocalSave.SetInt("BJS_CurrentDay", currentDay);

        LocalSave.SetFloat("BJS_XPJohn", xpJohn);

        LocalSave.SetFloat("BJS_XPNeeded", xpNeeded);

        LocalSave.SetInt("BJS_CurrentBellForLevel", currentBellForLevel);

        LocalSave.Save();
    }

    public void saveCurrentJohn()
    {
        LocalSave.SetInt("BJS_CurrentJohn", currentJohn);

        // Check if it is the max value
        int maxValue = 0;

        if (LocalSave.HasIntKey("BJS_MaxJohn"))
        {
            maxValue = LocalSave.GetInt("BJS_MaxJohn");
        }
        if (currentJohn > maxValue)
        {
            LocalSave.SetInt("BJS_MaxJohn", currentJohn);
        }
        LocalSave.Save();
    }

    public void saveJohnLevel()
    {
        LocalSave.SetInt("BJS_JohnLevel", levelJohn);

        LocalSave.Save();
    }

    public void saveCurrentDay()
    {
        LocalSave.SetInt("BJS_CurrentDay", currentDay);

        LocalSave.Save();
    }

    public void saveXpsAndLevel()
    {
        LocalSave.SetInt("BJS_JohnLevel", levelJohn);

        LocalSave.SetFloat("BJS_XPJohn", xpJohn);

        LocalSave.SetFloat("BJS_XPNeeded", xpNeeded);

        LocalSave.Save();
    }

    internal void saveXps()
    {
        LocalSave.SetFloat("BJS_XPJohn", xpJohn);

        LocalSave.Save();
    }

    internal void saveBellNb()
    {
        // We assume that our currentJohn is up to date!
        LocalSave.SetInt("BJS_CurrentBellForLevel", currentBellForLevel);

        int currentAbsValue = currentBellForLevel + currentJohn * 100;

        // Check if it is the max value
        int maxValue = 0;

        if (LocalSave.HasIntKey("BJS_MaxBellForLevel"))
        {
            maxValue = LocalSave.GetInt("BJS_MaxBellForLevel");
        }
        if (currentAbsValue > maxValue)
        {
            LocalSave.SetInt("BJS_MaxBellForLevel", currentAbsValue);
        }

        LocalSave.Save();
    }

    public void deleteAllSaves()
    {
        LocalSave.DeleteKey("BJS_CurrentJohn");

        LocalSave.DeleteKey("BJS_JohnLevel");

        LocalSave.DeleteKey("BJS_CurrentDay");

        LocalSave.DeleteKey("BJS_XPJohn");

        LocalSave.DeleteKey("BJS_XPNeeded");

        LocalSave.DeleteKey("BJS_CurrentBell");

        LocalSave.DeleteKey("BJS_MaxJohn");

        LocalSave.DeleteKey("BJS_MaxBellForLevel");

        LocalSave.DeleteKey("BJS_currentTimeSpent");

        LocalSave.DeleteKey("BJS_bestTimeSpent");

        LocalSave.Save();
    }

    public void deleteProgressionSaves()
    {
        LocalSave.DeleteKey("BJS_CurrentJohn");

        LocalSave.DeleteKey("BJS_JohnLevel");

        LocalSave.DeleteKey("BJS_CurrentDay");

        LocalSave.DeleteKey("BJS_XPJohn");

        LocalSave.DeleteKey("BJS_XPNeeded");

        LocalSave.DeleteKey("BJS_CurrentBell");

        LocalSave.Save();
    }

    public void deleteMaxSaves()
    {
        LocalSave.DeleteKey("BJS_MaxJohn");

        LocalSave.DeleteKey("BJS_MaxBellForLevel");

        LocalSave.Save();
    }

    /**
     * Ask for no title
     */
    public void saveNoTitle()
    {
        // After selecting a level, we want to restart without title

        LocalSave.SetInt("BJS_NoTitle", 1);
    }

    public bool isTitle()
    {
        int noTitleRequested = 0;

        if (LocalSave.HasIntKey("BJS_NoTitle"))
        {
            noTitleRequested = LocalSave.GetInt("BJS_NoTitle");

            // Always reset
            LocalSave.SetInt("BJS_NoTitle", 0);
        }
        return (noTitleRequested == 0);
    }
}
