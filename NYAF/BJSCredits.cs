using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BJSCredits : MonoBehaviour
{
    private GameObject johnWalk;
    private GameObject johnReturn;
    private GameObject triumphantJohn;

    Rigidbody2D johnWalkRB;
    Rigidbody2D johnReturnRB;
    Rigidbody2D triumphantJohnRB;

    private GameObject title;

    public GameObject creditText;

    AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        musicSource = GetComponent<AudioSource>();

        johnWalk = GameObject.FindWithTag("JohnWalk");
        johnWalkRB = johnWalk.GetComponent<Rigidbody2D>();

        johnReturn = GameObject.FindWithTag("JohnReturn");
        johnReturnRB = johnReturn.GetComponent<Rigidbody2D>();

        triumphantJohn = GameObject.FindWithTag("JohnTriumphant");
        triumphantJohnRB = triumphantJohn.GetComponent<Rigidbody2D>();

        title = GameObject.FindWithTag("Title");

        SetCreditText();

        creditText.transform.position = new Vector3(0, -5, -5);
    }

    private void SetCreditText()
    {
        TextMesh textToFill = creditText.GetComponent<TextMesh>();

        textToFill.text = "Brother John\nSimulator\n2.4.27\n"
            + "A very serious\nSimulation\nBy Alain Becam\n\nGraphics and ideas by\nSébastien Lesage\n\nMusic by\nJean-Philippe Rameau\nand Chris Collins\n\nFont by Gluk\nGlametrixBold\n\nDoor destruction:\nUnity-2D-Destruction\n(c) 2016 Matthew Holtzem\nunder MIT Licence\n\nBreaking sound from\nhttp://www.universal-soundbank.com/";
    }

    int iTurn = 0;
    int nextTurn = 10;
    // Update is called once per frame
    void Update()
    {
        if (title == null)
        {
            PlayMusic();

            creditText.transform.position = new Vector3(0, creditText.transform.position.y + 0.01f, -5);
            if (creditText.transform.position.y > 18)
            {
                creditText.transform.position = new Vector3(0, -5, -5);
            }
        }
        iTurn++;

        if (iTurn > nextTurn)
        {
            float newForcex = 10 *Random.value-5;
            float newForcey = 10 * Random.value-5;

            johnWalkRB.AddForce(new Vector2(newForcex, newForcey));

            newForcex = 10 * Random.value - 5;
            newForcey = 10 * Random.value - 5;

            johnReturnRB.AddForce(new Vector2(newForcex, newForcey));

            newForcex = 10 * Random.value - 5;
            newForcey = 10 * Random.value - 5;

            triumphantJohnRB.AddForce(new Vector2(newForcex, newForcey));

            iTurn = 0;
            nextTurn = 120 * ((int )Random.value);
        }
    }

    private void PlayMusic()
    {
        if (musicSource != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }
}
