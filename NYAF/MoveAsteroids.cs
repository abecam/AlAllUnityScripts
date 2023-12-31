using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAsteroids : MonoBehaviour
{
    public bool areFindable = false;

    void Start()
    {
        Vector3 initialTrust = new Vector3(600f * (Random.value - 0.5f), 600f * (Random.value - 0.5f), 600f * (Random.value - 0.5f));
        Vector3 initialTrust2 = new Vector3(5f * (Random.value - 0.5f), 5f * (Random.value - 0.5f), 5f * (Random.value - 0.5f));
        Vector3 initialPos = new Vector3(2f * (Random.value - 0.5f), 2f * (Random.value - 0.5f), 2f * (Random.value - 0.5f));
        this.GetComponent<Rigidbody>().AddForceAtPosition(initialTrust2, initialPos);
        this.GetComponent<Rigidbody>().AddForce(initialTrust);
    }

    // Update is called once per frame
    void Update()
    {
        //speed += new Vector3(0.02f * (Random.value - 0.5f), 0.02f * (Random.value - 0.5f), 0.02f * (Random.value - 0.5f));
    }

}
