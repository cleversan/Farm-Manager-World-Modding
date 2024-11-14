using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RoadconnectorCandidate : MonoBehaviour
{
    public void CheckIfValid(Collider requiredCol, Collider forbiddenCol)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0f);
        bool hasRequired = false;
        foreach (Collider collider in colliders)
        {
            if(collider == forbiddenCol)
            {
                DestroyImmediate(gameObject);
            }

            if (collider == requiredCol)
                hasRequired = true;
        }

        if (!hasRequired)        
            DestroyImmediate(gameObject);        
    }
}
