using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInWater : MonoBehaviour
{
    public bool areFindable = false;

    private int currentLevel = 0;

    public float waterLevel = 3.2f;
    private float groundLevel = 0.9f;

    void Start()
    {
        if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("FindingNYAF_currentLevel");
        }
        switch (currentLevel)
        {
            case 11:
                speed = new Vector3(0, 0, 0.00f);
                acc = new Vector3(0, 0, 0.00f);
                break;
            case 12:
                speed = new Vector3(0, 0, 0.00f);
                acc = new Vector3(0, 0, 0.00f);
                //acc = new Vector3(0.01f, 0.001f, 0.000f);
                break;
            //case 2:
            //    speed = new Vector3(0, 0.0f, -0.00f);
            //    break;
            //case 3:
            //    speed = new Vector3(0.1f, 0, -0.00f);
            //    acc = new Vector3(0.01f, 0, -0.000f);
            //    break;
            //case 4:
            //    speed = new Vector3(0.1f, 0, -0.00f);
            //    acc = new Vector3(0, 0, 0);
            //    break;
            //case 5:
            //    speed = new Vector3(0, 0, -0.00f);
            //    acc = new Vector3(0, 0, -0.000f);
            //    break;
            //case 6:
            //    speed = new Vector3(0, 0, -0.00f);
            //    acc = new Vector3(0, 0, -0.000f);
            //    break;
            //case 7:
            //    speed = new Vector3(0, 0, -0.00f);
            //    acc = new Vector3(0, 0, -0.000f);
            //    break;
            //case 8:
            //    speed = new Vector3(0, 0, -0.00f);
            //    acc = new Vector3(0, 0, -0.000f);
            //    break;
            //case 9:
            //    speed = new Vector3(0, 0, -0.00f);
            //    acc = new Vector3(0, 0, -0.000f);
            //    break;
            //case 10:
            //    speed = new Vector3(0, 0.0f, -0.00f);
            //    break;
            //case 15:
            //    speed = new Vector3(0, 0.0f, 0f);
            //    acc = new Vector3(0, 0, 0.000f);
            //    break;
            //case 16:
            //    speed = new Vector3(0, 0.0f, 0f);
            //    acc = new Vector3(0, 0, -0.000f);
            //    break;
            //case 17:
            //    speed = new Vector3(0, 0.0f, 0f);
            //    acc = new Vector3(0, 0, 0.000f);
            //    break;
        }

        // Find the ground level

        groundLevel = GenerateGround.highestGround + 0.01f;
    }

    Vector3 speed = new Vector3();
    Vector3 acc = new Vector3();

    float maxSpeed = 0.002f;

    bool inAcc = true;
    bool notStartDecc = true;

    // Update is called once per frame
    void Update()
    {
        //if (currentLevel != 2 && currentLevel != 10 && currentLevel < 11)
        {
            if (transform.position.x < 200)
            {
                transform.position += Time.deltaTime * speed;
            }
            bool wasConstrained = false;

            if (transform.position.x > 14 && transform.position.x < 200)
            {
                wasConstrained = true;

                speed.x -= 0.0002f;
            }
            else if (transform.position.x < -14)
            {
                wasConstrained = true;

                speed.x += 0.0002f;
            }
            if (transform.position.y > waterLevel)
            {
                wasConstrained = true;

                speed.y -= 0.0005f;
            }
            else if (transform.position.y < groundLevel)
            {
                wasConstrained = true;

                speed.y = 0.005f;
            }
            if (transform.position.z > 20)
            {
                wasConstrained = true;

                speed.z -= 0.0005f;
            }
            else if (transform.position.z < -20)
            {
                wasConstrained = true;

                speed.z += 0.0005f;
            }
            
            if (!wasConstrained)
            {
                //Debug.Log("====");

                acc = new Vector3(0.001f * (Random.value - 0.5f), 0.001f * (Random.value - 0.5f), 0);

                //speed = speed - 0.1f * speed;
            }
            else
            {
                acc = new Vector3(0.0005f * (Random.value - 0.5f), 0.0005f * (Random.value - 0.5f), 0);
            }

            {
                speed += acc;
            }
           
            Vector3 maxAllowedSpeed = speed.normalized;
            if (speed.sqrMagnitude > maxAllowedSpeed.sqrMagnitude)
            {
                speed = maxAllowedSpeed;
            }
        }
    }

    /*
     * if (transform.position.x > 20 && transform.position.x < 200)
            {
                transform.position = new Vector3(-20, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -20)
            {
                transform.position = new Vector3(20, transform.position.y, transform.position.z);
            }
            if (transform.position.y > waterLevel)
            {
                transform.position = new Vector3(transform.position.x, waterLevel, transform.position.z);
            }
            else if (transform.position.y < groundLevel)
            {
                transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
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

                float newYSpeed = 0.002f * (Random.value - 0.5f);

                if (transform.position.y < groundLevel2)
                {
                    // Don't go down if bellow the 2nd ground level
                    newYSpeed = 0.001f * (Random.value);
                }

                speed += new Vector3(0.002f * (Random.value - 0.5f), newYSpeed, 0);

                if (speed.sqrMagnitude > maxSpeed)
                {
                    inAcc = false;
                }
            }
            else //if (currentLevel == 3 || currentLevel == 7 || currentLevel == 9)
            {
                speed -= acc;

                if (speed.sqrMagnitude > maxSpeed && speed.x < 0)
                {
                    inAcc = true;
                }
            }
            //else if (currentLevel == 5)
            //{
            //    speed -= acc;

            //    if (speed.sqrMagnitude > maxSpeed * 4)
            //    {
            //        inAcc = true;
            //    }
            //}
     */

    Vector3 ourInitialPos;
    float ourShiftT = 0;
    internal void setInitialPos()
    {
        ourInitialPos = transform.position;
        groundLevel = ourInitialPos.y;

        ourShiftT = Random.value * Mathf.PI * 2;
    }
}
