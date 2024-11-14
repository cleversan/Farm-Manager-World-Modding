using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Garage building that holds <see cref="MachineProperties"/> based on ParkingSpaceCollider that is assigned to object by <see cref="BuildingEditor.ParkingSpaceContainer"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class GarageBuildingProperties : BuildingProperties
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