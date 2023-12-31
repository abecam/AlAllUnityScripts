using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOutroDayText : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMesh level;
    private int currentDay = 0;

    // Start is called before the first frame update
    void Start()
    {
        level = GetComponent<TextMesh>();

        setText();
    }

    // Update is called once per frame
    void Update()
    {
        level.text = "Day " + currentDay+"\n5 am";
    }

    void setText()
    {
        
        if (LocalSave.HasIntKey("BJS_CurrentDay"))
        {
            currentDay = LocalSave.GetInt("BJS_CurrentDay");
        }

        if (currentDay < 500)
        {
            currentDay = 1000;
        }
        else if (currentDay < 10000)
        {
            currentDay = 10000;
        }
        else
        {
            currentDay = 100000;
        }
    }
}
