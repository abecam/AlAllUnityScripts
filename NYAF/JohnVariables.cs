using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnVariables : MonoBehaviour
{
    // Defaults are for Unfit John
    public float strength = 400;
    public float speedMaxX = 0.99f;
    public float stepsForCrossing = 120; // Smaller is faster (how much is added to speed)
    public float speedMaxY = 0.04f;
    public float yJohnHeightJump = -3.93f;

    public float xJohnStartJump = 0.99f;
    public float yFloor = -4.05f;
}
