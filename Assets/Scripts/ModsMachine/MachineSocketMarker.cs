using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;

namespace FarmManagerWorld.Utils
{
    /// <summary>
    /// Defines where ingame machine will be attached to Vehicle
    /// </summary>
    public class MachineSocketMarker : MonoBehaviour
    {
        public MachineBaseMod Parent;
        public MachineBaseMod AttachedMachine;
        public bool IsFrontSocket = false;
        public Vector3 ForwardVelocity;
    }
}