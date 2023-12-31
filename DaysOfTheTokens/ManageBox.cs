using Common.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManageBox : MonoBehaviour
{
    private bool inPause = false;

    public const string PlayersPrefsTxtKey = "playersPrefs";
    public const string DateOfFirstPlay = "dateOfFirstPlay";
    public const string PlayerSeedKey = "PlayerSeed";
    public const string ExplodeAfterKey = "ExplodeAfter";
    public const string MaxPersonTravelledKey = "MaxPersonTravelled";
    public const string scoreKey = "score";
    public const string nbVisitorsMetKey = "nbVisitors";

    private long score = 0;

    public const int maxColumn = 2;
    public const int maxRow = 5;
    private readonly int MaxNbOfTokens = 10;

    private long playerToken = -1;
    private int explodeAfter = 0;
    private long maxPersonTravelled = 0;
    private long nbVisitorsMet = 0;

    public GameObject baseTokenGameObject;
    private List<VisitorTokensAndInfo> allBoxTokens;

    public GameObject localLoopFeedback;
    public GameObject ownTokenBackFeedback;
    public GameObject ownTokenNewRecordFeedback;
    public GameObject newTokenFeedback;
    public GameObject tokenAlreadyInBoxFeedback;
    public GameObject oldTokenInVaultFeedback;
    public GameObject noRoomLeftFeedback;

    public GameObject visitorTokenParent;

    public ManageToken ourTokenManageToken;

    public ManageVault ourVault;

    public TextMesh nbOfVisitorsTokens;
    public ShowAndMoveBadTokens ourBadTokensMover;

    String saltTxt;
    String password;
    private HashSet<long> allVaultTokens;

    Dictionary<long, VisitorTokensAndInfo> tokensInBox;

    internal List<VisitorTokensAndInfo> AllBoxTokens { get => allBoxTokens; }
    public long PlayerToken { get => playerToken; }
    public int ExplodeAfter { get => explodeAfter; }
    public long MaxPersonTravelled { get => maxPersonTravelled; }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        getOrSetLocalToken();

        // Get all the current seeds (if any) in the box
        getBoxTokens();

        // And get all the vault keys
        getVaultTokens();
    }

    internal void addABlueToken()
    {
        ourBadTokensMover.doSeekAsBlue(this);
    }

    private void getOrSetLocalToken()
    {
        bool isKey = LocalSave.HasStringKey(PlayerSeedKey);
        if (isKey)
        {
            String playerSeedTxt = LocalSave.GetString(PlayerSeedKey);

            long saltNb = LocalSave.GetLong(DateOfFirstPlay);
            saltTxt = CompressString.StringCompressor.longToString(saltNb);

            password = LocalSave.GetString(PlayersPrefsTxtKey);

            String playerSeedDecrypted = CryptFacility.Decrypt(playerSeedTxt, password, saltTxt);
            playerToken = CompressString.StringCompressor.GetLong(playerSeedDecrypted);

            Debug.Log("Returning: Own seed is " + playerToken + " and saved as" + playerSeedDecrypted);

            // Get explodeAfter
            String explodeAfterTxt = LocalSave.GetString(ExplodeAfterKey);
            String explodeAfterDecrypted = CryptFacility.Decrypt(explodeAfterTxt, password, saltTxt);
            explodeAfter = (int)CompressString.StringCompressor.GetLong(explodeAfterDecrypted);

            // Get maxPersonTravelled
            String maxPersonTravelledTxt = LocalSave.GetString(MaxPersonTravelledKey);
            String maxPersonTravelledDecrypted = CryptFacility.Decrypt(maxPersonTravelledTxt, password, saltTxt);
            maxPersonTravelled = CompressString.StringCompressor.GetLong(maxPersonTravelledDecrypted);

            // Get nb of visitors met
            String nbVisitorsMetTxt = LocalSave.GetString(nbVisitorsMetKey);
            Debug.Log("Nb of visitors saved is " + nbVisitorsMetTxt);
            String nbVisitorsMetDecrypted = CryptFacility.Decrypt(nbVisitorsMetTxt, password + saltTxt, saltTxt);
            Debug.Log("A - Nb of visitors decrypted is " + nbVisitorsMetDecrypted);
            nbVisitorsMet = CompressString.StringCompressor.GetLong(nbVisitorsMetDecrypted);
            Debug.Log("B - Nb of visitors decrypted is " + nbVisitorsMet);

            // Get score
            String scoreTxt = LocalSave.GetString(scoreKey);
            String scoreDecrypted = CryptFacility.Decrypt(scoreTxt, password, saltTxt);
            score = CompressString.StringCompressor.GetLong(scoreDecrypted);

            ourTokenManageToken.setNewNbSoFar((int )maxPersonTravelled);
        }
        else
        {
            // Show the seed and ask for the nb before it opens.
            Random.InitState((int)DateTime.Now.Ticks);

            long newSeed = (long)(Random.value * (long.MaxValue - 1000000)); // 1000000 -> Reserved keys
            playerToken = newSeed;
            string newSeedTxt = CompressString.StringCompressor.longToString(newSeed);

            Debug.Log("Own seed is " + newSeed +" and saved as"+ newSeedTxt);

            long saltNb = DateTime.Now.Ticks;

            LocalSave.SetLong(DateOfFirstPlay, saltNb);

            saltTxt = CompressString.StringCompressor.longToString(saltNb);

            long newPassword = (long)(Random.value * long.MaxValue);
            password = CompressString.StringCompressor.longToString(newPassword);

            LocalSave.SetString(PlayersPrefsTxtKey, password);

            string cryptedSeed = CryptFacility.Encrypt(newSeedTxt, password, saltTxt);

            LocalSave.SetString(PlayerSeedKey, cryptedSeed);

            explodeAfter = 10;
            String explodeAfterTxt = CompressString.StringCompressor.longToString(explodeAfter);
            String explodeAfterEncrypted = CryptFacility.Encrypt(explodeAfterTxt, password, saltTxt);
            LocalSave.SetString(ExplodeAfterKey, explodeAfterEncrypted);

            maxPersonTravelled = 0;
            String maxPersonTravelledTxt = CompressString.StringCompressor.longToString(maxPersonTravelled);
            String maxPersonTravelledEncrypted = CryptFacility.Encrypt(maxPersonTravelledTxt, password, saltTxt);
            LocalSave.SetString(MaxPersonTravelledKey, maxPersonTravelledEncrypted);

            ourTokenManageToken.setNewNbSoFar((int)maxPersonTravelled);

            saveScore();
            saveVisitorsMet();

            LocalSave.Save();
        }
    }

    internal void addRedToken(int count)
    {
        for (int iBadTokens = 0; iBadTokens < count; iBadTokens++)
        {
            ourBadTokensMover.doSeekAsRed(this);
        }
    }

    internal void addGreenToken(int count)
    {
        for (int iBadTokens = 0; iBadTokens < count; iBadTokens++)
        {
            ourBadTokensMover.doSeekAsGreen(this);
        }
    }

    internal void addSuperToken(int power)
    {
        Debug.Log("Got a super token");

        VisitorTokensAndInfo oneNewToken = new VisitorTokensAndInfo();
        oneNewToken.tokenId = (long)(long.MaxValue - 1000000 + Random.value * 1000000);
        oneNewToken.NbBeforeExplode = (int )(Random.value * 1000);
        oneNewToken.NbSoFar = power * 100 + 100;

        addANewToken(oneNewToken);
    }

    private void saveScore()
    {
        String scoreTxt = CompressString.StringCompressor.longToString(score);
        String scoreEncrypted = CryptFacility.Encrypt(scoreTxt, password, saltTxt);
        LocalSave.SetString(scoreKey, scoreEncrypted);

        Debug.Log(password + " - Score is " + score + " and in save " + scoreTxt + " encrypted " + scoreEncrypted);
    }

    private void calculateScore()
    {
        long scoreMultiplier = (2*maxPersonTravelled + 1) * (nbVisitorsMet);
        long scoreInitial = 0;

        foreach (VisitorTokensAndInfo oneToken in allBoxTokens)
        {
            scoreInitial += oneToken.NbSoFar;
        }
        // Add the premium token bonus if any
        scoreInitial += GetPremiumTokenInfo.MyNb;

        score = scoreMultiplier * scoreInitial;

        saveScore();
        submitToLeaderboard();
        LocalSave.Save();
    }

    private void submitToLeaderboard()
    {
        if (THGE.GameLogic.GameManager.Instance.Authenticated)
        {
            THGE.GameLogic.GameManager.Instance.PostToLeaderboard(score);

            // Check our rank and the best score
            THGE.GameLogic.GameManager.Instance.getBestScore();
        }
    }

    private void addANewVisitor()
    {
        nbVisitorsMet++;
        saveVisitorsMet();

        LocalSave.Save();
    }

    /*
     * Pref save must be done outside!
     */
    private void saveVisitorsMet()
    {
        String nbVisitorsMetTxt = CompressString.StringCompressor.longToString(nbVisitorsMet);
        String nbVisitorsMetEncrypted = CryptFacility.Encrypt(nbVisitorsMetTxt, password + saltTxt, saltTxt);
        LocalSave.SetString(nbVisitorsMetKey, nbVisitorsMetEncrypted);

        Debug.Log("A - " + (password + saltTxt) + " - Nb of visitors met is " + nbVisitorsMet + " and in save " + nbVisitorsMetTxt + " encrypted " + nbVisitorsMetEncrypted);

        String nbVisitorsMetTxtFromSave = LocalSave.GetString(nbVisitorsMetKey);
        String nbVisitorsMetDecrypted = CryptFacility.Decrypt(nbVisitorsMetTxtFromSave, password + saltTxt, saltTxt);
        long newNbVisitorsMet = CompressString.StringCompressor.GetLong(nbVisitorsMetDecrypted);

        Debug.Log("B - "+ (password + saltTxt)+" - Nb of visitors met is " + newNbVisitorsMet + " and in save "+ nbVisitorsMetTxtFromSave+" encrypted "+ nbVisitorsMetEncrypted);

    }

    public void setNewNbBeforeOpening(int nbBeforeOpening)
    {
        if (nbBeforeOpening < 10 || nbBeforeOpening > 100)
        {
            nbBeforeOpening = 10;
            Debug.Log("Nb before opening was not betwen 10 and 100 (" + nbBeforeOpening + ")");
        }

        long saltNb = LocalSave.GetLong(DateOfFirstPlay);

        String explodeAfterTxt = CompressString.StringCompressor.longToString(explodeAfter);
        String explodeAfterEncrypted = CryptFacility.Encrypt(explodeAfterTxt, password, saltTxt);

        LocalSave.SetString(ExplodeAfterKey, explodeAfterEncrypted);

        LocalSave.Save();
    }

    private void getBoxTokens()
    {
        // In case, remove all box visitors seeds
        foreach (Transform child in visitorTokenParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        tokensInBox = new Dictionary<long, VisitorTokensAndInfo>();

        allBoxTokens = LoadSaveTokens.loadDecodeDecryptAllSeeds();

        int column = 0;
        int row = 0;

        float minX = -1.2f;
        float maxY = 0.8f;

        float stepY = 0.2f;
        float stepX = 1.5f;

        foreach (VisitorTokensAndInfo oneToken in allBoxTokens)
        {
            GameObject oneNewTokenGameObject = Instantiate(baseTokenGameObject);

            oneNewTokenGameObject.transform.SetParent(visitorTokenParent.transform);

            oneNewTokenGameObject.transform.localPosition = new Vector2(minX + stepX * ((float )column), maxY - stepY * ((float )row));

            row++;
            if (row >= maxRow)
            {
                row = 0;
                column++;
            }

            ManageToken itsManageToken = oneNewTokenGameObject.GetComponent<ManageToken>();

            oneToken.OurGameObject = oneNewTokenGameObject;
            oneToken.ourManageToken = itsManageToken;

            Debug.Log("BBBB- Setting the new NbSoFar to " + oneToken.NbSoFar);
            oneToken.ourManageToken.setNewNbSoFar(oneToken.NbSoFar);

            tokensInBox.Add(oneToken.tokenId, oneToken);
        }
    }

    private void getVaultTokens()
    {
        allVaultTokens = new HashSet<long>();
        List<long> allVaultSeedsList = LoadVaultTokens.loadDecodeDecryptAllSeeds();

        foreach (long oneSeed in allVaultSeedsList)
        {
            allVaultTokens.Add(oneSeed);
        }

        ourVault.setANewNbOfEntries(allVaultSeedsList.Count);
    }

    float posOnScreen = 0;

    /**
     * Return true if the main token back is the same (not allowed-> direct copy/past from the same client).
     */
    internal bool addANewToken(VisitorTokensAndInfo aNewToken)
    {
        // First check if it is our seed
        if (aNewToken.tokenId == playerToken)
        {
            if (aNewToken.NbSoFar == 0)
            {
                // Good trial! It's copy/pasted from the same player
                GameObject localLoopBacKFB = Instantiate(localLoopFeedback);
                localLoopBacKFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
                posOnScreen += 1;

                return true;
            }

            // Give some visual feedback
            GameObject ownTokenBacKFB = Instantiate(ownTokenBackFeedback);
            ownTokenBacKFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
            posOnScreen += 1;

            // Give a bonus for the token back

            // Check how far it wents
            if (aNewToken.NbSoFar > maxPersonTravelled)
            {
                maxPersonTravelled = aNewToken.NbSoFar;

                String maxPersonTravelledTxt = CompressString.StringCompressor.longToString(maxPersonTravelled);
                String maxPersonTravelledEncrypted = CryptFacility.Encrypt(maxPersonTravelledTxt, password, saltTxt);
                LocalSave.SetString(MaxPersonTravelledKey, maxPersonTravelledEncrypted);

                LocalSave.Save();

                // Give another visual feedback
                GameObject ownTokenNewRecordFB = Instantiate(ownTokenNewRecordFeedback);
                ownTokenNewRecordFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
                posOnScreen += 1;

                ourTokenManageToken.setNewNbSoFar((int)maxPersonTravelled);

                calculateScore();
            }

            return false;
        }

        // Do we still have room?
        if (allBoxTokens.Count >= MaxNbOfTokens)
        {
            // Give another visual feedback
            GameObject noRoomLeftFB = Instantiate(noRoomLeftFeedback);
            noRoomLeftFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
            posOnScreen += 1;

            return false;
        }
        bool wasInBox = false;

        // Then if it is already in the box
        foreach (VisitorTokensAndInfo oneToken in allBoxTokens)
        {
            if (oneToken.tokenId == aNewToken.tokenId)
            {
                wasInBox = true;

                //// Check the new distance travelled
                //if (aNewToken.NbSoFar > oneToken.NbSoFar)
                //{
                //    oneToken.NbSoFar = aNewToken.NbSoFar;

                //    // Give another visual feedback
                //    Instantiate(oldTokenNewRecordBackFeedback);

                //    // The box need to be saved again
                //    saveBoxContent();
                //}
                GameObject tokenAlreadyInBoxFB = Instantiate(tokenAlreadyInBoxFeedback);
                tokenAlreadyInBoxFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
                posOnScreen += 1;

                oneToken.refusedNewEntry = true;
            }
        }
        if (!wasInBox)
        {
            // Or in the vault

            bool wasInVault = allVaultTokens.Contains(aNewToken.tokenId);

            // And it did one step more
            Debug.Log("Visitor did " + aNewToken.NbSoFar + " before coming here");

            aNewToken.NbSoFar++;

            // Check if it explodes
            if (aNewToken.NbSoFar == aNewToken.NbBeforeExplode)
            {
                aNewToken.exploding = true;

                // And a big bonus!
                aNewToken.NbSoFar *= 4;
            }
            // Brand new token, give a bonus
            allBoxTokens.Add(aNewToken);

            if (wasInVault)
            {
                // Move back to box with the new NbSoFar
                allVaultTokens.Remove(aNewToken.tokenId);

                ourVault.setANewNbOfEntries(allVaultTokens.Count);

                // Save the new vault
                saveVaultContent();

                aNewToken.isReturningFromVault = true;

                GameObject oldTokenInVaultFB = Instantiate(oldTokenInVaultFeedback);
                oldTokenInVaultFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
                posOnScreen += 1;
            }
            else
            {
                aNewToken.isNew = true;

                GameObject newTokenFB = Instantiate(newTokenFeedback);
                newTokenFB.GetComponent<ZoomAndLeave>().setPos(posOnScreen);
                posOnScreen += 1;

                addANewVisitor();
            }
            // The box need to be saved again
            saveBoxContent();

            // And update the tokens to show
            getBoxTokens();

            // And calculate the new score
            calculateScore();
        }
        return false;
    }

    public void moveFromBoxToVault(long tokenID)
    {
        if (!tokensInBox.ContainsKey(tokenID))
        {
            Debug.Log("Nothing to move, token is not here");
            return;
        }

        VisitorTokensAndInfo tokenToMove = tokensInBox[tokenID];
        allBoxTokens.Remove(tokenToMove);
        tokensInBox.Remove(tokenID);

        allVaultTokens.Add(tokenID);

        ourVault.setANewNbOfEntries(allVaultTokens.Count);

        // Save the new vault
        saveVaultContent();

        // The box need to be saved again
        saveBoxContent();

        // And update the tokens to show
        getBoxTokens();

        // And calculate the new score
        calculateScore();
    }

    private void saveVaultContent()
    {
        List<long> tokenIdArray = allVaultTokens.ToList();

        LoadVaultTokens.codeEncryptAndSaveAllSeed(tokenIdArray);
    }

    private void saveBoxContent()
    {
        LoadSaveTokens.codeEncryptAndSaveAllTokens(allBoxTokens);
    }

    List<VisitorTokensAndInfo> pendingVisitors = new List<VisitorTokensAndInfo>();

    internal void addVisitors(List<VisitorTokensAndInfo> allOthersTokens)
    {
        // These visitors will be added little by little.
        pendingVisitors.AddRange(allOthersTokens);
    }

    int iCheckVisitors = 0;

    bool dragMode = false;
    bool oneTokenSelected = false;
    VisitorTokensAndInfo selectedToken = null;
    Vector2 lastPos;

    // Update is called once per frame
    void Update()
    {
        if (!inPause)
        {
            if (iCheckVisitors++ % 20 == 0)
            {
                if (pendingVisitors.Count > 0)
                {
                    VisitorTokensAndInfo oneElement = pendingVisitors[0];
                    addANewToken(oneElement);
                    pendingVisitors.RemoveAt(0);
                }
                else
                {
                    posOnScreen = 0;
                }
            }

            nbOfVisitorsTokens.text = "" + allBoxTokens.Count;

            // Check if the trashcan has been clicked.
            if (Application.platform != RuntimePlatform.Android)
            {
                Vector3 touchPos = Input.mousePosition;

                // use the input stuff
                if (Input.GetMouseButton(0))
                {
                    if (!dragMode)
                    {
                        checkBoard(touchPos);
                        dragMode = true;
                    }
                    else
                    {
                        if (oneTokenSelected)
                        {
                            moveSelectedToken(touchPos);
                        }
                    }
                }
                else
                {
                    dragMode = false;
                    if (oneTokenSelected)
                    {
                        // Check if it was dropped on the Vault
                        dropSelectedToken(touchPos);

                        oneTokenSelected = false;
                    }
                }
                // Check if the user has clicked
                bool aTouch = Input.GetMouseButtonDown(0);
                bool rightClick = Input.GetMouseButton(1);

                if (aTouch)
                {
                    // Debug.Log( "Moused moved to point " + touchPos );
                }
                else
                {
                    // No touch reset the cool down
                }
            }
            else
            {
                if (Input.touchCount >= 1)
                {
                    Touch firstFinger = Input.GetTouch(0);

                    if (Input.touchCount == 1)
                    {
                        Vector3 touchPos = firstFinger.position;

                        if (firstFinger.phase == TouchPhase.Began)
                        {
                            checkBoard(touchPos);
                            dragMode = true;
                        }
                        else if (firstFinger.phase == TouchPhase.Moved)
                        {
                            if (oneTokenSelected)
                            {
                                moveSelectedToken(touchPos);
                                lastPos = touchPos;
                            }
                        }
                        else
                        {
                            // 
                        }
                    }
                    else if (Input.touchCount == 2)
                    {
                        Vector2 touchPos1 = firstFinger.position;
                        Touch secondFinger = Input.GetTouch(1);
                        Vector2 touchPos2 = secondFinger.position;

                        if (firstFinger.phase == TouchPhase.Moved && secondFinger.phase == TouchPhase.Moved)
                        {
                            //
                        }
                        else
                        {
                            //
                        }
                    }
                }
                else
                {
                    if (oneTokenSelected)
                    {
                        // Check if it was dropped on the Vault
                        dropSelectedToken(lastPos);

                        oneTokenSelected = false;
                    }
                    dragMode = false;
                }
            }
        }
    }

    Vector2 savedPos;

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        foreach (VisitorTokensAndInfo oneToken in allBoxTokens)
        {
            oneTokenSelected = oneToken.ourManageToken.checkIfSelected(touchPos2);

            if (oneTokenSelected)
            {
                selectedToken = oneToken;
                savedPos = oneToken.OurGameObject.transform.position;

                break;
            }
        }
    }

    private void moveSelectedToken(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        selectedToken.OurGameObject.transform.position = touchPos2;
    }

    private void dropSelectedToken(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        if ( ourVault.ourGameObject.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            Debug.Log("Dropped on Vault");
            // Move to the vault
            moveFromBoxToVault(selectedToken.tokenId);
        }
        else
        {
            selectedToken.OurGameObject.transform.position = savedPos;
        }
    }

    internal long getCurrentScore()
    {
        return score;
    }

    internal long getNbVisitorsMet()
    {
        return nbVisitorsMet;
    }

    public void setInPause(bool newInPause)
    {
        inPause = newInPause;
    }

    internal bool isInPause()
    {
        return inPause;
    }
}
