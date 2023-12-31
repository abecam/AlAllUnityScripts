using UnityEngine;
using System.Collections;

public class QuitBoard2ndMethod : MonoBehaviour {

	private bool GuiOn = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("escape") || Input.GetKey (KeyCode.Escape)) 
		{
			GuiOn = true;
		}
	}
	
	void OnGUI()
	{
		if (GuiOn)
		{//check if gui should be on. If false, the gui is off, if true,  the gui is on
				// Make a background box
			GUI.Box (new Rect (Screen.width/2-125, Screen.height/2-25,100,90), "You sure?");
			// Make the first button. If pressed, quit game 
				if (GUI.Button (new Rect (20,40,80,20), "Yes")) {
					Application.Quit();
				}
				// Make the second button.If pressed, sets the var to false so  that gui disappear
				if (GUI.Button (new Rect (20,70,80,20), "No")) {
					GuiOn=false;
				}
		}
	}

}