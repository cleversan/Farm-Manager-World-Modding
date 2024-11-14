using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Warehouse building stores Resources based on <see cref="ResourceProperties.StorageType"/>. If <see cref="BuildingProperties.StoredResourceType"/> contains it, that resource can be stored. Capacity is described in <see cref="BuildingProperties.MaxResourcesCapacity"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class WarehouseBuildingProperties : BuildingProperties
    {
        public override bool ValidateProperties()
        {
            bool validation = true;
            if (MaxAssignedStaffCount > 0)
            {
                Debug.LogWarning($"MaxAssignedStaffCount at WarehouseBuilding {Name} should be 0, changing it");
                MaxAssignedStaffCount = 0;
            }

            if (StoredResourceType.Count <= 0)
            {
                Debug.LogError($"StoredResourceType array at WarehouseBuilding {Name} cannot be empty, validation failed");
                validation = false;
            }

            if (MaxResourcesCapacity <= 0)
            {
                Debug.LogError($"MaxResourcesCapacity at WarehouseBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}