using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Logistic building moves resources between it and <see cref="WarehouseBuildingProperties"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class LogisticBuildingProperties : BuildingProperties
    {
        public override bool ValidateProperties()
        {
            bool validation = true;
            if (MaxAssignedStaffCount <= 0)
            {
                Debug.LogError($"MaxAssignedStaffCount at {Name} should beat least 1, validation failed");
                return false;
            }

            if (MaxResourcesCapacity <= 0)
            {
                Debug.LogWarning($"MaxResourcesCapacity at LogisticBuilding {Name} is less or equal 0, was it intentional?");
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }

    }
}