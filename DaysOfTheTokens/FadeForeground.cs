using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeForeground : MonoBehaviour
{
    const float timeToMove = 10;
    const float timeBeforeLoadingGame = 20;
    float currentTime = 0;

    const float timeBeforeFade = 0;
    const float fadeFactor = 1 / (timeToMove / 1.5f);
    float ourAlpha = 1;
    Material ourMaterial;

    public bool loadNextScene = true; // Automatically load the next scene?

    // Start is called before the first frame update
    void Start()
    {
        Renderer ourRenderer = GetComponent<Renderer>();

        ourMaterial = ourRenderer.material;

        ourMaterial.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > timeBeforeFade)
        {
            if (ourAlpha > 0)
            {
                ourAlpha = ourAlpha - fadeFactor * Time.deltaTime;

                if (ourAlpha <= 0)
                {
                    ourAlpha = 0;
                }

                ourMaterial.color = new Color(1, 1, 1, ourAlpha);
            }
        }
        if (loadNextScene && currentTime > timeBeforeLoadingGame)
        {
            SceneManager.LoadScene("main");
        }
    }
}
