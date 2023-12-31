using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetScore : MonoBehaviour
{
    public Text text1;
    public Text text2;
    public Text text3;

    public Text text4;
    public Text text5;
    public Text text6;

    public ManageBox theBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setScore();
    }

    void setScore()
    {
        long newScore = theBox.getCurrentScore();

        string newScoreTxt = "Score " + newScore;

        text1.text = newScoreTxt;
        text2.text = newScoreTxt;
        text3.text = newScoreTxt;

        long nbVisitorsMet = theBox.getNbVisitorsMet();

        string nbVisitorsMetTxt = nbVisitorsMet + " tokens met";

        text4.text = nbVisitorsMetTxt;
        text5.text = nbVisitorsMetTxt;
        text6.text = nbVisitorsMetTxt;
    }
}
