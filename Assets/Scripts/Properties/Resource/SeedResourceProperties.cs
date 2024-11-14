using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Seed resource are used during planting. Sown plant is referenced by <see cref="PlantName"/>, giving ability to add several SeedResources for one Plant.
    /// </summary>
	[Serializable]
	[XmlRoot(ElementName = "SeedResource")]
	public class SeedResourceProperties : ResourceProperties
	{
        /// <summary>
        /// Reference to plant, placed to ease editing.
        /// </summary>
        [XmlIgnore]
        public GameObject plant;

        /// <summary>
        /// Name of Sowing machine that will be used during sowing. It is stored inside seed resource as you might have different seeds and seedlings available for one plant. Those might have different prices, properties or productions.
        /// Available 
        /// </summary>
        public string SowingMachine;

        /// <summary>
        /// Defines if it is a seedling, which influences tooltips and animation during seeding
        /// </summary>
        public bool IsSeedling;

        /// <summary>
        /// Reference to plant that is sown
        /// </summary>
        public string PlantName;

        /// <summary>
        /// How much seeds are needed to seed per square meter. Use PlantEditor component to edit this value
        /// </summary>
        public float SeedAmountPerSqaureMeter = 1f;
        
        /// <summary>
        /// Defines if plant that is sown using this SeedResource can be planted in Glasshouse. Can be toggled either in inspector or via PlantEditor
        /// </summary>
        public bool CanPlantInGlassHouse = false;
    }
}