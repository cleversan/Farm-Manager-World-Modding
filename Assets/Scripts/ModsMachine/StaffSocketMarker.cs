using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmManagerWorld.Utils
{
    /// <summary>
    /// Defines GameObject where Staff will be placed during work
    /// </summary>
    public class StaffSocketMarker : MonoBehaviour
    {
        public StaffSocketType Type;
    }

    /// <summary>
    /// Defines if Staff will be sitting or standing while working on machine
    /// </summary>
    public enum StaffSocketType
    {
        Standing,
        Sitting
    }
}