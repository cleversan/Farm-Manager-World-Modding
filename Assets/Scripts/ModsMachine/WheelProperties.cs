using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmManagerWorld.Utils
{
    /// <summary>
    /// Component that defines what is wheel in a machine
    /// </summary>
    public class WheelProperties : MonoBehaviour, ICopyTo
    {
        public float wheelRadius = 1.0f;

        public void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy)
        {
            WheelProperties oldWheel = oldObject as WheelProperties;
            WheelProperties newWheel = newObject as WheelProperties;
            newWheel.wheelRadius = oldWheel.wheelRadius;
        }

        public void Start()
        {
            if (wheelRadius == 0)
            {
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                wheelRadius = (((boxCollider.size.y + boxCollider.size.z) * (transform.localScale.y + transform.localScale.z) / 2.0f) / 2.0f) * transform.lossyScale.y;

                Destroy(boxCollider);
            }
        }

        public void RotateWheel(float rotateValue)
        {
            transform.Rotate(rotateValue / wheelRadius, 0, 0);
        }
    }
}