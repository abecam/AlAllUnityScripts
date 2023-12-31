

using System.Collections.Generic;
using System.Numerics;

public class PadActionSet
{
    public enum button
    {
        a,
        b,
        x,
        y,
        start,
        back,
        rb,
        lb,
        cross_up,
        cross_down,
        cross_right,
        cross_left,
        leftStickButton,
        rightStickButton
    }

    public enum axes
    {
        left_stick,
        right_stick,
        dpad
    }

    public enum floatValues
    {
        rt,
        lt
    }

    public bool aPressed = false;
    public bool bPressed = false;
    public bool xPressed = false;
    public bool yPressed = false;

    public bool rbPressed = false;
    public bool lbPressed = false;

    public float rtPressed = 0;
    public float ltPressed = 0;

    public bool startPressed = false;
    public bool backPressed = false;

    public Vector2 leftStick = new Vector2();
    public Vector2 rightStick = new Vector2();

    public Vector2 dPad = new Vector2();

    public bool cross_upPressed = false;
    public bool cross_downPressed = false;
    public bool cross_rightPressed = false;
    public bool cross_leftPressed = false;

    public bool leftStickPress = false;
    public bool rightStickPress = false;
}
