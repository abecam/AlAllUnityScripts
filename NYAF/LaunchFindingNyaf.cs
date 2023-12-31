using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchFindingNyaf : MonoBehaviour
{
    const string nameOfScene = "FindingNYAFINGS/MainScene";

    public void loadFindingNYAF()
    {
        startFindingNYAF();
    }

    private static void startFindingNYAF()
    {
        int level = 0;
        PortalScript.typeOfScene forTypeOfScene = PortalScript.typeOfScene.groundNormal;

        if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
        {
            level = LocalSave.GetInt("FindingNYAF_currentLevel");
        }

        if (level == 11 || level == 12)
        {
            forTypeOfScene = PortalScript.typeOfScene.undersea;
        }

        switch (forTypeOfScene)
        {
            case PortalScript.typeOfScene.groundNormal:
                LocalSave.SetInt("FindingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/MainScene");
                break;
            case PortalScript.typeOfScene.spaceNormal:
                LocalSave.SetInt("MovingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/MovingNyafings");
                break;
            case PortalScript.typeOfScene.asteroidField:
                LocalSave.SetInt("MovingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/AsteroidsField");
                break;
            case PortalScript.typeOfScene.underground:
                LocalSave.SetInt("FindingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Underground");
                break;
            case PortalScript.typeOfScene.undersea:
                LocalSave.SetInt("FindingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Undersea2");
                break;
            case PortalScript.typeOfScene.oversea:
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Sea");
                break;
            case PortalScript.typeOfScene.swamp:
                LocalSave.SetInt("FindingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Swamp");
                break;
            case PortalScript.typeOfScene.halloween:
                LocalSave.SetInt("FindingNYAF_currentLevel", level);
                LocalSave.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Halloween");
                break;
        }
    }

    public static void loadFindingNYAFStatic()
    {
        startFindingNYAF();
    }
}
