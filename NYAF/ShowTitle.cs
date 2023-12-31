using UnityEngine;

public class ShowTitle : MonoBehaviour
{
    public GameObject brotherText;
    public GameObject brotherTextRandom;

    public GameObject johnText;
    public GameObject johnTextRandom;

    public GameObject simulatorText;
    public GameObject simulatorTextRandom;

    public GameObject versionText;
    public GameObject versionTextRandom;

    private GameObject[] allBrotherText = new GameObject[5];
    private GameObject[] allBrotherRandText = new GameObject[5];

    private GameObject[] allJohnText = new GameObject[5];
    private GameObject[] allJohnRandomText = new GameObject[5];

    private GameObject[] allSimulatorText = new GameObject[5];
    private GameObject[] allSimulatorRandomText = new GameObject[5];

    private GameObject[] allVersionText = new GameObject[5];
    private GameObject[] allVersionRandomText = new GameObject[5];

    int currentPlace = 0;

    const int nbFrameForPhase = 120;

    const int nbPhaseBrother = nbFrameForPhase;

    const int nbPhaseJohn = nbFrameForPhase/2;

    const int nbPhaseSimulator = nbFrameForPhase;

    const int nbPhaseVersion = nbFrameForPhase + nbFrameForPhase / 2;

    const int nbPhaseTotal = 3 * nbFrameForPhase;

    BJSSave ourSaveFacility = new BJSSave();

    // Start is called before the first frame update
    void Start()
    {
        // Check if we start without us
        if (ourSaveFacility.isTitle())
        {
            for (int iBrother = 0; iBrother < 5; iBrother++)
            {
                allBrotherText[iBrother] = Instantiate(brotherText);
                allBrotherRandText[iBrother] = Instantiate(brotherTextRandom);

                allBrotherText[iBrother].transform.parent = this.transform;
                allBrotherRandText[iBrother].transform.parent = this.transform;

                allBrotherText[iBrother].SetActive(true);
                allBrotherRandText[iBrother].SetActive(true);

                allBrotherText[iBrother].transform.localScale = new Vector2(10, 10);
                allBrotherRandText[iBrother].transform.localScale = new Vector2(10, 10);

                allJohnText[iBrother] = Instantiate(johnText);
                allJohnRandomText[iBrother] = Instantiate(johnTextRandom);

                allJohnText[iBrother].transform.parent = this.transform;
                allJohnRandomText[iBrother].transform.parent = this.transform;

                allJohnText[iBrother].SetActive(false);
                allJohnRandomText[iBrother].SetActive(false);

                allJohnText[iBrother].transform.localScale = new Vector2(10, 10);
                allJohnRandomText[iBrother].transform.localScale = new Vector2(10, 10);

                allSimulatorText[iBrother] = Instantiate(simulatorText);
                allSimulatorRandomText[iBrother] = Instantiate(simulatorTextRandom);

                allSimulatorText[iBrother].transform.parent = this.transform;
                allSimulatorRandomText[iBrother].transform.parent = this.transform;

                allSimulatorText[iBrother].SetActive(false);
                allSimulatorRandomText[iBrother].SetActive(false);

                allSimulatorText[iBrother].transform.localScale = new Vector2(10, 10);
                allSimulatorRandomText[iBrother].transform.localScale = new Vector2(10, 10);

                allVersionText[iBrother] = Instantiate(versionText);
                allVersionRandomText[iBrother] = Instantiate(versionTextRandom);

                allVersionText[iBrother].transform.parent = this.transform;
                allVersionRandomText[iBrother].transform.parent = this.transform;

                allVersionText[iBrother].SetActive(false);
                allVersionRandomText[iBrother].SetActive(false);

                allVersionText[iBrother].transform.localScale = new Vector2(10, 10);
                allVersionRandomText[iBrother].transform.localScale = new Vector2(10, 10);
            }
        }
        else
        {
            removeAll(); // Remove us immediately
        }
    }

    const float stepScale = (10 - 0.5f) / ((float )nbFrameForPhase/2);

    float lastSize = 10;

    int iFadingSteps = 0;
    const int nbOfFadingSteps = 30;
    const float stepFading = 1f / ((float)nbOfFadingSteps);

    int iWait = 0;

    // Update is called once per frame
    void Update()
    {
        if (iWait < 100)
        {
            iWait++;

            return;
        }
        currentPlace++;
        if (currentPlace > nbPhaseTotal)
        {
            //Debug.Log("Finished showing title, destroying ourself");
            if (iFadingSteps < nbOfFadingSteps)
            {
                float newAlpha = allBrotherText[0].GetComponent<Renderer>().material.color.a - stepFading;

                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    Renderer oneRendered = allBrotherText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allBrotherRandText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allJohnText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allJohnRandomText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allSimulatorText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allSimulatorRandomText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allVersionText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);

                    oneRendered = allVersionRandomText[iBrother].GetComponent<Renderer>();

                    oneRendered.material.color = new Color(oneRendered.material.color.r, oneRendered.material.color.g, oneRendered.material.color.b, newAlpha);
                }    

                iFadingSteps++;
            }
            else
            {
                removeAll();
            } 
        }
        else
        {
            //if (currentPlace < nbPhaseBrother)
            {
                lastSize = allBrotherText[0].transform.localScale.x;

                for (int iBrother = 0; iBrother < 5; ++iBrother)
                { 
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                    }
                    allBrotherText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }
                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                        allBrotherRandText[iBrother].SetActive(false);
                    }
                    allBrotherRandText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }

                if (currentPlace == nbPhaseJohn)
                {
                    for (int iBrother = 0; iBrother < 5; iBrother++)
                    {
                        allJohnText[iBrother].SetActive(true);
                        allJohnRandomText[iBrother].SetActive(true);
                    }
                }  
            }
            if (currentPlace > nbPhaseJohn)
            {
                lastSize = allJohnText[0].transform.localScale.x;

                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                    }
                    allJohnText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }
                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                        allJohnRandomText[iBrother].SetActive(false);
                    }
                    allJohnRandomText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }

                if (currentPlace == nbPhaseSimulator)
                {
                    for (int iBrother = 0; iBrother < 5; iBrother++)
                    {
                        allSimulatorText[iBrother].SetActive(true);
                        allSimulatorRandomText[iBrother].SetActive(true);
                    }
                }
            }

            if (currentPlace > nbPhaseSimulator)
            {
                lastSize = allSimulatorText[0].transform.localScale.x;

                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                    }
                    allSimulatorText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }
                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                        allSimulatorRandomText[iBrother].SetActive(false);
                    }
                    allSimulatorRandomText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }

                if (currentPlace == nbPhaseVersion)
                {
                    for (int iBrother = 0; iBrother < 5; iBrother++)
                    {
                        allVersionText[iBrother].SetActive(true);
                        allVersionRandomText[iBrother].SetActive(true);
                    }
                }
            }
            if (currentPlace > nbPhaseVersion)
            {
                lastSize = allVersionText[0].transform.localScale.x;

                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                    }
                    allVersionText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }
                for (int iBrother = 0; iBrother < 5; ++iBrother)
                {
                    lastSize = lastSize - stepScale;
                    if (lastSize < 0.5f)
                    {
                        lastSize = 0.5f;
                        allVersionRandomText[iBrother].SetActive(false);
                    }
                    allVersionRandomText[iBrother].transform.localScale = new Vector2(lastSize, lastSize);
                }

                return;
            }
        }
    }

    private void removeAll()
    {
        Destroy(this.gameObject);
    }
}
