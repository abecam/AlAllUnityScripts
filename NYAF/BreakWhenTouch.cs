using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWhenTouch : MonoBehaviour
{
    public Explodable ourExplodable;
    public GameObject containerOfFragment;
    private BJSMainLoop mainScript;

    // Start is called before the first frame update
    void Start()
    {
        ourExplodable.newParent = containerOfFragment;

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<BJSMainLoop>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("We explode!!!");
        ourExplodable.explodeBJSOutro();

        containerOfFragment.GetComponent<AudioSource>().Play();

        mainScript.sunIsBroken();
    }
}
