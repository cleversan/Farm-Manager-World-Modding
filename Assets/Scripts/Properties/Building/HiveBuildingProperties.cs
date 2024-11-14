using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Hive building produces <see cref="HoneyResourceProperties"/> based on Fields located in <see cref="MaxDistanceToPlants"/> and <see cref="HoneyPerHarvest"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class HiveBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Particle emmiter that represents bees, not required for building to work
        /// </summary>
        [XmlIgnore]
        public ParticleSystem Emitter;

        /// <summary>
        /// Point for employee that they will approach to gather honey
        /// </summary>
        [XmlIgnore]
        public GameObject HiveFeedingLocation;

        /// <summary>
        /// Max distance to plants to check for which type of Honey is being produced
        /// </summary>
        public float MaxDistanceToPlants = 75;

        /// <summary>
        /// Honey produced per harvest
        /// </summary>
        public float HoneyPerHarvest = 1f;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (MaxAssignedStaffCount > 0)
            {
                Debug.LogWarning($"MaxAssignedStaffCount at {Name} should be 0, changing it");
                MaxAssignedStaffCount = 0;
            }  

            if (HiveFeedingLocation == null)
            {
                Debug.LogError($"Hive Feeding location at HiveBuilding {Name} cannot be null, assign object to it, validation failed");
                validation = false;

            }
            else if (HiveFeedingLocation.name != "HiveFeedingLocation")
            {
                Debug.LogWarning($"Changing name of HiveFeedingLocation from {HiveFeedingLocation.name} to HiveFeedingLocation at HiveBuilding {Name}");
                HiveFeedingLocation.name = "HiveFeedingLocation";
            }

            if (Emitter == null)
            {
                Debug.LogWarning($"Emmiter is null, it is not required, hovewer if recommended you do for visuals at HiveBuilding {Name}");
            }
            else if (Emitter.name != "ParticleSystemEmitter")
            {
                Debug.LogWarning($"Changing name of Emmiter from {Emitter.name} to ParticleSystemEmitter at HiveBuilding {Name}");
                Emitter.name = "ParticleSystemEmitter";
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}