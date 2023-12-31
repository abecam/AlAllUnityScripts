using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
    private String rootNameOfLevel = "NYPB";

    public GameObject title;
    public GameObject backgroundImage;

    // We will roll through the background fx with always only one "emptied".
    private GameObject[] backgroundFx = new GameObject[maxNbOfFxLayers];
    private Texture2D[] textureFX = new Texture2D[maxNbOfFxLayers];
    private Renderer[] imageRendererFX = new Renderer[maxNbOfFxLayers];

    const int maxNbOfFxLayers = 30;

    private AudioSource bombSound;

    int shakeCamera = 0; // Give a visual clue a bomb has been touched.

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    private bool inFreeMode = false;

    private int coolDown = 0; // Do not immediately do something else after some action

    Vector3 targetPos;

    float currentTime;
    float lastTime;
    float initialTime;

    float lastTimeSpent = 0; // last saved time for the mode

    const int absMaxLevel = 650; // Max level reachable
    int maxLevel = 1; // Max level reached by the player

    private bool hasNature = false;
    private bool hasMonster = false;
    private bool isRestricted = true;

    float adsMultiplier = 1;
    float shopMultiplier = 1;
    float defShopMultiplier = 1;
    int rangeShop = 1;
    int nbClones = 0;
    bool isKameha = false;
    bool isKamehaForClone = false;
    int pwKameha = 1;

    public WatchAdsHero ourAdsWatcher;

    public enum faction
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

    Color colorPlayerBase = new Color(0.4f, 0.4f, 1);
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

    Color colorOurAttacked = new Color(0.7f, 0.7f, 1.0f, 0.2f);

    Color colorAllyBase = new Color(0.4f, 0.4f, 1);
    Color colorAllyRanged = new Color(0.3f, 0.3f, 1);
    Color colorAllyMachine = new Color(0.4f, 0.4f, 0.8f);
    Color colorAllyWall = new Color(0.6f, 0.6f, 1);

    Color colorEnemyBase = new Color(1, 0.4f, 0.4f);
    Color colorEnemyRanged = new Color(1, 0.2f, 0.2f);
    Color colorEnemyMachine = new Color(0.8f, 0.2f, 0.2f);
    Color colorEnemyWall = new Color(1, 0.8f, 0.8f);

    Color colorEnemyAllyBase = new Color(1, 0.4f, 0.4f);

    Color colorEnemyAttacked = new Color(1f, 0.7f, 0.7f, 0.2f);
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

    Color colorWhiteTrans = new Color(0.8f,0.8f, 0.8f,0.3f);

    const int sizeArena = 128;
    const int sizeArenaMinusOne = sizeArena -1;

    private const int maxSizeOfBrush = 4;
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

    public class Unit
    {
        public faction faction ; // 0-> player, 1-> enemy, 2->neutral
        public float strength;
        public float stamina;
        public float defense;
        public float health;

        public typeOfUnit type;
        public int fightRange;
        internal float staminaMax;
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

    int nbOfHero = 0;
    int nbOfSupport = 0;
    int nbOfMachines = 0;
    int nbOfMonks = 0;
    int nbOfSpies = 0;
    int nbOfBombs = 0;

    int currentNbOfClosed = 0;
    int currentNbOfRanged = 0;
    int currentNbOfMachines = 0;

    int availableNbOfClosed = 0;
    int availableNbOfRanged = 0;
    int availableNbOfMachines = 0;

    MMPGHeroStrength ourHeroStrength = new MMPGHeroStrength();

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
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
        nbOfHero = playersUnit.nbOfHero;
        nbOfSupport = playersUnit.nbOfSupport;
        nbOfMachines = playersUnit.nbOfMachineUnit;

        checkIfAvailable();

        prepareMap();

        // By default, add player units here
        prepareMapPlayer();

        // Prepare the main texture from there
        createTexture();

        float zBackground = 3.5f;

        // And fill the FX layers with full alpha
        for (int iFx = 0; iFx < maxNbOfFxLayers; ++iFx)
        {
            backgroundFx[iFx] = Instantiate(backgroundImage);

            backgroundFx[iFx].transform.position = new Vector3(backgroundFx[iFx].transform.position.x, backgroundFx[iFx].transform.position.y, zBackground);

            zBackground -= 0.2f;

            createTextureFX(iFx);
        }

        createRandom();

        // Get the kind of game from the menu
        initIntroductionBox();

        checkUpdatesShop();

        // Load the last time spent
        LoadTimeSpent();

        lastTime = Time.time; // Last "frame" time
        initialTime = lastTime; // Level start time
        currentTime = lastTime;  
    }

    private void checkUpdatesShop()
    {
        ManageShopPurchasesHero ourPurchaseManager = GetComponent<ManageShopPurchasesHero>();

        ourPurchaseManager.loadPurchases();

        shopMultiplier = 1;
        bool hasKey = LocalSave.HasFloatKey("HeroAttackMult");
        if (hasKey)
        {
            shopMultiplier = LocalSave.GetFloat("HeroAttackMult");
        }


        defShopMultiplier = 1;
        hasKey = LocalSave.HasFloatKey("HeroDefenseMult");
        if (hasKey)
        {
            defShopMultiplier = LocalSave.GetFloat("HeroDefenseMult");
        }

        hasKey = LocalSave.HasBoolKey("KamehaAvailable");
        if (hasKey)
        {
            isKameha = LocalSave.GetBool("KamehaAvailable");
        }

        hasKey = LocalSave.HasBoolKey("KamehaClone");
        if (hasKey)
        {
            isKamehaForClone = LocalSave.GetBool("KamehaClone");
        }

        hasKey = LocalSave.HasIntKey("PwKameha");
        if (hasKey)
        {
            pwKameha = LocalSave.GetInt("PwKameha");
        }

        hasKey = LocalSave.HasIntKey("HeroRange");
        if (hasKey)
        {
            rangeShop = LocalSave.GetInt("HeroRange") + 1;
        }
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
        textureFX[iFX].filterMode = FilterMode.Point;

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
    int bombsComeAt = 40;
    int machinesComeAt = 100;
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

        int nbOfNature = (int)((Random.value) * 20);
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

        if (currentLevel < 110)
        {
            nbOfClosedEnemies = currentLevel * 100;
            nbOfRangedEnemies = (currentLevel - rangedComeAt) * 50;
            nbOfMachinesEnemies = (currentLevel - machinesComeAt) * 50;
        }
        else if (currentLevel < 220)
        {
            int diffLevel = currentLevel - 110;

            nbOfRangedEnemies = diffLevel * 100;
            nbOfClosedEnemies = (diffLevel - rangedComeAt) * 50;
            nbOfMachinesEnemies = (diffLevel - machinesComeAt) * 50;

            wallsComeEvery = 2;
        }
        else if (currentLevel < 330)
        {
            int diffLevel = currentLevel - 220;

            nbOfMachinesEnemies = diffLevel * 100;
            nbOfRangedEnemies = (diffLevel - rangedComeAt) * 50;
            nbOfClosedEnemies = (diffLevel - machinesComeAt) * 50;

            wallsComeEvery = 1;
        }
        else if (currentLevel < 440)
        {
            int diffLevel = currentLevel - 330;

            nbOfMachinesEnemies = diffLevel * 100 + (diffLevel - machinesComeAt) * 50;
            nbOfRangedEnemies = (diffLevel - rangedComeAt) * 50;
            //nbOfClosedEnemies = (diffLevel - machinesComeAt) * 50;
            wallsComeEvery = 1;
        }
        else
        {
            int diffLevel = currentLevel -370;

            nbOfMachinesEnemies = diffLevel * 100 + (diffLevel - machinesComeAt) * 50 + (diffLevel - rangedComeAt) * 50;
            //nbOfRangedEnemies = (diffLevel - rangedComeAt) * 50;
            //nbOfClosedEnemies = (diffLevel - machinesComeAt) * 50;
            wallsComeEvery = 1;
        }
        bool isWall = (currentLevel % wallsComeEvery) == 0;

        int iClosed = nbOfClosedEnemies;
        int iRanged = nbOfRangedEnemies;
        int iMachine = nbOfMachinesEnemies;

        int yWall = 0; // Where to build the wall.

        float baseRangedHealth = 14000 + 10000*currentLevel;
        float baseRangedAttack = 10000 + 10000 * currentLevel;
        float baseRangedDefense = 20000 + 20000 * currentLevel;

        float baseMachineHealth = 500000000 + 100000000 * currentLevel;
        float baseMachineAttack = 50000000000 + 10000000000 * currentLevel;
        float baseMachineDefense = 3000000000 + 10000000000 * currentLevel;

        float baseClosedHealth = 100 + 2 * currentLevel;
        float baseClosedAttack = 4 + 2 * currentLevel;
        float baseClosedDefense = 4 + 5 * currentLevel;

        if (currentLevel > 400)
        {
            baseRangedHealth = 140000000000000 + 100000000000000 * currentLevel;
            baseRangedAttack = 100000000000000 + 100000000000000 * currentLevel;
            baseRangedDefense = 200000000000000 + 200000000000000 * currentLevel;

            baseMachineHealth = 50000000000000f + 10000000000000f * currentLevel;
            baseMachineAttack = 500000000000000000f + 100000000000000000f * currentLevel;
            baseMachineDefense = 300000000000000f + 1000000000000000f * currentLevel;

            baseClosedHealth = 100 + 2 * currentLevel;
            baseClosedAttack = 4 + 2 * currentLevel;
            baseClosedDefense = 4 + 5 * currentLevel;
        }
        else if (currentLevel > 300)
        {
            baseRangedHealth = 14000000000 + 10000000000 * currentLevel;
            baseRangedAttack = 10000000000 + 10000000000 * currentLevel;
            baseRangedDefense = 20000000000 + 20000000000 * currentLevel;

            baseMachineHealth = 50000000000000 + 10000000000000 * currentLevel;
            baseMachineAttack = 50000000000000000 + 10000000000000000 * currentLevel;
            baseMachineDefense = 300000000000000 + 1000000000000000 * currentLevel;

            baseClosedHealth = 10000000 + 2000000 * currentLevel;
            baseClosedAttack = 400000 + 2000000 * currentLevel;
            baseClosedDefense = 400000 + 5000000 * currentLevel;
        }
        //else if (currentLevel > 200)
        //{
        //    baseHealthWall = 6000000000 + 20000000000 * currentLevel;
        //    baseAttackWall = 1000000000 * currentLevel;
        //}
        //else if (currentLevel > 100)
        //{
        //    baseHealthWall = 6000 + 2000000 * (currentLevel - 10);
        //    baseAttackWall = 1000 * currentLevel;
        //}
        //else if (currentLevel > 50)
        //{
        //    baseHealthWall = 6000 + 20000 * (currentLevel - 10);
        //}

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

                            oneEnemyUnit.defense = baseClosedDefense; // Not strong

                            oneEnemyUnit.strength = baseClosedAttack;
                            oneEnemyUnit.stamina = 50;
                            oneEnemyUnit.staminaMax = 50;
                            oneEnemyUnit.health = baseClosedHealth;

                            iClosed--;

                            wasAnythingLeft = true;
                        }
                        else if (iMachine > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.machine;
                            oneEnemyUnit.fightRange = 5;

                            oneEnemyUnit.defense = baseMachineDefense;

                            oneEnemyUnit.strength = baseMachineAttack;
                            oneEnemyUnit.stamina = 800;
                            oneEnemyUnit.staminaMax = 800;
                            oneEnemyUnit.health = baseMachineHealth;

                            iMachine--;

                            wasAnythingLeft = true;
                        }
                        else if (iRanged > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.ranged;
                            oneEnemyUnit.fightRange = 3;

                            oneEnemyUnit.defense = baseRangedDefense; // Not strong

                            oneEnemyUnit.strength = baseRangedAttack;
                            oneEnemyUnit.stamina = 40;
                            oneEnemyUnit.staminaMax = 40;
                            oneEnemyUnit.health = baseRangedHealth;

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

                            oneEnemyUnit.defense = baseMachineDefense;

                            oneEnemyUnit.strength = baseMachineAttack;
                            oneEnemyUnit.stamina = 800;
                            oneEnemyUnit.staminaMax = 800;
                            oneEnemyUnit.health = baseMachineHealth;

                            iMachine--;

                            wasAnythingLeft = true;
                        }
                        else if (iRanged > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.ranged;
                            oneEnemyUnit.fightRange = 3;

                            oneEnemyUnit.defense = baseRangedDefense; // Not strong

                            oneEnemyUnit.strength = baseRangedAttack;
                            oneEnemyUnit.stamina = 40;
                            oneEnemyUnit.staminaMax = 40;
                            oneEnemyUnit.health = baseRangedHealth;

                            iRanged--;

                            wasAnythingLeft = true;
                        }
                        else if (iClosed > 0)
                        {
                            oneEnemyUnit.type = typeOfUnit.close;
                            oneEnemyUnit.fightRange = 1;

                            oneEnemyUnit.defense = baseClosedDefense; // Not strong

                            oneEnemyUnit.strength = baseClosedAttack;
                            oneEnemyUnit.stamina = 50;
                            oneEnemyUnit.staminaMax = 50;
                            oneEnemyUnit.health = baseClosedHealth;

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
                        if (yWall < 0)
                        {
                            isWall = false;
                        }
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

        float baseHealthWall = 6000;
        float baseAttackWall = 0;

        if (currentLevel > 400)
        {
            baseHealthWall = 600000000000000000f + 600000000000000000f * currentLevel;
            baseAttackWall = 100000000000000000f * currentLevel;
        }
        else if (currentLevel > 300)
        {
            baseHealthWall = 600000000000000f + 6000000000000000f * currentLevel;
            baseAttackWall = 100000000000000f * currentLevel;
        }
        else if (currentLevel > 200)
        {
            baseHealthWall = 6000000000 + 20000000000 * currentLevel;
            baseAttackWall = 1000000000 * currentLevel;
        }
        else if (currentLevel > 100)
        {
            baseHealthWall = 6000 + 2000000 * (currentLevel - 10);
            baseAttackWall = 1000 * currentLevel;
        }
        else if (currentLevel > 50)
        {
            baseHealthWall = 6000 + 20000 * (currentLevel - 10);
        }
        
        int baseRangeWall = 4;
        if (currentLevel > 200)
        {
            baseRangeWall = 8;
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
                oneEnemyUnit.fightRange = baseRangeWall;

                oneEnemyUnit.defense = 0; // Strong

                oneEnemyUnit.strength = baseAttackWall;
                oneEnemyUnit.stamina = 0;
                oneEnemyUnit.staminaMax = 0;
                oneEnemyUnit.health = baseHealthWall;

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

        int currentNbOfCLone = 0;

        bool hasKey = LocalSave.HasIntKey("HeroClones");
        if (hasKey)
        {
            currentNbOfCLone = LocalSave.GetInt("HeroClones");
        }

        availableNbOfClosed = 1 + currentNbOfCLone;
        availableNbOfRanged = 0;// nbOfSupport;
        availableNbOfMachines = 0; // nbOfMachines;

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

                        Unit baseHeroUnit = ourHeroStrength.getLatestHero();
                        playerNetUnit.fightRange = baseHeroUnit.fightRange;

                        playerNetUnit.defense = baseHeroUnit.defense;

                        playerNetUnit.strength = baseHeroUnit.strength;
                        playerNetUnit.stamina = baseHeroUnit.stamina;
                        playerNetUnit.staminaMax = baseHeroUnit.staminaMax;
                        playerNetUnit.health = baseHeroUnit.health;

                        availableNbOfClosed--;

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

        adsMultiplier = ourAdsWatcher.multiplier;

        if (iCooldownKameha > -1)
        {
            iCooldownKameha--;
        }
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
        if (isKameha && isKamehaForClone && iCooldownKameha == -1)
        {
            iCooldownKameha = countKameha;
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
                        float hit = unit.strength - unitEnemy.defense;
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
                    float hit = unit.strength - unitEnemy.defense;
                    if (hit < 0)
                    {
                        hit = 0.002f;
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


    int iCooldownKameha = 0;
    int countKameha = 90;
    /**
     * If possible change only one of the 2 methods, then replace the other!
     * We should try to see if there is any performance benefit in doing 2 methods
     */
    private void doAttackOrMovePlayer(int x, int y, Unit unit, coordInt target)
    {
        unit.defense = 5 * defShopMultiplier;

        if (unit.stamina <= 0)
        {
            // Need to rest first
            unit.stamina += 2000;

            return;
        }
        // Check the surrounding depending on the attack range
        int xUnitLow = x - rangeShop;
        int yUnitLow = y - rangeShop;
        if (rangeShop == 8)
        {
            yUnitLow = y - 20;
        }
        if (xUnitLow < 0)
        {
            xUnitLow = 0;
        }
        if (yUnitLow < 0)
        {
            yUnitLow = 0;
        }

        
        int xUnitHigh = x + rangeShop + 1;
        int yUnitHigh = y + rangeShop + 1;
       
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
            if (isKameha && iCooldownKameha == 0)
            {
                if (!isKamehaForClone) // Only the first one
                {
                    iCooldownKameha = countKameha;
                }
                Debug.Log("Attack of hero 1");
                // Kill all units in fronts
                // Check the surrounding depending on the attack range
                xUnitLow = x - 4 * pwKameha;
                yUnitLow = y;

                if (xUnitLow < 0)
                {
                    xUnitLow = 0;
                }
                if (yUnitLow < 0)
                {
                    yUnitLow = 0;
                }

                xUnitHigh = x + 4 * pwKameha;
                yUnitHigh = sizeArena;

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
                            float hit = pwKameha * 1000 - unitEnemy.defense;
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
            }

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone++)
            {
                for (int yUnitZone = yUnitLow; yUnitZone < yUnitHigh; yUnitZone++)
                {
                    Unit unitEnemy = allUnits[xUnitZone, yUnitZone];
                    if ((unitEnemy.faction == faction.enemy) || (unitEnemy.faction == faction.monster) || (unitEnemy.faction == faction.nature && unitEnemy.type != typeOfUnit.tree))
                    {
                        // Attack this unit. Do nothing else.
                        float hit = (adsMultiplier+shopMultiplier)*unit.strength - unitEnemy.defense;
                        Debug.Log("Hit is " + hit);

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

                        //return;
                    }
                    // Debug of range!
                    //updateTextureFX(xUnitZone, yUnitZone, Color.green);
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
        if (y < target.y && y < sizeArenaMinusOne)
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

        if (unit.type == typeOfUnit.hero1)
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
                        float hit = unit.strength - unitEnemy.defense;
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
        else if (unit.type == typeOfUnit.hero2)
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
                        float hit = unit.strength - unitEnemy.defense;
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
        else if (unit.type == typeOfUnit.hero3)
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
        else if (unit.type == typeOfUnit.hero4)
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
        else if (unit.type == typeOfUnit.hero5)
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
        else if (unit.type == typeOfUnit.hero6)
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

            for (int xUnitZone = xUnitLow; xUnitZone < xUnitHigh; xUnitZone+=2)
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
                            float hit = unit.strength - unitEnemy.defense;
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

    int iTurn = 0;
    bool inEditMode = true;

    // Update is called once per frame
    void Update()
    {
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

        currentTime = Time.time;

        if (shakeCamera > 0)
        {
            shakeCamera--;
            float xRand = 0.2f * Random.value - 0.1f;
            float yRand = 0.2f * Random.value - 0.1f;
            transform.position = new Vector3(xRand, yRand, -50);
        }
        else if (shakeCamera == 0)
        {
            transform.position = new Vector3(0, 0, -50);
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
                doATurn();
            }
        }
        else
        {
            //Debug.Log("Did the turn " + iTurn++);
            doSelectLevel();
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
    bool initialHowToPlaySetup = true;
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
                    //float width = GetScreenToWorldWidth();

                    //if (width > 7.6f)
                    //{
                    //    width = 7.6f;
                    //}
                    //title.transform.localScale = Vector2.one * width * 0.5f;

                    //// And the scale on z must stay 1, or we can't layer.
                    //title.transform.localScale = new Vector3(title.transform.localScale.x, title.transform.localScale.y, 1);

                    // Can show how to box
                    initialHowToPlaySetup = true;

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

    private int IntSqrt(int value)
    {
        return Convert.ToInt32(Math.Sqrt(value));
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

                            Unit baseHeroUnit = ourHeroStrength.getLatestHero();
                            playerNetUnit.fightRange = baseHeroUnit.fightRange;

                            playerNetUnit.defense = baseHeroUnit.defense;

                            playerNetUnit.strength = baseHeroUnit.strength;
                            playerNetUnit.stamina = baseHeroUnit.stamina;
                            playerNetUnit.staminaMax = baseHeroUnit.staminaMax;
                            playerNetUnit.health = baseHeroUnit.health;

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

        Debug.Log("Check touch at " + pos);

        // The game arena is always square!
        // Check what is larger-> height or width
        float width = getScreenWidth();
        float height = getScreenHeight();

        float ratio = width / height;

        Debug.Log("Width " + width + " - Height "+ height+ " - Ratio "+ratio);

        // (width/10 < height/14)
        if (width/10 > height/14)
        {
            // So on the height the pos translate almost directly
            arenaPos.y = (1.4f * pos.y/height + 0.5f) * sizeArena;

            // And on the width depending on the aspect ratio
            arenaPos.x = sizeArena - ((1.4f * pos.x/width) * ratio + 0.5f) * sizeArena ;

            Debug.Log("A- Found " + arenaPos);

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

            Debug.Log("B- Found " + arenaPos);

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

    // Was player small found?
    private bool playerSmallFound = false;
    private int currentIteration = 0;

    public bool InTitle { get => inTitle; }
    public int NbOfClosedEnemies { get => nbOfClosedEnemies; }
    public int NbOfRangedEnemies { get => nbOfRangedEnemies;  }
    public int NbOfMachinesEnemies { get => nbOfMachinesEnemies;  }
    public int NbOfClosed { get => nbOfHero; set => nbOfHero = value; }
    public int NbOfRanged { get => nbOfSupport; set => nbOfSupport = value; }
    public int NbOfMachines { get => nbOfMachines; set => nbOfMachines = value; }
    public int NbOfMonks { get => nbOfMonks; set => nbOfMonks = value; }
    public int NbOfSpies { get => nbOfSpies; set => nbOfSpies = value; }
    public int NbOfBombs { get => nbOfBombs; set => nbOfBombs = value; }
    public int MaxLevel { get => maxLevel; }
    public int CurrentNbOfClosed { get => currentNbOfClosed; set => currentNbOfClosed = value; }
    public int CurrentNbOfRanged { get => currentNbOfRanged; set => currentNbOfRanged = value; }
    public int CurrentNbOfMachines { get => currentNbOfMachines; set => currentNbOfMachines = value; }
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

            //inEditMode = true;

            SaveTimeSpent();

            giveReward();

            loadNext();

            checkIfAvailable();
        }
        if (!won && !lost && !inFreeMode)
        {
            //Debug.Log("Nbs are "+availableNbOfClosed + " "+ availableNbOfRanged + " " + availableNbOfMachines);

            if (availableNbOfClosed + availableNbOfRanged + availableNbOfMachines +
                currentNbOfClosed + currentNbOfRanged + currentNbOfMachines == 0)
            {
                // Game is lost
                lostMessage.SetActive(true);

                inEditMode = true;

                SaveTimeSpent();

                lost = true;
                //prepareNewLevel();
            }
        }
    }

    private void giveReward()
    {
        double nbOfCoins = 0;

        bool hasKey = LocalSave.HasDoubleKey("Coins");
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetDouble("Coins");
        }

        double coinsToAdd = nbOfCoins + nbOfCoins / 4;

        LocalSave.SetDouble("Coins", coinsToAdd);

        LocalSave.Save();
    }

    void gameover()
    {
        loadNext();
    }

    void loadNext()
    {
        // Outro or next level?
        if (currentLevel >= 450)
        {
            LocalSave.SetBool("MMHIWon", true);
            LocalSave.Save();

            UnityEngine.SceneManagement.SceneManager.LoadScene("OutroShop");
        }
        else
        {
            setNextLevelReached();

            if (currentLevel >= 100)
            {
                currentLevel += 10;
            }
            else if (currentLevel >= 50)
            {
                currentLevel += 5;
            }
            else if (currentLevel >= 10)
            {
                currentLevel += 2;
            }
            else
            {
                currentLevel++;
            }

            prepareNewLevel();
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

        // And show a * in the score.
        playerSmallFound = true;
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
                playerSmallFound = true;
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
        ManageShopPurchasesHero ourPurchaseManager = GetComponent<ManageShopPurchasesHero>();

        ourPurchaseManager.deleteAllUpdates();

        reloadUnits();

        loadFirstLevel();
    }

    private void reloadUnits()
    {
        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();
        nbOfHero = playersUnit.nbOfHero;
        nbOfSupport = playersUnit.nbOfSupport;
        nbOfMachines = playersUnit.nbOfMachineUnit;

        availableNbOfClosed = nbOfHero;
        availableNbOfRanged = nbOfSupport;
        availableNbOfMachines = nbOfMachines;

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

        availableNbOfClosed = nbOfHero;
        availableNbOfRanged = nbOfSupport;
        availableNbOfMachines = nbOfMachines;

        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();

        checkIfAvailable();

        inFreeMode = false;

        currentIteration = 0;

        currentNbOfClosedEnemies = 0;
        currentNbOfRangedEnemies = 0;
        currentNbOfMachinesEnemies = 0;

        currentNbOfClosed = 0;
        currentNbOfRanged = 0;
        currentNbOfMachines = 0;

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
    }
}
