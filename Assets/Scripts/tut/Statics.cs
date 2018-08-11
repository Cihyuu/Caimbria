using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics {

    public static string horizontal = "horizontal";
    public static string vertical = "horizontal";
    public static string special = "horizontal";
    public static string specialType = "horizontal";
    public static string onLocomotion = "horizontal";
    public static string Horizontal = "horizontal";
    public static string Vertical = "horizontal";

    public static int GetAnimSpecialType(AnimSpecials i)
    {
        int r = 0;
        switch(i)
        {
            case AnimSpecials.runToStop:
                r = 11;
                break;
            default:
                break;
        }
        return r;
    }
}

public enum AnimSpecials
{
    run, runToStop, jump_idle, run_jump
}
