using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Staff building where employees live. Place <see cref="DecoBuildingProperties"/> around it to increase their happiness
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public class StaffBuildingProperties : BuildingProperties
    {
        /// <summary>
        /// Determines if building houses seasonal staff
        /// </summary>
        public bool IsSeasonalBuilding;

        /// <summary>
        /// Represents how many employees can live in it
        /// </summary>
        public int MaxHireStaffNumber = 12;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (MaxHireStaffNumber < 0)
            {
                Debug.LogError($"MaxHireStaffNumber cannot be lower then 1 at StaffBuilding {Name}, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}