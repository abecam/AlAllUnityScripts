using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowOurFullScreenAds : MonoBehaviour
{
    private const float normalZoom = 5;
    private float ourZoom = 1;
    private const float nbZoom = 60;
    private float stepBetweenZoom;
    private bool zooming = true;
    private bool fading = false;

    public TextMesh ourText;
    public GameObject background;

    public GameObject backButton;
    public GameObject ourButton;
    public Text ourButtonText;

    private string urlOfGame;
    private string buttonText;

    private float time = 0;
    private const int timeInAds = 30;

    List<string> allTextAndUrl = new List<string>()
        {
           "Buy our\ngame now!", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here to surely\nnot go to the Play Store\n(Ah Ah)",
           "Please!?", "https://play.google.com/store/apps/details?id=com.tgb.nyafdemo","Press here to close\nthe ads (really!!!)\n(Ah Ah)",
           "Be nice\nGo there!\n(to suffer ;) )", "https://play.google.com/store/apps/details?id=com.tgb.thge.free","Press here\nif you think\nyou can do it!",
           "We won't\nshare your\ninfos\n(too much)", "https://play.google.com/store/apps/details?id=com.facebook.katana","Press here to be\nspied on!",
           "There is a\ngame beside\nour shop\nwe promise!", "https://play.google.com/store/apps/details?id=com.fluffyfairygames.idleminertycoon","Press here to pay\nhmmm, to play",
           "You might\nfinish\nwithout\nspending\n(in 1M years)", "https://play.google.com/store/apps/details?id=se.ace.fishinc","Press here to think\nyou are playing\na game!",
           "NYAF", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here",
           "NYAF", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here!",
           "NYAF", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here\nDo it!",
           "NYAF", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here\nYes, yes!",
           "NYAF", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here\nPlease?",
           "NYAF", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Press here\nI am sure you want it!",
           "NYAF?", "https://play.google.com/store/apps/details?id=com.tgb.nyafdemo","Don't you dare\nto press here!",
           "NYAF???", "https://play.google.com/store/apps/details?id=com.tgb.nyafdemo","Ok, do as you\nlike...",
           "NYAF?????", "https://play.google.com/store/apps/details?id=com.tgb.thge.free","Press here to have\nfun (and suffer)\n(Ah Ah)",
           "Still there?", "https://play.google.com/store/apps/details?id=com.tgb.thge.free","Press here to cry\nin agony",
           "Sure you\ndon't want\nto do\nsomething else?", "https://play.google.com/store/apps/details?id=com.tgb.thge.free","Don't press here\nPlease!",
           "Skip a beer,\nbuy a game!", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Go regret your beer\nhere!",
           "Skip 2 beers,\nbuy a game\nand save money!", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","Get save on your extra\nbeer here",
           "I am sure\nyou want it!\n", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","NYAF NYAF NYAF",
           "Hi!\n", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","How are you today?",
           "Don't miss\nthe best game\non the planet!\n(not Earth,\nsomewhere else)", "https://play.google.com/store/apps/details?id=com.tgb.nyaf","ZUGHRGH FFZUDF\n(Press here)",
           "You surely\nshould do\nsomething else!", "https://en.wikipedia.org/wiki/Main_Page","Press to do\nsomething better!",
           "Maybe,\nJust maybe\nmaybe...", "http://www.thegiantball.com/2020/07/the-giant-ball.html","Press to see\nThe History\n(of us)",
           "Marcel loves\nErnestine\nCamille\nworks at the\nhospital", "https://www.youtube.com/watch?v=o73xf6YFZyI","Press here to be sad!",
           "You are so\nbeautiful\nwhen you\nbuy our game!", "https://www.youtube.com/watch?v=5N8WhTkvenM","Press here to feel\nromantic",
           "The Giant Ball\nIs so...\nBig !!!", "https://www.youtube.com/watch?v=U38U_KLduyU","And it is waiting for you!",
           "The best\nSimulation\nEver!\nDon't miss it!", "https://www.youtube.com/watch?v=AsSDqgLFwIY","Press the best\nbutton simulation!",
           "About love,\nLife, and\nthe meaning Of\nBuying\nour game!!!!", "https://www.youtube.com/watch?v=rXjSLWe2RJg","Press here to be\nbored!",
           "Do you\nknow\nthat we have\nthese other\ngames?", "https://www.youtube.com/watch?v=ntqI8OPaqOw","Press here to buy\nALL!!!",
           "NYAF!\nNow on\nZX Spectrum!", "https://www.youtube.com/watch?v=1BlqrKf5wwo","Need a cluster of 100 ZX\nThough",
        };


    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;

    // Start is called before the first frame update
    void Start()
    {
        int nbAds = 0;

        bool haskey = LocalSave.HasIntKey("nbFullScreenAds");
        if (haskey)
        {
            nbAds = LocalSave.GetInt("nbFullScreenAds");
        }

        int posInList = nbAds * 3;
        if (posInList >= allTextAndUrl.Count)
        {
            posInList = 0;

            LocalSave.SetInt("nbFullScreenAds", 0);
            LocalSave.Save();
        }
        else
        {
            LocalSave.SetInt("nbFullScreenAds", ++nbAds);
            LocalSave.Save();
        }

        ourText.text = allTextAndUrl[posInList];
        int nbOfLines = allTextAndUrl[posInList].Count(f => f == '\n') + 1;

        background.transform.localScale = new Vector3(5, 1.15f * nbOfLines, 5);

        urlOfGame = allTextAndUrl[posInList + 1];
        buttonText = allTextAndUrl[posInList + 2];

        stepBetweenZoom = (ourZoom - normalZoom) / nbZoom;

        Camera.main.orthographicSize = ourZoom;

        foreach (Transform childIntrobox in transform)
        {
            //Debug.Log("Child: " + childIntrobox);
            Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
            if (renderer1 != null)
            {
                renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, 0);
            }
        }

        ourButtonText.text = buttonText;
        ourButton.SetActive(false);
        backButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (zooming)
        {
            ourZoom -= stepBetweenZoom;

            if (ourZoom > normalZoom)
            {
                ourZoom = normalZoom;

                zooming = false;
                fading = true;

                ourButton.SetActive(true);
                backButton.SetActive(true);
            }

            Camera.main.orthographicSize = ourZoom;
        }
        else if (fading)
        {
            float fadingFactor = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
            foreach (Transform childIntrobox in transform)
            {
                //Debug.Log("Child: " + childIntrobox);
                Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                if (renderer1 != null)
                {
                    renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, fadingFactor);
                }
            }

            zoomingTime++;

            if (zoomingTime > totalZoomingTime)
            {
                fading = false;
            }
        }
        else
        {
            // Leave after a while
            if (time > 30)
            {
                loadMainGame();
            }
        }
    }

    public void loadMainGame()
    {
        bool hasKey = LocalSave.HasBoolKey("IsHeroAds");

        if (hasKey)
        {
            LocalSave.DeleteKey("IsHeroAds");
            LocalSave.Save();

            UnityEngine.SceneManagement.SceneManager.LoadScene("NYPB");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainBoard");
        }
    }

    public void goToUrl()
    {
        Application.OpenURL(urlOfGame);
    }
}
