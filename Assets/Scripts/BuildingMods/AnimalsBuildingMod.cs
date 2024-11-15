using UnityEngine;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// AnimalsBuildingMod component that is attached to GameObject
    /// </summary>
    public class AnimalsBuildingMod : BuildingMod
    {
        public AnimalsBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (AnimalsBuildingProperties)value; }

        public override bool Validate()
        {
            bool validation = true;
            if (gameObject.FindChildrenWithNameContains(false, true, "spawn").Count <= 0)
            {
                Debug.LogError($"AnimalBuilding {building.Name} has no animal spawn points but require it, validation failed");
                validation = false;
            }

            return base.Validate() && validation;
        }
    }
}