using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Glasshouse building that will have Field created inside it and will allow to sow plants that are allowed via <see cref="SeedResourceProperties.CanPlantInGlassHouse"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class GlasshouseBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Passage Collider which will remove plants being grown inside its bounds
        /// </summary>
        [XmlIgnore]
        public BoxCollider PassageCollider;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (MaxAssignedStaffCount > 0)
            {
                Debug.LogWarning($"MaxAssignedStaffCount at {Name} should be 0, changing it");
                MaxAssignedStaffCount = 0;
            }

            if (PassageCollider == null)
            {
                Debug.LogError($"Glasshouse {Name} needs to have assigned PassageCollider, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}