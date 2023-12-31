using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMPGHideHeroesIcons : MonoBehaviour
{
    public GameObject hero1;
    public GameObject hero2;
    public GameObject hero3;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void hideOrShowHeroes(bool isHero1, bool isHero2, bool isHero3, bool isHero4, bool isHero5, bool isHero6)
    {
        hero1.SetActive(isHero1);
        hero2.SetActive(isHero2);
        hero3.SetActive(isHero3);
    }
}
