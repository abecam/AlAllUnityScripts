using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageShopPurchasesHero : MonoBehaviour
{
    float currentAttack = 1;
    float currentDefense = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void loadPurchases()
    {
        currentDefense = 1;
        currentAttack = 1;

        getPurchases();

        LocalSave.Save();
    }

    private void setNbOfClone(int newNb)
    {
        LocalSave.SetInt("HeroClones", newNb);
    }

    private void setPwKameha(int newNb)
    {
        LocalSave.SetInt("PwKameha", newNb);
    }

    private void multiplyAttack(float attackMultiplier)
    {
        currentAttack *= attackMultiplier; //  Mathf.Log(1 + attackMultiplier);

        Debug.Log("Attack of hero is " + currentAttack);

        LocalSave.SetFloat("HeroAttackMult", currentAttack);
    }

    private void multiplyDefense(float DefenseMultiplier)
    {
        currentDefense *= DefenseMultiplier; //  Mathf.Log(1 + DefenseMultiplier);

        LocalSave.SetFloat("HeroDefenseMult", currentDefense);
    }

    private void setRangedAtkRange(int newRange)
    {
        LocalSave.SetInt("HeroRange", newRange);
    }

    private void getPurchases()
    {
        bool hasKey = false;

        hasKey = LocalSave.HasBoolKey("HeroAttack");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack");
            if (isBought)
            {
                // The hero can attack!
                LocalSave.SetBool("HeroCanAttack", true);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense1");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack2");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack2");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense1");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack3");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack3");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense3");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense3");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack4");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack4");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense4");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense4");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack5");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack5");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack6");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack6");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense5");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense5");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack7");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack7");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone1");
            if (isBought)
            {
                // The hero is fast enough to be at 2 places at the same time!
                // Do something here
                setNbOfClone(1);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense6");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense6");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack8");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack8");
            if (isBought)
            {
                // The hero can attack at distance
                // Do something here
                setRangedAtkRange(1);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense17");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense17");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack9");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack9");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack16");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack16");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack17");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack17");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack18");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack18");
            if (isBought)
            {
                // Ranged attack from further away
                // Do something here
                setRangedAtkRange(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense19");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense19");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense20");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense20");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense21");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense21");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack22");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack22");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense23");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense23");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense24");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense24");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack25");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack25");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense26");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense26");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack27");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack27");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone28");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone28");
            if (isBought)
            {
                // The hero is fast enough to be at 2 places at the same time!
                // Do something here
                setNbOfClone(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense29");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense29");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack30");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack30");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense31");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense31");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack32");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack32");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense33");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense33");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense34");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense34");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack35");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack35");
            if (isBought)
            {
                // Ranged attack from further away
                // Do something here
                setRangedAtkRange(3);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense36");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense36");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack37");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack37");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone38");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone38");
            if (isBought)
            {
                // The hero is fast enough to be at 3 places at the same time!
                // Do something here
                setNbOfClone(3);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense39");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense39");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack40");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack40");
            if (isBought)
            {
                // Long ranged attack at the front
                // Do something here
                setRangedAtkRange(4);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense41");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense41");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack42");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack42");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack43");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack43");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyAttack(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack44");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack44");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone45");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone45");
            if (isBought)
            {
                // The hero is fast enough to be at 4 places at the same time!
                // Do something here
                setNbOfClone(4);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense46");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense46");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense47");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense47");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense48");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense48");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack49");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack49");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack50");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack50");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack51");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack51");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense52");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense52");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack53");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack53");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyAttack(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense54");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense54");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack55");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack55");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack56");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack56");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense57");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense57");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense58");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense58");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense59");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense59");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense60");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense60");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack61");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack61");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense62");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense62");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense63");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense63");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense64");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense64");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack65");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack65");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack66");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack66");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense67");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense67");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense68");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense68");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense69");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense69");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense70");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense70");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack71");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack71");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack72");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack72");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack73");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack73");
            if (isBought)
            {
                // The hero masters the ultimate art!
                // Do something here
                LocalSave.SetBool("KamehaAvailable", true);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense74");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense74");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack75");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack75");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense76");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense76");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack77");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack77");
            if (isBought)
            {
                // Stronger ultimate attack
                // Do something here
                setPwKameha(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack78");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack78");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyAttack(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack79");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack79");
            if (isBought)
            {
                // Even stronger ultimate attack
                // Do something here
                setPwKameha(4);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense80");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense80");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense81");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense81");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack82");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack82");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack83");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack83");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense84");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense84");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone85");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone85");
            if (isBought)
            {
                // The hero is fast enough to be at 5 places at the same time!
                // Do something here
                setNbOfClone(5);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense86");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense86");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense87");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense87");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone88");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone88");
            if (isBought)
            {
                // The hero is fast enough to be at 6 places at the same time!
                // Do something here
                setNbOfClone(6);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense89");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense89");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense90");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense90");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack91");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack91");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack92");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack92");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack93");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack93");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyAttack(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense94");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense94");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense95");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense95");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack96");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack96");
            if (isBought)
            {
                // The clones can also use the ultimate attack
                // Do something here
                LocalSave.SetBool("KamehaClone", true);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack97");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack97");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone98");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone98");
            if (isBought)
            {
                // The hero is fast enough to be at 7 places at the same time!
                // Do something here
                setNbOfClone(7);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense99");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense99");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack100");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack100");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack101");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack101");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense102");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense102");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack103");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack103");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack104");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack104");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack105");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack105");
            if (isBought)
            {
                // x10 attack!
                // Do something here
                multiplyAttack(10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense106");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense106");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense107");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense107");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack108");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack108");
            if (isBought)
            {
                // x4 attack
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroClone109");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroClone109");
            if (isBought)
            {
                // The hero is fast enough to be at 8 places at the same time!
                // Do something here
                setNbOfClone(8);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack110");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack110");
            if (isBought)
            {
                // x2 attack!
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense111");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense111");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense112");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense112");
            if (isBought)
            {
                // x2 defense
                // Do something here
                multiplyDefense(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack113");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack113");
            if (isBought)
            {
                // x2 attack
                // Do something here
                multiplyAttack(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack114");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack114");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyAttack(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack115");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack115");
            if (isBought)
            {
                // x3 attack
                // Do something here
                multiplyAttack(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroAttack116");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroAttack116");
            if (isBought)
            {
                // x4 attack!
                // Do something here
                multiplyAttack(4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense117");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense117");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense118");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense118");
            if (isBought)
            {
                // Better defense
                // Do something here
                multiplyDefense(1.1f);
            }
        }

        hasKey = LocalSave.HasBoolKey("HeroDefense119");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("HeroDefense119");
            if (isBought)
            {
                // x3 defense
                // Do something here
                multiplyDefense(3f);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void deleteAllUpdates()
    {
        LocalSave.DeleteKey("HeroAttack");
        LocalSave.DeleteKey("HeroDefense1");
        LocalSave.DeleteKey("HeroAttack2");
        LocalSave.DeleteKey("HeroDefense1");
        LocalSave.DeleteKey("HeroAttack3");
        LocalSave.DeleteKey("HeroDefense3");
        LocalSave.DeleteKey("HeroAttack4");
        LocalSave.DeleteKey("HeroDefense4");
        LocalSave.DeleteKey("HeroAttack5");
        LocalSave.DeleteKey("HeroAttack6");
        LocalSave.DeleteKey("HeroDefense5");
        LocalSave.DeleteKey("HeroAttack7");
        LocalSave.DeleteKey("HeroClone1");
        LocalSave.DeleteKey("HeroDefense6");
        LocalSave.DeleteKey("HeroAttack8");
        LocalSave.DeleteKey("HeroDefense17");
        LocalSave.DeleteKey("HeroAttack9");
        LocalSave.DeleteKey("HeroAttack16");
        LocalSave.DeleteKey("HeroAttack17");
        LocalSave.DeleteKey("HeroAttack18");
        LocalSave.DeleteKey("HeroDefense19");
        LocalSave.DeleteKey("HeroDefense20");
        LocalSave.DeleteKey("HeroDefense21");
        LocalSave.DeleteKey("HeroAttack22");
        LocalSave.DeleteKey("HeroDefense23");
        LocalSave.DeleteKey("HeroDefense24");
        LocalSave.DeleteKey("HeroAttack25");
        LocalSave.DeleteKey("HeroDefense26");
        LocalSave.DeleteKey("HeroAttack27");
        LocalSave.DeleteKey("HeroClone28");
        LocalSave.DeleteKey("HeroDefense29");
        LocalSave.DeleteKey("HeroAttack30");
        LocalSave.DeleteKey("HeroDefense31");
        LocalSave.DeleteKey("HeroAttack32");
        LocalSave.DeleteKey("HeroDefense33");
        LocalSave.DeleteKey("HeroDefense34");
        LocalSave.DeleteKey("HeroAttack35");
        LocalSave.DeleteKey("HeroDefense36");
        LocalSave.DeleteKey("HeroAttack37");
        LocalSave.DeleteKey("HeroClone38");
        LocalSave.DeleteKey("HeroDefense39");
        LocalSave.DeleteKey("HeroAttack40");
        LocalSave.DeleteKey("HeroDefense41");
        LocalSave.DeleteKey("HeroAttack42");
        LocalSave.DeleteKey("HeroAttack43");
        LocalSave.DeleteKey("HeroAttack44");
        LocalSave.DeleteKey("HeroClone45");
        LocalSave.DeleteKey("HeroDefense46");
        LocalSave.DeleteKey("HeroDefense47");
        LocalSave.DeleteKey("HeroDefense48");
        LocalSave.DeleteKey("HeroAttack49");
        LocalSave.DeleteKey("HeroAttack50");
        LocalSave.DeleteKey("HeroAttack51");
        LocalSave.DeleteKey("HeroDefense52");
        LocalSave.DeleteKey("HeroAttack53");
        LocalSave.DeleteKey("HeroDefense54");
        LocalSave.DeleteKey("HeroAttack55");
        LocalSave.DeleteKey("HeroAttack56");
        LocalSave.DeleteKey("HeroDefense57");
        LocalSave.DeleteKey("HeroDefense58");
        LocalSave.DeleteKey("HeroDefense59");
        LocalSave.DeleteKey("HeroDefense60");
        LocalSave.DeleteKey("HeroAttack61");
        LocalSave.DeleteKey("HeroDefense62");
        LocalSave.DeleteKey("HeroDefense63");
        LocalSave.DeleteKey("HeroDefense64");
        LocalSave.DeleteKey("HeroAttack65");
        LocalSave.DeleteKey("HeroAttack66");
        LocalSave.DeleteKey("HeroDefense67");
        LocalSave.DeleteKey("HeroDefense68");
        LocalSave.DeleteKey("HeroDefense69");
        LocalSave.DeleteKey("HeroDefense70");
        LocalSave.DeleteKey("HeroAttack71");
        LocalSave.DeleteKey("HeroAttack72");
        LocalSave.DeleteKey("HeroAttack73");
        LocalSave.DeleteKey("HeroDefense74");
        LocalSave.DeleteKey("HeroAttack75");
        LocalSave.DeleteKey("HeroDefense76");
        LocalSave.DeleteKey("HeroAttack77");
        LocalSave.DeleteKey("HeroAttack78");
        LocalSave.DeleteKey("HeroAttack79");
        LocalSave.DeleteKey("HeroDefense80");
        LocalSave.DeleteKey("HeroDefense81");
        LocalSave.DeleteKey("HeroAttack82");
        LocalSave.DeleteKey("HeroAttack83");
        LocalSave.DeleteKey("HeroDefense84");
        LocalSave.DeleteKey("HeroClone85");
        LocalSave.DeleteKey("HeroDefense86");
        LocalSave.DeleteKey("HeroDefense87");
        LocalSave.DeleteKey("HeroClone88");
        LocalSave.DeleteKey("HeroDefense89");
        LocalSave.DeleteKey("HeroDefense90");
        LocalSave.DeleteKey("HeroAttack91");
        LocalSave.DeleteKey("HeroAttack92");
        LocalSave.DeleteKey("HeroAttack93");
        LocalSave.DeleteKey("HeroDefense94");
        LocalSave.DeleteKey("HeroDefense95");
        LocalSave.DeleteKey("HeroAttack96");
        LocalSave.DeleteKey("HeroAttack97");
        LocalSave.DeleteKey("HeroClone98");
        LocalSave.DeleteKey("HeroDefense99");
        LocalSave.DeleteKey("HeroAttack100");
        LocalSave.DeleteKey("HeroAttack101");
        LocalSave.DeleteKey("HeroDefense102");
        LocalSave.DeleteKey("HeroAttack103");
        LocalSave.DeleteKey("HeroAttack104");
        LocalSave.DeleteKey("HeroAttack105");
        LocalSave.DeleteKey("HeroDefense106");
        LocalSave.DeleteKey("HeroDefense107");
        LocalSave.DeleteKey("HeroAttack108");
        LocalSave.DeleteKey("HeroClone109");
        LocalSave.DeleteKey("HeroAttack110");
        LocalSave.DeleteKey("HeroDefense111");
        LocalSave.DeleteKey("HeroDefense112");
        LocalSave.DeleteKey("HeroAttack113");
        LocalSave.DeleteKey("HeroAttack114");
        LocalSave.DeleteKey("HeroAttack115");
        LocalSave.DeleteKey("HeroAttack116");
        LocalSave.DeleteKey("HeroDefense117");
        LocalSave.DeleteKey("HeroDefense118");
        LocalSave.DeleteKey("HeroDefense119");

        getPurchases();

        setNbOfClone(0);
        setPwKameha(0);
        multiplyAttack(1);
        multiplyDefense(1);
        setRangedAtkRange(0);

        LocalSave.SetBool("KamehaAvailable", false);
    }
}
