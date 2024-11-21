using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Animals represented as resources when bought or moved to Slaughterhouse.
	/// </summary>
    [Serializable]
	[XmlRoot(ElementName = "AnimalAsResource")]
	public class AnimalAsResourceProperties : ResourceProperties
	{
		/// <summary>
		/// Sprite of young animal variant
		/// </summary>
		[XmlIgnore]
		public Sprite YoungSprite;

        /// <summary>
        /// Defines if ProductionProperties should be autopatched into Slaughterhouse (if so, you need to define resource name that will be produced from Animal)
        /// </summary>
        public bool EnableAutoPatching = true;

        /// <summary>
        /// Defines production values for this seed resource in Plant Factory that will be autopatched.
        /// </summary>
        public ProductionProperties SlaughterhouseProductionProperties;

        public override bool ValidateProperties()
        {
            bool validation = true;

            if (EnableAutoPatching)
            {
                if (SlaughterhouseProductionProperties != null)
                {
                    SlaughterhouseProductionProperties.ResourceNeededName = Name;                   
                    
                    if (string.IsNullOrEmpty(SlaughterhouseProductionProperties.ResourceProducedName))
                    {
                        Debug.LogError($"Validation failed, SlaughterhouseProductionProperties need to have ResourceProducedAmount greater then 0 in AnimalAsResource {Name}");
                        validation = false;
                    }

                    if (SlaughterhouseProductionProperties.ResourceProducedAmount <= 0)
                    {
                        Debug.LogError($"Validation failed, SlaughterhouseProductionProperties need to have ResourceProducedAmount greater then 0 in AnimalAsResource {Name}");
                        validation = false;
                    }

                    if (SlaughterhouseProductionProperties.ResourceAmountNeeded <= 0)
                    {
                        Debug.LogError($"Validation failed, SlaughterhouseProductionProperties need to have ResourceAmountNeeded greater then 0 in AnimalAsResource {Name}");
                        validation = false;
                    }

                    if (SlaughterhouseProductionProperties.ProductionLengthMultiplier <= 0)
                    {
                        Debug.LogError($"Validation failed, SlaughterhouseProductionProperties need to have ProductionLengthMultiplier greater then 0 in AnimalAsResource {Name}");
                        validation = false;
                    }
                }
                else
                {
                    Debug.LogError($"Validation failed, SlaughterhouseProductionProperties are null in AnimalAsResource {Name}");
                    validation = false;
                }
            }
            else
                Debug.LogWarning($"Warning, autopatching disabled in AnimalAsResource {Name}");

            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}