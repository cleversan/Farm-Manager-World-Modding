using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Apiary is building that produce <see cref="HoneyResourceProperties"/> through <see cref="HiveBuildingProperties"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class ApiaryBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Defines radius in meters in which Hive can be placed (1 tile - 4 meters)
        /// </summary>
        public float MaxDistanceToHive = 15;

        /// <summary>
        /// Defines max hive count that can be built and assigned to this apiary
        /// </summary>
        public int MaxHiveNumber = 10;

        /// <summary>
        /// Defines which Hive building will be used when placing. If left empty or no Hive with such name is found it will default to base game
        /// Hive
        /// </summary>
        public string HiveBuildingName;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (MaxAssignedStaffCount <= 0)
            {
                Debug.LogError($"MaxAssignedStaffCount at {Name} should beat least 1, validation failed");
                validation = false;
            }

            if (MaxResourcesCapacity <= 0)
            {
                Debug.LogError($"MaxResourcesCapacity at ApiaryBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            if (MaxProductionResourcesCapacity <= 0)
            {
                Debug.LogError($"MaxProductionResourcesCapacity at ApiaryBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            if (MaxHiveNumber <= 0)
            {
                Debug.LogError($"Apiary Building {Name} needs to have 1 as MaxHiveNumber, validation failed");
                validation = false;
            }

            if (MaxDistanceToHive <= 0)
            {
                Debug.LogError($"Apiary Building {Name} cannot have 0 as MaxDistanceToHive, raise it, validation failed");
                validation = false;
            }
            else if (MaxDistanceToHive <= 15)
            {
                Debug.LogWarning($"Apiary Building {Name} has MaxDistanceToHive lower then 15, the default one");
            }

            if (string.IsNullOrEmpty(HiveBuildingName))
                Debug.Log($"Apiary Building {Name} does not have assigned Hive Building Name, default ingame one will be assigned");


            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}