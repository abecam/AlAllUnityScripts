using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMPGGetNbOfUnitsEnemies : MonoBehaviour
{
    private Text level;
    private ManageNYPB mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageNYPB>();
        level = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        level.text = "Enemies\n\n" + "Close " + mainScript.CurrentNbOfClosedEnemies + " / " + (mainScript.NbOfClosedEnemies>0? mainScript.NbOfClosedEnemies:0) + "\n" +
            "Ranged " + mainScript.CurrentNbOfRangedEnemies + " / " + (mainScript.NbOfRangedEnemies > 0 ? mainScript.NbOfRangedEnemies : 0) + "\n" +
            "Tank " + mainScript.CurrentNbOfMachinesEnemies + " / " + (mainScript.NbOfMachinesEnemies > 0 ? mainScript.NbOfMachinesEnemies : 0) + "\n";
    }
}
