using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceLightCave : MonoBehaviour
{
    private int xSize = 5;
    private int zSize = 2;

    private int currentLevel = 0;

    public GameObject lightToInstantiate;

    // Start is called before the first frame update
    void Start()
    {
        if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("FindingNYAF_currentLevel");
        }

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float xShift = ((float)(x - xSize / 2)) * 8;
                float zShift = ((float)z) * 5;

                float xCurr, yCurr;

                float zOut; // Ignored here

                GameObject oneNewLight = Instantiate(lightToInstantiate);
                oneNewLight.transform.SetParent(this.transform);

                PlaceCharacters.applyFunction(xShift, 0, zShift, out xCurr, out yCurr, out zOut, currentLevel);

                oneNewLight.transform.localPosition = new Vector3(xCurr, yCurr+1, zShift - 4);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
