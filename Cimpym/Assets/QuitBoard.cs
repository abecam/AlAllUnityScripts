using UnityEngine;
using System.Collections;

public class QuitBoard : MonoBehaviour {
	public CreateBoard mainGameScript; // To pause gameplay

	public string mainMenu = "MenuLight";

	private bool quitRequested = false;
	private bool render = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("escape") || Input.GetKey (KeyCode.Escape)) 
		{
			  quitRequested = true;
			 mainGameScript.gameIsInPause = true;
			  render = true;
		}
	}

	Rect _quitWindowRect = new Rect ( Screen.width/2 - ((int )((((float)Screen.width) / 3.0f) / 2)), Screen.height/2 - ((int )((((float)Screen.height) / 4.0f) / 2)), (int )((((float)Screen.width) / 3.0f) ) ,  (int )((((float)Screen.height) / 4.0f) )  );
	
	void OnGUI()
	{
		if ( quitRequested )
		{
			GUI.Window ( 1, _quitWindowRect, QuitWindowFunction, "Quit?" );
		}
	}
	
	void QuitWindowFunction ( int id )
	{
		float screenWDB3 = (((float)Screen.width) / 9.0f);
		float screenHDB3 = (((float)Screen.height) / 10.0f);

		int buttonWith = (int )(screenWDB3 * 0.9f);
		int buttonHeight = (int )(screenHDB3 * 0.9f);
		int startWidth = ((int )(screenWDB3) - buttonWith) / 2;
		int startHeight = (int )(screenHDB3 );

		if ( GUI.Button ( new Rect ( startWidth, startHeight, buttonWith, buttonHeight ), "No" ))
		{
			render = false;
			quitRequested = false;
			mainGameScript.gameIsInPause = false;
		}
		if ( GUI.Button ( new Rect ( startWidth + screenWDB3, startHeight, buttonWith, buttonHeight ), "Menu" ))
		 {
			mainGameScript.gameIsInPause = false;
			Application.LoadLevel(mainMenu);
		}
		if ( GUI.Button ( new Rect ( startWidth + 2*screenWDB3, startHeight , buttonWith, buttonHeight ), "Quit Game" ))
		 {
			Application.Quit();
		}
	}

}