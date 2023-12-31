using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStars : MonoBehaviour
{
    public bool areFindable = false;
    Rigidbody rigidbodyStar;

    void Start()
    {
        Vector3 initialTrust = new Vector3(100f * (Random.value - 0.5f), 100f * (Random.value - 0.5f), 100f * (Random.value - 0.5f));
        Vector3 initialTrust2 = new Vector3(1f * (Random.value - 0.5f), 1f * (Random.value - 0.5f), 1f * (Random.value - 0.5f));
        Vector3 initialPos = new Vector3(2f * (Random.value - 0.5f), 2f * (Random.value - 0.5f), 2f * (Random.value - 0.5f));
        rigidbodyStar = this.GetComponent<Rigidbody>();
        rigidbodyStar.AddForceAtPosition(initialTrust2, initialPos);
        rigidbodyStar.AddForce(initialTrust);
    }

    // Update is called once per frame
    void Update()
    {
        // Push a bit from time to time

        if (Random.value > 0.9f)
        {
            Vector3 initialTrust = new Vector3(10f * (Random.value - 0.5f), 10f * (Random.value - 0.5f), 10f * (Random.value - 0.5f));
            Vector3 initialTrust2 = new Vector3(1f * (Random.value - 0.5f), 1f * (Random.value - 0.5f), 1f * (Random.value - 0.5f));
            Vector3 initialPos = new Vector3(2f * (Random.value - 0.5f), 2f * (Random.value - 0.5f), 2f * (Random.value - 0.5f));

            rigidbodyStar.AddForceAtPosition(initialTrust2, initialPos);
            rigidbodyStar.AddForce(initialTrust);
        }
    }
}
