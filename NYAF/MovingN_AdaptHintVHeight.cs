using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingN_AdaptHintVHeight : MonoBehaviour
{
    int currentDifficulty = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Read the current difficulty level

        bool haskey = LocalSave.HasIntKey("MovingNYAF_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("MovingNYAF_difficulty");
        }
        switch (currentDifficulty)
        {
            case 0:
                transform.localPosition = new Vector3(transform.localPosition.x, 2.3f, transform.localPosition.z);
                break;
            case 1:
                transform.localPosition = new Vector3(transform.localPosition.x, 2.5f, transform.localPosition.z);
                break;
            default:
                transform.localPosition = new Vector3(transform.localPosition.x, 2.7f, transform.localPosition.z);
                break;
        }
    }
}
