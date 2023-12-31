using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndDisappear : MonoBehaviour
{
    public float x, y, z = 0;
    float ySpeed = 0.005f;
    float timeOfStart = 0;
    public float timeToMove = 0.5f; // 2 seconds
    bool isStarted = false;
    float delayBeforeStart = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            if (delayBeforeStart <= 0)
            {
                timeOfStart += Time.deltaTime;
                if (timeOfStart > timeToMove)
                {
                    Destroy(transform.gameObject);
                }

                y = y + ySpeed;
                // And accelerate
                ySpeed += 0.002f;

                transform.position = new Vector3(x, y, z);
            }
            else
            {
                delayBeforeStart -= Time.deltaTime;
            }
        }
    }

    public void setPos(float xStart, float yStart)
    {
        x = xStart;
        y = yStart;

        isStarted = true;

        transform.position = new Vector2(x, y);
    }

    public void setPos3D(float xStart, float yStart, float z)
    {
        x = xStart;
        y = yStart;

        isStarted = true;

        transform.position = new Vector3(x, y, z);
    }

    public void setPosWithSpeed(float xStart, float yStart, float speed)
    {
        x = xStart;
        y = yStart;

        // Also delay start
        isStarted = true;

        ySpeed = speed;

        delayBeforeStart = ySpeed * 100;

        transform.position = new Vector2(x, y);
    }

    internal void setPosWithSpeed3D(float xStart, float yStart, float atPosZ, float speed)
    {
        x = xStart;
        y = yStart;
        z = atPosZ;

        // Also delay start
        isStarted = true;

        ySpeed = speed;

        delayBeforeStart = ySpeed * 100;

        transform.position = new Vector3(x, y, z);
    }
}
