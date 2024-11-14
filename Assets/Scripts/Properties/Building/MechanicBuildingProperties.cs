using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Machine building sends mechanics to fix broken machines in <see cref="BuildingProperties.RangeRadius"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class MechanicBuildingProperties : BuildingProperties
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