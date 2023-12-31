using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindingN_AdaptHintHeight : MonoBehaviour
{
    int currentDifficulty = 2;
    public bool isUnderwater = false; // Due to the blurring it needs to be closer

    // Start is called before the first frame update
    void Start()
    {
        // Read the current difficulty level

        bool haskey = LocalSave.HasIntKey("FindingNYAF_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("FindingNYAF_difficulty");
        }
        if (!isUnderwater)
        {
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
        else
        {
            switch (currentDifficulty)
            {
                case 0:
                    transform.localPosition = new Vector3(transform.localPosition.x, 1.3f, transform.localPosition.z);
                    break;
                case 1:
                    transform.localPosition = new Vector3(transform.localPosition.x, 1.5f, transform.localPosition.z);
                    break;
                default:
                    transform.localPosition = new Vector3(transform.localPosition.x, 1.7f, transform.localPosition.z);
                    break;
            }
        }
    }
}
