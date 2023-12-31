using UnityEngine;
using System.Collections;

public class CreateBoard : MonoBehaviour {
	public bool gameIsInPause = false; // Allow to not take any more selection, like when a menu is displayeds
	private bool inGameOver = false; // Game won or lost, to show the end message

	public int numberOfRows = 16;
	public int numberOfColumns = 10;
	
	float spaceBetweenTiles = 2.7f;
	
	public GameObject CircleCut;
	public GameObject CircleFull;
	public GameObject CircleIn;
	//	public GameObject CircleOut;
	public GameObject Line;
	public GameObject LineBroken;
	public GameObject LineEnd;
	public GameObject LineHalf;
	public GameObject LineMiddle;
	public GameObject LineSide;
	public GameObject ErrorTile;

    public GameObject timeCurtain;
	
	public Color red ; //= new Color(0.8f, 0.1f,0.0f);
	public Color green ; //= new Color(0.1f, 0.9f,0.1f);
	public Color blue ; //= new Color(0.1f, 0.2f,1.0f);
	public Color velvet ; //= new Color(0.1f, 0.2f,1.0f);
	
	public Color redLight ; //= new Color(1.0f, 0.3f,0.0f);
	public Color greenLight ; // = new Color(0.3f, 1.0f,0.2f);
	public Color blueLight ; //= new Color(0.3f, 0.3f,1.0f);
	
	public Color yellow = new Color(1.0f, 0.8f,0.1f);
	
	public GameObject[,] newObject;
	public GameObject[] markedObject;
	RowColumn[] markedObjectPositions;
	
	public GameObject particle;
	//	public GameObject dynParticle;
	
	public GameObject particleEndOfTile;
	public GameObject[] dynParticleEndOfTile;
	
	public GameObject particleEndOfTilePath;
	public GameObject[] dynParticleEndOfTilePath;

	private GameObject debugMarker;
	public GameObject statDebugMarker;
	//	public GameObject markerPath;
	public GameObject linkingPathMarker;

	public GameObject comboObject;
	public GameObject superComboObject;
	public GameObject megaComboObject;

	private GameObject dynComboObject;

	private Combo lastCombo = Combo.none;

	// Elements of the combo meter
	public GameObject SegmentCombo;
	public GameObject SphereCombo;
	public GameObject SegmentCombo2;
	public GameObject SphereCombo2;
	public GameObject SegmentCombo3;
	public GameObject SphereCombo3;
	public GameObject SegmentCombo4;
	public GameObject SphereCombo4;

    enum Combo
	{
		none,
		normal,
		super,
		mega
	};

	int currentSelected = 0;
	
	public int scoreOfLevel = 0;

	static int totalScore = 0;

	static int bestScore = 0;
	
	private int coolDown = 0; // Do not immediately do something else after some action

	private int nbOfPiecesLeft = 0;

	// Count the nb of combos for special win conditions and achievements
	private int nbOfNormalCombo = 0;
	private int nbOfSuperCombo = 0;
	private int nbOfMegaCombo = 0;

	private bool wasInNormalCombo = false;
	private bool wasInSuperCombo = false;
	private bool 	wasInMegaCombo = false;

    public bool isTimed = false;

	public class RowColumn
	{
		public int x;
		public int y;
		
		public void Set (int iRow, int iColumn)
		{
			x = iRow;
			y = iColumn;
		}
		
		public RowColumn(int newx,int newy)
		{
			this.x = newx;
			this.y = newy;
		}
		
		public RowColumn()
		{
			this.x = 0;
			this.y = 0;
		}
	}
	
	Vector2[,] listOfAllPosition;
	
	enum Colors
	{
		red,green,blue,velvet // velvet currently used for error
	};
	
	enum Symbol
	{
		CircleCut,
		CircleFull,
		CircleIn,
		//	CircleOut;
		Line,
		LineBroken,
		LineEnd,
		LineHalf,
		LineMiddle,
		LineSide,
		other // Error
	};
	
	class Piece
	{
		bool hasBeenRemoved;

		public Colors myColour;
		public Symbol mySymbol;

		public Piece(Colors newColour, Symbol newSymbol)
		{
			hasBeenRemoved = false;

			myColour = newColour;
			mySymbol = newSymbol;
		}

		public bool HasBeenRemoved {
			get {
				return hasBeenRemoved;
			}
			set {
				hasBeenRemoved = value;
			}
		}
	}
	
	ArrayList piecesToDistribute;

	Piece[,] piecesOnBoard;

	enum KindOfMatches
	{
		None,
		OnlyPathColours,
		OnlyPathSymbols,
		AllMatchNoPath,
		AllMatchPathFor2,
		AllMatchPathFor3
	};

    public enum TypeOfGame
    {
        Campaign, // Mix of other modes, earning XPs and "super power" (?). Earn characters, give tips by Table's character and serve as tutorial
        QuickPlay, // Raw game, score, no time
        Score, // Faster-> better score. No time limit. Can go to next level if stuck!
        TimeAttack, // Need to finish X tables in X minutes (can be selected at start-up, in setup?)
        Challenges, // A random challenge, or a table of challenges (table better)
        TimedScore // Unlimited nb of table in a limited time (can be selected ?)
    };

    public int nbOfTablesInTimeAttack = 4;
    public int nbOfMinutesInTimeAttack = 4;
    public int nbOfMinutesInTimedScore = 4;
    
    public TypeOfGame selectedTypeOfGame = TypeOfGame.QuickPlay;

	float comboMultiplier = 1.0f;
	int nbOfCombo = 0;
	int thresoldForPathOnly = 5;
	int thresoldAllMatch = 4;
	int thresoldAllMatchPathFor2 = 3;
	int thresoldForPathFor3 = 3;
	int thresoldForMega = 12;

	private KindOfMatches lastMatchWas = KindOfMatches.None;

	// Sounds
	public AudioClip pieceSelectedSound;
	public AudioClip pieceUnselectedSound;
	public AudioClip simpleMatchSound;
	public AudioClip bestMatchSound;
	public AudioClip goodMatchSound;
	public AudioClip comboMatchSound;
	public AudioClip superComboSound;
	public AudioClip megaComboSound;
	public AudioClip brokenComboSound;
	public AudioClip brokenMatchSound;
	public AudioClip gameOverSound;


	// Achievements will be defined by an interface and free numbers of achievements
    
	// Use this for initialization
	void Start () {
        // Get the kind of game from the menu
        selectedTypeOfGame = TypeOfGame.QuickPlay; // (TypeOfGame)StartScene.selectedScene;

        initIntroductionBox();

        Debug.Log("Starting " + selectedTypeOfGame);

		updateScore();

		updateBestScore();

		nbOfPiecesLeft = numberOfRows * numberOfColumns;

        initComboMeter();

//		debugMarker = (GameObject )GameObject.Instantiate(statDebugMarker);
		// Not correct and doing nothing...
		//this.transform.position.Set(((float )numberOfRows)/(spaceBetweenTiles*2),((float )numberOfColumns)/(spaceBetweenTiles*2),-10);
		
		//		dynParticle = (GameObject) Instantiate(particle, Vector3.one, Quaternion.identity);
		
		markedObject = new GameObject[3];
		
		for (int iMarkedObject = 0; iMarkedObject < 3; ++iMarkedObject)
		{
			markedObject[iMarkedObject] = new GameObject();
		}
		
		markedObjectPositions = new RowColumn[3];
		
		for (int iMarkedObject = 0; iMarkedObject < 3; ++iMarkedObject)
		{
			markedObjectPositions[iMarkedObject] = new RowColumn();
		}
		
		dynParticleEndOfTile = new GameObject[3];
		dynParticleEndOfTilePath = new GameObject[3];
		
		newObject = new GameObject[numberOfRows, numberOfColumns];
		
		listOfAllPosition = new Vector2[numberOfRows, numberOfColumns];

		piecesOnBoard = new Piece[numberOfRows, numberOfColumns];
		
		int iColor = 0;
		int maxNbColor = 3;
		int iSymbol = 0;
		int maxNbSymbol = 9;
		int currentPlaceInLinkedPieces = 0;
		int nbOfPiecesNeeded = 3;
		int lastCompletePlace = 0;
		
		piecesToDistribute = new ArrayList(); // List of Piece
		
		for (int iPieces = 0; iPieces < numberOfRows*numberOfColumns ; ++iPieces)
		{
			if (currentPlaceInLinkedPieces < nbOfPiecesNeeded - 1)
			{
				// Use the same piece and add it (external method)
				addPiece(iColor, iSymbol, piecesToDistribute);
				
				currentPlaceInLinkedPieces++;
			}
			else
			{
				// Add the last piece (external method)
				addPiece(iColor, iSymbol, piecesToDistribute);

				// Save the last completed place
				lastCompletePlace = iPieces; // !!!
				
				currentPlaceInLinkedPieces = 0;
				// Go to the next symbol or the next color
				if (iSymbol == (maxNbSymbol - 1))
				{
					iSymbol = 0;
					if (iColor == (maxNbColor - 1))
					{
						iColor = 0;
					}
					else
					{
						iColor++;
					}
				}
				else
				{
					iSymbol++;
				}
			}
		}
		// If currentPlaceInLinkedPieces is not may, then we don't have a complete set at the end
		if (currentPlaceInLinkedPieces != nbOfPiecesNeeded - 1)
		{
			Debug.Log("Ended: " + currentPlaceInLinkedPieces + " on " + (nbOfPiecesNeeded - 1) );
			Debug.Log("Did not end on a complete set: " + lastCompletePlace + " complete pieces on " + numberOfRows*numberOfColumns + " places");
		}
		
		int iPieceUsed = 0;
		
		for (int iRow = 0; iRow < numberOfRows; ++iRow)
		{
			for (int iColumn = 0; iColumn < numberOfColumns; ++iColumn)
			{
				// GameObject oneTile = new GameObject("Tile "+iRow+","+iColumn);
				
				// Sprite tileCircleIn = oneTile.AddComponent("Line-Cloud32") as Sprite;
				int myRandom = (int )(Random.value*((float )(piecesToDistribute.Count)));
				
				Piece foundPiece = (Piece )piecesToDistribute[myRandom];

				piecesOnBoard[iRow,iColumn] = foundPiece;

				// REMOVE THE PIECE from the list
				piecesToDistribute.RemoveAt(myRandom);
				
				GameObject myTileToInstanciate;
				
				if (foundPiece.mySymbol == Symbol.CircleCut)
				{
					myTileToInstanciate = CircleCut;
				}
				else if (foundPiece.mySymbol == Symbol.CircleFull)
				{
					myTileToInstanciate = CircleFull;
				}
				else if (foundPiece.mySymbol == Symbol.CircleIn)
				{
					myTileToInstanciate = CircleIn;
				}
				else if (foundPiece.mySymbol == Symbol.LineBroken)
				{
					myTileToInstanciate = LineBroken;
				}
				else if (foundPiece.mySymbol == Symbol.LineEnd)
				{
					myTileToInstanciate = LineEnd;
				}
				else if (foundPiece.mySymbol == Symbol.LineHalf)
				{
					myTileToInstanciate = LineHalf;
				}
				else if (foundPiece.mySymbol == Symbol.LineMiddle)
				{
					myTileToInstanciate = LineMiddle;
				}
				else if (foundPiece.mySymbol == Symbol.LineSide)
				{
					myTileToInstanciate = LineSide;
				}
				else if (foundPiece.mySymbol == Symbol.other)
				{
					myTileToInstanciate = ErrorTile;
				}
				else
				{
					myTileToInstanciate = Line;
				}
				// myTileToInstanciate = CircleOut;
				newObject[iRow,iColumn] = (GameObject )GameObject.Instantiate(myTileToInstanciate);
				newObject[iRow,iColumn].transform.position = new Vector3 (((float )iRow)/spaceBetweenTiles, ((float )iColumn)/spaceBetweenTiles, 0);
				listOfAllPosition[iRow,iColumn] = new Vector2 (((float )iRow)/spaceBetweenTiles, ((float )iColumn)/spaceBetweenTiles);
				
				if (foundPiece.myColour == Colors.red)
				{
					newObject[iRow,iColumn].GetComponent<Renderer>().material.color = red;
				}
				else if (foundPiece.myColour == Colors.green)
				{
					newObject[iRow,iColumn].GetComponent<Renderer>().material.color = green;
				}
				else if (foundPiece.myColour == Colors.blue)
				{
					newObject[iRow,iColumn].GetComponent<Renderer>().material.color = blue;
				}
				else
				{
					newObject[iRow,iColumn].GetComponent<Renderer>().material.color = velvet;
				}
				
				iPieceUsed++;
				
				if (iPieceUsed == lastCompletePlace + 1)
				{
					break;
				}
			}
		}
	}
	
	void addPiece(int iColor, int iSymbol, ArrayList piecesToDistribute)
	{
		Colors myColor = (Colors )iColor;
		/*
		Colors myColor = velvet;
		if (iColor == 0)
		{
			myColor = red;
		}
		else if (iColor == 1)
		{
			myColor = green;
		}
		if (iColor == 2)
		{
			myColor = blue;
		}
		*/
		Symbol mySymbol = (Symbol )iSymbol;
		
		Piece myPiece = new Piece(myColor,mySymbol);
		piecesToDistribute.Add(myPiece);

//		Debug.Log("Added " + myColor + " , " + mySymbol);
	}
	
	// Update is called once per frame
	void Update () {
        if (startingGame)
        {
            // Manage the introduction box
            showIntroductionBox(true);
            return;
        }

		if (inGameOver)
		{
			// Manage the introduction box
			showIntroductionBox(false);
			return;
		}

		if (gameIsInPause) 
		{
			// Nothing to do
			 return;
		}
		/**
		 * Cleaning
		 */
		if (timeBeforeCleaningOfPath == 1)
		{
			cleanLinkingPaths();
			timeBeforeCleaningOfPath = 0;
		} else if (timeBeforeCleaningOfPath > 0)
		{
			timeBeforeCleaningOfPath--;
			Debug.Log("timeBeforeCleaningOfPath: "+timeBeforeCleaningOfPath);
		}
		
        if (isTimed)
        {
            updateTimerCurtain();
        }

		if (coolDown != 0)
		{
			
			coolDown--;
		}
		
		if (Application.platform != RuntimePlatform.Android)
		{
			// use the input stuff
			
			bool aTouch = Input.GetMouseButton(0);
			if( aTouch)
			{
				Vector3 touchPos = Input.mousePosition;
				
				// Debug.Log( "Moused moved to point " + touchPos );
				
				checkBoard (touchPos);
			}
			else
			{
				// No touch reset the cool down
				coolDown=0;
			}
		} 
		else
		{
			if( Input.touchCount >= 1 )			
			{
				Vector3 touchPos = Input.GetTouch(0).position;
				
				checkBoard (touchPos);
			}
			else
			{
				// No touch reset the cool down
				coolDown=0;
			}
		}
	}

    // Introduction text
    public GameObject introductionBox;

    private bool startingGame = true;

    string introTitle;
    string introText;

    // Introduction texts for all "normal" modes (so not campaign)
    void setIntroductionTexts()
    {
        switch (selectedTypeOfGame)
        {
            case TypeOfGame.QuickPlay:
                introTitle = "Quick Play";
                introText = "Play a single board, without any\ntime limit. Try to get the best score,\nor just relax...";
                
				{
					bool haskey = PlayerPrefs.HasKey ("highScoreSimpleGame");
					if (haskey)
					{
						bestScore = PlayerPrefs.GetInt("highScoreSimpleGame");
					}
				}

                break;
            // Campaign will go to another scene, while saving the progression
            case TypeOfGame.Campaign:
                // Depend on the level
                Debug.Log("C");
                break;
            // All excepted challenge are linked to time
            case TypeOfGame.Score:
                introTitle = "Score";
                introText = "Be fast, with a trick.\nYou must avoid clearing all the \ntable, or you will get a game over!\nBut you will still get a bonus\nif you finish fast!\nYou cannot do all perfect here!";
                Debug.Log("Sc");
                break;
            case TypeOfGame.TimeAttack:
                introTitle = "Time Attack";
                introText = "Can you clear " + nbOfTablesInTimeAttack + " table in " + nbOfMinutesInTimeAttack + " minutes?";
                Debug.Log("TA");
                break;
            // Offer different type of challenge. Should be difficult.
            case TypeOfGame.Challenges:
                introTitle = "Challenges";
                introText = "Select from a selection of easy\nto difficult challenges.";
                Debug.Log("CH");
                break;
            case TypeOfGame.TimedScore:
                introTitle = "Times Score";
                introText = "Try to clear as much tables as\nyou can in " + nbOfMinutesInTimedScore + " minutes.\nCan you beat the bests?";
                Debug.Log("TS");
                break;
            default:
                introTitle = "Quick Play";
                introText = "Play a single board, without any\ntime limit. Try to get the best score,\nor just relax...";
				{
			    	bool haskey = PlayerPrefs.HasKey ("highScoreSimpleGame");
					if (haskey)
					{
			    		bestScore = PlayerPrefs.GetInt("highScoreSimpleGame");
					}
				}
                Debug.Log("Default");
                break;
        }
        Debug.Log("Intro title: " + introTitle);
        Debug.Log("Intro text: " + introText);
    }

	// Introduction texts for all "normal" modes (so not campaign)
	void setOutroTexts()
	{
		switch (selectedTypeOfGame)
		{
		case TypeOfGame.QuickPlay:
			introTitle = "Game Over";
			if (scoreOfLevel < bestScore)
			{
				introText = "Thank you for playing!\n\nYour score: "+scoreOfLevel+"\nBest score: "+bestScore;
			}
			else
			{
				introText = "Thank you for playing!\n\nBest score: "+scoreOfLevel+"!!!\nWell done!";
			}
			Debug.Log("QP");
			break;
			// Campaign will go to another scene, while saving the progression
		case TypeOfGame.Campaign:
			// Depend on the level
			Debug.Log("C");
			break;
			// All excepted challenge are linked to time
		case TypeOfGame.Score:
			introTitle = "Score";
			introText = "Be fast, with a trick.\nYou must avoid clearing all the \ntable, or you will get a game over!\nBut you will still get a bonus\nif you finish fast!\nYou cannot do all perfect here!";
			Debug.Log("Sc");
			break;
		case TypeOfGame.TimeAttack:
			introTitle = "Time Attack";
			introText = "Can you clear " + nbOfTablesInTimeAttack + " table in " + nbOfMinutesInTimeAttack + " minutes?";
			Debug.Log("TA");
			break;
			// Offer different type of challenge. Should be difficult.
		case TypeOfGame.Challenges:
			introTitle = "Challenges";
			introText = "Select from a selection of easy\nto difficult challenges.";
			Debug.Log("CH");
			break;
		case TypeOfGame.TimedScore:
			introTitle = "Times Score";
			introText = "Try to clear as much tables as\nyou can in " + nbOfMinutesInTimedScore + " minutes.\nCan you beat the bests?";
			Debug.Log("TS");
			break;
		default:
			introTitle = "Game Over";
			if (scoreOfLevel < bestScore)
			{
				introText = "Thank you for playing!\nYour score: "+scoreOfLevel+"\nBest score: "+bestScore;
			}
			else
			{
				introText = "Thank you for playing!\nBest score: "+scoreOfLevel+"!!!\nWell done!";
			}
			bool haskey = PlayerPrefs.HasKey ("highScoreSimpleGame");
			if (haskey)
			{
				bestScore = PlayerPrefs.GetInt("highScoreSimpleGame");
			}
			Debug.Log("Default");
			break;
		}
		Debug.Log("Intro title: " + introTitle);
		Debug.Log("Intro text: " + introText);
	}

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;
    public int dimingTime = 60; // 60 frames
    float zoomFactor;
    float currentZoomForIntroBox;

    void updateIntroText(bool isIntro)
    {
		if (isIntro) 
		{
				setIntroductionTexts ();
		}
		else
		{
				setOutroTexts ();
		}

        GameObject title = (GameObject)(GameObject.Find("IntroTitle"));
        if (title != null)
        {
            ((TextMesh)title.GetComponent("TextMesh")).text = introTitle;
            Debug.Log("Intro title: "+introTitle);
        }

        GameObject text = (GameObject)(GameObject.Find("IntroText"));
        if (text != null)
        {
            ((TextMesh)text.GetComponent("TextMesh")).text = introText;
            Debug.Log("Intro text: " + introText);
        }

		if (!isIntro) 
		{
			GameObject touchToContinue = (GameObject)(GameObject.Find("IntroTouchToContinue"));
			if (text != null)
			{
				((TextMesh)touchToContinue.GetComponent("TextMesh")).text = "Touch to return to menu";
				Debug.Log("Intro touch to cont: " + "Touch to return to menu");
			}
		}
    }

    private void initIntroductionBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        zoomFactor = (0.2f / ((float)totalZoomingTime));
        currentZoomForIntroBox = 0;
        timeCurtain.transform.localScale = new Vector3(0, timeCurtain.transform.localScale.y, timeCurtain.transform.localScale.z);
        // Z = -9
        introductionBox.transform.localPosition = new Vector3(introductionBox.transform.localPosition.x, introductionBox.transform.localPosition.y, -9);

        updateIntroText(true);
    }

	// Using same box, show the game over text
	private void initIOutroBox()
	{
		zoomingTime = 0;
		currentStep = 0;
		
		startingGame = true; // Show the introduction box
		zoomFactor = (0.2f / ((float)totalZoomingTime));
		currentZoomForIntroBox = 0;
		timeCurtain.transform.localScale = new Vector3(0, timeCurtain.transform.localScale.y, timeCurtain.transform.localScale.z);
		// Z = -9
		introductionBox.transform.localPosition = new Vector3(introductionBox.transform.localPosition.x, introductionBox.transform.localPosition.y, -9);
		
		updateIntroText(false);
	}

    private void showIntroductionBox(bool isIntro)
    {
       if (currentStep == 0)
       {
           // Zoom the box into view
           if (zoomingTime <= totalZoomingTime)
           {
               float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
               foreach (Transform childIntrobox in introductionBox.transform)
               {
                   Debug.Log("Child: " + childIntrobox);
                   childIntrobox.GetComponent<Renderer>().material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                   zoomingTime++;
               }

               //introductionBox.transform.localScale = new Vector3(0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox);
               currentZoomForIntroBox += zoomFactor;
               zoomingTime++;
           }
           else
           {
               currentStep = 1;
               zoomingTime = 0;
           }
       }
       else if (currentStep == 1)
       {
           // Wait for user input
           if (Application.platform != RuntimePlatform.Android)
           {
               // use the input stuff

               bool aTouch = Input.GetMouseButton(0);
               if (aTouch)
               {
                   currentStep = 2;
               }
           }
           else
           {
               if (Input.touchCount >= 1)
               {
                   currentStep = 2;
               }
           }
       }
       else if (currentStep == 2)
       {
           if (zoomingTime <= dimingTime)
           {
               // Fading out of the box
               float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

               foreach (Transform childIntrobox in introductionBox.transform)
               {
                   Debug.Log("Child: " + childIntrobox);
                   childIntrobox.GetComponent<Renderer>().material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                   zoomingTime++;
               }
           }
           else
           {
               currentStep = 3;
           }
       }
       else if (currentStep ==3)
       {
           foreach (Transform childIntrobox in introductionBox.transform)
           {
               Debug.Log("Child: " + childIntrobox);
               childIntrobox.GetComponent<Renderer>().material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, 1);
           }
           // All done
			if (isIntro)
			{
           		startingGame = false;
           		introductionBox.transform.localPosition = new Vector3(introductionBox.transform.localPosition.x, introductionBox.transform.localPosition.y, 4);
			}
			else
			{
				inGameOver = false;
				goToMenu();
			}
       }
    }

    // 0.0 -> 2.1 to full extend
    float currentScale = 0;
    public float timeToFinish = 1 * 60; // In seconds, 4 minutes
    float currentTime = 4 * 60;
    float timeScoreMultiplier = 1; // No bonus yet

    public void setUpTimedMode(float timeToFinish)
    {
        currentScale = 0;
        this.timeToFinish = timeToFinish;
        currentTime = timeToFinish;
    }
    private void updateTimerCurtain()
    {
        // Using the current settings, scale the curtain
        // We will do 2.1 units in timeToFinish seconds
        // So the speed is 2.1*lastUpdateTime
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            // Each game mode will need to know what to do
            indicateTimerDone();
        }
        currentScale = 2.1f * ((timeToFinish - currentTime) / timeToFinish);

        timeCurtain.transform.localScale = new Vector3(currentScale, timeCurtain.transform.localScale.y, timeCurtain.transform.localScale.z);
    }

    private void indicateTimerDone()
    {
        // Dummy thing now
        timeToFinish = timeToFinish * 0.9f;
        timeScoreMultiplier -= 0.1f;

		if (timeScoreMultiplier < 0.2f) 
		{
			timeScoreMultiplier = 0.2f;
		}

        setUpTimedMode(timeToFinish);
    }
	
	RowColumn lastTileUnselected = new RowColumn();
	int timeBeforeCleaningOfPath = 0;
	
	void checkBoard (Vector3 touchPos)
	{
		bool hasBeenUnselected = false;
		
		Vector3 wp = Camera.main.ScreenToWorldPoint (touchPos);
		Vector2 touchPos2 = new Vector2 (wp.x, wp.y);
		
		// First check the selected object
		for (int iTileSelected = 0; iTileSelected < currentSelected; ++iTileSelected) {
			//			Debug.Log("Tile selected: "+iTileSelected+", last selected: "+currentSelected+", coolDown: "+coolDown);
			
			if (markedObject[iTileSelected] != null && markedObject[iTileSelected].GetComponent<Collider2D>() == Physics2D.OverlapPoint (touchPos2)) 
			{
//				debugMarker.transform.position = new Vector3(markedObjectPositions [iTileSelected].x, markedObjectPositions [iTileSelected].y, -5);
//				debugMarker.renderer.material.color = green;
				// If it is the last selected object, do nothing until we cool down
				//				Debug.Log("Tile selected: "+iTileSelected+", last selected: "+currentSelected+", coolDown: "+coolDown);
				if ((iTileSelected != currentSelected - 1) || (coolDown == 0))
				{
					//					Debug.Log ("Hit detected on marker " + iTileSelected);
					hasBeenUnselected = true;

					AudioSource.PlayClipAtPoint(pieceUnselectedSound, markedObject[iTileSelected].transform.position);

					Destroy (markedObject[iTileSelected]);
					
					RowColumn positionOfObject = markedObjectPositions [iTileSelected];
					lastTileUnselected = positionOfObject;
					
					GameObject currentObject = newObject [positionOfObject.x, positionOfObject.y];
					
					((TileStatus )currentObject.GetComponent("TileStatus")).isSelected = false;
				
					Debug.Log ("Hit detected on marker " + iTileSelected + " on "+(currentSelected-1));

					for (int iNextSelected = 0; iNextSelected < currentSelected;++iNextSelected)
					{
						Debug.Log (iNextSelected + ": " + markedObjectPositions [iNextSelected].x + " , "+markedObjectPositions [iNextSelected].y + "(Removed "+positionOfObject.x+" , "+ positionOfObject.y+")");
					}

					// If not the current last
					if (iTileSelected+1 < currentSelected)
					{
						// We must shift all next elements...
						for (int iNextSelected = iTileSelected; iNextSelected < currentSelected-1;++iNextSelected)
						{
							markedObject[iNextSelected] = markedObject[iNextSelected+1] ;
							markedObjectPositions [iNextSelected].Set (markedObjectPositions [iNextSelected+1].x, markedObjectPositions [iNextSelected+1].y);
							Debug.Log ("Shift from " + (iNextSelected+1) + " to "+iNextSelected);
						}
					}

					if (currentSelected >0)
					{
						currentSelected--;
					}
//					foreach (RowColumn oneRC in  markedObjectPositions)
//					{
//						Debug.Log ("Now "+oneRC.x + ", "+oneRC.y);
//					}
					for (int iNextSelected = 0; iNextSelected < currentSelected;++iNextSelected)
					{
						Debug.Log (iNextSelected + ": " + markedObjectPositions [iNextSelected].x + " , "+markedObjectPositions [iNextSelected].y + "(Removed "+positionOfObject.x+" , "+ positionOfObject.y+")");
					}
					Debug.Log ("---");
					for (int iNextSelected = 0; iNextSelected < 3;++iNextSelected)
					{
						Debug.Log (iNextSelected + ": " + markedObjectPositions [iNextSelected].x + " , "+markedObjectPositions [iNextSelected].y + "(Removed "+positionOfObject.x+" , "+ positionOfObject.y+")");
					}
					Debug.Log ("Current Selected=" + currentSelected);
					coolDown = 100;
				}
			}
		}
		if (!hasBeenUnselected)
		{
			for (int iRow = 0; iRow < numberOfRows; ++iRow) {
				for (int iColumn = 0; iColumn < numberOfColumns; ++iColumn) {
					GameObject currentObject = newObject [iRow, iColumn]; // NRE here --- BUG
					if (currentObject != null && currentObject.GetComponent<Collider2D>() == Physics2D.OverlapPoint (touchPos2) && !((TileStatus )currentObject.GetComponent("TileStatus")).isSelected) 
					{
						//						Debug.Log ("Hit detected on object " + currentObject.name);
//						debugMarker.transform.position = new Vector3(currentObject.transform.position.x, currentObject.transform.position.y, -5);
//						debugMarker.renderer.material.color = red;
						// Find if the object has been unselected just previously
						if (lastTileUnselected.x != iRow || lastTileUnselected.y != iColumn ||  coolDown == 0)
						{
							coolDown = 100;
							
							((TileStatus )currentObject.GetComponent("TileStatus")).isSelected = true;
							
							Vector3 newPosition = new Vector3 (currentObject.transform.position.x, currentObject.transform.position.y, -4);

							Debug.Log ("currentSelected= " + currentSelected);
							markedObject [currentSelected] = (GameObject)Instantiate (currentObject, newPosition, currentObject.transform.rotation);
							Color lightColor = markedObject [currentSelected].GetComponent<Renderer>().material.color;
							if (lightColor == red) {
								lightColor = redLight;
							}
							else
							if (lightColor == blue) {
								lightColor = blueLight;
							}
							else
							if (lightColor == green) {
								lightColor = greenLight;
							}
							markedObject [currentSelected].GetComponent<Renderer>().material.color = lightColor;
							markedObject [currentSelected].transform.localScale = new Vector3 (1.2f, 1.2f);
							// Save the positions of the actual object, so we can remove it
							markedObjectPositions [currentSelected].Set (iRow, iColumn);

							Debug.Log ("---");
							for (int iNextSelected = 0; iNextSelected < 3;++iNextSelected)
							{
								Debug.Log (iNextSelected + ": " + markedObjectPositions [iNextSelected].x + " , "+markedObjectPositions [iNextSelected].y );
							}

							currentSelected++;
							if (currentSelected > 2) {
								bool okColor = true;
								bool okSymbol = true;
								int iFoundOk = 0;
								// First possibility, inside the board, must have identical color & symbol
								// Check the current selection
								for (int iSelected = 0; iSelected < 2; ++iSelected) {
									// Colors
									if (markedObject [iSelected].GetComponent<Renderer>().material.color != markedObject [iSelected + 1].GetComponent<Renderer>().material.color) {
										okColor = false;
									}
									if (((SpriteRenderer )(markedObject[iSelected].GetComponent<Renderer>())).sprite != ((SpriteRenderer )(markedObject[iSelected + 1].GetComponent<Renderer>())).sprite) {
										okSymbol = false;
									}
									
									// If both ok, check them ok so we don't check them again later on.
									if  (okColor && okSymbol)
									{
										RowColumn positionOfObject = markedObjectPositions [iSelected];
										
										GameObject FoundObject = newObject [positionOfObject.x, positionOfObject.y];
										((TileStatus )FoundObject.GetComponent("TileStatus")).isOk = true;
										
										iFoundOk++;
									}
									
									// In all cases, remove the marked one
									Destroy (markedObject [iSelected]);
									// To do later once we also check for the external paths
								}
								// Destroy the last marker
								Destroy (markedObject [2]);
								
								// Here check for the external path as well
								bool okPath = false;
								bool okPathTwoPieces = false;

								// Always check the path, for removal or for score
								if ((okColor || okSymbol)) {
									// Check for the existence of the path
									okPath = checkForPathBetweenSelected();
								}
								if (!okPath)
								{
									// Check if two pieces are linked
									okPathTwoPieces = checkForPathBetweenTwoOfTheSelected();
								}
								if (okColor) {
									// Special effect for the color
								}
								else {
									// Break the path (Start color path from each three then break)
								}
								if (okSymbol) {
									// Special effect for the symbol
								}
								else {
									// Break the path, link what work then break (???)
								}
								if (okColor && okSymbol) {
									// Increase the score then destroy the objects.
									// Check if similar symbols exist aside the others
									// Then clean all (to change by swipe of marked ones)
									for (int iTileSelected = 0; iTileSelected < 3; ++iTileSelected) {
										RowColumn positionOfObject = markedObjectPositions [iTileSelected];
										
										dynParticleEndOfTile[iTileSelected] = (GameObject )Instantiate(particleEndOfTile,newObject [positionOfObject.x, positionOfObject.y].transform.position,Quaternion.identity);

										Debug.Log("Destroying object at "+positionOfObject.x + " , " +positionOfObject.y);
										Destroy (newObject [positionOfObject.x, positionOfObject.y]);
										piecesOnBoard[positionOfObject.x, positionOfObject.y].HasBeenRemoved = true;
									}
									float scoreToAdd = 10.0f ;

									if (okPath)
									{
										// Should add depending of the distance maybe
										timeBeforeCleaningOfPath = 15;
										scoreToAdd+=20.0f;

										KindOfMatches usedKindOfMatches = KindOfMatches.AllMatchPathFor3;
										checkCombo (usedKindOfMatches);
									}
									else if (okPathTwoPieces)
									{
										drawLinkingPaths();
										timeBeforeCleaningOfPath = 15;
										scoreToAdd+=5.0f;

										KindOfMatches usedKindOfMatches = KindOfMatches.AllMatchPathFor2;
										checkCombo (usedKindOfMatches);
									}
									else
									{
										KindOfMatches usedKindOfMatches = KindOfMatches.AllMatchNoPath;
										checkCombo (usedKindOfMatches);

									}
                                    scoreOfLevel += (int)(scoreToAdd * comboMultiplier * timeScoreMultiplier);

									updateScore();
									nbOfPiecesLeft-=3;
									checkForGameOver ();
								}
								else if (okPath)
								{
									timeBeforeCleaningOfPath = 15;
									
									for (int iTileSelected = 0; iTileSelected < 3; ++iTileSelected) {
										RowColumn positionOfObject = markedObjectPositions [iTileSelected];
										
										dynParticleEndOfTilePath[iTileSelected] = (GameObject )Instantiate(particleEndOfTilePath,newObject [positionOfObject.x, positionOfObject.y].transform.position,Quaternion.identity);
										
										Destroy (newObject [positionOfObject.x, positionOfObject.y]);
										piecesOnBoard[positionOfObject.x, positionOfObject.y].HasBeenRemoved = true;
									}

									float scoreToAdd = 5.0f ;

									KindOfMatches usedKindOfMatches = KindOfMatches.None;
									
									if (okColor)
									{
										usedKindOfMatches = KindOfMatches.OnlyPathColours;
									}
									else
									{
										usedKindOfMatches = KindOfMatches.OnlyPathSymbols;
									}
									checkCombo (usedKindOfMatches);

                                    scoreOfLevel += (int)(scoreToAdd * comboMultiplier * timeScoreMultiplier);

									updateScore();
									nbOfPiecesLeft-=3;
									checkForGameOver ();
								}
								else
								{
									AudioSource.PlayClipAtPoint(brokenComboSound, newPosition);
									// Clean-up all ok tiles.
									for (int iTileSelected = 0; iTileSelected < 3; ++iTileSelected) {
										RowColumn positionOfObject = markedObjectPositions [iTileSelected];
										
										GameObject FoundObject = newObject [positionOfObject.x, positionOfObject.y];
										((TileStatus )FoundObject.GetComponent("TileStatus")).isOk = false;
										((TileStatus )FoundObject.GetComponent("TileStatus")).isSelected = false;
									}
								}
								currentSelected = 0;
							}
							else
							{
								// Play the select sound only for the 2 first pieces
								AudioSource.PlayClipAtPoint(pieceSelectedSound, newPosition);
							}
						}
					}
				}
			}
		}
	}

	void checkForGameOver ()
	{
		if (nbOfPiecesLeft == 0) {
			gameover ();
		}
		if (nbOfPiecesLeft < 12)
		{
			Debug.Log("Checking for game over condition");

			int nbCircleCut = 0;
			int nbCircleFull = 0;
			int nbCircleIn = 0;
			//	public GameObject CircleOut;
			int nbLine = 0;
			int nbLineBroken = 0;
			int nbLineEnd = 0;
			int nbLineHalf = 0;
			int nbLineMiddle = 0;
			int nbLineSide = 0;

			int nbRed = 0;
			int nbGreen = 0;
			int nbBlue = 0;

			bool isGameOver = true;

			// Check if one possible combination is left, 3 same colors or 3 same symbols.

			for (int iRow = 0; iRow < numberOfRows; ++iRow) {
				for (int iColumn = 0; iColumn < numberOfColumns; ++iColumn) {

					Piece currentPiece = piecesOnBoard[iRow, iColumn];

					if (!currentPiece.HasBeenRemoved)
					{
						Colors lightColor = currentPiece.myColour;
						if (lightColor == Colors.red) {
							nbRed++;
							if (nbRed == 3)
							{
								Debug.Log("A");
								isGameOver = false;
								break;
							}
						}
						else
						if (lightColor == Colors.blue) {
							nbBlue++;
							if (nbBlue == 3)
							{
								Debug.Log("B");
								isGameOver = false;
								break;
							}
						}
						else
						if (lightColor == Colors.green) {
							nbGreen++;
							if (nbGreen == 3)
							{
								Debug.Log("C");
								isGameOver = false;
								break;
							}
						}

						if (currentPiece.mySymbol == Symbol.CircleCut)
						{
							nbCircleCut++;
							if (nbCircleCut == 3)
							{
								Debug.Log("D");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.CircleFull)
						{
							nbCircleFull++;
							if (nbCircleFull == 3)
							{
								Debug.Log("E");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.CircleIn)
						{
							nbCircleIn++;
							if (nbCircleIn == 3)
							{
								Debug.Log("F");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.Line)
						{
							nbLine++;
							if (nbLine == 3)
							{
								Debug.Log("G");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.LineBroken)
						{
							nbLineBroken++;
							if (nbLineBroken == 3)
							{
								Debug.Log("H");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.LineEnd)
						{
							nbLineEnd++;
							if (nbLineEnd == 3)
							{
								Debug.Log("I");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.LineHalf)
						{
							nbLineHalf++;
							if (nbLineHalf == 3)
							{
								Debug.Log("J");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.LineMiddle)
						{
							nbLineMiddle++;
							if (nbLineMiddle == 3)
							{
								Debug.Log("K");
								isGameOver = false;
								break;
							}
						}
						else if (currentPiece.mySymbol == Symbol.LineSide)
						{
							nbLineSide++;
							if (nbLineSide == 3)
							{
								Debug.Log("L");
								isGameOver = false;
								break;
							}
						}
					}
				}
			}
			if (isGameOver)
			{
				gameover ();
			}
		}
	}

	void checkCombo (KindOfMatches usedKindOfMatches)
	{
		int thresoldUsed = 100;

		switch (usedKindOfMatches)
		{
		case KindOfMatches.AllMatchNoPath:
			thresoldUsed = thresoldAllMatch;
			break;
		case KindOfMatches.AllMatchPathFor2:
			thresoldUsed = thresoldAllMatchPathFor2;
			break;
		case KindOfMatches.AllMatchPathFor3:
			thresoldUsed = thresoldForPathFor3;
			break;
		case KindOfMatches.OnlyPathColours:
			thresoldUsed = thresoldForPathOnly;
			break;
		case KindOfMatches.OnlyPathSymbols:
			thresoldUsed = thresoldForPathOnly;
			break;
		}

		if ((lastMatchWas == usedKindOfMatches) || isOnSphere)
        {
            lastMatchWas = usedKindOfMatches;

			nbOfCombo++;

            progressComboMeter(nbOfCombo, usedKindOfMatches);

			if (nbOfCombo < thresoldUsed) {
				comboMultiplier += 0.1f;

				Combo myCombo = Combo.normal;
				GameObject myComboObject = comboObject;

				showComboObject (myCombo, myComboObject);

				AudioSource.PlayClipAtPoint(comboMatchSound, Camera.main.transform.position);

				if (!wasInNormalCombo)
				{
					nbOfNormalCombo++;
					wasInNormalCombo = true;
					nbOfSuperCombo = 0;
					nbOfMegaCombo = 0;
				}
			}
			else if  (nbOfCombo < thresoldForMega){
				comboMultiplier += 1f;

				Combo myCombo = Combo.super;
				GameObject myComboObject = superComboObject;
				
				showComboObject (myCombo, myComboObject);

				AudioSource.PlayClipAtPoint(superComboSound, Camera.main.transform.position);

				if (!wasInSuperCombo)
				{
					nbOfSuperCombo++;
					wasInSuperCombo = true;
				}
			}
			else{
				comboMultiplier += 3f;

				Combo myCombo = Combo.mega;
				GameObject myComboObject = megaComboObject;
				
				showComboObject (myCombo, myComboObject);

				AudioSource.PlayClipAtPoint(megaComboSound, Camera.main.transform.position);

				if (!wasInMegaCombo)
				{
					nbOfMegaCombo++;
					wasInMegaCombo=true;
				}
			}
		}
		else {
			nbOfCombo = 0;
			comboMultiplier = 1.0f;
			lastMatchWas = usedKindOfMatches;

			Combo myCombo = Combo.none;
			
			showComboObject (myCombo, null);

			if (wasInNormalCombo || wasInSuperCombo || wasInMegaCombo)
			{
				AudioSource.PlayClipAtPoint(brokenComboSound, Camera.main.transform.position);
			}
			else
			{

				switch (usedKindOfMatches)
				{
				case KindOfMatches.AllMatchNoPath:
					AudioSource.PlayClipAtPoint(goodMatchSound, Camera.main.transform.position);
					break;
				case KindOfMatches.AllMatchPathFor2:
					AudioSource.PlayClipAtPoint(goodMatchSound, Camera.main.transform.position);
					break;
				case KindOfMatches.AllMatchPathFor3:
					AudioSource.PlayClipAtPoint(bestMatchSound, Camera.main.transform.position);
					break;
				case KindOfMatches.OnlyPathColours:
					AudioSource.PlayClipAtPoint(simpleMatchSound, Camera.main.transform.position);
					break;
				case KindOfMatches.OnlyPathSymbols:
					AudioSource.PlayClipAtPoint(simpleMatchSound, Camera.main.transform.position);
					break;
				}
			}

            resetComboMeter();

			wasInNormalCombo = false;
			wasInSuperCombo = false;
			wasInMegaCombo = false;
		}
	}

	void showComboObject (Combo myCombo, GameObject myComboObject)
	{
		if (lastCombo != myCombo) {
			if (lastCombo != Combo.none) {
				// Destroy the last object
				Destroy (dynComboObject);
			}
			lastCombo = myCombo;

			if (myCombo != Combo.none)
			{
				dynComboObject = (GameObject ) GameObject.Instantiate (myComboObject,new Vector3(2.8f,-0.25f,0.0f), Quaternion.identity);
				dynComboObject.transform.localScale = new Vector3(0.4f,0.4f,1.0f);
			}
		}
	}

    bool isOnSphere = false;

    float segment1Pos; // on Y axe
    float segment2Pos; // on X axe
    float segment3Pos; // on Y axe
    float segment4Pos; // on X axe

    float segment1Scale; // on Y axe
    float segment2Scale; // on X axe
    float segment3Scale; // on Y axe
    float segment4Scale; // on X axe

	/// <summary>
	/// Inits the combo meter.
	/// </summary>
	void initComboMeter()
	{
		SegmentCombo.SetActive(false);
		SphereCombo.SetActive(false);
		SegmentCombo2.SetActive(false);
		SphereCombo2.SetActive(false);
		SegmentCombo3.SetActive(false);
		SphereCombo3.SetActive(false);
		SegmentCombo4.SetActive(false);
		SphereCombo4.SetActive(false);

        segment1Scale = SegmentCombo.transform.localScale.y;
        segment2Scale = SegmentCombo2.transform.localScale.x;
        segment3Scale = SegmentCombo3.transform.localScale.y;
        segment4Scale = SegmentCombo4.transform.localScale.x;

        segment1Pos = SegmentCombo.transform.localPosition.y;
        segment2Pos = SegmentCombo2.transform.localPosition.x;
        segment3Pos = SegmentCombo3.transform.localPosition.y;
        segment4Pos = SegmentCombo4.transform.localPosition.x;
	}

    void resetComboMeter()
    {
        SegmentCombo.SetActive(false);
        SphereCombo.SetActive(false);
        SegmentCombo2.SetActive(false);
        SphereCombo2.SetActive(false);
        SegmentCombo3.SetActive(false);
        SphereCombo3.SetActive(false);
        SegmentCombo4.SetActive(false);
        SphereCombo4.SetActive(false);

        lastPosInQuadran = 10;
        isOnSphere = false;
    }

	static int nbOfVerticalSteps = 3; 
	static int nbOfHorizontalSteps = 5;
	static int nbInALoop = (nbOfVerticalSteps + nbOfHorizontalSteps) * 2;

	int lastPosInQuadran = 10;

    bool startOfSeg1 = false;
    bool startOfSeg2 = false;
    bool startOfSeg3 = false;
    bool startOfSeg4 = false;

    const float sizeWide = 15.5f; // Size of a segment to touch both sides
    const float sizeHigh = 9.3f;

    float stepWide = sizeWide / nbOfHorizontalSteps;
    float stepHigh = sizeHigh / nbOfVerticalSteps;

	/**
	 * progress the combo meter depending on the current nb of combo and of the current kind of match (2 linked, 1 linked, no link, only color, ...) 
	 */
	void progressComboMeter(int currentComboNb, KindOfMatches usedKindOfMatches)
	{
        int posInLoop = currentComboNb % nbInALoop;
        int posInSeg = 0;

		// Check how much complete loop we have for changing colors
        int nbOfLoopsDone = nbInALoop / currentComboNb; //currentComboNb - (nbInALoop * posInLoop);

		// This will change the overall hue
        // Depending of the kind of match and the nb of loops, define a color
        switch (usedKindOfMatches)
        {
            case KindOfMatches.AllMatchNoPath:
                setColorsOfComboMeters(new Color(0.8f, 0.3f, 0.0f), nbOfLoopsDone);
                break;
            case KindOfMatches.AllMatchPathFor2:
                setColorsOfComboMeters(new Color(0.8f, 0.6f, 0.0f), nbOfLoopsDone);
                break;
            case KindOfMatches.AllMatchPathFor3:
                setColorsOfComboMeters(new Color(1.0f, 1.0f, 1.0f), nbOfLoopsDone);
                break;
            case KindOfMatches.OnlyPathColours:
                setColorsOfComboMeters(new Color(0.2f, 0.6f, 0.0f), nbOfLoopsDone);
                break;
            case KindOfMatches.OnlyPathSymbols:
                setColorsOfComboMeters(new Color(0.2f, 0.0f, 0.8f), nbOfLoopsDone);
                break;
        }

          

		int posInQuadran = 0;
        posInSeg = posInLoop;

		// Check current post in loop (1-4)
		if ((posInLoop > nbOfVerticalSteps) && (posInLoop <= (nbOfVerticalSteps + nbOfHorizontalSteps)))
		{
			posInQuadran = 1;
            posInSeg = posInSeg - nbOfVerticalSteps;
		}
        else if ((posInLoop > (nbOfVerticalSteps + nbOfHorizontalSteps)) && (posInLoop <= (2 * nbOfVerticalSteps + nbOfHorizontalSteps)))
        {
            posInQuadran = 2;
            posInSeg = posInSeg - (nbOfVerticalSteps + nbOfHorizontalSteps);
        } 
        else if ((posInLoop > (2 * nbOfVerticalSteps + nbOfHorizontalSteps)) && (posInLoop <= (2 * nbOfVerticalSteps + 2 * nbOfHorizontalSteps)))
        {
            posInQuadran = 3;
            posInSeg = posInSeg - (2 * nbOfVerticalSteps + nbOfHorizontalSteps);
        }      

		// See if it changed
        if (posInQuadran != lastPosInQuadran)
        {
            // If so we need to switch on one new quadran (well sphere first)
            lastPosInQuadran = posInQuadran;
            isOnSphere = true; // We are now allowed to change combo type.

            // Switch on the correct sphere
            if (posInQuadran == 0)
            {
                startOfSeg1 = true; // next will be the first segment, so no resizing
                SegmentCombo.SetActive(false);
                SphereCombo.SetActive(true);
                SegmentCombo2.SetActive(false);
                SphereCombo2.SetActive(false);
                SegmentCombo3.SetActive(false);
                SphereCombo3.SetActive(false);
                SegmentCombo4.SetActive(false);
                SphereCombo4.SetActive(false);

                SphereCombo.GetComponent<Renderer>().material.color = new Color(0.8f,0.8f,0.2f);
            }
            else if (posInQuadran == 1)
            {
                startOfSeg2 = true; // next will be the first segment, so no resizing

                SegmentCombo.SetActive(true);
                SphereCombo.SetActive(true);
                SegmentCombo2.SetActive(false);
                SphereCombo2.SetActive(true);
                SegmentCombo3.SetActive(false);
                SphereCombo3.SetActive(false);
                SegmentCombo4.SetActive(false);
                SphereCombo4.SetActive(false);

                SphereCombo2.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.2f);

                // SegmentCombo.transform.localScale = new Vector3(3.333333f, 3.333333f, stepWide * nbOfHorizontalSteps);
            }
            else if (posInQuadran == 2)
            {
                startOfSeg3 = true; // next will be the first segment, so no resizing

                SegmentCombo.SetActive(true);
                SphereCombo.SetActive(true);
                SegmentCombo2.SetActive(true);
                SphereCombo2.SetActive(true);
                SegmentCombo3.SetActive(false);
                SphereCombo3.SetActive(true);
                SegmentCombo4.SetActive(false);
                SphereCombo4.SetActive(false);

                SphereCombo3.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.2f);

                // SegmentCombo2.transform.localScale = new Vector3(3.333333f, 3.333333f, stepHigh * nbOfVerticalSteps);
            }
            else if (posInQuadran == 3)
            {
                startOfSeg4 = true; // next will be the first segment, so no resizing

                SegmentCombo.SetActive(true);
                SphereCombo.SetActive(true);
                SegmentCombo2.SetActive(true);
                SphereCombo2.SetActive(true);
                SegmentCombo3.SetActive(true);
                SphereCombo3.SetActive(true);
                SegmentCombo4.SetActive(false);
                SphereCombo4.SetActive(true);

                SphereCombo4.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.2f);

                // SegmentCombo3.transform.localScale = new Vector3(3.333333f, 3.333333f, stepWide * nbOfHorizontalSteps);
            }
        }
        else
        {
            isOnSphere = false; // We are *not* allowed to change combo type.

            // Resize the current segment
            if (startOfSeg1)
            {
                SegmentCombo.SetActive(true);
                startOfSeg1 = false;

                SegmentCombo.transform.localScale = new Vector3(3.333333f, 3.333333f, stepHigh);
            }
            else if (startOfSeg2)
            {
                SegmentCombo2.SetActive(true);
                startOfSeg2 = false;

                SegmentCombo2.transform.localScale = new Vector3(3.333333f, 3.333333f, stepWide);
            }
            else if (startOfSeg3)
            {
                SegmentCombo3.SetActive(true);
                startOfSeg3 = false;

                SegmentCombo3.transform.localScale = new Vector3(3.333333f, 3.333333f, stepHigh);
            }
            else if (startOfSeg4)
            {
                SegmentCombo4.SetActive(true);
                startOfSeg4 = false;

                SegmentCombo4.transform.localScale = new Vector3(3.333333f, 3.333333f, stepWide);
            }
            else if (posInQuadran == 0)
            {
                Debug.Log("Quadran 0, size " + SegmentCombo.transform.localScale.y + ", pos " + SegmentCombo.transform.localPosition.y+", pos in seg "+posInSeg);
                SegmentCombo.SetActive(true);
                SegmentCombo.transform.localScale = new Vector3(3.333333f, 3.333333f, stepHigh * posInSeg);

                Debug.Log("New size " + SegmentCombo.transform.localScale.y + ", new pos " + SegmentCombo.transform.localPosition.y);
            }
            else if (posInQuadran == 1)
            {
                Debug.Log("Quadran 1");
                SegmentCombo2.SetActive(true);
                SegmentCombo2.transform.localScale = new Vector3(3.333333f, 3.333333f, stepWide * posInSeg);
            }
            else if (posInQuadran == 2)
            {
                Debug.Log("Quadran 2");
                SegmentCombo3.SetActive(true);
                SegmentCombo3.transform.localScale = new Vector3(3.333333f, 3.333333f, stepHigh * posInSeg);
            }
            else if (posInQuadran == 3)
            {
                Debug.Log("Quadran 3");
                SegmentCombo4.SetActive(true);
                SegmentCombo4.transform.localScale = new Vector3(3.333333f, 3.333333f, stepWide * posInSeg);
            }
        }
		// Now see which segments need to be on if the nb is below 1.
		if (nbOfLoopsDone < 1)
		{

		}

		// Check if we are on a sphere, and set the flag to allow all combos
        //SegmentCombo.SetActive(false);
        //SphereCombo.SetActive(false);
        //SegmentCombo2.SetActive(false);
        //SphereCombo2.SetActive(false);
        //SegmentCombo3.SetActive(false);
        //SphereCombo3.SetActive(false);
        //SegmentCombo4.SetActive(false);
        //SphereCombo4.SetActive(false);
	}

    private void setColorsOfComboMeters(Color color, int nbOfLoopsDone)
    {
        Color alteredColor = color;

        // Dim the color depending on the position
        if (nbOfLoopsDone < 5)
        {
            float dimFactor = 5 - nbOfLoopsDone;
            dimFactor = dimFactor / 5;

            alteredColor.b = color.b / dimFactor;
            alteredColor.r = color.r / dimFactor;
            alteredColor.g = color.g / dimFactor;
        }
        SegmentCombo.GetComponent<Renderer>().material.color = alteredColor;
        SphereCombo.GetComponent<Renderer>().material.color = alteredColor;
        SegmentCombo2.GetComponent<Renderer>().material.color = alteredColor;
        SphereCombo2.GetComponent<Renderer>().material.color = alteredColor;
        SegmentCombo3.GetComponent<Renderer>().material.color = alteredColor;
        SphereCombo3.GetComponent<Renderer>().material.color = alteredColor;
        SegmentCombo4.GetComponent<Renderer>().material.color = alteredColor;
        SphereCombo4.GetComponent<Renderer>().material.color = alteredColor;
    }
	
	void updateScore ()
	{
		GameObject score = (GameObject )(GameObject.Find("Score2")); // .GetComponent("GUItext"));
		if (score != null)
		{
			((TextMesh )score.GetComponent("TextMesh")).text = ""+scoreOfLevel;
		}
	}

	void updateBestScore()
	{
		GameObject bestscore = (GameObject )(GameObject.Find("Bestscore2")); // .GetComponent("GUItext"));
		if (bestscore != null)
		{
			// bestscore.guiText.text = "Best "+bestScore;
			((TextMesh )bestscore.GetComponent("TextMesh")).text = "Best "+bestScore;
		}
		GameObject totalscore = (GameObject )(GameObject.Find("Totalscore2")); // .GetComponent("GUItext"));
		if (totalscore != null)
		{
			//totalscore.guiText.text = "Last "+totalScore;
			((TextMesh )totalscore.GetComponent("TextMesh")).text = "Last "+totalScore;
		}
	}

	void gameover ()
	{
		// Do something depending on the current mode
        switch (selectedTypeOfGame)
		{
		case TypeOfGame.QuickPlay:
			simpleGameOver();
			break;
        // Campaign will go to another scene, while saving the progression
        case TypeOfGame.Campaign:
            simpleGameOver();
            break;
        // All excepted challenge are linked to time
        case TypeOfGame.Score:
            simpleGameOver();
            break;
        case TypeOfGame.TimeAttack:
            simpleGameOver();
            break;
        // Offer different type of challenge. Should be difficult.
        case TypeOfGame.Challenges:
            simpleGameOver();
            break;
        case TypeOfGame.TimedScore:
            simpleGameOver();
            break;
		default:
			simpleGameOver();
			break;
		}
	}
	
	void simpleGameOver()
	{
		AudioSource.PlayClipAtPoint(brokenComboSound, Camera.main.transform.position);

		totalScore=scoreOfLevel;
		if (scoreOfLevel > bestScore)
		{
			bestScore = scoreOfLevel;
			PlayerPrefs.SetInt("highScoreSimpleGame",bestScore);
			PlayerPrefs.Save();
		}
		Debug.Log("Game Over");

		inGameOver = true;

		initIOutroBox ();
	}

	void goToMenu()
	{
		Application.LoadLevel("MenuLight");
	}

	ArrayList allPoints;
	
	void drawLinkingPaths()
	{
		foreach (RowColumn oneRC in allPoints)
		{
			Vector2 currentPos = listOfAllPosition[oneRC.x,oneRC.y];
			
			Instantiate(linkingPathMarker, new Vector3(currentPos.x, currentPos.y, 0), Quaternion.identity);
		}
	}
	
	void cleanLinkingPaths()
	{
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("LinkingPath");
		
		for(var i = 0 ; i < gameObjects.Length ; i ++)
			Destroy(gameObjects[i]);
	}

	/**
	 * Use a simple Depth First Search to see if the paths are connected
	 */
	bool checkForPathBetweenTwoOfTheSelected()
	{
		allPoints = new ArrayList();

		for (int iPoint = 0; iPoint < currentSelected - 1; ++iPoint)
		{
			if (isPathFrom(markedObjectPositions [iPoint], markedObjectPositions [iPoint+1]))
			{
				allPoints.AddRange(previousPlaces);
				return true;
			}
		}
	
		// We need to check the case where one selected tile block the other
		// Only workaround for 3!
			
		allPoints = new ArrayList();
			
		if (isPathFrom(markedObjectPositions [0], markedObjectPositions [2]))
		{
			allPoints.AddRange(previousPlaces);
			return true;
		}
		else if (isPathFrom(markedObjectPositions [2], markedObjectPositions [1]))
		{
			allPoints.AddRange(previousPlaces);
			return true;
		}

		// We need to check the case where one selected tile block the other
		// Only workaround for 3!
			
		allPoints = new ArrayList();
			
		if (isPathFrom(markedObjectPositions [0], markedObjectPositions [2]))
		{
			allPoints.AddRange(previousPlaces);
			return true;
		}
		else if (isPathFrom(markedObjectPositions [0], markedObjectPositions [1]))
		{
			// Draw the paths
			allPoints.AddRange(previousPlaces);
			return true;
		}

		return false;
	}

	/**
	 * Use a simple Depth First Search to see if the paths are connected
	 */
	bool checkForPathBetweenSelected()
	{
		allPoints = new ArrayList();
		
		bool foundPath = true;
		
		for (int iPoint = 0; iPoint < currentSelected - 1; ++iPoint)
		{
			if (!isPathFrom(markedObjectPositions [iPoint], markedObjectPositions [iPoint+1]))
			{
				foundPath = false;
				break;
			}
			allPoints.AddRange(previousPlaces);
		}
		if  (!foundPath)
		{
			// We need to check the case where one selected tile block the other
			// Only workaround for 3!
			foundPath = true;
			
			allPoints = new ArrayList();
			
			if (!isPathFrom(markedObjectPositions [0], markedObjectPositions [2]))
			{
				foundPath = false;
			}
			else if (!isPathFrom(markedObjectPositions [2], markedObjectPositions [1]))
			{
				foundPath = false;
			}
			allPoints.AddRange(previousPlaces);
			
		}
		if  (!foundPath)
		{
			// We need to check the case where one selected tile block the other
			// Only workaround for 3!
			foundPath = true;
			
			allPoints = new ArrayList();
			
			if (!isPathFrom(markedObjectPositions [0], markedObjectPositions [2]))
			{
				return false;
			}
			else if (!isPathFrom(markedObjectPositions [0], markedObjectPositions [1]))
			{
				return false;
			}
			allPoints.AddRange(previousPlaces);
			
		}
		// Draw the paths
		drawLinkingPaths();
		
		return true;
	}
	
	ArrayList previousPlaces ;
	ArrayList previousDirection;
	bool [,] isVisited;
	
	bool isPathFrom (RowColumn rowColumn, RowColumn rowColumn2)
	{
		bool notFinished = true; // Will be if out of the moving loop, or if the exit has been found (then immediately return true).
		int direction = 0; // Only 4, right, up,left, down
		int currentLength = 0;
		int nextX = 0;
		int nextY = 0;
		RowColumn currentRC = rowColumn;
		previousPlaces = new ArrayList();
		previousDirection = new ArrayList();
		
		isVisited = new bool[numberOfRows,numberOfColumns];
		
		for (int iRow = 0; iRow < numberOfRows; ++iRow) {
			for (int iColumn = 0; iColumn < numberOfColumns; ++iColumn) {
				isVisited[iRow,iColumn] = false;
			}
		}
		// From the initial point, always start one direction, then straight on until blocked. Once blocked, go backward once, and turn. If blocked, go backward.
		while (notFinished)
		{
			if (direction == 0)
			{
				// Check what next.
				nextX = currentRC.x +1;
				nextY = currentRC.y;
			}
			else if (direction == 1)
			{
				// Check what next.
				nextX = currentRC.x;
				nextY = currentRC.y + 1;
			}
			else if (direction == 2)
			{
				// Check what next.
				nextX = currentRC.x - 1;
				nextY = currentRC.y;
			}
			else // if (direction ==3)
			{
				// Check what next.
				nextX = currentRC.x;
				nextY = currentRC.y - 1;
			}
			
			// Did we reach the target (no need to actually go there)
			if (nextX == rowColumn2.x && nextY == rowColumn2.y)
			{
				// Keep the last coordinates to draw the path
				previousPlaces.Add(new RowColumn(nextX,nextY));
				return true;
			}
			
			// Now check if it is possible to go there
			bool okToGo = true;
			// Out of range
			if (nextX < 0 || nextX >= numberOfRows || nextY < 0 || nextY >= numberOfColumns)
			{
				okToGo = false;
			}
			else	if (newObject[nextX,nextY] == null)
			{
				// Null, Already visited
				if (isVisited[nextX,nextY])
				{
					okToGo = false;
				}
				// Null & not visited -> empty, ok
			}
			else
			{
				// Something is there
				okToGo = false;
			}
			
			if (okToGo)
			{
				// We keep the next place to be able to back-trace
				previousPlaces.Add(currentRC);
				previousDirection.Add (direction);
				direction = 0;
				currentRC = new RowColumn(nextX,nextY);
				// Do not visit again
				isVisited[nextX,nextY] = true;
				currentLength++;
				
				// Move the marker
				//				Vector2 currentPos = listOfAllPosition[nextX,nextY];
				//
				//				markerPath.transform.position.Set(currentPos.x,currentPos.y, 0.0f);
			}
			else
			{
				if (direction < 3)
				{
					// Look in another direction
					direction++;
				}
				else
				{
					// if we are back to our first place, return false.
					if (currentRC == rowColumn)
					{
						return false;
					}
					// Go back one position
					//					Debug.Log("Back to position "+currentLength+" Nb of RC: "+previousPlaces.Count+" Nb of dir: "+previousDirection.Count);
					
					currentLength--;
					
					currentRC = (RowColumn )previousPlaces[currentLength];
					direction = (int )previousDirection[currentLength];
					
					// Remove the last elements 
					// !!!!
					previousPlaces.RemoveAt(currentLength);
					previousDirection.RemoveAt(currentLength);
				}
			}
		}
		return false; // Never reached
	}
	
}
