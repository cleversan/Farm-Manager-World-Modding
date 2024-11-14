using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Building where stored animals produce resources based on <see cref="AnimalProperties.Production"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class AnimalsBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Array of animals that can live inside this building by <see cref="Properties.Name"/>. Please refer to documentation when introducing animals from base game
        /// </summary>
        public string[] PossibleAnimals;

        /// <summary>
        /// Max count of animals that can live inside this building.
        /// </summary>
        public int MaxAnimalsCount;

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
                Debug.LogError($"MaxResourcesCapacity at AnimalsBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            if (MaxProductionResourcesCapacity <= 0)
            {
                Debug.LogError($"MaxProductionResourcesCapacity at AnimalsBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            if (PossibleAnimals.Length <= 0)
            {
                Debug.LogError($"Animals Buildings {Name} need to have at least one animal type that can be assigned, validation failed");
                validation = false;
            }

            if (MaxAnimalsCount <= 0)
            {
                Debug.LogError($"Animals Buildings {Name} need to have space for at least one animal, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}