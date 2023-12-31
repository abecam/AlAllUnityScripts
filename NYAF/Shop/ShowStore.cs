using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowStore : MonoBehaviour
{
    public GameObject theStore;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showTheStore()
    {
        theStore.SetActive(!theStore.activeInHierarchy);
    }
}
