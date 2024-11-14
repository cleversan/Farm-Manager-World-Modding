using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeCalculator 
{
    public static Vector3 ConfigureSize(Vector3 vector)
    {
        return new Vector3(make4((int)vector.x), make4((int)vector.y), make4((int)vector.z));
    }

    public static float make4(int i)
    {
        while (true)
        {
            if (i % 4 != 0)
            {
                i++;

            }
            else break;
        }
        return i;
    }
}
