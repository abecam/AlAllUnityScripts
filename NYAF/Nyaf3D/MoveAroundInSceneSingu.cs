using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundInSceneSingu : MonoBehaviour
{
    float movementSpeed = 0.1f;
    Vector3 ourOrigin;
    Vector2 target;
    Vector2 ourPosInOurSpace;
    public bool areFindable = false;

    // Start is called before the first frame update
    void Start()
    {
        ourOrigin = transform.position;

        findNewTarget();

        ourPosInOurSpace = new Vector2(0, 0);
    }

    private void findNewTarget()
    {
        float targetX = 0.1f * Random.value - 0.05f;
        float targetY = 0.1f * Random.value - 0.05f;

        target = new Vector2(target.x, target.y);

        accX = targetX / 40;
        accY = targetY / 40;

        inAcc = true;
        notStartDecc = true;

        Debug.Log("Will start accelerating");
    }

    float speedX = 0.0f;
    float speedY = 0.0f;

    float accX = 0.00f;
    float accY = 0.00f;

    float maxSpeed = 0.05f;

    bool inAcc = true;
    bool notStartDecc = true;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speedX;
        transform.position += transform.right * Time.deltaTime * speedY;

        if (transform.position.x > 20 && transform.position.x < 200)
        {
            transform.position = new Vector3(10, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -20)
        {
            transform.position = new Vector3(-10, transform.position.y, transform.position.z);
        }
        if (transform.position.y > 20)
        {
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }
        else if (transform.position.y < -20)
        {
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
        }
        if (transform.position.z > 20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        }
        else if (transform.position.z < -2 && areFindable)
        {
            transform.position = new Vector3(transform.position.x, -transform.position.y, 4);
        }
        else if (transform.position.z < -20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }

        ourPosInOurSpace.x += Time.deltaTime * speedX;
        ourPosInOurSpace.y += Time.deltaTime * speedY;

        if (inAcc)
        {
            speedX += accX;
            speedY += accY;

            if (speedX > maxSpeed)
            {
                inAcc = false;
            }
            else if (speedY > maxSpeed)
            {
                inAcc = false;
            }
        }
        else if (notStartDecc)
        {
            if (Mathf.Abs(ourPosInOurSpace.x - target.x) < Mathf.Abs(target.x / 2))
            {
                notStartDecc = false;

                Debug.Log("Starting decelerating");
            }
        }
        else
        {
            speedX -= accX;
            speedY -= accY;

            if (Mathf.Abs(ourPosInOurSpace.x - target.x) < 0.01f)
            {
                Debug.Log("Finding new target");
                findNewTarget();
            }
        }
    }
}
