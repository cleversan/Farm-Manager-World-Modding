using System.Collections.Generic;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Used ingame to indicate to game to add to gameObject Entrance of given type
    /// </summary>
    [ExecuteInEditMode]
    public class EntranceProperties : MonoBehaviour, ICopyTo
    {
        public Vector3 Forward;
        public ModEntranceType Type;
        public bool MoveForward;
        public Vector3 WaypointInside;

        public void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy)
        {
            EntranceProperties oldEntrance = oldObject as EntranceProperties;
            EntranceProperties newEntrance = newObject as EntranceProperties;

            newEntrance.Forward = oldEntrance.Forward;
            newEntrance.Type = oldEntrance.Type;
            newEntrance.MoveForward = oldEntrance.MoveForward;
            newEntrance.WaypointInside = oldEntrance.WaypointInside;
        }        
    }

    public enum ModEntranceType
    {
        Entrance,   // Non visible, non animated entrance for both - Staff and Vehicles (currently used in LargeGarage)
        Door,       // Animated doors for Staff (need opening/closing coroutine)
        Gate,       // Animated gates for Vehicles (need opening/closing coroutine)
        SlideUp,     // Animated sliding up gates for Vehicles (need opening/closing coroutine)
        EntranceForStaffAnim //Animated entrance for staff used in building animation
    }    
}