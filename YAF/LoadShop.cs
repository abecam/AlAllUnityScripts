using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadShop : MonoBehaviour
{
    public bool isHeroShop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadShop()
    {
        LocalSave.SetBool("IsHeroShop", isHeroShop); // The value is ignored, the watch checks only the key presence.

        LocalSave.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
    }
}
