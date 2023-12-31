
var nextScene = "MainBoard";

private var nbOfLoopInDrag = 0; // Should be less than something to start the next scene
private var mouseWasDown = false;
private var nextSceneLoaded = false;

static public var selectedScene = 1;
public var sceneToStart = 1; // Set-up by each menu item

//public enum TypeOfGame
//    {
//        Campaign, // Mix of other modes, earning XPs and "super power" (?). Earn characters
//        QuickPlay, // Raw game, score, no time
//        Score, // Faster-> better score. No time limit. Can go to next level if stuck!
//        TimeAttack, // Need to finish X tables in X minutes (can be selected at start-up, in setup?)
//        Challenges, // A random challenge, or a table of challenges (table better)
//       TimedScore // Unlimited nb of table in a limited time (can be selected ?)
//    };
    
function Update ()
{
	// Make sure the user pressed the mouse down
	if (!Input.GetMouseButton(0))
	{
		if (mouseWasDown && nbOfLoopInDrag < 30)
		{
		    mouseWasDown = false;
		    nbOfLoopInDrag = 0;
		    selectedScene = sceneToStart;
			  Application.LoadLevel(nextScene);
		}
		mouseWasDown = false;
		nbOfLoopInDrag = 0;
		
		return;
	}

	var mainCamera = FindCamera();
		
	// We need to actually hit an object
	var hit : RaycastHit;
	if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),  hit))
		return;
	Debug.Log("Hit "+hit.collider.gameObject.name);
	if (hit.collider.gameObject === gameObject)
	{ 
		Debug.Log("Ok, will increase "+nbOfLoopInDrag);
		if (mouseWasDown)
		{
			nbOfLoopInDrag++;
		}
		else
		{
			mouseWasDown=true;
		}
	}
}

function FindCamera ()
{
	if (camera)
		return camera;
	else
		return Camera.main;
}