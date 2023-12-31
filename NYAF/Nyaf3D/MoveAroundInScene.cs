using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundInScene : MonoBehaviour
{
    public bool areFindable = false;

    private int currentLevel = 0;

    float t = 0;
    float speedFormulas = 0.1f;

    void Start()
    {
        if (LocalSave.HasIntKey("MovingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("MovingNYAF_currentLevel");
        }
        switch (currentLevel)
        {
            case 0:
                speed = new Vector3(0, 0, -0.2f);
                acc = new Vector3(0, 0, -0.04f);
                break;
            case 1:
                speed = new Vector3(0, 0, 0.1f);
                acc = new Vector3(0.01f, 0.001f, 0.02f);
                break;
            case 2:
                speed = new Vector3(0, 0.0f, -0.8f);
                break;
            case 3:
                speed = new Vector3(0.1f, 0, -0.1f);
                acc = new Vector3(0.01f, 0, -0.01f);
                break;
            case 4:
                speed = new Vector3(0.1f, 0, -0.1f);
                acc = new Vector3(0, 0, 0);
                break;
            case 5:
                speed = new Vector3(0, 0, -0.1f);
                acc = new Vector3(0, 0, -0.01f);
                break;
            case 6:
                speed = new Vector3(0, 0, -0.1f);
                acc = new Vector3(0, 0, -0.01f);
                break;
            case 7:
                speed = new Vector3(0, 0, -0.1f);
                acc = new Vector3(0, 0, -0.01f);
                break;
            case 8:
                speed = new Vector3(0, 0, -0.1f);
                acc = new Vector3(0, 0, -0.01f);
                break;
            case 9:
                speed = new Vector3(0, 0, -0.1f);
                acc = new Vector3(0, 0, -0.01f);
                break;
            case 10:
                speed = new Vector3(0, 0.0f, -0.8f);
                break;
            case 15:
                speed = new Vector3(0, 0.0f, 2f);
                acc = new Vector3(0, 0, 0.02f);
                break;
            case 16:
                speed = new Vector3(0, 0.0f, -3f);
                acc = new Vector3(0, 0, -0.02f);
                break;
            case 17:
                speed = new Vector3(0, 0.0f, 5f);
                acc = new Vector3(0, 0, 0.04f);
                break;
        }
        int currentDifficulty = 2;

        bool haskey = LocalSave.HasIntKey("MovingNYAF_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("MovingNYAF_difficulty");
        }
        switch (currentDifficulty)
        {
            case 0:
                speedFormulas = 0.04f;
                maxSpeed = 0.04f;
                break;
            case 1:
                speedFormulas = 0.06f;
                maxSpeed = 0.06f;
                break;
            case 2:
                speedFormulas = 0.08f;
                maxSpeed = 0.08f;
                break;
            case 3:
                speedFormulas = 0.1f;
                maxSpeed = 0.1f;
                break;
            case 4:
                speedFormulas = 0.12f;
                maxSpeed = 0.12f;
                break;
            case 5:
                speedFormulas = 0.14f;
                maxSpeed = 0.14f;
                break;
            case 6:
                speedFormulas = 0.16f;
                maxSpeed = 0.16f;
                break;
            case 7:
                speedFormulas = 0.2f;
                maxSpeed = 0.2f;
                break;
        }
    }

    Vector3 speed = new Vector3();
    Vector3 acc = new Vector3();

    float maxSpeed = 0.1f;

    bool inAcc = true;
    bool notStartDecc = true;

    // Update is called once per frame
    void Update()
    {
        if (currentLevel != 2 && currentLevel != 10 && currentLevel < 11)
        {
            transform.position += Time.deltaTime * speed;

            if (transform.position.x > 20 && transform.position.x < 200)
            {
                transform.position = new Vector3(-20, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -20)
            {
                transform.position = new Vector3(20, transform.position.y, transform.position.z);
            }
            if (transform.position.y > 20)
            {
                transform.position = new Vector3(transform.position.x, -20, transform.position.z);
            }
            else if (transform.position.y < -20)
            {
                transform.position = new Vector3(transform.position.x, 20, transform.position.z);
            }
            if (transform.position.z > 20)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 40);
            }
            //else if (transform.position.z < -2 && areFindable)
            //{
            //    transform.position = new Vector3(transform.position.x, -transform.position.y, 4);
            //}
            else if (transform.position.z < -20)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 40);
            }

            if (inAcc)
            {
                speed += acc;

                speed += new Vector3(0.02f * (Random.value - 0.5f), 0.02f * (Random.value - 0.5f), 0.02f * (Random.value - 0.5f));

                if (speed.sqrMagnitude > maxSpeed)
                {
                    inAcc = false;
                }
            }
            else if (currentLevel == 3 || currentLevel == 7 || currentLevel == 9)
            {
                speed -= acc;

                if (speed.sqrMagnitude > maxSpeed && speed.x < 0)
                {
                    inAcc = true;
                }
            }
            else if (currentLevel == 5)
            {
                speed -= acc;

                if (speed.sqrMagnitude > maxSpeed * 4)
                {
                    inAcc = true;
                }
            }
            Vector3 maxAllowedSpeed = 4*speed.normalized;
            if (speed.sqrMagnitude > maxAllowedSpeed.sqrMagnitude)
            {
                speed = maxAllowedSpeed;
            }
        }
        else if (currentLevel < 11)
        {
            if (transform.localPosition.x < 200)
            {
                // Take our direction and try to go up a bit while continuing to go front...
                transform.localPosition = new Vector3(ourInitialPos.x * (2.8f+ 2.4f * Mathf.Cos(t * 0.1f +ourShiftT)), ourInitialPos.y * (2.8f + 2.4f * Mathf.Cos(t * 0.1f + ourShiftT)),  (6 * Mathf.Sin(t * 0.1f + ourShiftT)));

                //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]
                //transform.position += (ourPosInCircle * ourFront);

                t += speedFormulas;
            }
        }
        else if (currentLevel == 11)
        {
            if (transform.localPosition.x < 200)
            {
                // Take our direction and try to go up a bit while continuing to go front...
                transform.localPosition = new Vector3(ourInitialPos.x + 1.5f * Mathf.Cos(t * 0.1f + ourShiftT)* Mathf.Sin(t * 0.2f + ourShiftT), ourInitialPos.y + 1.5f * Mathf.Cos(t * 0.1f + ourShiftT) * Mathf.Sin(t * 0.2f + ourShiftT), (2 + 4 * Mathf.Sin(t * 0.1f + ourShiftT)));

                //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]
                //transform.position += (ourPosInCircle * ourFront);

                t += speedFormulas;
            }
        }
        else if (currentLevel == 12)
        {
            if (transform.localPosition.x < 200)
            {
                // Take our direction and try to go up a bit while continuing to go front...
                transform.localPosition = new Vector3(ourInitialPos.x + 1.5f * Mathf.Cos(t * 0.1f + ourShiftT), ourInitialPos.y + 1.5f * Mathf.Cos(t * 0.1f + ourShiftT), (2 + 4 * Mathf.Sin(t * 0.1f + ourShiftT)));

                //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]
                //transform.position += (ourPosInCircle * ourFront);

                t += speedFormulas;
            }
        }
        else if (currentLevel == 13)
        {
            if (transform.localPosition.x < 200)
            {
                // Take our direction and try to go up a bit while continuing to go front...
                transform.localPosition = new Vector3(ourInitialPos.x * (2.8f + 2.4f * Mathf.Sin(t * 0.1f) * Mathf.Cos(ourShiftT)), ourInitialPos.y * (2.8f + 2.4f * Mathf.Sin(t * 0.1f) * Mathf.Sin(ourShiftT)), (6 * Mathf.Cos(t * 0.1f)*Mathf.Sin(t * ourShiftT * 0.2f)));

                //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]
                //transform.position += (ourPosInCircle * ourFront);

                t += speedFormulas;
            }
        }
        else if (currentLevel == 14)
        {
            if (transform.localPosition.x < 200)
            {
                // Take our direction and try to go up a bit while continuing to go front...
                transform.localPosition = new Vector3(ourInitialPos.x + (2.4f * Mathf.Sin(t * 0.1f) * Mathf.Cos(ourShiftT)), ourInitialPos.y + (2.4f * Mathf.Sin(t * 0.1f) * Mathf.Sin(ourShiftT)), ourInitialPos.z + (2.4f * Mathf.Cos(t * 0.1f)));

                //Cos[u](3 + Cos[t]), Sin[u](3 + Cos[t]), Sin[t]
                //transform.position += (ourPosInCircle * ourFront);

                t += speedFormulas;
            }
        }
        else if ((currentLevel == 15) || (currentLevel <= 17))
        {
            if (transform.localPosition.x < 200)
            {
                transform.position += Time.deltaTime * speed;

                if (transform.position.x > 20)
                {
                    transform.position = new Vector3(-20, transform.position.y, transform.position.z);
                }
                else if (transform.position.x < -20)
                {
                    transform.position = new Vector3(20, transform.position.y, transform.position.z);
                }
                if (transform.position.y > 20)
                {
                    transform.position = new Vector3(transform.position.x, -20, transform.position.z);
                }
                else if (transform.position.y < -20)
                {
                    transform.position = new Vector3(transform.position.x, 20, transform.position.z);
                }
                if (transform.position.z > 20)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 40);
                }
                //else if (transform.position.z < -2 && areFindable)
                //{
                //    transform.position = new Vector3(transform.position.x, -transform.position.y, 4);
                //}
                else if (transform.position.z < -20)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 40);
                }

                if (inAcc)
                {
                    speed += acc;

                    if (speed.sqrMagnitude > maxSpeed)
                    {
                        inAcc = false;
                    }
                }

                Vector3 maxAllowedSpeed = 4 * speed.normalized;
                if (speed.sqrMagnitude > maxAllowedSpeed.sqrMagnitude)
                {
                    speed = maxAllowedSpeed;
                }
            }
        }
    }

    Vector3 ourInitialPos;
    float ourShiftT = 0;
    internal void setInitialPos(float dummy)
    {
        ourInitialPos = transform.position;

        ourShiftT = Random.value*Mathf.PI*2;
    }
}
