using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleBanner : MonoBehaviour
{
    public GameObject bannerBckgrd;
    public TextMesh ourText;
    public TextMesh ourText2;

    float currentTime;
    const float timeForAds = 120; // 2 min/ads

    List<string> allText = new List<string>()
        {
           "Buy NYAF!\n(or don't)",
           "Buy our\ngame now!",
           "Please!?", 
           "NYAF???", 
           "NYAF?????", 
           "Still there?", 
           "THGE",
           "Days of\nThe Tokens"
        };

    private class ClickedAds
    {
        public string text;
        public float size;
        public float timer = -1; // Will give a different text depending on the time needed to click on the banner

        public ClickedAds(string text, float size)
        {
            this.text = text;
            this.size = size;
        }

        public ClickedAds(string text, float size, float timer)
        {
            this.text = text;
            this.size = size;
            this.timer = timer;
        }
    }
    List<ClickedAds> clickedText = new List<ClickedAds>()
    {
        new ClickedAds("Very annoying Ads here!\nDo not click!", 1),
new ClickedAds("Buy this! Buy that!", 1),
new ClickedAds("I told you not to click!", 1),
new ClickedAds("Are you reading this?\nDon’t click!", 1),
new ClickedAds("YAF is the best game ever!\nPlay it, it’s totally free*\n* Might have some small IAP", 1),
new ClickedAds("You really like clicking the ads,\ndon’t you?", 1),
new ClickedAds("Buy me, buy that!\nBuy whatever you like!", 1),
new ClickedAds("Well, keep going,\nyou do not care what I say anyway.", 1),
new ClickedAds("Idle Gamer, the game that’s you!", 1),
new ClickedAds("Oh, oh! Okay.\nWe make a deal, you don’t click\nand I won’t report it.", 1),
new ClickedAds("Ok, I reported it.\nI don’t know who,\nbut somebody knows that you clicked.\nYou should be afraid.", 1),
new ClickedAds("Idle Gambler, pay more to play less", 1),
new ClickedAds("And down the rabbit hole…", 1),
new ClickedAds("Well try again now…", 0.5f),
new ClickedAds("You really like it, don’t you?", 0.25f),
new ClickedAds("Do you still see me?", 0.15f),
new ClickedAds("NOPE", 0.1f),
new ClickedAds("LOREN IPSUM", 0.05f),
new ClickedAds("Ok, you want me you got me!\nI will sell you\nwhatever you don’t like,\nwhatever you don’t want\nand whatever\nyou don’t need!!!!!", 4),
new ClickedAds("Uh… You thought you could\nget rid of me so easily,\ndidn’t you?", 4),
new ClickedAds("And you were right.\nNow don’t try it again.", 1),
new ClickedAds("Seriously…", 1),
new ClickedAds("Oh no, you don’t really think there will\nbe a different text each time\nyou click, do you?", 1),
new ClickedAds("Very annoying Ads here!\nDo not click!", 1),
new ClickedAds("Ah ah! I got you!\nEh!? It’s a different text now.\nWell that does not count,\nI am just answering.", 1),
new ClickedAds("Buy this! Buy that!", 1),
new ClickedAds("See, it was the same text again.\nWhat? Now it’s different again.\n But that still doesn’t count, come on!", 1),
new ClickedAds("I told you not to click!", 1),
new ClickedAds("And we continue again like before!", 1),
new ClickedAds("Blah blah blah fake ads,\nblah blah blah don’t click me,\nblah blah blah.", 1),
new ClickedAds("Blah blah blah,\nblah really like clicking\nblah blah blah.", 1),
new ClickedAds("Blah blah blah,\nblah blah the game that’s you! blah\nblah blah blah.", 1),
new ClickedAds("Blah blah blah,\nblah You should be afraid. blah\nblah blah blah.", 1),
new ClickedAds("Blah blah blah,\nblah blah the rabbit is pink,\nblah blah blah.", 1),
new ClickedAds("Blah blah blah,\nblah and the elephant transparent,\nblah blah blah.", 1),
new ClickedAds("Blah blah blah,\nblah blah blah,\nblah blah blah.", 1),
new ClickedAds("It’s getting late, isn’t it.", 1),
new ClickedAds("Shouldn’t you be going to bed?", 1),
new ClickedAds("Well, that’s it for me.\nI don’t know where you live,\ndon’t care, but for me\nit’s the middle of the night.\nGood night!", 1),
new ClickedAds("Please don’t do that.", 1),
new ClickedAds("Each time you click\nthere is a small metal pick\nthat hurts me.", 1),
new ClickedAds("What?\nYou thought it was all already written.\nOh how wrong you were.\nIt’s me there, somewhere\nat the other end of the internet.\nAnd now I would really like to sleep.", 1),
new ClickedAds("Oooooaaaawwww.\nHmmm?! What time is it?", 1),
new ClickedAds("Da di la, da di la,\nI don’t hear you, sleeping I told you.", 1),
new ClickedAds("Could you let me sleep,\nfor a moment?", 1),
new ClickedAds("Well, that was not much sleep...\nBad, bad player,\nyou should be ashamed!", 1, 20),
new ClickedAds("Well,\nbetter than nothing I guess.\nI might wake you like that\nnext time you sleep, though!", 1, 40),
new ClickedAds("Wow, you resisted a lot, didn't you?\nThank you I guess.", 1, 120),
new ClickedAds("Thank you, feeling better really.\nSo, would you stop clicking\non my ads already?", 1, 300),
new ClickedAds("Yep, feeling good,\nnow stop clicking here!", 1, 600),
new ClickedAds("You did it!\nYou resisted clicking for a while.\nNow, what about resisting forever???", 1, 1200),
new ClickedAds("Great, you resisted a great while.\nNow resist even more!", 1, 3600),
new ClickedAds("A big thank you to you.\nSend this code to our contact:\nZFZU2885ZZZ!", 1, 7200),
new ClickedAds("You can stop clicking now!", 1),
new ClickedAds("Seriously!\nI will start repeating myself! :)", 1)
    };

    int lastClickedAds = 0;

    bool isRotating = true;

    // Start is called before the first frame update
    void Start()
    {
        bool hasKey = LocalSave.HasIntKey("lastClickedAds");
        if (hasKey)
        {
            lastClickedAds = LocalSave.GetInt("lastClickedAds");
        }
        rotateAds();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (isRotating && currentTime > timeForAds)
        {
            rotateAds();
        }
    }

    void OnMouseDown()
    {
        manageClickableAds();
    }

    private void manageClickableAds()
    {
        isRotating = false;

        // Show the next clickable ads
        if (lastClickedAds >= clickedText.Count)
        {
            lastClickedAds = 0;
        }

        DateTime now = DateTime.Now;
        bool skipTimedLines = false;

        float withTimer = clickedText[lastClickedAds].timer;
        if (withTimer > 0)
        {
            long lastTimeClicked = 0;

            // Check the time spent, then find the right banner!
            bool hasKey = LocalSave.HasLongKey("lastClickedAdsTime");
            if (hasKey)
            {
                lastTimeClicked = LocalSave.GetLong("lastClickedAdsTime");
            }


            DateTime lastTimeInDateT = new DateTime(lastTimeClicked);
            double diffTime = now.Subtract(lastTimeInDateT).TotalSeconds;

            int iBreak = 0;

            while (diffTime > withTimer)
            {
                lastClickedAds++;

                withTimer = clickedText[lastClickedAds].timer;

                if (withTimer < 0 || iBreak++ > 10)
                {
                    break;
                }
            }
            skipTimedLines = true;
        }
        LocalSave.SetLong("lastClickedAdsTime", now.Ticks);

        ourText.text = clickedText[lastClickedAds].text;
        ourText2.text = clickedText[lastClickedAds].text;

        float sizeAds = clickedText[lastClickedAds].size;
        float textSize = 0.005f * sizeAds;
        if (sizeAds > 1)
        {
            textSize = 0.007f;
        }
        ourText.characterSize = textSize;
        ourText2.characterSize = textSize;

        if (sizeAds > 1)
        {
            ourText.transform.localPosition = new Vector3(0, -0.82f, -1);
            ourText2.transform.localPosition = new Vector3(0, -0.82f, -0.8f);
        }
        else
        {
            ourText.transform.localPosition = new Vector3(0, 0, -1);
            ourText2.transform.localPosition = new Vector3(0, 0, -0.8f);
        }

        int lengthText = clickedText[lastClickedAds].text.Length;

        //if (lengthText > 20)
        //{
        //    float sizeDesc = 0.018f * (1.1f * 20f / (lengthText));

        //    ourText.characterSize = sizeDesc * clickedText[lastClickedAds].size;
        //    ourText2.characterSize = sizeDesc * clickedText[lastClickedAds].size;
        //}

        currentTime = 0;

        if (isBanner)
        {
            Destroy(currentBanner);
        }
        currentBanner = Instantiate(bannerBckgrd.transform.GetChild(iGfx).gameObject);
        currentBanner.transform.SetParent(this.transform);
        currentBanner.transform.localPosition = new Vector3(0, 0, 0);
        if (sizeAds > 1)
        {
            currentBanner.transform.localPosition = new Vector3(0, 0, -9);
        }
        currentBanner.transform.localScale = new Vector3(sizeAds * 0.7f, sizeAds * 1.4f, 0);
        Renderer bannerRenderer = currentBanner.gameObject.GetComponent<Renderer>();
        bannerRenderer.material.color =new Color(0.4f, 0.4f, 0.4f, 1f);

        isBanner = true;

        iGfx++;

        if (iGfx >= bannerBckgrd.transform.childCount)
        {
            iGfx = 0;
        }

        lastClickedAds++;
        if (skipTimedLines)
        {
            withTimer = clickedText[lastClickedAds].timer;

            while (withTimer > 0)
            {
                lastClickedAds++;

                withTimer = clickedText[lastClickedAds].timer;
            }
        }

        LocalSave.SetInt("lastClickedAds", lastClickedAds);
    }

    int iGfx = 0;

    bool isBanner = false;
    private GameObject currentBanner;

    private void rotateAds()
    {
        int nbAds = 0;

        bool haskey = LocalSave.HasIntKey("nbBannerAds");
        if (haskey)
        {
            nbAds = LocalSave.GetInt("nbBannerAds");
        }

        int posInList = nbAds;
        if (posInList >= allText.Count)
        {
            posInList = 0;

            LocalSave.SetInt("nbBannerAds", 0);
            LocalSave.Save();
        }
        else
        {
            LocalSave.SetInt("nbBannerAds", ++nbAds);
            LocalSave.Save();
        }

        ourText.text = allText[posInList];
        ourText2.text = allText[posInList];

        currentTime = 0;

        if (isBanner)
        {
            Destroy(currentBanner);
        }
        currentBanner = Instantiate(bannerBckgrd.transform.GetChild(iGfx).gameObject);
        currentBanner.transform.SetParent(this.transform);
        currentBanner.transform.localPosition = new Vector3(0, 0, 0);
        currentBanner.transform.localScale = new Vector3(0.7f, 1.4f, 0);

        isBanner = true;

        iGfx++;

        if (iGfx >= bannerBckgrd.transform.childCount)
        {
            iGfx = 0;
        }
    }
}
