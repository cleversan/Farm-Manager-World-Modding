using System;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// General properties for animals that will live in <see cref="AnimalsBuildingProperties"/>
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "Animal")]
	public class AnimalProperties : Properties
	{
        /// <summary>
        /// Defines daily chance for animal to become sick based on formula: SicknessChance * (1 - FeedQuality * 0.5f - StaffFactor * 0.5f),
		/// where FeedQuality is quality of currently provided food (summed <see cref="ResourceProperties.FoodQuality"/>) and StaffFactor is defined in another formula:
        /// MeanStaffSkill * 0.5f + AssignedStaffCount / <see cref="BuildingProperties.MaxAssignedStaffCount"/>
        /// </summary>
        [Header("General gameplay properties")]
		public float SicknessChance = 0.01f;

		/// <summary>
		/// Defines what kind of animation will Staff play during treatment
		/// <list type="bullet">
		/// <item>S, M  - crouching</item>
		/// <item>L, XL - standing</item>
		/// </list>
		/// </summary>
		public AnimalSize AnimalSize;

		/// <summary>
		/// Represents minimum and maximum reproduction for animals inside building. 
		/// </summary>
		public Vector2 ReproductionPerYear;

		/// <summary>
		/// Reference by name to AnimalAsResource
		/// </summary>
		public string AsResourceName;

		/// <summary>
		/// Defines when Animal becomes adult. 1 means 1 year, 2 means 2 years etc. To get value in days divide Days/365
		/// </summary>
		public float ProductionAge;

        /// <summary>
        /// Defines Animal when animal dies from old age. 1 means 1 year, 2 means 2 years etc. To get value in days divide Days/365
        /// </summary>
        public float MaxAge;

        /// <summary>
        /// Defines if animal should be visible in world. For instance, fish are not visible (have IsHidden set to true)
        /// </summary>
        public bool IsHidden;

		/// <summary>
		/// Array of resources that are food for this animal. See also <seealso cref="ResourceProperties.FoodQuality"/>
		/// </summary>
		[XmlArray]
		[XmlArrayItem(ElementName = "AllowedFood")]
		public List<string> AllowedFood;

		/// <summary>
		/// How much food will adult animal consume daily
		/// </summary>
		public float MaxFoodAmountPerFeed;

		/// <summary>
		/// List of daily production
		/// </summary>
		public List<ResourceAnimalProduction> Production;

		/// <summary>
		/// Price for adult animal in the shop. Young animals have price modifier of 50% i.e. Adult Animal costing 2500$ and young one will cost 1250$
		/// </summary>
		public float BasePrice;

		/// <summary>
		/// Defines how long will animal spent when it wanders. It is Vector2 as random value will be picked from range it defines
		/// </summary>
		[Header("Animations, Wandering and Idle properties")]
		public Vector2 WanderTime;

		/// <summary>
		/// Distance that animal will go when wandering
		/// </summary>
		public float WalkingDistance;

		/// <summary>
		/// Base speed of animal when wandering
		/// </summary>
		public float WalkingSpeedBaseValue;

		/// <summary>
		/// Percentage chance for animal to start wandering
		/// </summary>
		public float WalkingChance;

		/// <summary>
		/// Radius of the animal and how much world space it will take. Purely cosmetic thing
		/// </summary>
		public float Radius;

		/// <summary>
		/// List of possible SFX that animal will play randomly
		/// </summary>
		[Header("Sounds")]
		[XmlIgnore]
		public List<AudioClip> SoundFX;

		/// <summary>
		/// Volume on the AudioSource assigned at animal
		/// </summary>
		public float Volume;

		/// <summary>
		/// Distance from which sound will be heard by player
		/// </summary>
		public float SoundDistance;

		/// <summary>
		/// How many Idle animations are assigned to this animal in adult stage.
		/// </summary>
        public int AdultIdleAnimationCount = -1;

        /// <summary>
        /// How many Idle animations are assigned to this animal in young stage.
        /// </summary>
        public int YoungIdleAnimationCount = -1;

        public override bool ValidateProperties()
        {
			bool validation = true;
			if (string.IsNullOrEmpty(AsResourceName))
			{
				Debug.LogError($"AsResourceName at Animal {Name} cannot be empty, it needs to reference AnimalAsResource object by its name, validation failed");
                validation = false;
			}

			if (AllowedFood.Count <= 0)
			{
				Debug.LogError($"AllowedFood list at Animal {Name} cannot be empty, validation failed");
                validation = false;
			}

			for (int i = 0; i < AllowedFood.Count; ++i)
			{ 
				if (string.IsNullOrEmpty(AllowedFood[i]))
                {
					Debug.LogError($"AllowedFood at index {i} at Animal {Name} is empty, either remove it or fill it, validation failed");
					validation = false;
                }
			}

			if (MaxFoodAmountPerFeed <= 0.0f)
            {
				Debug.LogError($"MaxFoodAmountPerFeed at Animal {Name} is lower or equal to 0, validation failed");
                validation = false;
            }

			if (Production.Count <= 0)
			{
				Debug.LogError($"Production list at Animal {Name} cannot be empty, validation failed");
                validation = false;
			}

			if (ProductionAge >= MaxAge)
			{
                Debug.LogError($"ProductionAge ({ProductionAge}) is greater then MaxAge({MaxAge}) at {Name}, animals will die when they become an adult, validation failed");
                validation = false;
            }

			if (SicknessChance > 0.2)
			{
				Debug.Log($"SicknessChance was greater then 0.2 or 20% ({SicknessChance} or {(int)(SicknessChance * 100)}%)");
				SicknessChance = Mathf.Clamp(SicknessChance, 0, 0.2f);
			}

			for (int i = 0; i < Production.Count; ++i)
			{
				if (string.IsNullOrEmpty(Production[i].Resource))
				{
					Debug.LogError($"Produced resource at index {i} at Animal {Name} is not assigned, validation failed");
					validation = false;
				}
				
				if (Production[i].AmountPerDay.x < 0.0f || Production[i].AmountPerDay.y < 0.0f)
                {
					Debug.LogError($"AmountPerDay production at index {i} at Animal {Name} is not assigned, validation failed");
                    validation = false;
				}
			}

			// moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
			bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }

	public enum AnimalSize { S, M, L, XL }

	/// <summary>
	/// Struct defining animal production
	/// </summary>
	[Serializable]
	public struct ResourceAnimalProduction
	{
		/// <summary>
		/// How much resource will be produced per day as it takes random value from it which is then multiplied depending on difficulty setting:
		/// <list type="bullet">
		/// <item>Easy		- 120%</item>
		/// <item>Normal	- 100%</item>
		/// <item>Hard		- 85%</item>
		/// <item>Extreme	- 70%</item>
		/// </list>
		/// </summary>
		public Vector2 AmountPerDay;

		/// <summary>
		/// Name of the resource <see cref="Properties.Name"/>
		/// </summary>
		public string Resource;

		/// <summary>
		/// Defines if animal need pair to produce this resource
		/// </summary>
		public bool NeedPair;
	}	
}
