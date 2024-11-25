using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    public class FireProperties : MonoBehaviour
    {
        public BuildingFireParticleType BuildingFireParticleType;
    }

    public enum BuildingFireParticleType
    {
        FireFromWalls, FireFromRoofs
    }
}