using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Random = UnityEngine.Random;

public class ShowCreditsMMPG : MonoBehaviour
{
    public GameObject thisCamera;
    private GameObject mainCamera; // When activated, we deactivate the mainCamera, then start showing the credits
    private SwitchPrefsOnOff prefGui;
    public GameObject fullGUI;

    public GameObject titleLine;
    public GameObject nameLine;

    public GameObject ourSprite;

    public GameObject closeCreditObject; // For mobile, provide a button to close the credits

    //bool showCredit;

    List<string> allTitlesAndNames = new List<string>()
        {
           "MMPG","Maxi Micro\nPaintball\nGame",
           "A Game by","Alain Becam\n(Dev & gameplay)",
           "Graphics by","Nobody mostly :)\nand Sébastien Lesage\n(Monk & Bomb)",
           "Testing, gameplay ideas", "William",
           "Music by","Alexander Nakarada",
           "Music by","Chan Redfield",
           "Font", "GlametrixBold by\nGluk\nhttp://www.glukfonts.pl",
           "Special Thanks to",
           "You!"
        };

    public LocalizedString outroLocalizedAGameBy;
    private string localizedAGameByText = "";

    public LocalizedString outroLocalizedDevGamePlay;
    private string localizedDevGamePlayText = "";

    public LocalizedString outroLocalizedGraphicsBy;
    private string localizedGraphicsByTxt = "";

    public LocalizedString outroLocalizedNobody;
    private string localizedNobodyTxt = "";

    public LocalizedString outroLocalizedAnd;
    private string localizedAndTxt = "";

    public LocalizedString outroLocalizedMonkBomb;
    private string localizedMonkBombTxt = "";

    public LocalizedString outroLocalizedTestingGamePlay;
    private string localizedTestingGamePlayTxt = "";

    public LocalizedString outroLocalizedMusicBy;
    private string localizedMusicByTxt = "";

    public LocalizedString outroLocalizedFontBy;
    private string localizedFontByTxt = "";

    public LocalizedString outroLocalizedThanksTo;
    private string localizedThanksToTxt = "";

    void OnEnable()
    {
        outroLocalizedAGameBy.StringChanged += UpdateStringAGameBy;
        outroLocalizedDevGamePlay.StringChanged += UpdateStringDevGamePlay;
        outroLocalizedGraphicsBy.StringChanged += UpdateStringGraphicsBy;
        outroLocalizedNobody.StringChanged += UpdateStringNobody;
        outroLocalizedAnd.StringChanged += UpdateStringAnd;
        outroLocalizedMonkBomb.StringChanged += UpdateStringMonkBomb;
        outroLocalizedTestingGamePlay.StringChanged += UpdateStringTestingGamePlay;
        outroLocalizedMusicBy.StringChanged += UpdateStringMusicBy;
        outroLocalizedFontBy.StringChanged += UpdateStringFontBy;
        outroLocalizedThanksTo.StringChanged += UpdateStringThanksTo;
    }

    void OnDisable()
    {
        outroLocalizedAGameBy.StringChanged -= UpdateStringAGameBy;
        outroLocalizedDevGamePlay.StringChanged -= UpdateStringDevGamePlay;
        outroLocalizedGraphicsBy.StringChanged -= UpdateStringGraphicsBy;
        outroLocalizedNobody.StringChanged -= UpdateStringNobody;
        outroLocalizedAnd.StringChanged -= UpdateStringAnd;
        outroLocalizedMonkBomb.StringChanged -= UpdateStringMonkBomb;
        outroLocalizedTestingGamePlay.StringChanged -= UpdateStringTestingGamePlay;
        outroLocalizedMusicBy.StringChanged -= UpdateStringMusicBy;
        outroLocalizedFontBy.StringChanged -= UpdateStringFontBy;
        outroLocalizedThanksTo.StringChanged -= UpdateStringThanksTo;
    }

    void UpdateStringAGameBy(string s)
    {
        localizedAGameByText = s;
    }

    void UpdateStringDevGamePlay(string s)
    {
        localizedDevGamePlayText = s;
    }

    void UpdateStringGraphicsBy(string s)
    {
        localizedGraphicsByTxt = s;
    }

    void UpdateStringNobody(string s)
    {
        localizedNobodyTxt = s;
    }

    void UpdateStringAnd(string s)
    {
        localizedAndTxt = s;
    }

    void UpdateStringMonkBomb(string s)
    {
        localizedMonkBombTxt = s;
    }

    void UpdateStringTestingGamePlay(string s)
    {
        localizedTestingGamePlayTxt = s;
    }

    void UpdateStringMusicBy(string s)
    {
        localizedMusicByTxt = s;
    }

    void UpdateStringFontBy(string s)
    {
        localizedFontByTxt = s;
    }

    void UpdateStringThanksTo(string s)
    {
        localizedThanksToTxt = s;

        SetCreditText();
    }

    private void SetCreditText()
    {
        allTitlesAndNames = new List<string>()
        {
           "MMPG","Maxi Micro\nPaintball\nGame",
           localizedAGameByText,"Alain Becam\n"+localizedDevGamePlayText,
           localizedGraphicsByTxt,localizedNobodyTxt + "\n"+localizedAndTxt+" Sébastien Lesage\n" + localizedMonkBombTxt,
           localizedTestingGamePlayTxt, "William",
           localizedMusicByTxt,"Alexander Nakarada",
           localizedMusicByTxt,"Chan Redfield",
           localizedFontByTxt, "GlametrixBold by\nGluk\nhttp://www.glukfonts.pl",
           localizedThanksToTxt,
           ""
        };
    }
    // Start is called before the first frame update
    void Start()
    {
        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(outroLocalizedAGameBy.TableReference, outroLocalizedAGameBy.TableEntryReference);
    }

    const int nbOfFramesForTitle = 250;
    const int nbOfFramesForText = 350;
    const int offsetForText = 100;
    const int offsetForSprite = 100;
    const int nbOfFramesForSprite = 400;
    private readonly float stepZoomTitle = 0.015f / ((float)nbOfFramesForText);
    private readonly float stepZoomName = 0.015f / ((float)(nbOfFramesForText));

    int currentFrameForText = 0;
    int currentFrameForSprite = 0;

    private readonly float stepZoomSprite = 1f / ((float)nbOfFramesForSprite);

    float currentZoomTitle = 0.015f;
    float currentZoomName = 0.015f;
    float currentZoomSprite = 0;

    int currentLine = 0;
    int currentSprite = 0;

    int iTurn = 0;

    private void setAllElements()
    {
        Debug.Log("Setting up elements");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject prefImage = GameObject.FindGameObjectWithTag("PrefsSwitch");
        prefGui = prefImage.GetComponent<SwitchPrefsOnOff>();
    }
    // Update is called once per frame
    void Update()
    {
        checkForExit();

        iTurn++;
        if (iTurn % 2 == 0)
        {
            doATurn();
        }

        // For each 2 line:
        // Zoom the first one, then the 2nd one
        if (currentFrameForText < nbOfFramesForTitle)
        {
            currentZoomTitle -= stepZoomTitle;
            titleLine.transform.localScale = new Vector3(currentZoomTitle, currentZoomTitle);  
        }
        if (currentFrameForText < nbOfFramesForText && currentFrameForText > offsetForText)
        {
            currentZoomName -= stepZoomName;
            nameLine.transform.localScale = new Vector3(currentZoomName, currentZoomName);
        }

        currentFrameForText++;

        if (currentFrameForText > offsetForSprite && currentFrameForSprite < nbOfFramesForSprite)
        {
            currentZoomSprite += stepZoomSprite;
            ourSprite.transform.localScale = new Vector3(currentZoomSprite, currentZoomSprite, currentZoomSprite);

            currentFrameForSprite++;
        }
        if (currentFrameForSprite >= nbOfFramesForSprite)
        {
            // Start again
            setUpLinesAndSprite();
        }
        // Then zoom the next sprite until it covers everything
    }

    private void setUpLinesAndSprite()
    {
        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0.015f;
        currentZoomName = 0.015f;
        currentZoomSprite = 0;

        if (allTitlesAndNames.Count <= currentLine)
        {
            // Back to start
            currentLine = 0;
        }
        String title = allTitlesAndNames[currentLine++];
        String name = allTitlesAndNames[currentLine++];

        ((TextMesh)titleLine.GetComponent("TextMesh")).text = title;
        ((TextMesh)nameLine.GetComponent("TextMesh")).text = name;
        titleLine.transform.localScale = new Vector3(0.0f, 0.0f);
        nameLine.transform.localScale = new Vector3(0.0f, 0.0f);

        if (currentSprite >= ourSprite.transform.childCount)
        {
            currentSprite = 0;
        }
        // Start a new simulation
        InitMMPGEngine();

        ourSprite.transform.localPosition = new Vector3(0.0f, 0.0f, 4);
        ourSprite.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void checkForExit()
    {
        if (thisCamera.activeSelf)
        {
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                hideCredits();
            }
            else
            {
                // use the input stuff
                if (Mouse.current.leftButton.isPressed)
                {
                    hideCredits();
                }
                // Check if the user has clicked
                bool aTouch = Mouse.current.leftButton.isPressed;

                if (aTouch)
                {
                    hideCredits();
                }

                if (Keyboard.current.escapeKey.wasReleasedThisFrame)
                {
                    hideCredits();
                }
            }
        }
    }

    public void showCredits()
    {
        setAllElements();

        mainCamera.SetActive(false);
        thisCamera.SetActive(true);
        prefGui.hideMe();
        fullGUI.SetActive(false);

        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0;
        currentZoomName = 0;
        currentZoomSprite = 0;

        currentLine = 0;

        currentSprite = 0;

        closeCreditObject.SetActive(true);

        setUpLinesAndSprite();

        InitMMPGEngine();
    }

    public void hideCredits()
    {

        closeCreditObject.SetActive(false);

        mainCamera.SetActive(true);
        thisCamera.SetActive(false);
        if (mainCamera.GetComponent<AudioSource>() != null)
        {
            ((AudioSource)mainCamera.GetComponent<AudioSource>()).Play();
        }
        prefGui.showMe();
        fullGUI.SetActive(true);
    }

    float halfX;
    float halfY;

    private bool hasNature = true;
    private bool hasMonster = true;


    enum faction
    {
        none,
        player,
        ally,
        enemy,
        enemyAlly,
        neutral,
        nature,
        monster
    };

    Color colorPlayerBase = new Color(0, 0, 1);
    Color colorPlayerRanged = new Color(0.2f, 0.2f, 1);
    Color colorPlayerMonk = new Color(0.4f, 0.2f, 1);
    Color colorPlayerSpy = new Color(0, 0, 0.6f);
    Color colorPlayerBomb = new Color(0.5f, 0.5f, 1);
    Color colorPlayerMachine = new Color(0.2f, 0.2f, 0.8f);
    Color colorPlayerWall = new Color(0.8f, 0.8f, 1);
    Color colorHero1 = new Color(1.0f, 0.8f, 0.6f);
    Color colorHero2 = new Color(0.4f, 1.0f, 0.8f);
    Color colorHero3 = new Color(0.8f, 0.9f, 0.4f);
    Color colorHero4 = new Color(0.4f, 0.8f, 0.6f);
    Color colorHero5 = new Color(0.6f, 0.6f, 0.4f);
    Color colorHero6 = new Color(0.8f, 0.6f, 0.6f);

    Color colorAllyBase = new Color(0.4f, 0.4f, 1);
    Color colorAllyRanged = new Color(0.3f, 0.3f, 1);
    Color colorAllyMachine = new Color(0.4f, 0.4f, 0.8f);
    Color colorAllyWall = new Color(0.6f, 0.6f, 1);

    Color colorEnemyBase = new Color(1, 0, 0);
    Color colorEnemyRanged = new Color(1, 0.2f, 0.2f);
    Color colorEnemyMachine = new Color(0.8f, 0.2f, 0.2f);
    Color colorEnemyWall = new Color(1, 0.8f, 0.8f);

    Color colorEnemyAllyBase = new Color(1, 0.4f, 0.4f);
    // TODO

    Color colorNeutralBase = new Color(0.4f, 0.4f, 0.4f);
    // TODO

    Color colorNatureBase = new Color(0, 1f, 0); // Plants
    Color colorNatureAnimal = new Color(0.3f, 1f, 0.2f);
    Color colorNatureAggrAnimal = new Color(0.1f, 1f, 0.3f);

    Color colorMonsterBase = new Color(0.6f, 0.6f, 0.4f);
    Color colorMonsterRanged = new Color(0.8f, 0.6f, 0.4f);

    const int sizeArena = 256;
    const int sizeArenaMinusOne = sizeArena - 1;

    public enum typeOfUnit
    {
        typeEmpty,
        close,
        ranged,
        monk,
        spy,
        bomb,
        machine,
        wall,
        tree,
        animal,
        aggr_animal,
        occupied, // Type to signal we do not push any unit here-> around a monster for instance.
        hero1, // Hero found with secret codes
        hero2,
        hero3,
        hero4,
        hero5,
        hero6,
    };

    private class Unit
    {
        public faction faction; // 0-> player, 1-> enemy, 2->neutral
        public int strength;
        public int stamina;
        public int defense;
        public int health;

        public typeOfUnit type;
        public int fightRange;
        internal int staminaMax;
        internal int iteration;
    }

    private Unit[,] allUnits = new Unit[sizeArena, sizeArena];

    Texture2D texture;

    private int[] someRandom = new int[2 * sizeArena];

    int nbOfClosedEnemies = 0;
    int nbOfRangedEnemies = 0;
    int nbOfMachinesEnemies = 0;

    int currentNbOfClosedEnemies = 0;
    int currentNbOfRangedEnemies = 0;
    int currentNbOfMachinesEnemies = 0;

    int nbOfClosed = 0;
    int nbOfRanged = 0;
    int nbOfMachines = 0;
    int nbOfMonks = 0;
    int nbOfSpies = 0;
    int nbOfBombs = 0; //  currentLevel;

    int currentNbOfClosed = 0;
    int currentNbOfRanged = 0;
    int currentNbOfMachines = 0;
    int currentNbOfMonks = 0;
    int currentNbOfSpies = 0;
    int currentNbOfBombs = 0;

    int availableNbOfClosed = 0;
    int availableNbOfRanged = 0;
    int availableNbOfMachines = 0;
    int availableNbOfMonks = 0;
    int availableNbOfSpies = 0;
    int availableNbOfBombs = 0;

    bool isHero1 = true;
    bool isHero2 = true;
    bool isHero3 = true;
    bool isHero4 = true;
    bool isHero5 = true;
    bool isHero6 = true;

    // Heroes are activated to get their superpower
    bool hero1Activated = false;
    bool hero2Activated = false;
    bool hero3Activated = false;
    bool hero4Activated = false;
    bool hero5Activated = false;
    bool hero6Activated = false;

    int currentIteration = 0;
    // Use this for initialization
    void InitMMPGEngine()
    {
        currentIteration = 0;

        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();

        isHero1 = playersUnit.isHero1;
        isHero2 = playersUnit.isHero2;
        isHero3 = playersUnit.isHero3;
        isHero4 = playersUnit.isHero4;
        isHero5 = playersUnit.isHero5;
        isHero6 = playersUnit.isHero6;

        prepareMap();

        // For testing, add player units here
        prepareMapPlayer();

        createTexture();

        createRandom();
    }

    private void createRandom()
    {
        for (int iRand = 0; iRand < sizeArena * 2; iRand++)
        {
            someRandom[iRand] = ((int)(10 * Random.value)) - 5;
        }
    }

    private void createTexture()
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        texture = new Texture2D(sizeArena, sizeArena, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < sizeArena; y++)
        {
            for (int x = 0; x < sizeArena; x++)
            {
                Unit unitAtPlace = allUnits[x, y];
                Color colorAtPlace = Color.black;

                //Debug.Log("Unit at " + x + "," + y + " is of faction " + unitAtPlace.faction);
                if (unitAtPlace.faction == faction.player)
                {
                    colorAtPlace = doColouringPlayer(unitAtPlace);
                }
                else if (unitAtPlace.faction == faction.ally)
                {
                    colorAtPlace = doColouringAllies(unitAtPlace);
                }
                else if (unitAtPlace.faction == faction.enemy)
                {
                    colorAtPlace = doColouringEnemies(unitAtPlace);
                }
                else if (unitAtPlace.faction == faction.enemyAlly)
                {
                    colorAtPlace = doColouringEnemiesAllies(unitAtPlace);
                }
                else if (unitAtPlace.faction == faction.nature)
                {
                    colorAtPlace = doColouringNature(unitAtPlace);
                }
                else if (unitAtPlace.faction == faction.monster)
                {
                    colorAtPlace = doColouringMonster(unitAtPlace);
                }

                // Then taint it depending on the type
                texture.SetPixel(x, y, colorAtPlace);
            }
        }

        // Apply all SetPixel calls
        texture.Apply();

        // connect texture to material of GameObject this script is attached to

        Renderer imageRenderer = ourSprite.GetComponent<Renderer>();
        imageRenderer.material.mainTexture = texture;
    }

    private Color doColouringMonster(Unit forUnit)
    {
        switch (forUnit.type)
        {
            case typeOfUnit.close:
                return colorMonsterBase;

            default:
                return colorMonsterRanged;
        }
    }

    private Color doColouringNature(Unit forUnit)
    {
        switch (forUnit.type)
        {
            case typeOfUnit.tree:
                return colorNatureBase;

            case typeOfUnit.animal:
                return colorNatureAnimal;

            default:
                return colorNatureAggrAnimal;
        }
    }

    /**
     * No allies yet
     */
    private Color doColouringEnemiesAllies(Unit forUnit)
    {
        return colorEnemyAllyBase;
    }

    private Color doColouringEnemies(Unit forUnit)
    {
        switch (forUnit.type)
        {
            case typeOfUnit.close:
                return colorEnemyBase;

            case typeOfUnit.ranged:
                return colorEnemyRanged;

            case typeOfUnit.machine:
                return colorEnemyMachine;

            default:
                return colorEnemyWall;
        }
    }

    private Color doColouringAllies(Unit forUnit)
    {
        switch (forUnit.type)
        {
            case typeOfUnit.close:
                return colorAllyBase;

            case typeOfUnit.ranged:
                return colorAllyRanged;

            case typeOfUnit.machine:
                return colorAllyMachine;

            default:
                return colorAllyWall;
        }
    }

    private Color doColouringPlayer(Unit forUnit)
    {
        switch (forUnit.type)
        {
            case typeOfUnit.close:
                return colorPlayerBase;

            case typeOfUnit.ranged:
                return colorPlayerRanged;

            case typeOfUnit.machine:
                return colorPlayerMachine;

            case typeOfUnit.monk:
                return colorPlayerMonk;

            case typeOfUnit.spy:
                return colorPlayerSpy;

            case typeOfUnit.wall:
                return colorPlayerWall;

            case typeOfUnit.bomb:
                return colorPlayerBomb;

            // And heroes
            case typeOfUnit.hero1:
                return colorHero1;

            case typeOfUnit.hero2:
                return colorHero2;

            case typeOfUnit.hero3:
                return colorHero3;

            case typeOfUnit.hero4:
                return colorHero4;

            case typeOfUnit.hero5:
                return colorHero5;

            default:
                return colorHero6;
        }
    }

    int rangedComeAt = 10; // After level 10, we start adding ranged units.
    int machinesComeAt = 100;
    int wallsComeEvery = 30; // Every 30 levels, add a wall.

    Unit oneEmptyZone = new Unit();

    /**
     * Place nature (animals), monsters and enemies.
     */
    private void prepareMap()
    {
        Debug.Log("Initialise with empty");

        int currentLevel = (int )(Random.value * 500);
        oneEmptyZone.type = typeOfUnit.typeEmpty;
        oneEmptyZone.faction = faction.none;
        oneEmptyZone.iteration = 0;

        for (int y = sizeArenaMinusOne; y >= 0; y--)
        {
            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                allUnits[x, y] = oneEmptyZone;
            }
        }

        Debug.Log("Place nature");

        int nbOfNature = (int)((Random.value) * 1000);
        float thresholdMonster = (0.5f) * Random.value + 0.5f;

        // The unit to codemn the area around the monsters
        Unit oneMonsterZone = new Unit();
        oneMonsterZone.type = typeOfUnit.occupied;
        oneMonsterZone.faction = faction.none;
        oneMonsterZone.iteration = 0;

        Debug.Log("Will place " + nbOfNature + " nature elements");

        for (int iNature = 0; iNature < nbOfNature; iNature++)
        {
            if (hasMonster)
            {
                // Check if we place a monster
                float valMonster = Random.value;

                if (valMonster > thresholdMonster)
                {
                    Debug.Log("Will place a monster");

                    float xMonster = Random.value * sizeArena;
                    float yMonster = Random.value * sizeArena;

                    float valTypeMonster = Random.value;

                    Unit oneMonster = new Unit();


                    oneMonster.faction = faction.monster;
                    oneMonster.defense = 6; // Quite strong
                    oneMonster.fightRange = 2;

                    oneMonster.type = typeOfUnit.close;

                    if (valTypeMonster > 0.7f)
                    {
                        oneMonster.type = typeOfUnit.ranged;
                        oneMonster.fightRange = 5;
                    }


                    oneMonster.strength = 90;
                    oneMonster.stamina = 300;
                    oneMonster.staminaMax = 300;
                    oneMonster.health = 1000;

                    allUnits[(int)xMonster, (int)yMonster] = oneMonster;

                    // If another monster is there, we simply replace it, the risk of collision are rather low anyway

                    // Then condemn the surrounding places
                    int xMonsterLow = ((int)xMonster) - 1;
                    int yMonsterLow = ((int)yMonster) - 1;

                    if (xMonsterLow < 0)
                    {
                        xMonsterLow = 0;
                    }
                    if (yMonsterLow < 0)
                    {
                        yMonsterLow = 0;
                    }

                    int xMonsterHigh = ((int)xMonster) + 1;
                    int yMonsterHigh = ((int)yMonster) + 1;

                    if (xMonsterHigh > sizeArena)
                    {
                        xMonsterHigh = sizeArena;
                    }
                    if (yMonsterHigh > sizeArena)
                    {
                        yMonsterHigh = sizeArena;
                    }

                    for (int xMonsterZone = xMonsterLow; xMonsterZone < xMonsterHigh; xMonsterZone++)
                    {
                        for (int yMonsterZone = yMonsterLow; yMonsterZone < yMonsterHigh; yMonsterZone++)
                        {
                            Debug.Log("At " + xMonsterZone + ", " + yMonsterZone + " on " + xMonsterLow + ", " + xMonsterHigh + " : " + yMonsterLow + ", " + yMonsterHigh);
                            if (allUnits[xMonsterZone, yMonsterZone].type == typeOfUnit.typeEmpty)
                            {
                                allUnits[xMonsterZone, yMonsterZone] = oneMonsterZone;
                                Debug.Log("Placing a monster exclusion zone at " + xMonsterZone + ", " + yMonsterZone);
                            }
                        }
                    }
                }
            }
            if (hasNature)
            {
                float xNature = Random.value * sizeArena;
                float yNature = Random.value * sizeArena;

                float valTypeNature = Random.value;

                Unit oneNatureUnit = new Unit();

                oneNatureUnit.iteration = 0;
                oneNatureUnit.faction = faction.nature;

                // Nature can be tree (wall), animal (closed), agressive animal (ranged)
                if (valTypeNature > 0.7f)
                {
                    oneNatureUnit.type = typeOfUnit.aggr_animal;
                    oneNatureUnit.fightRange = 1;

                    oneNatureUnit.defense = 2; // Not strong

                    oneNatureUnit.strength = 5;
                    oneNatureUnit.stamina = 10;
                    oneNatureUnit.staminaMax = 10;
                    oneNatureUnit.health = 100;
                }
                else if (valTypeNature > 0.5f)
                {
                    // Will attack if attacked?
                    oneNatureUnit.type = typeOfUnit.animal;
                    oneNatureUnit.fightRange = 1;

                    oneNatureUnit.defense = 2; // Not strong

                    oneNatureUnit.strength = 4;
                    oneNatureUnit.stamina = 8;
                    oneNatureUnit.staminaMax = 8;
                    oneNatureUnit.health = 60;
                }
                else
                {
                    // Obstacle
                    oneNatureUnit.type = typeOfUnit.tree;
                    oneNatureUnit.fightRange = 0;

                    oneNatureUnit.defense = 6; // Strong

                    oneNatureUnit.strength = 0;
                    oneNatureUnit.stamina = 20;
                    oneNatureUnit.staminaMax = 20;
                    oneNatureUnit.health = 200;
                }

                allUnits[(int)xNature, (int)yNature] = oneNatureUnit;
            }
        }
        Debug.Log("Place Enemies for level " + currentLevel);

        nbOfClosedEnemies = currentLevel * 50;
        nbOfRangedEnemies = (currentLevel - rangedComeAt) * 25;
        nbOfMachinesEnemies = (currentLevel - machinesComeAt) * 25;
        bool isWall = (currentLevel % wallsComeEvery) == 0;

        int iClosed = nbOfClosedEnemies;
        int iRanged = nbOfRangedEnemies;
        int iMachine = nbOfMachinesEnemies;

        int yWall = 0; // Where to build the wall.

        // Place the enemies and create the initial texture
        for (int y = sizeArenaMinusOne; y >= 0; y--)
        {
            bool leavingLoop = false;
            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                // Check if we can place
                if (allUnits[x, y].type == typeOfUnit.typeEmpty)
                {
                    //Debug.Log("Placing 1 enemy at " + x + ", " + y+" : "+ iMachine+" : "+iRanged+" : "+iClosed);

                    Unit oneEnemyUnit = new Unit();

                    oneEnemyUnit.iteration = 0;
                    oneEnemyUnit.faction = faction.enemy;

                    bool wasAnythingLeft = false;

                    // Check what we place
                    if (isWall)
                    {
                        if (iClosed > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.close;
                            oneEnemyUnit.fightRange = 1;

                            oneEnemyUnit.defense = 4; // Not strong

                            oneEnemyUnit.strength = 10;
                            oneEnemyUnit.stamina = 10;
                            oneEnemyUnit.staminaMax = 10;
                            oneEnemyUnit.health = 60;

                            iClosed--;

                            wasAnythingLeft = true;
                        }
                        else if (iMachine > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.machine;
                            oneEnemyUnit.fightRange = 5;

                            oneEnemyUnit.defense = 3;

                            oneEnemyUnit.strength = 50;
                            oneEnemyUnit.stamina = 8;
                            oneEnemyUnit.staminaMax = 8;
                            oneEnemyUnit.health = 50;

                            iMachine--;

                            wasAnythingLeft = true;
                        }
                        else if (iRanged > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.ranged;
                            oneEnemyUnit.fightRange = 3;

                            oneEnemyUnit.defense = 2; // Not strong

                            oneEnemyUnit.strength = 10;
                            oneEnemyUnit.stamina = 20;
                            oneEnemyUnit.staminaMax = 20;
                            oneEnemyUnit.health = 40;

                            iRanged--;

                            wasAnythingLeft = true;
                        }
                    }
                    else
                    {
                        if (iMachine > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.machine;
                            oneEnemyUnit.fightRange = 5;

                            oneEnemyUnit.defense = 3;

                            oneEnemyUnit.strength = 50;
                            oneEnemyUnit.stamina = 8;
                            oneEnemyUnit.staminaMax = 8;
                            oneEnemyUnit.health = 50;

                            iMachine--;

                            wasAnythingLeft = true;
                        }
                        else if (iRanged > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.ranged;
                            oneEnemyUnit.fightRange = 3;

                            oneEnemyUnit.defense = 2; // Not strong

                            oneEnemyUnit.strength = 10;
                            oneEnemyUnit.stamina = 20;
                            oneEnemyUnit.staminaMax = 20;
                            oneEnemyUnit.health = 40;

                            iRanged--;

                            wasAnythingLeft = true;
                        }
                        else if (iClosed > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.close;
                            oneEnemyUnit.fightRange = 1;

                            oneEnemyUnit.defense = 4; // Not strong

                            oneEnemyUnit.strength = 10;
                            oneEnemyUnit.stamina = 10;
                            oneEnemyUnit.staminaMax = 10;
                            oneEnemyUnit.health = 60;

                            iClosed--;

                            wasAnythingLeft = true;
                        }
                    }
                    if (wasAnythingLeft)
                    {
                        //Debug.Log("Placing enemy of type " + oneEnemyUnit.type);

                        allUnits[x, y] = oneEnemyUnit;

                        countUnit(oneEnemyUnit, false);
                    }
                    else
                    {
                        yWall = y - 1;
                        leavingLoop = true;
                        break; // Nothing left to do
                    }
                }
            }
            if (leavingLoop)
            {
                break;
            }
        }

        // And add a wall if needed
        Debug.Log("Wall would be at " + yWall);
        if (isWall)
        {
            for (int x = 0; x < sizeArena; x++)
            {
                Unit oneEnemyUnit = new Unit();

                oneEnemyUnit.iteration = 0;
                oneEnemyUnit.faction = faction.enemy;

                oneEnemyUnit.type = typeOfUnit.wall;
                oneEnemyUnit.fightRange = 0;

                oneEnemyUnit.defense = 0; // Strong

                oneEnemyUnit.strength = 0;
                oneEnemyUnit.stamina = 0;
                oneEnemyUnit.staminaMax = 0;
                oneEnemyUnit.health = 6000;

                allUnits[x, yWall] = oneEnemyUnit;
            }
        }
        // Remove the occupied places around the monster
        for (int y = sizeArenaMinusOne; y >= 0; y--)
        {
            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                if (allUnits[x, y].type == typeOfUnit.occupied)
                {
                    allUnits[x, y] = oneEmptyZone;
                }
            }
        }
    }

    /**
     * Place player units.
     * For testing: same as enemies, just reversed
     */
    private void prepareMapPlayer()
    {
        int currentLevel = (int)(Random.value * 500);

        Debug.Log("Place player units for level " + currentLevel);

        nbOfClosed = currentLevel * 50;
        nbOfRanged = (currentLevel - rangedComeAt) * 25;
        nbOfMachines = (currentLevel - machinesComeAt) * 25;
        nbOfMonks = (currentLevel - machinesComeAt) * 10;
        nbOfSpies = (currentLevel - machinesComeAt) * 2;
        nbOfBombs = currentLevel;

        bool isWall = (currentLevel % wallsComeEvery) == 0;

        availableNbOfClosed = nbOfClosed;
        availableNbOfRanged = nbOfRanged;
        availableNbOfMachines = nbOfMachines;
        availableNbOfMonks = nbOfMonks;
        availableNbOfSpies = nbOfSpies;

        // Place the enemies and create the initial texture
        for (int y = 0; y < sizeArena; y++)
        {
            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                // Check if we can place
                if (allUnits[x, y].type == typeOfUnit.typeEmpty)
                {
                    //Debug.Log("Placing 1 enemy at " + x + ", " + y+" : "+ iMachine+" : "+iRanged+" : "+iClosed);

                    Unit playerNetUnit = new Unit();

                    playerNetUnit.iteration = 0;
                    playerNetUnit.faction = faction.player;

                    bool wasAnythingLeft = false;

                    // Check what we place
                    if (availableNbOfMachines > 0)
                    {
                        playerNetUnit.type = typeOfUnit.machine;
                        playerNetUnit.fightRange = 5;

                        playerNetUnit.defense = 3;

                        playerNetUnit.strength = 50;
                        playerNetUnit.stamina = 8;
                        playerNetUnit.staminaMax = 8;
                        playerNetUnit.health = 50;

                        availableNbOfMachines--;

                        wasAnythingLeft = true;
                    }
                    else if (availableNbOfRanged > 0)
                    {
                        playerNetUnit.type = typeOfUnit.ranged;
                        playerNetUnit.fightRange = 3;

                        playerNetUnit.defense = 2; // Not strong

                        playerNetUnit.strength = 10;
                        playerNetUnit.stamina = 20;
                        playerNetUnit.staminaMax = 20;
                        playerNetUnit.health = 40;

                        availableNbOfRanged--;

                        wasAnythingLeft = true;
                    }
                    else if (availableNbOfClosed > 0)
                    {
                        playerNetUnit.type = typeOfUnit.close;
                        playerNetUnit.fightRange = 1;

                        playerNetUnit.defense = 4; // Not strong

                        playerNetUnit.strength = 10;
                        playerNetUnit.stamina = 10;
                        playerNetUnit.staminaMax = 10;
                        playerNetUnit.health = 600;

                        availableNbOfClosed--;

                        wasAnythingLeft = true;
                    }
                    else if (availableNbOfMonks > 0)
                    {
                        playerNetUnit.type = typeOfUnit.monk;
                        playerNetUnit.fightRange = 3;

                        playerNetUnit.defense = 4;

                        playerNetUnit.strength = 0;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 600;

                        availableNbOfMonks--;

                        wasAnythingLeft = true;
                    }
                    else if (isHero1)
                    {
                        playerNetUnit.type = typeOfUnit.hero1;
                        playerNetUnit.fightRange = 1;

                        playerNetUnit.defense = 120; // Very strong

                        playerNetUnit.strength = 40;
                        playerNetUnit.stamina = 100;
                        playerNetUnit.staminaMax = 100;
                        playerNetUnit.health = 6000;

                        isHero1 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isHero2)
                    {
                        playerNetUnit.type = typeOfUnit.hero2;
                        playerNetUnit.fightRange = 10;

                        playerNetUnit.defense = 120; // Very strong

                        playerNetUnit.strength = 400;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 6000;

                        isHero2 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isHero2)
                    {
                        playerNetUnit.type = typeOfUnit.hero2;
                        playerNetUnit.fightRange = 20;

                        playerNetUnit.defense = 100; // Very strong

                        playerNetUnit.strength = 400;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 6000;

                        isHero2 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isHero3)
                    {
                        playerNetUnit.type = typeOfUnit.hero3;
                        playerNetUnit.fightRange = 8;

                        playerNetUnit.defense = 100; // Very strong

                        playerNetUnit.strength = 200;
                        playerNetUnit.stamina = 10000;
                        playerNetUnit.staminaMax = 10000;
                        playerNetUnit.health = 6000;

                        isHero3 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isHero4)
                    {
                        playerNetUnit.type = typeOfUnit.hero4;
                        playerNetUnit.fightRange = 8;

                        playerNetUnit.defense = 500; // Very strong

                        playerNetUnit.strength = 20;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 6000;

                        isHero4 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isHero5)
                    {
                        playerNetUnit.type = typeOfUnit.hero5;
                        playerNetUnit.fightRange = 20;

                        playerNetUnit.defense = 100; // Very strong

                        playerNetUnit.strength = 80;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 4000;

                        isHero5 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isHero6)
                    {
                        playerNetUnit.type = typeOfUnit.hero6;
                        playerNetUnit.fightRange = 20;

                        playerNetUnit.defense = 100; // Very strong

                        playerNetUnit.strength = 400;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 600;

                        isHero6 = false;
                        wasAnythingLeft = true;
                    }
                    else if (isWall)
                    {
                        // Do a line of wall...
                        //wasAnythingLeft = true;
                    }
                    if (wasAnythingLeft)
                    {
                        //Debug.Log("Placing enemy of type " + oneEnemyUnit.type);

                        allUnits[x, y] = playerNetUnit;

                        countUnit(playerNetUnit, true);
                    }
                    else
                    {
                        break; // Nothing left to do
                    }
                }
            }
        }

        for (int iBomb = 0; iBomb < nbOfBombs; iBomb++)
        {
            int x = (int)(Random.value * (sizeArena - 30) + 15);
            int y = (int)(Random.value * 100 + sizeArena / 2);

            Unit playerNetUnit = new Unit();

            playerNetUnit.iteration = 0;
            playerNetUnit.faction = faction.player;

            playerNetUnit.type = typeOfUnit.bomb;
            playerNetUnit.fightRange = 1;

            playerNetUnit.defense = 100;

            playerNetUnit.strength = 4;
            playerNetUnit.stamina = 8;
            playerNetUnit.staminaMax = 8;
            playerNetUnit.health = 50;

            allUnits[x, y] = playerNetUnit;

            availableNbOfBombs--;
        }
        for (int iSpy = 0; iSpy < nbOfSpies; iSpy++)
        {
            int x = (int)(Random.value * (sizeArena - 30) + 15);
            int y = (int)(sizeArena - Random.value * 100);

            Unit playerNetUnit = new Unit();

            playerNetUnit.iteration = 0;
            playerNetUnit.faction = faction.player;

            playerNetUnit.type = typeOfUnit.spy;
            playerNetUnit.fightRange = 1;

            playerNetUnit.defense = 6;

            playerNetUnit.strength = 40;
            playerNetUnit.stamina = 800;
            playerNetUnit.staminaMax = 800;
            playerNetUnit.health = 500;

            allUnits[x, y] = playerNetUnit;

            availableNbOfSpies--;
        }
    }
    private struct coordInt
    {
        public int x;
        public int y;
    }

    private void doATurn()
    {
        // First phase: preparation
        currentNbOfClosedEnemies = 0;
        currentNbOfRangedEnemies = 0;
        currentNbOfMachinesEnemies = 0;

        currentNbOfClosed = 0;
        currentNbOfRanged = 0;
        currentNbOfMachines = 0;
        currentNbOfMonks = 0;
        currentNbOfSpies = 0;
        currentNbOfBombs = 0;

        // Some units to pursue
        coordInt[] sampleEnemies = new coordInt[10];
        coordInt[] samplePlayer = new coordInt[10];

        coordInt middleUp = new coordInt();
        middleUp.x = sizeArena / 2;
        middleUp.y = 1023;

        coordInt middleDown = new coordInt();
        middleDown.x = 0; // sizeArena / 2;
        middleDown.y = 0;

        // First found is always used, so we have at least one

        int iEnemy = 0;
        int iPlayer = 0;

        int nextIteration = currentIteration + 1;

        for (int y = sizeArenaMinusOne; y >= 0; y--)
        {
            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                // Check if we can place
                if (allUnits[x, y].faction == faction.enemy)
                {
                    if (x % 10 == 0)
                    {
                        sampleEnemies[iEnemy].x = x;
                        sampleEnemies[iEnemy++].y = y;
                    }
                }
                if (iEnemy == 10)
                {
                    break;
                }
            }
            if (iEnemy == 10)
            {
                break;
            }
        }
        for (int y = 0; y < sizeArena; y++)
        {
            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                if (iPlayer < 10 && allUnits[x, y].faction == faction.player)
                {
                    if (x % 10 == 0)
                    {
                        samplePlayer[iPlayer].x = x;
                        samplePlayer[iPlayer++].y = y;
                    }
                }
                if (iPlayer == 10)
                {
                    break;
                }
            }
            if (iPlayer == 10)
            {
                break;
            }
        }
        int sampleIndexEnemy; // Depending where we are horizontaly, target another unit
        int sampleIndexPlayer;

        //int iEnemyToManage = 0;
        //int iPlayerToManage = 0;

        for (int y = 0; y < sizeArena; y++)
        {
            sampleIndexEnemy = 0;
            sampleIndexPlayer = 0;

            // Top to bottom
            for (int x = 0; x < sizeArena; x++)
            {
                if (sampleIndexEnemy < iEnemy - 1 && x % 10 == 0)
                {
                    sampleIndexEnemy++;
                }
                if (sampleIndexPlayer < iPlayer - 1 && x % 10 == 0)
                {
                    sampleIndexPlayer++;
                }

                Unit unit = allUnits[x, y];

                if (unit.type != typeOfUnit.typeEmpty && unit.iteration == currentIteration)
                {
                    // In all case, do not do anything anymore in this turn
                    unit.iteration = nextIteration;

                    if (unit.faction == faction.enemy)
                    {
                        // Deplace or attack
                        doAttackOrMoveEnemy(x, y, unit, samplePlayer[sampleIndexPlayer]);

                        countUnit(unit, false);
                        //doAttackOrMoveEnemy(x, y, unit, middleDown);
                    }
                    else if (unit.faction == faction.player)
                    {
                        // Deplace or attack. Duplicated code for hopefuly faster execution
                        doAttackOrMovePlayer(x, y, unit, sampleEnemies[sampleIndexEnemy]);

                        countUnit(unit, true);
                        //doAttackOrMovePlayer(x, y, unit, middleUp);
                    }
                    else if (unit.type != typeOfUnit.tree && unit.type != typeOfUnit.wall)
                    {
                        // Deplace or attack
                        doAttackOrMoveNatureMonster(x, y, unit);
                        //doAttackOrMoveEnemy(x, y, unit, middleDown);
                    }
                }
                //if (iEnemyToManage >= 100 && iPlayerToManage >= 100)
                //{
                //    break;
                //}
            }
            //if (iEnemyToManage >= 100 && iPlayerToManage >= 100)
            //{
            //    break;
            //}
        }
        currentIteration++;

        texture.Apply();
    }

    private void countUnit(Unit unit, bool isPlayer)
    {
        if (isPlayer)
        {
            switch (unit.type)
            {
                case typeOfUnit.close:
                    currentNbOfClosed++;
                    break;

                case typeOfUnit.ranged:
                    currentNbOfRanged++;
                    break;

                case typeOfUnit.machine:
                    currentNbOfMachines++;
                    break;

                case typeOfUnit.monk:
                    currentNbOfMonks++;
                    break;

                case typeOfUnit.spy:
                    currentNbOfSpies++;
                    break;

                case typeOfUnit.bomb:
                    currentNbOfBombs++;
                    break;
            }
        }
        else
        {
            switch (unit.type)
            {
                case typeOfUnit.close:
                    currentNbOfClosedEnemies++;
                    break;

                case typeOfUnit.ranged:
                    currentNbOfRangedEnemies++;
                    break;

                case typeOfUnit.machine:
                    currentNbOfMachinesEnemies++;
                    break;
            }
        }
    }

    private void doAttackOrMoveNatureMonster(int x, int y, Unit unit)
    {
        // Go around and eventually attack
        if (unit.stamina <= 0)
        {
            // Need to rest first
            unit.stamina += 2;

            return;
        }
        // Check the surrounding depending on the attack range
        int xUnitLow = x - unit.fightRange;
        int yUnitLow = y - unit.fightRange;

        if (xUnitLow < 0)
        {
            xUnitLow = 0;
        }
        if (yUnitLow < 0)
        {
            yUnitLow = 0;
        }

        int xUnitHigh = x + unit.fightRange;
        int yUnitHigh = y + unit.fightRange;

        if (xUnitHigh > sizeArena)
        {
            xUnitHigh = sizeArena;
        }
        if (yUnitHigh > sizeArena)
        {
            yUnitHigh = sizeArena;
        }

        for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
        {
            for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
            {
                Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                if (unit.type != typeOfUnit.animal && (unitEnemy.faction == faction.player) || (unitEnemy.faction == faction.enemy))
                {
                    // Attack enemies and player
                    {
                        // Attack this unit. Do nothing else.
                        int hit = unit.strength - unitEnemy.defense;
                        if (hit < 0)
                        {
                            hit = 2;
                        }
                        unitEnemy.health -= hit;

                        unit.stamina--;

                        if (unitEnemy.health < 0)
                        {
                            // Remove this unit
                            allUnits[xUnitZone, yUnitZone] = oneEmptyZone;

                            updateTexture(xUnitZone, yUnitZone, oneEmptyZone);
                        }

                        return;
                    }
                }
            }
        }

        // There was no fight, move to the "closest" enemy
        int random = someRandom[(currentIteration + x + x) % (sizeArena + sizeArena)];
        //Debug.Log("Our Random is " + random);

        if (random > 2 && y > 1)
        {
            if (allUnits[x, y - 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y - 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y - 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (random > 0 && x > 1)
        {
            if (allUnits[x - 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x - 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x - 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (random < -2 && y < sizeArenaMinusOne)
        {
            if (allUnits[x, y + 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y + 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y + 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (random < 0 && x < sizeArenaMinusOne)
        {
            if (allUnits[x + 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x + 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x + 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        // Finally an unit doing nothing can regain some stamina.

        if (unit.stamina < unit.staminaMax)
        {
            unit.stamina++;
        }
    }

    private void doAttackOrMoveEnemy(int x, int y, Unit unit, coordInt target)
    {
        if (unit.type == typeOfUnit.wall)
        {
            return;
        }
        if (unit.stamina <= 0)
        {
            // Need to rest first
            unit.stamina += 20;

            return;
        }
        // Check the surrounding depending on the attack range
        int xUnitLow = x - unit.fightRange;
        int yUnitLow = y - unit.fightRange;

        if (xUnitLow < 0)
        {
            xUnitLow = 0;
        }
        if (yUnitLow < 0)
        {
            yUnitLow = 0;
        }

        int xUnitHigh = x + unit.fightRange;
        int yUnitHigh = y + unit.fightRange;

        if (xUnitHigh > sizeArena)
        {
            xUnitHigh = sizeArena;
        }
        if (yUnitHigh > sizeArena)
        {
            yUnitHigh = sizeArena;
        }

        for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
        {
            for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
            {
                Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                if ((unitEnemy.faction == faction.player) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                {
                    // Attack this unit. Do nothing else.
                    int hit = unit.strength - unitEnemy.defense;
                    if (hit < 0)
                    {
                        hit = 2;
                    }
                    unitEnemy.health -= hit;

                    unit.stamina--;

                    if (unitEnemy.health < 0)
                    {
                        // Remove this unit
                        allUnits[xUnitZone, yUnitZone] = oneEmptyZone;

                        updateTexture(xUnitZone, yUnitZone, oneEmptyZone);
                    }

                    return;
                }
            }
        }

        // There was no fight, move to the "closest" enemy
        if (y > target.y && y > 1)
        {
            if (allUnits[x, y - 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y - 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y - 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (x > target.x && x > 1)
        {
            if (allUnits[x - 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x - 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x - 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (y < target.y && y < sizeArenaMinusOne)
        {
            if (allUnits[x, y + 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y + 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y + 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (x < target.x && x < sizeArenaMinusOne)
        {
            if (allUnits[x + 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x + 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x + 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        // Finally an unit doing nothing can regain some stamina.

        if (unit.stamina < unit.staminaMax)
        {
            unit.stamina++;
        }
    }

    private void updateTexture(int x, int y, Unit unitAtPlace)
    {
        Color colorAtPlace = Color.black;

        //Debug.Log("Unit at " + x + "," + y + " is of faction " + unitAtPlace.faction);
        if (unitAtPlace.faction == faction.player)
        {
            colorAtPlace = doColouringPlayer(unitAtPlace);
        }
        else if (unitAtPlace.faction == faction.ally)
        {
            colorAtPlace = doColouringAllies(unitAtPlace);
        }
        else if (unitAtPlace.faction == faction.enemy)
        {
            colorAtPlace = doColouringEnemies(unitAtPlace);
        }
        else if (unitAtPlace.faction == faction.enemyAlly)
        {
            colorAtPlace = doColouringEnemiesAllies(unitAtPlace);
        }
        else if (unitAtPlace.faction == faction.nature)
        {
            colorAtPlace = doColouringNature(unitAtPlace);
        }
        else if (unitAtPlace.faction == faction.monster)
        {
            colorAtPlace = doColouringMonster(unitAtPlace);
        }

        // Then taint it depending on the type
        texture.SetPixel(x, y, colorAtPlace);
    }

    /**
     * If possible change only one of the 2 methods, then replace the other!
     * We should try to see if there is any performance benefit in doing 2 methods
     */
    private void doAttackOrMovePlayer(int x, int y, Unit unit, coordInt target)
    {
        // First bombs. Just check if there is anything in the surrounding.
        if (unit.type == typeOfUnit.bomb)
        {
            int xBombLow = x - unit.fightRange;
            int yBombLow = y - unit.fightRange;

            if (xBombLow < 0)
            {
                xBombLow = 0;
            }
            if (yBombLow < 0)
            {
                yBombLow = 0;
            }

            int xBombHigh = x + unit.fightRange;
            int yBombHigh = y + unit.fightRange;

            if (xBombHigh > sizeArena)
            {
                xBombHigh = sizeArena;
            }
            if (yBombHigh > sizeArena)
            {
                yBombHigh = sizeArena;
            }

            for (int xUnitZone = xBombLow; xUnitZone < xBombHigh; xUnitZone++)
            {
                for (int yUnitZone = yBombLow; yUnitZone < yBombHigh; yUnitZone++)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy && unitEnemy.type != typeOfUnit.bomb && unitEnemy.type != typeOfUnit.wall)
                        || (unitEnemy.faction == faction.player && unitEnemy.type != typeOfUnit.bomb && unitEnemy.type != typeOfUnit.wall)
                        || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        explodeArea(x, y, unit);

                        return;
                    }
                }
            }
            return;
        }

        if (unit.type >= typeOfUnit.hero1)
        {
            doAttackOrMoveHeroes(x, y, unit, target);

            return;
        }

        if (unit.stamina <= 0)
        {
            // Need to rest first
            unit.stamina += 20;

            return;
        }
        // Check the surrounding depending on the attack range
        int xUnitLow = x - unit.fightRange;
        int yUnitLow = y - unit.fightRange;

        if (xUnitLow < 0)
        {
            xUnitLow = 0;
        }
        if (yUnitLow < 0)
        {
            yUnitLow = 0;
        }

        int xUnitHigh = x + unit.fightRange;
        int yUnitHigh = y + unit.fightRange;

        if (xUnitHigh > sizeArena)
        {
            xUnitHigh = sizeArena;
        }
        if (yUnitHigh > sizeArena)
        {
            yUnitHigh = sizeArena;
        }

        if (unit.type != typeOfUnit.monk)
        {
            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        // Attack this unit. Do nothing else.
                        int hit = unit.strength - unitEnemy.defense;
                        if (hit < 0)
                        {
                            hit = 2;
                        }
                        unitEnemy.health -= hit;

                        unit.stamina--;

                        if (unitEnemy.health < 0)
                        {
                            // Remove this unit
                            allUnits[xUnitZone, yUnitZone] = oneEmptyZone;

                            updateTexture(xUnitZone, yUnitZone, oneEmptyZone);
                        }

                        return;
                    }
                }
            }
        }
        else
        {
            // Monk turn one enemy to ally from time to time
            if (iTurn % 30 == 0)
            {
                for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
                {
                    for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                    {
                        Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                        if (unitEnemy.faction == faction.enemy && unitEnemy.type != typeOfUnit.wall)//&& unitEnemy.type != typeOfUnit.machine)
                        {
                            // Attack this unit. Do nothing else.
                            unitEnemy.faction = faction.player; // Or ally ?

                            updateTexture(xUnitZone, yUnitZone, unitEnemy);

                            return;
                        }
                    }
                }
            }
            // And can still move
        }

        // There was no fight, move to the "closest" enemy
        if (y < target.y && y < sizeArenaMinusOne)
        {
            if (allUnits[x, y + 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y + 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y + 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (x > target.x && x > 1)
        {
            if (allUnits[x - 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x - 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x - 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (y > target.y && y > 1)
        {
            if (allUnits[x, y - 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y - 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y - 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (x < target.x && x < sizeArenaMinusOne)
        {
            if (allUnits[x + 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x + 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x + 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        // Finally an unit doing nothing can regain some stamina.

        if (unit.stamina < unit.staminaMax)
        {
            unit.stamina++;
        }
    }

    private void doAttackOrMoveHeroes(int x, int y, Unit unit, coordInt target)
    {
        if (unit.stamina <= 0)
        {
            // Need to rest first
            unit.stamina += 200;

            return;
        }

        if (unit.type == typeOfUnit.hero1 && hero1Activated)
        {
            Debug.Log("Attack of hero 1");
            // Kill all units in fronts
            // Check the surrounding depending on the attack range
            int xUnitLow = x - unit.fightRange;
            int yUnitLow = y;

            if (xUnitLow < 0)
            {
                xUnitLow = 0;
            }
            if (yUnitLow < 0)
            {
                yUnitLow = 0;
            }

            int xUnitHigh = x + unit.fightRange;
            int yUnitHigh = sizeArena;

            if (xUnitHigh > sizeArena)
            {
                xUnitHigh = sizeArena;
            }
            if (yUnitHigh > sizeArena)
            {
                yUnitHigh = sizeArena;
            }

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        // Attack this unit. Do nothing else.
                        int hit = unit.strength - unitEnemy.defense;
                        if (hit < 0)
                        {
                            hit = 2;
                        }
                        unitEnemy.health -= hit;

                        if (unitEnemy.health < 0)
                        {
                            // Remove this unit
                            allUnits[xUnitZone, yUnitZone] = oneEmptyZone;

                            updateTexture(xUnitZone, yUnitZone, oneEmptyZone);
                        }
                    }
                }
            }

            // And the hero is dead
            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            return;
        }
        else if (unit.type == typeOfUnit.hero2 && hero2Activated)
        {
            Debug.Log("Attack of hero 2");
            // Kill all units in fronts
            // Check the surrounding depending on the attack range
            int xUnitLow = x - unit.fightRange;
            int yUnitLow = y - unit.fightRange;

            if (xUnitLow < 0)
            {
                xUnitLow = 0;
            }
            if (yUnitLow < 0)
            {
                yUnitLow = 0;
            }

            int xUnitHigh = x + unit.fightRange;
            int yUnitHigh = y + unit.fightRange;

            if (xUnitHigh > sizeArena)
            {
                xUnitHigh = sizeArena;
            }
            if (yUnitHigh > sizeArena)
            {
                yUnitHigh = sizeArena;
            }

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        // Attack this unit. Do nothing else.
                        int hit = unit.strength - unitEnemy.defense;
                        if (hit < 0)
                        {
                            hit = 2;
                        }
                        unitEnemy.health -= hit;

                        if (unitEnemy.health < 0)
                        {
                            // Remove this unit
                            allUnits[xUnitZone, yUnitZone] = oneEmptyZone;

                            updateTexture(xUnitZone, yUnitZone, oneEmptyZone);
                        }
                    }
                }
            }

            // And the hero is dead
            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            return;
        }
        else if (unit.type == typeOfUnit.hero3 && hero3Activated)
        {
            Debug.Log("Attack of hero 3");
            // The hero is turned into a normal, albeit very strong, soldier
            unit.type = typeOfUnit.spy;

            // Jump up screen
            allUnits[x, sizeArenaMinusOne] = unit;

            updateTexture(x, sizeArenaMinusOne, unit);

            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            return;
        }
        else if (unit.type == typeOfUnit.hero4 && hero4Activated)
        {
            Debug.Log("Attack of hero 4");
            // Convert all units in fronts
            // Check the surrounding depending on the attack range
            int xUnitLow = x - unit.fightRange;
            int yUnitLow = y;

            if (xUnitLow < 0)
            {
                xUnitLow = 0;
            }
            if (yUnitLow < 0)
            {
                yUnitLow = 0;
            }

            int xUnitHigh = x + unit.fightRange;
            int yUnitHigh = sizeArena;

            if (xUnitHigh > sizeArena)
            {
                xUnitHigh = sizeArena;
            }
            if (yUnitHigh > sizeArena)
            {
                yUnitHigh = sizeArena;
            }

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if (unitEnemy.faction == faction.enemy && unitEnemy.type != typeOfUnit.wall)//&& unitEnemy.type != typeOfUnit.machine)
                    {
                        // Attack this unit. Do nothing else.
                        unitEnemy.faction = faction.player; // Or ally ?

                        updateTexture(xUnitZone, yUnitZone, unitEnemy);

                        return;
                    }
                }
            }

            // And the hero is dead
            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            return;
        }
        else if (unit.type == typeOfUnit.hero5 && hero5Activated)
        {
            Debug.Log("Attack of hero 5");
            // Make all player units stronger around
            // Check the surrounding depending on the attack range
            int xUnitLow = x - unit.fightRange;
            int yUnitLow = y - unit.fightRange;

            if (xUnitLow < 0)
            {
                xUnitLow = 0;
            }
            if (yUnitLow < 0)
            {
                yUnitLow = 0;
            }

            int xUnitHigh = x + unit.fightRange;
            int yUnitHigh = y + unit.fightRange;

            if (xUnitHigh > sizeArena)
            {
                xUnitHigh = sizeArena;
            }
            if (yUnitHigh > sizeArena)
            {
                yUnitHigh = sizeArena;
            }

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                {
                    Unit unitPlayer = allUnits[xUnitZone, yUnitZone];
                    if (unitPlayer.faction == faction.player && unitPlayer.type != typeOfUnit.bomb)
                    {
                        // Attack this unit. Do nothing else.
                        unitPlayer.strength += unit.strength;
                        unitPlayer.stamina += unit.stamina;
                        unitPlayer.health += unit.health;
                    }
                }
            }

            // And the hero is dead
            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            return;
        }
        else if (unit.type == typeOfUnit.hero6 && hero6Activated)
        {
            Debug.Log("Attack of hero 6");
            // Turn all enemy units into trees
            // Check the surrounding depending on the attack range
            int xUnitLow = x - unit.fightRange;
            int yUnitLow = y;

            if (xUnitLow < 0)
            {
                xUnitLow = 0;
            }
            if (yUnitLow < 0)
            {
                yUnitLow = 0;
            }

            int xUnitHigh = x + unit.fightRange;
            int yUnitHigh = sizeArena;

            if (xUnitHigh > sizeArena)
            {
                xUnitHigh = sizeArena;
            }

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone += 2)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone += 2)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        // Attack this unit. Do nothing else.

                        unitEnemy.faction = faction.nature;
                        unitEnemy.type = typeOfUnit.tree;

                        updateTexture(xUnitZone, yUnitZone, unitEnemy);
                    }
                }
            }

            // And the hero is dead
            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            return;
        }

        {
            // Check the surrounding depending on the attack range
            int xUnitLow = x - unit.fightRange;
            int yUnitLow = y - unit.fightRange;

            if (xUnitLow < 0)
            {
                xUnitLow = 0;
            }
            if (yUnitLow < 0)
            {
                yUnitLow = 0;
            }

            int xUnitHigh = x + unit.fightRange;
            int yUnitHigh = y + unit.fightRange;

            if (xUnitHigh > sizeArena)
            {
                xUnitHigh = sizeArena;
            }
            if (yUnitHigh > sizeArena)
            {
                yUnitHigh = sizeArena;
            }

            if (unit.type != typeOfUnit.monk)
            {
                for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
                {
                    for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                    {
                        Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                        if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                        {
                            // Attack this unit. Do nothing else.
                            int hit = unit.strength - unitEnemy.defense;
                            if (hit < 0)
                            {
                                hit = 2;
                            }
                            unitEnemy.health -= hit;

                            unit.stamina--;

                            if (unitEnemy.health < 0)
                            {
                                // Remove this unit
                                allUnits[xUnitZone, yUnitZone] = oneEmptyZone;

                                updateTexture(xUnitZone, yUnitZone, oneEmptyZone);
                            }

                            return;
                        }
                    }
                }
            }
            else
            {
                // Monk turn one enemy to ally from time to time
                if (iTurn % 30 == 0)
                {
                    for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
                    {
                        for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                        {
                            Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                            if (unitEnemy.faction == faction.enemy && unitEnemy.type != typeOfUnit.wall)//&& unitEnemy.type != typeOfUnit.machine)
                            {
                                // Attack this unit. Do nothing else.
                                unitEnemy.faction = faction.player; // Or ally ?

                                updateTexture(xUnitZone, yUnitZone, unitEnemy);

                                return;
                            }
                        }
                    }
                }
                // And can still move
            }
        }

        // There was no fight, move to the "closest" enemy
        if (y < target.y && y < sizeArenaMinusOne)
        {
            if (allUnits[x, y + 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y + 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y + 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (x > target.x && x > 1)
        {
            if (allUnits[x - 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x - 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x - 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (y > target.y && y > 1)
        {
            if (allUnits[x, y - 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y - 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y - 1, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        if (x < target.x && x < sizeArenaMinusOne)
        {
            if (allUnits[x + 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x + 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x + 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                return;
            }
        }

        // Finally an unit doing nothing can regain some stamina.

        if (unit.stamina < unit.staminaMax)
        {
            unit.stamina++;
        }
    }

    private void explodeArea(int x, int y, Unit unit)
    {
        // Explode and damage everything around. Do nothing else.
        int xExploLow = x - unit.strength;
        int yExploLow = y - unit.strength;

        if (xExploLow < 0)
        {
            xExploLow = 0;
        }
        if (yExploLow < 0)
        {
            yExploLow = 0;
        }

        int xExploHigh = x + unit.fightRange;
        int yExploHigh = y + unit.fightRange;

        if (xExploHigh > sizeArenaMinusOne)
        {
            xExploHigh = sizeArenaMinusOne;
        }
        if (yExploHigh > sizeArenaMinusOne)
        {
            yExploHigh = sizeArenaMinusOne;
        }

        for (int xUnitExplo = xExploLow; xUnitExplo < xExploHigh; xUnitExplo++)
        {
            for (int yUnitExplo = yExploLow; yUnitExplo < yExploHigh; yUnitExplo++)
            {
                Unit unitVictim = allUnits[xUnitExplo, yUnitExplo]; // Bomb included :)
                if ((unitVictim.faction == faction.enemy) || (unitVictim.faction == faction.player) || (unitVictim.faction == faction.monster) || (unitVictim.faction == faction.nature && unitVictim.type != typeOfUnit.tree))
                {
                    // Explode and damage everything around. Do nothing else.
                    unitVictim.health -= 200;

                    unit.stamina--;

                    if (unitVictim.health < 0)
                    {
                        // Remove this unit
                        allUnits[xUnitExplo, yUnitExplo] = oneEmptyZone;

                        updateTexture(xUnitExplo, yUnitExplo, oneEmptyZone);
                    }
                }
            }
        }
    }
}
