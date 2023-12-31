using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfAvailable : MonoBehaviour
{
    bool isAvailable = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        bool haskey = false;

        // First level has been passed, so there at at least some closed units
        bool canAttack = false;

        haskey = LocalSave.HasBoolKey("HeroAttack");
        if (haskey)
        {
            canAttack = LocalSave.GetBool("HeroAttack");
        }
        if (canAttack)
        {
            isAvailable = true;
        }

        gameObject.SetActive(isAvailable);
    }
}
