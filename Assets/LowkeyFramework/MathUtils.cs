using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float RecalculateAngleToBetween0And360(float angle)
    {
        float recalculatedAngle = angle % 360;
        recalculatedAngle = recalculatedAngle >= 0 ? recalculatedAngle : 360 + recalculatedAngle;

        return recalculatedAngle;
    }

    public static float RecalculateAngleToBetweenMinus180And180(float angle)
    {
        float recalculatedAngle = angle % 360;
        recalculatedAngle = recalculatedAngle > 180 ? recalculatedAngle - 360 : recalculatedAngle;
        recalculatedAngle = recalculatedAngle < -180 ? recalculatedAngle + 360 : recalculatedAngle;

        return recalculatedAngle;
    }
}
