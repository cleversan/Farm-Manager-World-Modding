using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Irrigation building supplies or removes water from fields depending on settings in <see cref="BuildingProperties.RangeRadius"/> 
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class IrrigationBuildingProperties : BuildingProperties
    {
        public override bool ValidateProperties()
        {
            if (MaxAssignedStaffCount > 0)
            {
                Debug.LogWarning($"MaxAssignedStaffCount at {Name} should be 0, changing it");
                MaxAssignedStaffCount = 0;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation;
        }
    }
}