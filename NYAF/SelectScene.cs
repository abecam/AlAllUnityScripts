using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class SelectScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];

        var optionDataList = new List<Dropdown.OptionData>();

        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));

            optionDataList.Add(new Dropdown.OptionData(scenes[i]));
        }

        GetComponent<Dropdown>().ClearOptions();
        GetComponent<Dropdown>().AddOptions(optionDataList);
    }

    public void startAScene(int nbSelected)
    {
        string nameOfScene = GetComponent<Dropdown>().options[nbSelected].text;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfScene);
    }
}
