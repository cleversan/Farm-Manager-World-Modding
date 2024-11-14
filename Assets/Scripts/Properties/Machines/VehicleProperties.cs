using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Vehicles are machines that use other machines during field work, that are defined in <see cref="MachineProperties.MachineTag"/>. Vehicles also need to be refueled as they consume fuel.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Vehicle")]
    public class VehicleProperties : MachineProperties
    {
        /// <summary>
        /// Power of the Vehicle that is then measured when searching for machine when selecting one for field work. Power needs to be higher or equal then <see cref="MachineProperties.RequiredPower"/> to be considered for usage. 
        /// If value is higher or equal to 500, this Vehicle will require "High-Power Agricultural Machinery" training
        /// </summary>
        [Header("If value is higher or equal to 500, this Vehicle will require \n\"High-Power Agricultural Machinery\" training")]
        public float Power = 650;

        /// <summary>
        /// Speed of the Vehicle
        /// </summary>
        public float MaxSpeed = 50;

        /// <summary>
        /// Should staff be hidden when entering the vehicle
        /// </summary>
        public bool HideStaffWhenDriving = true;

        public override bool ValidateProperties()
        {
            bool validate = true;
            if (Power <= 0)
            {
                Debug.LogError($"Power of Vehicle {Name} is lower or equal to 0, validation failed");
                validate = false;
            }

            if (MaxSpeed <= 0)
            {
                Debug.LogError($"Power of MaxSpeed {Name} is lower or equal to 0, validation failed");
                validate = false;
            }

            bool baseValidation = base.ValidateProperties();
            return baseValidation && validate;
        }
    }
}