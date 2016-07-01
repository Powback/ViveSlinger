using System;
using UnityEngine;
using System.Collections;

public static class Util  {

    public static bool AlmostEquals(double double1, double double2, double precision)
    {
        return (Math.Abs(double1 - double2) <= precision);
    }
}
