using System;
using System.Xml.Serialization;
using FarmManagerWorld.Editors;
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

        /// <summary>
        /// Defines if ProductionProperties should be autopatched into Plant Factory
        /// </summary>
        public bool EnableAutoPatching = true;

        /// <summary>
        /// Defines production values for this seed resource in Plant Factory that will be autopatched.
        /// </summary>
        public ProductionProperties SeedProductionProperties;

        public override bool ValidateProperties()
        {
            bool validation = true;

            if (EnableAutoPatching)
            {
                if (SeedProductionProperties != null)
                {
                    SeedProductionProperties.ResourceProducedName = Name;

                    if (plant != null && plant.TryGetComponent(out PlantEditor editor))
                    {
                        SeedProductionProperties.ResourceNeededName = editor.foliageResourceMod.Resource.Name;

                        if (SeedProductionProperties.ResourceProducedAmount <= 0)
                        {
                            Debug.LogError($"Validation failed, SeedProductionProperties need to have ResourceProducedAmount greater then 0 in SeedResource {Name}");
                            validation = false;
                        }

                        if (SeedProductionProperties.ResourceAmountNeeded <= 0)
                        {
                            Debug.LogError($"Validation failed, SeedProductionProperties need to have ResourceAmountNeeded greater then 0 in SeedResource {Name}");
                            validation = false;
                        }

                        if (SeedProductionProperties.ProductionLengthMultiplier <= 0)
                        {
                            Debug.LogError($"Validation failed, SeedProductionProperties need to have ProductionLengthMultiplier greater then 0 in SeedResource {Name}");
                            validation = false;
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Validation failed, SeedProductionProperties are null in SeedResource {Name}");
                    validation = false;
                }
            }
            else
                Debug.LogWarning($"Warning, autopatching disabled in SeedResource {Name}");

            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}