using UnityEngine;

public class ResetBJS : MonoBehaviour
{
    BJSSave ourSaveFacility = new BJSSave();
    LoadBJS ourLoadBJS;

    // Start is called before the first frame update
    void Start()
    {
        ourLoadBJS = GetComponent<LoadBJS>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetAllBJS()
    {
        // Remove all saves
        ourSaveFacility.deleteAllSaves();

        // And restart BJS
        ourLoadBJS.loadBJSGame();
    }

    public void RestartBJS()
    {
        // Remove all saves excepted records
        ourSaveFacility.deleteProgressionSaves();

        ourSaveFacility.deleteMaxSaves();

        // And restart BJS
        ourLoadBJS.loadBJSGame();
    }
}
