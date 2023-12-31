using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMPGHideHeroesIcons : MonoBehaviour
{
    public Toggle hero1;
    public Toggle hero2;
    public Toggle hero3;
    public Toggle hero4;
    public Toggle hero5;
    public Toggle hero6;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void hideOrShowHeroes(bool isHero1, bool isHero2, bool isHero3, bool isHero4, bool isHero5, bool isHero6)
    {
        hero1.interactable = isHero1;
        hero2.interactable = isHero2;
        hero3.interactable = isHero3;
        hero4.interactable = isHero4;
        hero5.interactable = isHero5;
        hero6.interactable = isHero6;
    }
}
