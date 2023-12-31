using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ManageNYPB : MainApp
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.
    
    private bool inGameOver = false; // Game won or lost, to show the end message

    public GameObject lostMessage;
    public GameObject wonMessage;

    private bool won = false;
    private bool lost = false;

    int currentLevel = 1;

    public GameObject title;
    public GameObject backgroundImage;

    // We will roll through the background fx with always only one "emptied".
    private GameObject[] backgroundFx = new GameObject[maxNbOfFxLayers];
    private Texture2D[] textureFX = new Texture2D[maxNbOfFxLayers];
    private Renderer[] imageRendererFX = new Renderer[maxNbOfFxLayers];

    const int maxNbOfFxLayers = 10;

    private AudioSource bombSound;

    int shakeCamera = 0; // Give a visual clue a bomb has been touched.

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    public GameObject brushCursor, brushContainer;

    private bool inFreeMode = false;

    private int coolDown = 0; // Do not immediately do something else after some action

    Vector3 targetPos;

    float currentTime;
    float lastTime;
    float initialTime;

    float lastTimeSpent = 0; // last saved time for the mode

    const int absMaxLevel = 650; // Max level reachable
    int maxLevel = 1; // Max level reached by the player

    private bool hasNature = true;
    private bool hasMonster = true;
    private bool isRestricted = true;


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

    Color colorOurAttacked = new Color(0.9f, 0.9f, 1.0f);

    Color colorAllyBase = new Color(0.4f, 0.4f, 1);
    Color colorAllyRanged = new Color(0.3f, 0.3f, 1);
    Color colorAllyMachine = new Color(0.4f, 0.4f, 0.8f);
    Color colorAllyWall = new Color(0.6f, 0.6f, 1);

    Color colorEnemyBase = new Color(1, 0, 0);
    Color colorEnemyRanged = new Color(1, 0.2f, 0.2f);
    Color colorEnemyMachine = new Color(0.8f, 0.2f, 0.2f);
    Color colorEnemyWall = new Color(1, 0.8f, 0.8f);

    Color colorEnemyAllyBase = new Color(1, 0.4f, 0.4f);

    Color colorEnemyAttacked = new Color(1f, 0.9f, 0.9f);
    // TODO

    Color colorNeutralBase = new Color(0.4f, 0.4f, 0.4f);
    // TODO

    Color colorNatureBase = new Color(0, 1f, 0); // Plants
    Color colorNatureAnimal = new Color(0.3f, 1f, 0.2f);
    Color colorNatureAggrAnimal = new Color(0.1f, 1f, 0.3f);

    Color colorMonsterBase = new Color(0.6f, 0.6f, 0.4f);
    Color colorMonsterRanged = new Color(0.8f, 0.6f, 0.4f);

    Color colorMonsterNatureAttacked = new Color(0.9f, 1, 0.9f);

    private Color colorFullTran = new Color(0, 0, 0, 0);

    Color colorWhiteTrans = new Color(1f,1f, 1.0f,0.5f);

    const int sizeArena = 256;
    const int sizeArenaMinusOne = sizeArena -1;

    private const int maxSizeOfBrush = 8;
    private int sizeOfBrush = maxSizeOfBrush;

    private typeOfUnit currentUnitType = typeOfUnit.close;

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
        public faction faction ; // 0-> player, 1-> enemy, 2->neutral
        public int strength;
        public int stamina;
        public int defense;
        public int health;

        public typeOfUnit type;
        public int fightRange;
        internal int staminaMax;
        internal int iteration;
        internal int wasBlocked;
    }

    private Unit[,] allUnits = new Unit[sizeArena,sizeArena];

    Texture2D texture;

    private int[] someRandom = new int[2*sizeArena];

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
    int nbOfBombs = 0;

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

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

        if (PlayAndKeepMusic.Instance != null)
        {
        PlayAndKeepMusic.Instance.Activated = false;
        PlayAndKeepMusic.Instance.stopMusic();
        }

        // Check how far we are
        bool haskey = LocalSave.HasIntKey("nypb_levelReached");
        maxLevel = 1;
        if (haskey)
        {
            maxLevel = LocalSave.GetInt("nypb_levelReached");
        }
        currentLevel = maxLevel;

        haskey = LocalSave.HasIntKey("nypb_hasNature");

        if (haskey)
        {
            hasNature = LocalSave.GetInt("nypb_hasNature") == 1;
        }

        haskey = LocalSave.HasIntKey("nypb_hasMonster");

        if (haskey)
        {
            hasMonster = LocalSave.GetInt("nypb_hasMonster") == 1;
        }

        haskey = LocalSave.HasIntKey("nypb_hasRestrict");

        if (haskey)
        {
            isRestricted = LocalSave.GetInt("nypb_hasRestrict") == 1;
        }
 
        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();
        nbOfClosed = playersUnit.nbOfCloseUnit;
        nbOfRanged = playersUnit.nbOfRangedUnit;
        nbOfMachines = playersUnit.nbOfMachineUnit;
        nbOfMonks = playersUnit.nbOfMonkUnit;
        nbOfSpies = playersUnit.nbOfSpyUnit;
        nbOfBombs = playersUnit.nbOfBomb;

        availableNbOfClosed = nbOfClosed;
        availableNbOfRanged = nbOfRanged;
        availableNbOfMachines = nbOfMachines;
        availableNbOfMonks = nbOfMonks;
        availableNbOfSpies = nbOfSpies;
        availableNbOfBombs = nbOfBombs;

        isHero1 = playersUnit.isHero1;
        isHero2 = playersUnit.isHero2;
        isHero3 = playersUnit.isHero3;
        isHero4 = playersUnit.isHero4;
        isHero5 = playersUnit.isHero5;
        isHero6 = playersUnit.isHero6;

        checkIfAvailable();

        prepareMap();

        // By default, add player units here
        prepareMapPlayer();

        // Prepare the main texture from there
        createTexture();

        float zBackground = 3.5f;
        float shiftBackground = 0.01f;

        // And fill the FX layers with full alpha
        for (int iFx = 0; iFx < maxNbOfFxLayers; ++iFx)
        {
            backgroundFx[iFx] = Instantiate(backgroundImage);

            backgroundFx[iFx].transform.position = new Vector3(backgroundFx[iFx].transform.position.x+ shiftBackground, backgroundFx[iFx].transform.position.y+ shiftBackground, zBackground);

            zBackground -= 0.5f;
            shiftBackground += 0.01f;

            createTextureFX(iFx);
        }

        createRandom();

        // Get the kind of game from the menu
        initIntroductionBox();

        // Load the last time spent
        LoadTimeSpent();

        lastTime = Time.time; // Last "frame" time
        initialTime = lastTime; // Level start time
        currentTime = lastTime;  
    }

    private void createRandom()
    {
        for (int iRand = 0; iRand < sizeArena*2; iRand++)
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

        Renderer imageRenderer = backgroundImage.GetComponent<Renderer>();
        imageRenderer.material.mainTexture = texture;
    }

    private void createTextureFX(int iFX)
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        textureFX[iFX] = new Texture2D(sizeArena, sizeArena, TextureFormat.RGBA32, false);
        textureFX[iFX].filterMode = FilterMode.Bilinear;

        var fillColorArray = texture.GetPixels();

        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = colorFullTran;
        }

        textureFX[iFX].SetPixels(fillColorArray);

        // Apply all SetPixel calls
        textureFX[iFX].Apply();

        // connect texture to material of GameObject this script is attached to

        imageRendererFX[iFX] = backgroundFx[iFX].GetComponent<Renderer>();
        imageRendererFX[iFX].material.mainTexture = textureFX[iFX];
    }

    /**
     * Roll the texture so they "go away"
     */
    private void rotateTexturesFx()
    {
        for (var iFX = 1; iFX < textureFX.Length; ++iFX)
        {
            imageRendererFX[iFX].material.mainTexture = textureFX[iFX-1];
        }
        imageRendererFX[0].material.mainTexture = textureFX[textureFX.Length - 1];

        for (var iFX = 0; iFX < textureFX.Length; ++iFX)
        {
            textureFX[iFX] = (Texture2D )imageRendererFX[iFX].material.mainTexture;
        }
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
    int machinesComeAt = 60;
    int wallsComeEvery = 10; // Every 10 levels, add a wall.
    int minimumYEnemies = 0; // Where do the enemies stop

    Unit oneEmptyZone = new Unit();

    /**
     * Place nature (animals), monsters and enemies.
     */
    private void prepareMap()
    {
        Debug.Log("Initialise with empty");
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
                    //Debug.Log("Will place a monster");

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
                            if (allUnits[xMonsterZone, yMonsterZone].type == typeOfUnit.typeEmpty)
                            {
                                allUnits[xMonsterZone, yMonsterZone] = oneMonsterZone;
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

        nbOfClosedEnemies = currentLevel * 200;
        nbOfRangedEnemies = (currentLevel - rangedComeAt) * 100;
        nbOfMachinesEnemies = (currentLevel - machinesComeAt) * 100;
        wallsComeEvery -= currentLevel / 10;
        if (wallsComeEvery <= 0)
        {
            wallsComeEvery = 1;
        }
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
                        minimumYEnemies = yWall;
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
        Debug.Log("Place player units for level " + currentLevel);

        //nbOfClosed = currentLevel * 50;
        //nbOfRanged = (currentLevel - rangedComeAt) * 25;
        //nbOfMachines = (currentLevel - machinesComeAt) * 25;
        //nbOfMonks = (currentLevel - machinesComeAt) * 10;
        //nbOfSpies = (currentLevel - machinesComeAt) * 2;
        //nbOfBombs = 0; //  currentLevel;
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
                        playerNetUnit.fightRange = 10;

                        playerNetUnit.defense = 120; // Very strong

                        playerNetUnit.strength = 400;
                        playerNetUnit.stamina = 1000;
                        playerNetUnit.staminaMax = 1000;
                        playerNetUnit.health = 6000;
                        
                        isHero1 = false;
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

                        playerNetUnit.defense = 1000; // Very strong

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
            int x = (int )(Random.value * (sizeArena - 30) + 15);
            int y = (int)(Random.value * 100 + sizeArena/2);

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

        // Roll the textures
        rotateTexturesFx();

        // And "clean" the top one
        createTextureFX(0);

        // Some units to pursue
        coordInt[] sampleEnemies = new coordInt[10];
        coordInt[] samplePlayer = new coordInt[10];

        // In case we haven't got sample enemies, try all of them, not one in 10
        coordInt[] sampleEnemiesBak = new coordInt[10];
        coordInt[] samplePlayerBak = new coordInt[10];
        int iEnemyBak = 0;
        int iPlayerBak = 0;

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
                    if (iEnemyBak < 10)
                    {
                        sampleEnemiesBak[iEnemyBak].x = x;
                        sampleEnemiesBak[iEnemyBak++].y = y;
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
        if (iEnemy < 5)
        {
            sampleEnemies = sampleEnemiesBak;
            iEnemy = iEnemyBak;
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
                    if (iPlayerBak < 10)
                    {
                        samplePlayerBak[iPlayerBak].x = x;
                        samplePlayerBak[iPlayerBak++].y = y;
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
        if (iPlayer < 5)
        {
            samplePlayer = samplePlayerBak;
            iPlayer = iPlayerBak;
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
                if (sampleIndexEnemy < iEnemy-1 && x % 10 == 0)
                {
                    sampleIndexEnemy++;
                }
                if (sampleIndexPlayer < iPlayer-1 && x % 10 == 0)
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

                        updateTextureFX(sampleEnemies[sampleIndexEnemy].x, sampleEnemies[sampleIndexEnemy].y, Color.green);

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
        textureFX[0].Apply();

        checkForGameOver();
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
        int random = someRandom[(currentIteration + x + x)% (sizeArena+sizeArena)];
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

        if (random < 0  && x < sizeArenaMinusOne)
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

    // When the unit is blocked, remember which way he went to be unlocked, and try to go that way.
    int unlockWay = 0;
    int iUnlock = 0;

    private void doAttackOrMoveEnemy(int x, int y, Unit unit, coordInt target)
    {
        if (unit.type == typeOfUnit.wall)
        {
            return;
        }
        if (unit.stamina <= 0)
        {
            // Need to rest first
            unit.stamina+=20;

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

                    updateTextureFX(xUnitZone, yUnitZone, colorOurAttacked);

                    return;
                }
            }
        }

        // There was no fight, move to the "closest" enemy
        if ((y > target.y || (unit.wasBlocked > 40)) && y > 1)
        {
            if (allUnits[x, y - 1].type == typeOfUnit.typeEmpty && (unlockWay == 0 || iUnlock-- > 0))
            {
                allUnits[x, y - 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y - 1, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;
                if (unit.wasBlocked > 40)
                {
                    unlockWay = 0;
                }

                return;
            }
            if (iUnlock == 0)
            {
                unlockWay = 1;
            }
            unit.wasBlocked++;
            iUnlock = 10;
        }

        if ((x > target.x || (unit.wasBlocked > 40)) && x > 1)
        {
            if (allUnits[x - 1, y].type == typeOfUnit.typeEmpty && (unlockWay == 1 || iUnlock-- > 0))
            {
                allUnits[x - 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x - 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;
                if (unit.wasBlocked > 40)
                {
                    unlockWay = 1;
                }
                return;
            }
            if (iUnlock == 0)
            {
                unlockWay = 2;
            }
            unit.wasBlocked++;
            iUnlock = 10;
        }

        if ((y < target.y || (unit.wasBlocked > 40)) && y < sizeArenaMinusOne)
        {
            if (allUnits[x, y + 1].type == typeOfUnit.typeEmpty && (unlockWay == 2 || iUnlock-- > 0))
            {
                allUnits[x, y + 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y + 1, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;
                if (unit.wasBlocked > 40)
                {
                    unlockWay = 2;
                }

                return;
            }
            if (iUnlock == 0)
            {
                unlockWay = 3;
            }
            unit.wasBlocked++;
            iUnlock = 10;
        }

        if ((x < target.x || (unit.wasBlocked > 40)) && x < sizeArenaMinusOne)
        {
            if (allUnits[x + 1, y].type == typeOfUnit.typeEmpty && (unlockWay == 3 || iUnlock-- > 0))
            {
                allUnits[x + 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x + 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;
                if (unit.wasBlocked > 40)
                {
                    unlockWay = 3;
                }

                return;
            }
            if (iUnlock == 0)
            { 
                unlockWay = 0;
            }
            unit.wasBlocked++;
            iUnlock = 10;
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

    private void updateTextureFX(int x, int y, Color newColor)
    {
        // Then taint it depending on the type
        textureFX[0].SetPixel(x, y, newColor);
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

                            updateTextureFX(xUnitZone, yUnitZone, colorEnemyAttacked);
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
                        if (unitEnemy.faction == faction.enemy && unitEnemy.type != typeOfUnit.wall )//&& unitEnemy.type != typeOfUnit.machine)
                        {
                            // Attack this unit. Do nothing else.
                            unitEnemy.faction = faction.player; // Or ally ?

                            updateTexture(xUnitZone, yUnitZone, unitEnemy);

                            updateTextureFX(xUnitZone, yUnitZone, Color.cyan);

                            return;
                        }
                    }
                }
            }
            // And can still move
        }

        // There was no fight, move to the "closest" enemy
        if ((y < target.y || (unit.wasBlocked > 40)) && y < sizeArenaMinusOne)
        {
            if (allUnits[x, y + 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y + 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y + 1, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;

                return;
            }
            unit.wasBlocked++;
        }

        if ((x > target.x || (unit.wasBlocked > 40)) && x > 1)
        {
            if (allUnits[x - 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x - 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x - 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;

                return;
            }
            unit.wasBlocked++;
        }

        if ((y > target.y || (unit.wasBlocked > 40)) && y > 1)
        {
            if (allUnits[x, y - 1].type == typeOfUnit.typeEmpty)
            {
                allUnits[x, y - 1] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x, y - 1, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;

                return;
            }
            unit.wasBlocked++;
        }

        if ((x < target.x || (unit.wasBlocked > 40)) && x < sizeArenaMinusOne)
        {
            if (allUnits[x + 1, y].type == typeOfUnit.typeEmpty)
            {
                allUnits[x + 1, y] = unit;
                allUnits[x, y] = oneEmptyZone;

                updateTexture(x + 1, y, unit);
                updateTexture(x, y, oneEmptyZone);

                unit.wasBlocked = 0;

                return;
            }
            unit.wasBlocked++;
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

                    updateTextureFX(xUnitZone, yUnitZone, Color.white);
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

                        updateTextureFX(xUnitZone, yUnitZone, Color.white);
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
            allUnits[x, sizeArenaMinusOne ] = unit;

            updateTexture(x, sizeArenaMinusOne, unit);

            allUnits[x, y] = oneEmptyZone;

            updateTexture(x, y, oneEmptyZone);

            updateTextureFX(x, y, Color.yellow);

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

                    updateTextureFX(xUnitZone, yUnitZone, colorWhiteTrans);
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

                    updateTextureFX(xUnitZone, yUnitZone, Color.blue);
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

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone+=4)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone+=2)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        // Attack this unit. Do nothing else.
                       
                        unitEnemy.faction = faction.nature;
                        unitEnemy.type = typeOfUnit.tree;

                        updateTexture(xUnitZone, yUnitZone, unitEnemy);
                    }
                    updateTextureFX(xUnitZone, yUnitZone, Color.yellow);
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
                            updateTextureFX(xUnitZone, yUnitZone, colorEnemyAttacked);

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

                                updateTextureFX(xUnitZone, yUnitZone, Color.cyan);

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

                    updateTextureFX(xUnitExplo, yUnitExplo, Color.white);
                }
            }
        }
    }

    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    int iTurn = 0;
    bool inEditMode = true;

    // Update is called once per frame
    void Update()
    {
        if (startingGame)
        {
            // Manage the introduction box
            showIntroductionBox(true);

            //return;
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

        currentTime = Time.time;

        if (shakeCamera > 0)
        {
            shakeCamera--;
            float xRand = 0.2f * Random.value - 0.1f;
            float yRand = 0.2f * Random.value - 0.1f;
            transform.position = new Vector3(xRand, yRand, -7);
        }
        else if (shakeCamera == 0)
        {
            transform.position = new Vector3(0, 0, -7);
            shakeCamera = -1;
        }

        if (coolDown != 0)
        {
            coolDown--;
        }

        if (!inEditMode)
        {
            if (iTurn % 2 == 0)
            {
                if (Keyboard.current[Key.Digit1].isPressed || Keyboard.current[Key.Numpad1].isPressed)
                {
                    activateHero(1);
                }

                if (Keyboard.current[Key.Digit2].isPressed || Keyboard.current[Key.Numpad2].isPressed)
                {
                    activateHero(2);
                }

                if (Keyboard.current[Key.Digit3].isPressed || Keyboard.current[Key.Numpad3].isPressed)
                {
                    activateHero(3);
                }

                if (Keyboard.current[Key.Digit4].isPressed || Keyboard.current[Key.Numpad4].isPressed)
                {
                    activateHero(4);
                }

                if (Keyboard.current[Key.Digit5].isPressed || Keyboard.current[Key.Numpad5].isPressed)
                {
                    activateHero(5);
                }

                if (Keyboard.current[Key.Digit6].isPressed || Keyboard.current[Key.Numpad6].isPressed)
                {
                    activateHero(6);
                }

                if (Keyboard.current[Key.Space].isPressed)
                {
                    setEditMode(true);
                    return;
                }

                doATurn();
            }
        }
        else
        {
            if (coolDown > 0)
            {
                coolDown--;
            }
            if (Keyboard.current[Key.Enter].wasReleasedThisFrame)
            {
                setEditMode(false);
                return;
            }
            if (coolDown ==0 &&  (Keyboard.current[Key.NumpadPlus].isPressed))
            {
                if (currentLevel < maxLevel)
                { 
                    increaseLevel();
                }
                coolDown = 10;
            }
            if (coolDown == 0 && (Keyboard.current[Key.Minus].isPressed || Keyboard.current[Key.NumpadMinus].isPressed))
            {
                decreaseLevel();

                coolDown = 10;
            }

            //Debug.Log("Did the turn " + iTurn++);
            doSelectLevel();

            {
                Vector3 touchPos = Mouse.current.position.ReadValue();

                // use the input stuff
                if (Mouse.current.leftButton.isPressed)
                {
                    // Depending on the state, we might be painting units.
                    // moveCamera(touchPos);
                    UpdateBrushCursor(touchPos);
                }

                // Check if the user has clicked
                bool aTouch = Mouse.current.leftButton.isPressed;
                bool rightClick = Mouse.current.rightButton.isPressed;

                if (aTouch)
                {
                    // Debug.Log( "Moused moved to point " + touchPos );
                    UpdateBrushCursor(touchPos);
                }

                if (rightClick)
                {
                    ;
                }
                else
                {
                    ;
                }

                if (Mouse.current.scroll.ReadValue().y > 0f) // forward
                {
                    ;
                }
                else if (Mouse.current.scroll.ReadValue().y < 0f) // backwards
                {
                    ;
                }
            }
        }
        // Move the camera if needed
        //updateCameraPos();

        // Keep absolute scale of GUI elements
        //score.transform.localScale = score.transform.localScale;
        lastTime = currentTime; // Last "frame" time
    }

    private bool startingGame = true;

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;
    public int dimingTime = 200; // 200 frames
    float zoomFactor;
    float currentZoomForIntroBox;

    private void initIntroductionBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        // Z = -9

        //backgroundImage.transform.localScale = new Vector3(globalZoom, globalZoom, 1);
        Camera.main.orthographicSize = 8;
    }

    // Using same box, show the game over text
    private void initIOutroBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        zoomFactor = (0.2f / ((float)totalZoomingTime));
    }

    public static float GetScreenToWorldWidth()
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        float width = edgeVector.x * 2;
        return width;
    }

    bool initialTitleSetup = true;
    private bool inTitle = false;

    private void showIntroductionBox(bool isIntro)
    {
        if (currentStep == 0)
        {
            // Zoom the box into view
            if (zoomingTime <= totalZoomingTime)
            {
                if (initialTitleSetup)
                {
                    float width = GetScreenToWorldWidth();

                    if (width > 7.6f)
                    {
                        width = 7.6f;
                    }
                    title.transform.localScale = Vector2.one * width * 0.5f;

                    // And the scale on z must stay 1, or we can't layer.
                    title.transform.localScale = new Vector3(title.transform.localScale.x, title.transform.localScale.y, 1);

                    // And the title (and its selections) is active
                    inTitle = true;
                }
                float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, fading);
                    }
                }

                // backgroundImage.transform.localScale = new Vector3(0.02f + currentZoomForIntroBox, 0.02f + currentZoomForIntroBox, 1);

                //introductionBox.transform.localScale = new Vector3(0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox);
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

            // use the input stuff
            bool aTouch = Mouse.current.leftButton.wasReleasedThisFrame || Keyboard.current.anyKey.wasReleasedThisFrame;
            if (aTouch)
            {
                currentStep = 2;
            }

        }
        else if (currentStep == 2)
        {
            inTitle = false;

            if (zoomingTime <= dimingTime)
            {
                // Fading out of the box
                float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                    }
                }

                zoomingTime++;
            }
            else
            {
                currentStep = 3;
            }
        }
        else if (currentStep == 3)
        {
            // All done
            if (isIntro)
            {
                startingGame = false;

                // Move the menu away
                // We can't remove it (deactivate it) as we need to know if its sound (drum) is still playing
                // We also need to push it aside, as otherwise we could still select the level (even if they are behind us).
                title.transform.localPosition = new Vector3(600, title.transform.localPosition.y, 20);
            }
            else
            {
                inGameOver = false;
                startingGame = true;
                initialTitleSetup = true;
            }
        }
    }

    void UpdateBrushCursor(Vector3 touchPos)
    {
        Vector2 arenaPos = Vector2.zero;
        if (touchToArenaCoords(ref arenaPos, touchPos))
        {
            Debug.Log("Touched at " + arenaPos.x + ", " + arenaPos.y);
            brushAtPos(arenaPos);
        }
    }

    private int IntSqrt(int value)
    {
        if (value > 0)
            return Convert.ToInt32(Math.Sqrt(value));
        else
            return 0;
    }

    private void brushAtPos(Vector2 arenaPos)
    {
        int x = (int)arenaPos.x;
        int y = (int)arenaPos.y;

        if (IsRestricted)
        {
            Debug.Log("RESTRICTED!!!! at "+ minimumYEnemies+" - "+y+" : "+ currentUnitType);
            // If below the lowest enemy, don't allow
            if (y > minimumYEnemies && currentUnitType != typeOfUnit.spy && currentUnitType != typeOfUnit.typeEmpty)
            {
                return;
            }
        }
        // Check for which size

        // See the actual size we can use
        int actualSizeOfBrush = sizeOfBrush;

        if (!inFreeMode)
        {
            int nbAvailable = 0;

            switch (currentUnitType)
            {
                case typeOfUnit.close:
                    nbAvailable = availableNbOfClosed;
                    break;

                case typeOfUnit.ranged:
                    nbAvailable = availableNbOfRanged;

                    break;

                case typeOfUnit.machine:
                    nbAvailable = availableNbOfMachines;

                    break;

                case typeOfUnit.monk:
                    nbAvailable = availableNbOfMonks;

                    break;

                case typeOfUnit.spy:
                    nbAvailable = availableNbOfSpies;

                    break;

                case typeOfUnit.hero1:
                    nbAvailable = 1;

                    break;

                case typeOfUnit.hero2:
                    nbAvailable = 1;

                    break;

                case typeOfUnit.hero3:
                    nbAvailable = 1;
                    break;

                case typeOfUnit.hero4:
                    nbAvailable = 1;
                    break;

                case typeOfUnit.hero5:
                    nbAvailable = 1;
                    break;

                case typeOfUnit.hero6:
                    nbAvailable = 1;
                    break;

                case typeOfUnit.bomb:
                    nbAvailable = availableNbOfBombs;

                    break;
                default:
                    nbAvailable = 1000000000;
                    break;
            }

            // Now calculate the actual sizeOfBrush.
            if (nbAvailable < areaBrush)
            {
                actualSizeOfBrush = IntSqrt(nbAvailable);

                if (actualSizeOfBrush == 0)
                {
                    actualSizeOfBrush = 1;
                }
            }
        }

        int xExploLow = x - actualSizeOfBrush;
        int yExploLow = y - actualSizeOfBrush;

        if (xExploLow < 0)
        {
            xExploLow = 0;
        }
        if (yExploLow < 0)
        {
            yExploLow = 0;
        }

        int xExploHigh = x + actualSizeOfBrush;
        int yExploHigh = y + actualSizeOfBrush;

        if (xExploHigh > sizeArena)
        {
            xExploHigh = sizeArena;
        }
        if (yExploHigh > sizeArena)
        {
            yExploHigh = sizeArena;
        }

        for (int xUnitExplo = xExploLow; xUnitExplo < xExploHigh; xUnitExplo++)
        {
            for (int yUnitExplo = yExploLow; yUnitExplo < yExploHigh; yUnitExplo++)
            {
                // Also need to check if there are any units left
                // Or inFreeMode
                if (currentUnitType == typeOfUnit.typeEmpty)
                {
                    if (allUnits[xUnitExplo, yUnitExplo].faction == faction.player)
                    {
                        // Update the values

                        // First the "pool" of units, only in restricted mode
                        if (!inFreeMode)
                        {
                            switch (allUnits[xUnitExplo, yUnitExplo].type)
                            {
                                case typeOfUnit.close:
                                    availableNbOfClosed++;
                                    break;

                                case typeOfUnit.ranged:
                                    availableNbOfRanged++;
                                    break;

                                case typeOfUnit.machine:
                                    availableNbOfMachines++;
                                    break;

                                case typeOfUnit.monk:
                                    availableNbOfMonks++;
                                    break;

                                case typeOfUnit.spy:
                                    availableNbOfSpies++;
                                    break;

                                case typeOfUnit.hero1:
                                    isHero1 = true;
                                    break;

                                case typeOfUnit.hero2:
                                    isHero2 = true;
                                    break;

                                case typeOfUnit.hero3:
                                    isHero3 = true;
                                    break;

                                case typeOfUnit.hero4:
                                    isHero4 = true;
                                    break;

                                case typeOfUnit.hero5:
                                    isHero5 = true;
                                    break;

                                case typeOfUnit.hero6:
                                    isHero6 = true;
                                    break;

                                case typeOfUnit.bomb:
                                    availableNbOfBombs++;
                                    break;
                            }
                        }

                        // Then what is on screen, in all cases
                        switch (currentUnitType)
                        {
                            case typeOfUnit.close:
                                currentNbOfClosed--;
                                break;

                            case typeOfUnit.ranged:
                                currentNbOfRanged--;
                                break;

                            case typeOfUnit.machine:
                                currentNbOfMachines--;
                                break;

                            case typeOfUnit.monk:
                                currentNbOfMonks--;
                                break;

                            case typeOfUnit.spy:
                                currentNbOfSpies--;
                                break;

                            case typeOfUnit.bomb:
                                currentNbOfBombs--;
                                break;
                        }
                        allUnits[xUnitExplo, yUnitExplo] = oneEmptyZone;

                        updateTexture(xUnitExplo, yUnitExplo, oneEmptyZone);
                    }
                }
                else if (allUnits[xUnitExplo, yUnitExplo] == oneEmptyZone)
                {
                    Unit playerNetUnit = new Unit();

                    playerNetUnit.iteration = currentIteration;
                    playerNetUnit.faction = faction.player;


                    switch (currentUnitType)
                    {
                        case typeOfUnit.close:
                            if (!inFreeMode)
                            {
                                if (availableNbOfClosed == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfClosed--;
                            }
                            currentNbOfClosed++;

                            playerNetUnit.type = typeOfUnit.close;
                            playerNetUnit.fightRange = 1;

                            playerNetUnit.defense = 4; // Not strong

                            playerNetUnit.strength = 10;
                            playerNetUnit.stamina = 10;
                            playerNetUnit.staminaMax = 10;
                            playerNetUnit.health = 600;

                            break;

                        case typeOfUnit.ranged:
                            if (!inFreeMode)
                            {
                                if (availableNbOfRanged == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfRanged--;
                            }
                            currentNbOfRanged++;

                            playerNetUnit.type = typeOfUnit.ranged;
                            playerNetUnit.fightRange = 3;

                            playerNetUnit.defense = 2; // Not strong

                            playerNetUnit.strength = 10;
                            playerNetUnit.stamina = 20;
                            playerNetUnit.staminaMax = 20;
                            playerNetUnit.health = 40;
                            break;

                        case typeOfUnit.machine:

                            if (!inFreeMode)
                            {
                                if (availableNbOfMachines == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfMachines--;
                            }
                            currentNbOfMachines++;

                            playerNetUnit.type = typeOfUnit.machine;
                            playerNetUnit.fightRange = 5;

                            playerNetUnit.defense = 3;

                            playerNetUnit.strength = 50;
                            playerNetUnit.stamina = 8;
                            playerNetUnit.staminaMax = 8;
                            playerNetUnit.health = 50;
                            break;

                        case typeOfUnit.monk:
                            if (!inFreeMode)
                            {
                                if (availableNbOfMonks == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfMonks--;
                            }
                            currentNbOfMonks++;

                            playerNetUnit.type = typeOfUnit.monk;
                            playerNetUnit.fightRange = 3;

                            playerNetUnit.defense = 4;

                            playerNetUnit.strength = 0;
                            playerNetUnit.stamina = 1000;
                            playerNetUnit.staminaMax = 1000;
                            playerNetUnit.health = 600;
                            break;

                        case typeOfUnit.spy:
                            if (!inFreeMode)
                            {
                                if (availableNbOfSpies == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfSpies--;
                            }
                            currentNbOfSpies++;

                            playerNetUnit.type = typeOfUnit.spy;
                            playerNetUnit.fightRange = 1;

                            playerNetUnit.defense = 6;

                            playerNetUnit.strength = 40;
                            playerNetUnit.stamina = 800;
                            playerNetUnit.staminaMax = 800;
                            playerNetUnit.health = 500;
                            break;

                        case typeOfUnit.hero1:
                            if (!inFreeMode)
                            {
                                if (!isHero1)
                                {
                                    texture.Apply();
                                    return;
                                }
                                isHero1=false;
                            }

                            playerNetUnit.type = typeOfUnit.hero1;
                            playerNetUnit.fightRange = 10;

                            playerNetUnit.defense = 120; // Very strong

                            playerNetUnit.strength = 400;
                            playerNetUnit.stamina = 1000;
                            playerNetUnit.staminaMax = 1000;
                            playerNetUnit.health = 6000;

                            break;

                        case typeOfUnit.hero2:
                            if (!inFreeMode)
                            {
                                if (!isHero2)
                                {
                                    texture.Apply();
                                    return;
                                }
                                isHero2 = false;
                            }
                            playerNetUnit.type = typeOfUnit.hero2;
                            playerNetUnit.fightRange = 20;

                            playerNetUnit.defense = 100; // Very strong

                            playerNetUnit.strength = 400;
                            playerNetUnit.stamina = 1000;
                            playerNetUnit.staminaMax = 1000;
                            playerNetUnit.health = 6000;

                            break;

                        case typeOfUnit.hero3:
                            if (!inFreeMode)
                            {
                                if (!isHero3)
                                {
                                    texture.Apply();
                                    return;
                                }
                                isHero3 = false;
                            }
                            playerNetUnit.type = typeOfUnit.hero3;
                            playerNetUnit.fightRange = 8;

                            playerNetUnit.defense = 100; // Very strong

                            playerNetUnit.strength = 200;
                            playerNetUnit.stamina = 10000;
                            playerNetUnit.staminaMax = 10000;
                            playerNetUnit.health = 6000;
                            break;

                        case typeOfUnit.hero4:
                            if (!inFreeMode)
                            {
                                if (!isHero4)
                                {
                                    texture.Apply();
                                    return;
                                }
                                isHero4 = false;
                            }
                            playerNetUnit.type = typeOfUnit.hero4;
                            playerNetUnit.fightRange = 8;

                            playerNetUnit.defense = 500; // Very strong

                            playerNetUnit.strength = 20;
                            playerNetUnit.stamina = 1000;
                            playerNetUnit.staminaMax = 1000;
                            playerNetUnit.health = 6000;
                            break;

                        case typeOfUnit.hero5:
                            if (!inFreeMode)
                            {
                                if (!isHero5)
                                {
                                    texture.Apply();
                                    return;
                                }
                                isHero5 = false;
                            }
                            playerNetUnit.type = typeOfUnit.hero5;
                            playerNetUnit.fightRange = 20;

                            playerNetUnit.defense = 100; // Very strong

                            playerNetUnit.strength = 80;
                            playerNetUnit.stamina = 1000;
                            playerNetUnit.staminaMax = 1000;
                            playerNetUnit.health = 4000;
                            break;

                        case typeOfUnit.hero6:
                            if (!inFreeMode)
                            {
                                if (!isHero6)
                                {
                                    texture.Apply();
                                    return;
                                }
                                isHero6 = false;
                            }
                            playerNetUnit.type = typeOfUnit.hero6;
                            playerNetUnit.fightRange = 20;

                            playerNetUnit.defense = 100; // Very strong

                            playerNetUnit.strength = 400;
                            playerNetUnit.stamina = 1000;
                            playerNetUnit.staminaMax = 1000;
                            playerNetUnit.health = 600;
                            break;

                        case typeOfUnit.bomb:
                            if (!inFreeMode)
                            {
                                if (availableNbOfBombs == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfBombs--;
                            }
                            currentNbOfBombs++;

                            playerNetUnit.type = typeOfUnit.bomb;
                            playerNetUnit.fightRange = 1;

                            playerNetUnit.defense = 100;

                            playerNetUnit.strength = 4;
                            playerNetUnit.stamina = 8;
                            playerNetUnit.staminaMax = 8;
                            playerNetUnit.health = 50;
                            break;
                        default:
                            if (!inFreeMode)
                            {
                                if (availableNbOfClosed == 0)
                                {
                                    texture.Apply();
                                    return;
                                }
                                availableNbOfClosed--;
                            }
                            playerNetUnit.type = typeOfUnit.close;
                            playerNetUnit.fightRange = 1;

                            playerNetUnit.defense = 4; // Not strong

                            playerNetUnit.strength = 10;
                            playerNetUnit.stamina = 10;
                            playerNetUnit.staminaMax = 10;
                            playerNetUnit.health = 600;
                            break;
                    }
                    allUnits[xUnitExplo, yUnitExplo] = playerNetUnit;
                    updateTexture(xUnitExplo, yUnitExplo, playerNetUnit);
                }
            }
        }

        texture.Apply();
    }

    bool touchToArenaCoords(ref Vector2 arenaPos, Vector3 touchPos)
    {

        var pos = Camera.main.ScreenToWorldPoint(touchPos);

        //Debug.Log("Check touch at " + pos);

        // The game arena is always square!
        // Check what is larger-> height or width
        float width = getScreenWidth();
        float height = getScreenHeight();

        float ratio = width / height;

        //Debug.Log("Width " + width + " - Height "+ height+ " - Ratio "+ratio);

        // (width/10 < height/14)
        if (width/10 > height/14)
        {
            // So on the height the pos translate almost directly
            arenaPos.y = (1.4f * pos.y/height + 0.5f) * sizeArena;

            // And on the width depending on the aspect ratio
            arenaPos.x = sizeArena - ((1.4f * pos.x/width) * ratio + 0.5f) * sizeArena ;

            //Debug.Log("A- Found " + arenaPos);

            if (arenaPos.x > 0 && arenaPos.x < sizeArena)
            {
                return true;
            }
        }
        else
        {

            // The opposite

            arenaPos.x = sizeArena - (pos.x/width + 0.5f) * sizeArena;

            // And on the width depending on the aspect ratio
            arenaPos.y = ((pos.y / height) / ratio + 0.5f) * sizeArena;

            //Debug.Log("B- Found " + arenaPos);

            if (arenaPos.y > 0 && arenaPos.y < sizeArena)
            {
                return true;
            }
        }

        return false;
    }

    float getScreenWidth()
    {
        return (Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height);
    }

    float getScreenHeight()
    {
        return (Camera.main.orthographicSize * 2.0f);
    }

    private int currentIteration = 0;

    public bool InTitle { get => inTitle; }
    public int NbOfClosedEnemies { get => nbOfClosedEnemies; }
    public int NbOfRangedEnemies { get => nbOfRangedEnemies;  }
    public int NbOfMachinesEnemies { get => nbOfMachinesEnemies;  }
    public int NbOfClosed { get => nbOfClosed; set => nbOfClosed = value; }
    public int NbOfRanged { get => nbOfRanged; set => nbOfRanged = value; }
    public int NbOfMachines { get => nbOfMachines; set => nbOfMachines = value; }
    public int NbOfMonks { get => nbOfMonks; set => nbOfMonks = value; }
    public int NbOfSpies { get => nbOfSpies; set => nbOfSpies = value; }
    public int NbOfBombs { get => nbOfBombs; set => nbOfBombs = value; }
    public int MaxLevel { get => maxLevel; }
    public int CurrentNbOfClosed { get => currentNbOfClosed; set => currentNbOfClosed = value; }
    public int CurrentNbOfRanged { get => currentNbOfRanged; set => currentNbOfRanged = value; }
    public int CurrentNbOfMachines { get => currentNbOfMachines; set => currentNbOfMachines = value; }
    public int CurrentNbOfMonks { get => currentNbOfMonks; set => currentNbOfMonks = value; }
    public int CurrentNbOfSpies { get => currentNbOfSpies; set => currentNbOfSpies = value; }
    public int CurrentNbOfBombs { get => currentNbOfBombs; set => currentNbOfBombs = value; }
    public int CurrentNbOfClosedEnemies { get => currentNbOfClosedEnemies; set => currentNbOfClosedEnemies = value; }
    public int CurrentNbOfRangedEnemies { get => currentNbOfRangedEnemies; set => currentNbOfRangedEnemies = value; }
    public int CurrentNbOfMachinesEnemies { get => currentNbOfMachinesEnemies; set => currentNbOfMachinesEnemies = value; }
    public bool HasNature { get => hasNature; set => hasNature = value; }
    public bool HasMonster { get => hasMonster; set => hasMonster = value; }
    public bool IsRestricted { get => isRestricted; set => isRestricted = value; }

    private void LoadTimeSpent()
    {
        bool haskey = LocalSave.HasFloatKey("nypb_allTimeSpent");

        if (haskey)
        {
            lastTimeSpent = LocalSave.GetFloat("nypb_allTimeSpent");
        }
    }

    private void SaveTimeSpent()
    {
        LocalSave.SetFloat("nypb_allTimeSpent", currentTime - initialTime + lastTimeSpent);
        LocalSave.Save();
    }

    void checkForGameOver()
    {
        if (!lost && !won && currentNbOfClosedEnemies + currentNbOfRangedEnemies + currentNbOfMachinesEnemies == 0)
        {
            // Game is won
            won = true;

            wonMessage.SetActive(true);

            inEditMode = true;

            LocalSave.SaveCopy();

            SaveTimeSpent();

            loadNext();

            checkIfAvailable();
        }
        if (!won && !lost && !inFreeMode)
        {
            //Debug.Log("Nbs are "+availableNbOfClosed + " "+ availableNbOfRanged + " " + availableNbOfMachines + " " + availableNbOfMonks + " " + availableNbOfSpies);

            if (availableNbOfClosed + availableNbOfRanged + availableNbOfMachines + availableNbOfMonks + availableNbOfSpies +
                currentNbOfClosed + currentNbOfRanged + currentNbOfMachines + currentNbOfMonks + currentNbOfSpies == 0)
            {
                // Game is lost
                lostMessage.SetActive(true);

                inEditMode = true;

                LocalSave.SaveCopy();

                SaveTimeSpent();

                lost = true;
                //prepareNewLevel();
            }
        }
    }

    void gameover()
    {
        loadNext();
    }

    void loadNext()
    {
        // Outro or next level?
        if (currentLevel >= 140)
        {
            InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.MMPG_END);

            UnityEngine.SceneManagement.SceneManager.LoadScene("OutroNYPB");
        }
        else
        {
            setNextLevelReached();
        }
    }



    public void setNextLevelReached()
    {
        bool haskey = LocalSave.HasIntKey("nypb_levelReached");
        int levelReached = 0;
        if (haskey)
        {
            levelReached = LocalSave.GetInt("nypb_levelReached");
        }

        if (currentLevel >= levelReached)
        {
            LocalSave.SetInt("nypb_levelReached", currentLevel + 1);
            LocalSave.Save();

            maxLevel = currentLevel + 1;
        }
    }

    public void resetLevelReached()
    {
        LocalSave.SetInt("nypb_levelReached", 1);
        LocalSave.Save();
    }

    public void resetTimeSpent()
    {
        LocalSave.DeleteKey("nypb_bestTime");     
        LocalSave.Save();
    }

    public void smallFound()
    {
        bool haskey = LocalSave.HasIntKey("nypb_smallFound");
        int allSmallFoundFlag = 0;
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("nypb_smallFound");
        }

        int currentOne = 2 << currentLevel;

        allSmallFoundFlag = allSmallFoundFlag | currentOne;

        LocalSave.SetInt("nypb_smallFound", allSmallFoundFlag);
        LocalSave.Save();
    }

    public bool wasCurrentSmallFound()
    {
        int currentOne = 2 << currentLevel;

        bool haskey = LocalSave.HasIntKey("nypb_smallFound");
        int allSmallFoundFlag = 0;
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("nypb_smallFound");

            if ((allSmallFoundFlag & currentOne) > 0)
            {
                return true;
            }
        }
        //Debug.Log("Am here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"+ currentOne+" - "+ levelReached);
        return false;
    }

    public void resetSmallFound()
    {
        LocalSave.SetInt("nypb_smallFound", 0);
        LocalSave.Save();
    }

    public void resetProgression()
    {
        resetLevelReached();
        resetSmallFound();
        resetTimeSpent();
        deleteLevelSaves();

        loadFirstLevel();
    }

    public void resetUnits()
    {
        MMPGAddNewPlayerUnits.resetAllUnits();
        reloadUnits();

        restartLevel();
    }

    public void resetAll()
    {
        resetLevelReached();
        resetSmallFound();
        resetTimeSpent();
        deleteLevelSaves();

        MMPGAddNewPlayerUnits.resetAllUnits();
        reloadUnits();

        loadFirstLevel();
    }

    private void reloadUnits()
    {
        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();
        nbOfClosed = playersUnit.nbOfCloseUnit;
        nbOfRanged = playersUnit.nbOfRangedUnit;
        nbOfMachines = playersUnit.nbOfMachineUnit;
        nbOfMonks = playersUnit.nbOfMonkUnit;
        nbOfSpies = playersUnit.nbOfSpyUnit;
        nbOfBombs = playersUnit.nbOfBomb;

        availableNbOfClosed = nbOfClosed;
        availableNbOfRanged = nbOfRanged;
        availableNbOfMachines = nbOfMachines;
        availableNbOfMonks = nbOfMonks;
        availableNbOfSpies = nbOfSpies;
        availableNbOfBombs = nbOfBombs;

        isHero1 = playersUnit.isHero1;
        isHero2 = playersUnit.isHero2;
        isHero3 = playersUnit.isHero3;
        isHero4 = playersUnit.isHero4;
        isHero5 = playersUnit.isHero5;
        isHero6 = playersUnit.isHero6;
    }

    private void loadFirstLevel()
    {
        currentLevel = 1;

        prepareNewLevel();
    }

    private void prepareNewLevel()
    {
        won = false;
        lost = false;

        availableNbOfClosed = nbOfClosed;
        availableNbOfRanged = nbOfRanged;
        availableNbOfMachines = nbOfMachines;
        availableNbOfMonks = nbOfMonks;
        availableNbOfSpies = nbOfSpies;
        availableNbOfBombs = nbOfBombs;

        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();

        isHero1 = playersUnit.isHero1;
        isHero2 = playersUnit.isHero2;
        isHero3 = playersUnit.isHero3;
        isHero4 = playersUnit.isHero4;
        isHero5 = playersUnit.isHero5;
        isHero6 = playersUnit.isHero6;

        checkIfAvailable();

        inFreeMode = false;

        hero1Activated = false;
        hero2Activated = false;
        hero3Activated = false;
        hero4Activated = false;
        hero5Activated = false;
        hero6Activated = false;

        currentIteration = 0;

        currentNbOfClosedEnemies = 0;
        currentNbOfRangedEnemies = 0;
        currentNbOfMachinesEnemies = 0;

        currentNbOfClosed = 0;
        currentNbOfRanged = 0;
        currentNbOfMachines = 0;
        currentNbOfMonks = 0;
        currentNbOfSpies = 0;
        currentNbOfBombs = 0;

        prepareMap();

        // For testing, add player units here
        prepareMapPlayer();

        createTexture();

        createRandom();
    }

    // After the drum roll, start the music
    internal void playMusic()
    {
        if (!((AudioSource)GetComponent<AudioSource>()).isPlaying)
        {
            if (GetComponent<AudioSource>() != null)
            {
                ((AudioSource)GetComponent<AudioSource>()).Play();
            }
        }
    }

    /**
     * Return to title screen to select another level
     */
    public void goToTitle()
    {
        gameIsInPause = false;

        title.SetActive(true);

        title.transform.localPosition = new Vector3(0, 0, 0.7f);

        Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);

        initIntroductionBox();
    }

    internal void deleteLevelSaves()
    {
        LocalSave.SetInt("nypb_levelReached", 1);
        maxLevel = 1;
        LocalSave.Save();
    }

    public void SelectTypeOfBrush(typeOfUnit newType)
    {
        currentUnitType = newType;
    }

    public void SelectTypeOfBrushInt(int newType)
    {
        currentUnitType = (typeOfUnit )newType;
    }

    int areaBrush = maxSizeOfBrush * maxSizeOfBrush;

    public void IncreaseBrushSize()
    {
        if (sizeOfBrush < 500)
        {
            sizeOfBrush++;

            areaBrush = sizeOfBrush * sizeOfBrush;
        }
    }

    public void DecreaseBrushSize()
    {
        if (sizeOfBrush > 2)
        {
            sizeOfBrush--;

            areaBrush = sizeOfBrush * sizeOfBrush;
        }
    }

    public void setEditMode(bool newInEditMode)
    {
        inEditMode = newInEditMode;
    }

    public void playGame(bool isPlayGame)
    {
        inEditMode = !isPlayGame;
    }

    internal int getCurrentLevel()
    {
        return currentLevel;
    }

    public void increaseLevel()
    {
        if (inEditMode)
        {
            currentLevel++;

            prepareNewLevel();
        }
    }

    public void decreaseLevel()
    {
        if (inEditMode && currentLevel >= 0)
        {
            currentLevel--;

            prepareNewLevel();
        }
    }

    public void restartLevel()
    {
        if (inEditMode)
        {
            prepareNewLevel();
        }
    }

    int coolDownSlider = 0;
    int levelRequested = 0;

    public void selectLevel(float nbOfLevelFloat)
    {
        if (inEditMode)
        {
            levelRequested = (int)nbOfLevelFloat;
            coolDownSlider = 20;
        }
    }

    // Call by the update while in edit mode
    public void doSelectLevel()
    {
        if (coolDownSlider == 0)
            return;

        if (coolDownSlider > 1)
        {
            coolDownSlider--;
        }
        else
        {
            currentLevel = levelRequested;

            prepareNewLevel();

            coolDownSlider = 0;
        }
    }

    public void activateHero(int nbHero)
    {
        if (!inEditMode)
        {
            switch (nbHero)
            {
                case 1:
                    hero1Activated = true;
                    break;

                case 2:
                    hero2Activated = true;
                    break;

                case 3:
                    hero3Activated = true;
                    break;

                case 4:
                    hero4Activated = true;
                    break;

                case 5:
                    hero5Activated = true;
                    break;

                case 6:
                    hero6Activated = true;
                    break;
            }
        }
    }

    // If the current level is not the last one, allow to play in free mode (infinite number of resources)
    public void setInFreeMode()
    {
        if (currentLevel < maxLevel)
        { 
            inFreeMode = true;
        }
    }

    public Toggle ourToggle;
    public Button ourLevelPlus;
    public MMPGHideHeroesIcons ourHeroesSelector;

    public void checkIfAvailable()
    {
        // Get the max level and set up the max in the slide from that
        if (currentLevel >= maxLevel)
        {
            ourToggle.interactable = false;
            ourLevelPlus.interactable = false;
        }
        else
        {
            ourToggle.interactable = true;
            ourLevelPlus.interactable = true;
        }
        ourHeroesSelector.hideOrShowHeroes(isHero1, isHero2, isHero3, isHero4, isHero5, isHero6);
    }
}
