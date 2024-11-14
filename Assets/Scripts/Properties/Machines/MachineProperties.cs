using FarmManagerWorld.Utils;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Base machine properties class from which other will inherit. Plain machines need <see cref="VehicleProperties"/> that is defined in <see cref="VehicleTag"/> to be used.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Machine")]
    public class MachineProperties : Properties
    {
        /// <summary>
        /// Icon of the machine visible on the UI if machine has 100% durability. For values between 0% and 100%, icon is interpolated between both states.
        /// </summary>
        [XmlIgnore]
        public Sprite Icon;

        /// <summary>
        /// Icon of the machine visible on the UI if machine has 0% durability. For values between 0% and 100%, icon is interpolated between both states.
        /// </summary>
        [XmlIgnore]
        public Sprite IconDestroyed;

        /// <summary>
        /// Type of the machine and what role will it serve
        /// </summary>
        public MachineType MachineType;

        /// <summary>
        /// Defines what Vehicles can use this machine. Only used by Machines
        /// Those are stored as single string divided by comma sign ','
        /// </summary>
        public string VehicleTag = "";

        /// <summary>
        /// Defines what kind of machine this is and what rale will it serve, similar to <see cref="MachineType"/>. 
        /// It also allows adding new workflow that is not covered by MachineType. 
        /// </summary>
        public string MachineTag = "";

        /// <summary>
        /// Price of the machine in shop. If machine is taken into leasing, its monthly price will be calculated using this formula: Price * 1.1f / 24
        /// </summary>
        [Header("Price settings")]
        public int Price;

        /// <summary>
        /// How much fuel can this machine store. When refueling, resource is taken in 1 to 1 ratio - to refuel 50% out of default value of 10, 5 units of fuel (either bio or normal) will be used.
        /// </summary>
        [Header("Fuel settings")]
        public float MaxFuel = 10.0f;

        /// <summary>
        /// How much fuel will be consumed while moving 1 meter (default unity distance unit). Default value means per 1 kilometer it will use 0.05f units of fuel or 5 units per 100 km (really efficent eh?)
        /// Consumption is dependend on difficulty settings:
        /// <list type="bullet">
        /// <item>Easy      - 80% </item>
        /// <item>Normal    - 100%</item>
        /// <item>Hard      - 120%</item>
        /// <item>Extreme   - 150%</item>
        /// </list>
        /// </summary>
        [Header ("Fuel consumped per distance")]
        public float FuelConsumption = 0.00005f;

        /// <summary>
        /// How much fuel will be consumed during field work per 1 square meter. Default value mean per 1 ha (max size field) it will use 2.5 units of fuel. It is also affected by difficulty settings.
        /// <see cref="FuelConsumption"/>
        /// </summary>
        [Header("Fuel consumed per square meter")]
        public float FuelConsumptionOnField = 0.00025f;

        /// <summary>
        /// Defines how much can be stored inside machine. (Currently disabled during field works)
        /// </summary>
        [Header("Other settings")]
        public float Capacity;

        /// <summary>
        /// Defines how much power is required for Vehicle to use this machine.
        /// </summary>
        public float RequiredPower;

        /// <summary>
        /// Defines if after attaching to vehicle, this machine will have fixed position or will be "dragged" behind (combine headers vs chest trailer)
        /// </summary>
        public bool IsFixedJoint;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (Icon == null)
            {
                Debug.LogError($"Icon at Machine {Name} is null, validation failed");
                validation = false;
            }

            if (IconDestroyed == null)
            {
                Debug.LogError($"IconDestroyed at Machine {Name} is null, validation failed");
                validation = false;
            }

            if (this is not VehicleProperties)
            {
                if (string.IsNullOrEmpty(VehicleTag))
                {
                    Debug.LogError($"VehicleTag at Machine {Name} is empty, machines that are not vehicles require those to sign which vehicles can use this machines, validation failed");
                    validation = false;
                }
            }

            if (string.IsNullOrEmpty(MachineTag))
            {
                Debug.LogError($"MachineTag at Machine {Name} is empty, machines use this, validation failed");
                validation = false;
            }

            if (Price <= 0)
            {
                Debug.LogError($"Price at Machine {Name} is lower or equal then 0, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }

        /// <summary>
        /// Check if machine is not Vehicle
        /// </summary>
        public bool IsMachine()
        {
            return this is not VehicleProperties;
        }
    }

    /// <summary>
    /// Describes what role will machine fill and what properties will it have. See also <see cref="Static.StaticInformation.MachinesDictionary"/>
    /// </summary>
    public enum MachineType
    {
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        Tractor,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        Plow,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        Cultivator,
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        Combine,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        ChestTrailer,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        ManureSpreader,
        /// <summary>Properties - <seealso cref="SowingMachineProperties"/></summary>
        Seeder,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        CombineHeader,
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        DeliveryTruck,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        WaterCart,
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        PickUp,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        Sprayer,
        /// <summary>Properties - <seealso cref="SowingMachineProperties"/></summary>
        Planter,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        RoundBaler,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        TrailerForBalesOfStraw,
        /// <summary>Properties - <seealso cref="SowingMachineProperties"/></summary>
        TreePlanter,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        WindRower,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        AnimalTrailer,
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        FireTruck,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        Mower,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        SelfLoadingTrailer,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        ChopperCornMachine,
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        Reaper,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        PotatoHarvester,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        None,
        /// <summary>Properties - <seealso cref="MachineProperties"/></summary>
        ChopperHeader,
        /// <summary>Properties - <seealso cref="SowingMachineProperties"/></summary>
        PotatoPlanter,
        /// <summary>Properties - <seealso cref="SowingMachineProperties"/></summary>
        PointPlanter,
        /// <summary>Properties - <seealso cref="VehicleProperties"/></summary>
        CombineForWine 
    }
}