using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShowAndMoveBadTokens;

public class SeekOrMoveToken : MonoBehaviour
{
    private GameObject explosionTemplate;
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

    float maxSpeed = 1; // 800; // In knots
    float standardSpeed = 0.5f; // 400;
    float accelerationMax = 0.2f; // In knots/sec

    bool isInPursuit = false;
    private bool exploding = false;
    private bool dead = false;
    float distToTarget;
    float stopTargetingRadiusSq = 0.02f;

    ShowAndMoveBadTokens.TypeOfToken type;

    VisitorTokensAndInfo targetToken = null;

    int stepsToDie = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool hasTarget = false;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update, inPursuit " + isInPursuit + " hasTarget " + hasTarget+" - speed "+speedX+", "+speedY);

        // If needed, move the bad token from initial to target.   
        if (isInPursuit)
        {
            // Green follows a fixed direction
            // So do nothing
            // But check if we touched something
            if (type == TypeOfToken.green)
            {
                checkIfExplodeGreen();
            }
            // Red seeks whatever it finds
            else
            {
                if (type == TypeOfToken.red)
                {
                    if (!hasTarget)
                    {
                        hasTarget = seek();
                    }
                    else
                    {
                        seekWP();
                    }
                }
                // Blue seeks biggest token
                else if (type == TypeOfToken.blue)
                {
                    seekWP();
                }

                // Check if close enough
                checkIfExplode();
            }

            // In all cases move the token
            updatePosition(Time.deltaTime);
        }

        if (exploding)
        {
            // Do something until we die...
            if (stepsToDie > 0)
            {
                transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

                stepsToDie--;
            }
            else
            {
                Destroy(transform.gameObject);
            }
        }
    }

    internal void startWorking(TypeOfToken type, float posX, float posY, float speedX, float speedY, float xTarget, float yTarget, GameObject explosionTemplate, ManageBox theBox)
    {
        isInPursuit = true;

        this.type = type;
        this.posX = posX;
        this.posY = posY;
        this.speedX = speedX;
        this.speedY = speedY;
        this.xTarget= xTarget;
        this.yTarget = yTarget;

        this.explosionTemplate = explosionTemplate;

        this.theBox = theBox;

        ourTokens = theBox.AllBoxTokens;
    }

    private void checkIfExplodeGreen()
    {
        foreach (VisitorTokensAndInfo tmpToken in ourTokens)
        {
            if (tmpToken.OurGameObject != null)
            {
                Vector2 posToken = tmpToken.OurGameObject.transform.position;

                distToTarget = distSq(this.posX, this.posY, posToken.x, posToken.y);
                if (distToTarget < stopTargetingRadiusSq)
                {
                    // Explode !!!
                    isInPursuit = false;
                    exploding = true;
                    dead = true;

                    stepsToDie = 20;

                    explosion = Instantiate(explosionTemplate);

                    explosion.transform.position = new Vector3(this.posX, this.posY, 0);
                    explosion.SetActive(true);

                    // And inform the box to remove the target Token
                    damageNeighbours();

                    break;
                }
            }
        }
    }

    private void checkIfExplode()
    {
        distToTarget = distSq(this.posX, this.posY, this.xTarget, this.yTarget);
        if (distToTarget < stopTargetingRadiusSq)
        {
            // Explode !!!
            isInPursuit = false;
            exploding = true;
            dead = true;

            stepsToDie = 20;

            explosion = Instantiate(explosionTemplate);

            explosion.transform.position = new Vector3(this.posX, this.posY, 0);
            explosion.SetActive(true);

            // And inform the box to remove the target Token
            damageNeighbours();
        }
    }

    float damageRadiusSq = 1;

    private void damageNeighbours()
    {
        float distanceTmp;

        foreach (VisitorTokensAndInfo tmpToken in ourTokens)
        {
            if (tmpToken.OurGameObject != null)
            {
                // Update !!!
                Vector2 posToken = tmpToken.OurGameObject.transform.position;
                // Update !!!


                distanceTmp = distSq(this.posX, this.posY, posToken.x, posToken.y);
                //System.out.println("Found our sub - dist "+distanceTmp);
                if (distanceTmp < damageRadiusSq)
                {
                    Debug.Log("Would move token " + tmpToken.tokenId + " (" + tmpToken.NbSoFar + ") to the vault");
                    theBox.moveFromBoxToVault(tmpToken.tokenId);
                }
            }
        }
    }

    private void updatePosition(float time)
    {
        this.posX = this.posX + time * speedX; // this.currentSpeed*Math.cos(this.orientation);
        this.posY = this.posY + time * speedY; // this.currentSpeed*Math.sin(this.orientation);

        transform.position = new Vector2(posX, posY);

        if (posX > 2)
        {
            Destroy(transform.gameObject);
        }
        if (posY > 2)
        {
            Destroy(transform.gameObject);
        }
    }

    float detectionStrength = 1;
    float power = 10; // Power of its sensor.
    float angle = Mathf.PI / 4; // Angle of detection
    private List<VisitorTokensAndInfo> ourTokens;

    public bool seek()
    {
        // For this type of missile: front radar-based.
        float xT2, yT2, xT3, yT3; // Coordinate (+ position of the Missile), of the corner of the detection triangle

        float distanceTmp;


        bool found = false;
        float distanceFound = 100000; // Reset the distance of detection.

        // Determine the detection triangle...
        xT2 = this.posX + detectionStrength * power * 30 * Mathf.Cos(this.orientation + this.angle / 2);
        yT2 = this.posY + detectionStrength * power * 30 * Mathf.Sin(this.orientation + this.angle / 2);
        xT3 = this.posX + detectionStrength * power * 30 * Mathf.Cos(this.orientation - this.angle / 2);
        yT3 = this.posY + detectionStrength * power * 30 * Mathf.Sin(this.orientation - this.angle / 2);

        //detectLine1.setPos(this.posX, this.posY, 0);
        //detectLine1.setPosEnd(xT2, yT2);

        //detectLine2.setPos(this.posX, this.posY, 0);
        //detectLine2.setPosEnd(xT3, yT3);

        //detectLine3.setPos(xT3, yT3, 0);
        //detectLine3.setPosEnd(xT2, yT2);


        ourTokens = theBox.AllBoxTokens;

        foreach (VisitorTokensAndInfo tmpToken in ourTokens)
        {
            if (tmpToken.OurGameObject != null)
            {
                // Update !!!
                Vector2 posToken = tmpToken.OurGameObject.transform.position;

                if (isInTriangle(posToken.x, posToken.y, this.posX, this.posY, xT2, yT2, xT3, yT3))
                {
                    distanceTmp = distSq(this.posX, this.posY, posToken.x, posToken.y);
                    //System.out.println("Found our sub - dist "+distanceTmp);
                    if (distanceTmp < distanceFound)
                    {
                        distanceFound = distanceTmp;
                        this.xTarget = posToken.x;
                        this.yTarget = posToken.y;

                        found = true;
                    }
                }
            }
        }

        return found;
    }

    public static bool isInTriangle(float x, float y, float xT1, float yT1, float xT2, float yT2, float xT3, float yT3)
    {
        // Calculate the three sub-triangle and check the surface.
        float surfaceTotal;

        surfaceTotal = calculateAreaTriangle(x, y, xT1, yT1, xT2, yT2);
        surfaceTotal += calculateAreaTriangle(x, y, xT2, yT2, xT3, yT3);
        surfaceTotal += calculateAreaTriangle(x, y, xT3, yT3, xT1, yT1);

        if (surfaceTotal > calculateAreaTriangle(xT1, yT1, xT2, yT2, xT3, yT3))
            return false;
        else
            return true;
    }

    public static float calculateAreaTriangle(float xT1, float yT1, float xT2, float yT2, float xT3, float yT3)
    {
        xT2 -= xT1;
        yT2 -= yT1;
        xT3 -= xT1;
        yT3 -= yT1;
        xT1 = 0;
        yT1 = 0;
        return (0.5f * Mathf.Abs(xT2 * yT3 - yT2 * xT3));
    }

    public void checkAndNormaliseSpeed()
    {
        actualSpeed = Mathf.Sqrt(speedX * speedX + speedY * speedY);


        if (speedX != 0)
        {
            this.orientation = Mathf.Acos(speedX / actualSpeed);
        }
        else
        {
            this.orientation = Mathf.PI / 2;
        }
        if (speedY < 0)
        {
            this.orientation = -this.orientation + 2 * Mathf.PI;
        }

        if (actualSpeed > this.maxSpeed)
        {
            this.speedX = maxSpeed * (this.speedX / actualSpeed);
            this.speedY = maxSpeed * (this.speedY / actualSpeed);

            //this.currentSpeed = this.maxSpeed;	
        }
        //System.out.println("Current speed "+currentSpeed);
    }

    public static float distSq(float x, float y, float x1, float y1)
    {
        return (Mathf.Pow(x - x1, 2) + Mathf.Pow(y - y1, 2));
    }

    public void seekWP()
    {
        {
            this.tmpSpeedX = (this.xTarget - this.posX);
            this.tmpSpeedY = (this.yTarget - this.posY);
            this.tmpSpeedN = Mathf.Sqrt(tmpSpeedX * tmpSpeedX + tmpSpeedY * tmpSpeedY);
            this.wantedSpeedX = this.tmpSpeedX / tmpSpeedN;
            this.wantedSpeedY = this.tmpSpeedY / tmpSpeedN;
            this.wantedSpeedX *= this.standardSpeed / 4;
            this.wantedSpeedY *= this.standardSpeed / 4;

            // Try to accelerate
            if ((distToTarget < stopTargetingRadiusSq * 2) && (this.currentSpeed > this.standardSpeed))
                this.currentSpeed -= 2;
            else if (this.currentSpeed < this.maxSpeed)
                this.currentSpeed += 2;

            //checkAndNormaliseSpeed();

            this.accX(wantedSpeedX);
            this.accY(wantedSpeedY);

            checkAndNormaliseSpeed();
        }
    }

    public void accX(float xAcc)
    {
        speedX += xAcc;
        //checkAndNormaliseSpeed();
    }

    public void accY(float yAcc)
    {
        speedY += yAcc;
        //checkAndNormaliseSpeed();
    }
}
