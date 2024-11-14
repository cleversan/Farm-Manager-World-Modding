using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Decorational building that increase happiness of employees if placed next to <see cref="StaffBuildingProperties"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class DecoBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Offset of the building if snapping is enabled (i.e. fences in base game)
        /// </summary>
        public Vector3 Offset;

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