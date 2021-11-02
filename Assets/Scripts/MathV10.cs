// Math v1.0
// -------------
// November 1st, 2021
// Matthew Doucette, Xona Games
// http://xona.com/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathV10
{
    public static float RandomVariance(float value, float maxRandomVariance)
    {
        // e.g. if value = 100 and maxRandomVariance = 5% = 0.05 = 5 (of 100) then return 95..105
        float randomVariance = value * maxRandomVariance;
        float minValue = value - randomVariance;
        float maxValue = value + randomVariance;
        return Random.Range(minValue, maxValue);
    }
}
