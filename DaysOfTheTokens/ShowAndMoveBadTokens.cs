using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShowAndMoveBadTokens : MonoBehaviour
{
    public GameObject badTokenGeneric;
    private GameObject badToken;

    public GameObject explosionTemplate;
    private GameObject explosion;

    private ManageBox theBox;

    float posX;
    float posY;
    float speedX;
    float speedY;

    float xTarget;
    float yTarget;

    float actualSpeed;

    float orientation;

    float wantedSpeedX, wantedSpeedY, wantedSpeedZ = 0;
    float tmpSpeedX, tmpSpeedY, tmpSpeedZ, tmpSpeedN = 0;

    float currentSpeed = 0; // In knots

    float maxSpeed = 2000; // 800; // In knots
    float standardSpeed = 1200; // 400;
    float accelerationMax = 200; // In knots/sec

    bool isInPursuit = false;
    private bool exploding = false;
    private bool dead = false;
    float distToTarget;
    float stopTargetingRadiusSq = 100;

    TypeOfToken type;

    VisitorTokensAndInfo targetToken = null;

    public enum TypeOfToken
    {
        green, red, blue
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void doSeekAsGreen(ManageBox theBox)
    {
        this.theBox = theBox;
        type = TypeOfToken.green;
        // Instanciate a green token
        badToken = Instantiate(badTokenGeneric);

        // Make it green
        Renderer tokenRendered = badToken.GetComponent<Renderer>();

        tokenRendered.material.color = new Color(0, 1, 0);
        // Find a random start
        posX = Random.value-1;
        posY = Random.value-1;
        // Find a random direction
        speedX = 0.4f * Random.value;
        speedY = 0.4f * Random.value;

        SeekOrMoveToken theTokenScript = tokenRendered.GetComponent<SeekOrMoveToken>();

        theTokenScript.startWorking(type, posX, posY, speedX, speedY, -1f, -1f, explosionTemplate, theBox);
    }

    public void doSeekAsRed(ManageBox theBox)
    {
        this.theBox = theBox;
        type = TypeOfToken.red;

        // Instanciate a red token
        badToken = Instantiate(badTokenGeneric);

        // Make it red
        Renderer tokenRendered = badToken.GetComponent<Renderer>();

        tokenRendered.material.color = new Color(1, 0, 0);
        // Find a random start
        posX = Random.value-1;
        posY = Random.value-1;
        // Find a random direction
        speedX = 0.4f * Random.value;
        speedY = 0.4f * Random.value;

        SeekOrMoveToken theTokenScript = tokenRendered.GetComponent<SeekOrMoveToken>();

        theTokenScript.startWorking(type, posX, posY, speedX, speedY, -1f, -1f, explosionTemplate, theBox);
    }

    public void doSeekAsBlue(ManageBox theBox)
    {
        this.theBox = theBox;
        type = TypeOfToken.blue;

        // Instanciate a blue token
        badToken = Instantiate(badTokenGeneric);

        // Make it blue
        Renderer tokenRendered = badToken.GetComponent<Renderer>();

        tokenRendered.material.color = new Color(0, 0, 1);
        // Find a random start
        posX = Random.value-1;
        posY = Random.value-1;
        // Find the direction to the best token
        VisitorTokensAndInfo bestToken = new VisitorTokensAndInfo();
        long maxNbPass = 0;

        foreach (VisitorTokensAndInfo tmpToken in theBox.AllBoxTokens)
        {
            if (tmpToken.NbSoFar > maxNbPass)
            {
                bestToken = tmpToken;
                maxNbPass = tmpToken.NbSoFar;
            }
        }
        if (maxNbPass > 0)
        {
            Vector2 posToken = bestToken.OurGameObject.transform.position;

            this.xTarget = posToken.x;
            this.yTarget = posToken.y;
        }
        else
        {
            // Like the other ones, nothing to target anyway...
            speedX = 0.1f * Random.value;
            speedY = 0.1f * Random.value;

            this.xTarget = Random.value;
            this.yTarget = Random.value;
        }

        SeekOrMoveToken theTokenScript = tokenRendered.GetComponent<SeekOrMoveToken>();

        theTokenScript.startWorking(type, posX, posY, speedX, speedY, xTarget, yTarget, explosionTemplate, theBox);
    }
}
