
/**
* Get the current stats from what was bought
*/
public class MMPGHeroStrength
{
    ManageNYPB.Unit heroUnit = new ManageNYPB.Unit();

    public int nbClones = 0; // THe hero is fast enough to "clone" himself.
    public bool hasKameha = false;
    public bool hasConversion = false;
    public bool hasMegaPunch = false;
    public float strongerWhenHitBy = 1; // Can get stronger by this number each time he is hit
    public float strongerWhenHitting = 1; // Can get stronger by this number each time he hits
    public float betterDefendedWhenHit = 1; // Can get better defended by this number each time he is hit

    public enum typeOfHero
    {
        mainHero, support1, support2
    };

    typeOfHero ourType = typeOfHero.mainHero;

    public MMPGHeroStrength()
    {
        heroUnit.defense = 30;
        heroUnit.fightRange = 1;

        heroUnit.defense = 1;

        heroUnit.strength = 1;
        heroUnit.stamina = 2000;
        heroUnit.staminaMax = 2000;
        heroUnit.health = 160;
    }

    public MMPGHeroStrength(typeOfHero anotherType)
    {
        switch (anotherType)
        {
            case typeOfHero.mainHero:
                heroUnit.defense = 30;
                heroUnit.fightRange = 1;

                heroUnit.defense = 30;

                heroUnit.strength = 0;
                heroUnit.stamina = 100;
                heroUnit.staminaMax = 100;
                heroUnit.health = 160;
                break;

            case typeOfHero.support1:
                heroUnit.defense = 30;
                heroUnit.fightRange = 1;

                heroUnit.defense = 30;

                heroUnit.strength = 50;
                heroUnit.stamina = 200;
                heroUnit.staminaMax = 300;
                heroUnit.health = 160;
                break;

            case typeOfHero.support2:
                heroUnit.defense = 10;
                heroUnit.fightRange = 4;

                heroUnit.defense = 30;

                heroUnit.strength = 100;
                heroUnit.stamina = 300;
                heroUnit.staminaMax = 300;
                heroUnit.health = 160;
                break;

            default:
                heroUnit.defense = 30;
                heroUnit.fightRange = 1;

                heroUnit.defense = 30;

                heroUnit.strength = 0;
                heroUnit.stamina = 100;
                heroUnit.staminaMax = 100;
                heroUnit.health = 160;
                break;
        }
    }

    public ManageNYPB.Unit getLatestHero()
    {
        return heroUnit;
    }
}
