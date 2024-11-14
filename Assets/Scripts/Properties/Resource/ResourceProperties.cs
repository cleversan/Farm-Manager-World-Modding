using FarmManagerWorld.Utils;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static FarmManagerWorld.Static.StaticInformation;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Base Resource Properties class. Every resource available in game inherits from this class.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "Resource")]
	public abstract class ResourceProperties : Properties
	{
		/// <summary>
		/// Sprite of the resource visible in the UI
		/// </summary>
		[XmlIgnore]
		public Sprite Sprite;

		/// <summary>
		/// What unit should be used when displaying amount of the given resource
		/// </summary>
		public UnitOfMeasure Unit;

		/// <summary>
		/// Base amount which will be initially added to Cart when buying or increased/decreased when shift+clicked on +/-
		/// </summary>
		public float BaseMarketAmount = 100.0f;

		/// <summary>
		/// Type of the resource
		/// </summary>
		public ResourceModType Type;

		/// <summary>
		/// List of Region modifiers which influence initial price depending on its region. If left empty it will treat it as 1
		/// </summary>
		public List<RegionModifier> Regions = new List<RegionModifier>();

		/// <summary>
		/// Expiration date of the product expressed in days
		/// </summary>
		public float ExpirationDate = 1000;

		/// <summary>
		/// Initial sell price of the product. Buy price is dependent on difficulty:
		/// <list>
		/// <item>Easy		105% </item>
		/// <item>Normal	130% </item>
		/// <item>Hard		170% </item>
		/// <item>Extreme	220% </item>
		/// </list>
		/// Subject to change in the future
		/// </summary>
		public float InitialBasePrice = 8;

		/// <summary>
		/// Max percentage growth change of the resource (capped to 0.5%). Increase it to allow more volatile changes
		/// </summary>
		public float MaxPercentageDailyPriceGrowth = 0.005f;

        /// <summary>
        /// Max percentage drop change of the resource (capped to 0.5%). Increase it to allow more volatile changes 
        /// </summary>
        public float MaxPercentageDailyPriceDrop = -0.005f;

		/// <summary>
		/// Tag of the transporting tool that will be used by staff to transport given resource. Available tags:
		/// <list>
		/// <item>bag</item>
		/// <item>bucket</item>
		/// <item>kisten</item>
		/// <item>watercan</item>
		/// </list>
		/// </summary>
		public string TransportToolTag;

		/// <summary>
		/// Describes what kind of storage warehouse needs to have in order to store inside it given resource
		/// </summary>
		public ResourceType StorageType;

		/// <summary>
		/// Animal food quality this resource will provide. Default value 0.3 equals to 30%
		/// </summary>
		public float FoodQuality = 0.3f;

		public override bool ValidateProperties()
        {
			bool validation = true;
			if (Sprite == null)
            {
				Debug.LogError($"Sprite at Resource {Name} is null, validation failed");
                validation = false;
			}

			if (BaseMarketAmount <= 0)
			{
				Debug.LogError($"BaseMarketAmount at Resource {Name} is lower or equal to 0, validation failed");
                validation = false;
			}

			if (string.IsNullOrEmpty(TransportToolTag))
			{
				Debug.LogWarning($"TransportToolTag at Resource {Name} is empty, default one will be used");
			}

			if (FoodQuality <= 0)
			{
				Debug.LogError($"FoodQuality at Resource {Name} is lower or equal to 0, validation failed");
                validation = false;
			}
			else
			{
                FoodQuality = Mathf.Clamp01(FoodQuality);
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }

    }

	/// <summary>
	/// Type of resource
	/// </summary>
	public enum ResourceModType
	{
		AnimalsAsResources, AnimalResources, ChemicalResources, FertilizingResources, FoliageResources, SeedResources, HoneyResource, OtherResource, ProductionResource
	}

	/// <summary>
	/// Unit of measure that defines how resource amount will be displayed ingame
	/// </summary>
	[Serializable]
	public enum UnitOfMeasure
	{
		/// <summary>
		/// Fallback, same as Amount
		/// </summary>
		Default, 
		/// <summary>
		/// Weight measured in kilograms
		/// </summary>
		Weight, 
		/// <summary>
		/// Volume measured in liters
		/// </summary>
		Volume, 
		/// <summary>
		/// Amount as item is countable i.e. eggs or cakes
		/// </summary>
		Amount
	}	

	/// <summary>
	/// Type of the resource that determines where it can be stored
	/// </summary>

	[Serializable]
	public enum ResourceType
	{
        Meal,
        Eggs,
        Flour,
        Meats,
        Honey,
        Milk,
        FrozenFood,
        SeedsAndPlants,
        Fertilizer,
        Manure,
        Oil,
        Peanut,
        Fruit,
        Fuel,
        Fodder,
        OtherProducts,
        DairyProducts,
        VegetableProducts,
        FruitProducts,
        OtherPlant,
        OilPlant,
        Fish,
        PlantProtProduct, // plant protection product
        Vegetable,
        Wool,
        SmokedMeat,
        BakedGoods,
        Cereals
    }
}