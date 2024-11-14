using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Produces <see cref="ProductionResourceProperties"/> that are specified in <see cref="AvailableProduction"/> based on Assigned Staff count and their skill
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class ProductionBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Array of machines and vehicles that will be spawned inside the building if it contains ParkingSpaceCollider
        /// </summary>
        public string[] MachinesAndVehicles;

        /// <summary>
        /// List of available productions
        /// </summary>
        public ProductionProperties[] AvailableProduction;

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
                Debug.LogError($"MaxResourcesCapacity at ProductionBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            if (MaxProductionResourcesCapacity <= 0)
            {
                Debug.LogError($"MaxProductionResourcesCapacity at ProductionBuilding {Name} cannot be less or equal 0, validation failed");
                validation = false;
            }

            for (int i = 0; i < MachinesAndVehicles.Length; ++i)
            {
                if (string.IsNullOrEmpty(MachinesAndVehicles[i]))
                {
                    Debug.LogError($"Machine/Vehicle name at index {i} at ProductionBuilding {Name} cannot be empty, validation failed");
                    validation = false;
                }
            }

            if (AvailableProduction.Length <= 0)
            {
                Debug.LogError($"ProductionBuilding {Name} needs to have at least one Production to function, validation failed");
                validation = false;
            }

            for (int i = 0; i < AvailableProduction.Length; ++i)
            {
                if (string.IsNullOrEmpty(AvailableProduction[i].ResourceNeededName))
                {
                    Debug.LogError($"Resource needed name at index {i} cannot be empty at ProductionBuilding {Name}, validation failed");
                    validation = false;
                }

                if (string.IsNullOrEmpty(AvailableProduction[i].ResourceProducedName))
                {
                    Debug.LogError($"Resource produced name at index {i} cannot be empty at ProductionBuilding {Name}, validation failed");
                    validation = false;
                }

                if (AvailableProduction[i].ResourceAmountNeeded < 1.0f)
                {
                    Debug.LogError($"Resource amount needed at index {i} cannot be less then 1.0f at ProductionBuilding {Name}, validation failed");
                    validation = false;
                }

                if (AvailableProduction[i].ResourceProducedAmount < 1.0f)
                {
                    Debug.LogError($"Resource produced amount at index {i} cannot be less then 1.0f at ProductionBuilding {Name}, validation failed");
                    validation = false;
                }

                if (AvailableProduction[i].ProductionLengthMultiplier <= 0.0f)
                {
                    Debug.LogError($"Production length modifier at index {i} cannot be less or equal to 0 at ProductionBuilding {Name}, validation failed");
                    validation = false;
                }
            }


            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }

    /// <summary>
    /// Single production entry that specifies what is produced, how much and for how long
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Production")]
    public class ProductionProperties
    {
        /// <summary>
        /// Base ingredient needed to produce resource
        /// </summary>
        public string ResourceNeededName;

        /// <summary>
        /// Amount needed to complete production
        /// </summary>
        public float ResourceAmountNeeded;

        /// <summary>
        /// Name of the resource that is produced
        /// </summary>
        public string ResourceProducedName;

        /// <summary>
        /// Amount produced of given resource
        /// </summary>
        public float ResourceProducedAmount;

        /// <summary>
        /// Production multiplier that modifies duration of the production. Default duration in days if Building is fully staffed with employees with no skill is 3 Days
        /// </summary>
        public float ProductionLengthMultiplier = 1.0f;
    }
}