using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static FarmManagerWorld.Static.StaticInformation;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Plant properties class, containing information about graphics and gameplay statistics like temperature influence or harvest yield.
    /// It contains reference to <see cref="FoliageResourceProperties"/> by <see cref="FoliageName"/> and to <see cref="SeedResourceProperties"/> by <see cref="SeedResource"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Plant")]
    public class PlantProperties : Properties
    {
        /// <summary>
        /// Name of the <see cref="SeedResourceProperties"/>, automatically included during initial stage at the start of the creation process
        /// </summary>
        public string SeedResource;

        /// <summary>
        /// Name of the produced <see cref="FoliageResourceProperties"/>, automatically included during initial stage at the start of the creation process.
        /// </summary>
        public string FoliageName;

        /// <summary>
        /// Icon of the resource as visible on UI
        /// </summary>
        [XmlIgnore]
        public Sprite Icon;

        /// <summary>
        /// Padding from the edges of the field
        /// </summary>
        public float Padding;

        /// <summary>
        /// Uniform value for random offsetting single plant copy
        /// </summary>
        public float PosVariation;

        /// <summary>
        /// Vector value for random offsetting single plant copy, with ability to do so in both X and Z axis
        /// </summary>
        public Vector3 PosVariationVector = Vector3.zero;

        /// <summary>
        /// Should plant be scaled during growth in X axis
        /// </summary>
        public bool GrowScaleX = true;

        /// <summary>
        /// Should plant be scaled during growth in X axis
        /// </summary>
        public bool GrowScaleY = true;

        /// <summary>
        /// Should plant be scaled during growth in Z axis
        /// </summary>
        public bool GrowScaleZ = true;

        /// <summary>
        /// Base scale of the plant in X axis
        /// </summary>
        public float BaseScaleX = 1.0f;

        /// <summary>
        /// Base scale of the plant in X axis
        /// </summary>
        public float BaseScaleY = 1.0f;

        /// <summary>
        /// Base scale of the plant in X axis
        /// </summary>
        public float BaseScaleZ = 1.0f;

        /// <summary>
        /// List of the grow states
        /// </summary>
        public List<GrowingStateProperties> GrowStates = new List<GrowingStateProperties>();

        /// <summary>
        /// Gameplay parameters like preffered pH or temperature
        /// </summary>
        public PlantAttributeProperties PlantAttributes = new PlantAttributeProperties();

        /// <summary>
        /// Type of the plant that affects visuals, used machines and skill used during its growth.
        /// </summary>
        public PlantType Type;

        /// <summary>
        /// What kind of animation will be used when harvesting
        /// </summary>
        public StaffMeshState HarvestState = StaffMeshState.HarvestingMedium;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (Icon == null)
            {
                Debug.LogError($"Icon at Plant {Name} is null, validation failed");
                validation = false;
            }

            if (string.IsNullOrEmpty(SeedResource))
            {
                Debug.LogError($"SeedResource name at Plant {Name} is empty, validation failed");
                validation = false;
            }

            if (string.IsNullOrEmpty(FoliageName))
            {
                Debug.LogError($"FoliageName name at Plant {Name} is empty, validation failed");
                validation = false;
            }

            // if all of GrowScale are set to false, plant will not grow
            if (BaseScaleX <= 0.0f || BaseScaleY <= 0.0f || BaseScaleZ <= 0.0f)
            {
                Debug.LogError($"One of components of BaseScale at Plant {Name} is smaller or equal to 0, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            bool plantValidation = PlantAttributes.Validate(Name);
            return plantValidation && baseValidation && validation;
        }
    }

    /// <summary>
    /// Gameplay parameters like preffered pH or temperature packaged into separate class for ease of work (please dont question it pretty please - JGuzek)
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "PlantAttributes")]
    public class PlantAttributeProperties
    {
        /// <summary>
        /// Should leave hay after harvest
        /// </summary>
        public bool LeaveHayAfterCrop = false;

        /// <summary>
        /// Can be harvested multiple times a year
        /// </summary>
        public bool MutiHarvest = false;

        /// <summary>
        /// Can survive winter/dormant season
        /// </summary>
        public bool Perennial = false;

        /// <summary>
        /// What is the influence on the nitrogen values
        /// </summary>
        public NitrogenInfluence NitrogenInfluence;

        /// <summary>
        /// Lower optimal pH value
        /// </summary>
        public float minPh = 2;

        /// <summary>
        /// Upper optimal pH value
        /// </summary>
        public float maxPh = 11;

        /// <summary>
        /// Lower survivable Temperature value, below it Plant will receive damage from cold
        /// </summary>
        public int MinSurvivableTemperature = 0;

        /// <summary>
        /// Upper survivable Temperature value, below it Plant will receive damage from warmth
        /// </summary>
        public int MaxSurvivableTemperature = 40;

        /// <summary>
        /// Lower growth Temperature value, below which Plant will not receive bonus from optimal temperature
        /// </summary>
        public int MinGrowthTemeprature = 10;

        /// <summary>
        /// Upper growth Temperature value, above which Plant will not receive bonus from optimal temperature
        /// </summary>
        public int MaxGrowthTemperature = 30;

        /// <summary>
        /// Daily chance for field to be infected by Fungus (inclusive 0, 1 range, where 1 equals 100%). The higher value, the larger chance for infection.
        /// </summary>
        public float FungsChance = 0.8f;

        /// <summary>
        /// Daily chance for field to be infected by Herbs (inclusive 0, 1 range, where 1 equals 100%). The higher value, the larger chance for infection.
        /// </summary>
        public float HerbsChance = 0.8f;

        /// <summary>
        /// Daily chance for field to be infected by Insects (inclusive 0, 1 range, where 1 equals 100%). The higher value, the larger chance for infection.
        /// </summary>
        public float InsectsChance = 0.8f;

        /// <summary>
        /// Parameter required to calculate harvest 
        /// </summary>
        public float GrowEfficiency = 1;

        /// <summary>
        /// Growing time of the plant expressed in days
        /// </summary>
        public float GrowingTime = 80;

        /// <summary>
        /// Percentage value of Growth when plant is sown for the first time (usually starts at 0, but sometimes like in case of some bushes its raised)
        /// </summary>
        public float GrowAtStart = 0;

        /// <summary>
        /// Percentage value of Growth to which plant will be set after harvest (matters if plant is Multiharvest)
        /// </summary>
        public float GrowthAfterHarvest = 0.3f;

        /// <summary>
        /// Machine tag of machine used during harvest, defaulted to "chesttrailer"
        /// </summary>
        public string HarvestingMachine = "";

        /// <summary>
        /// Machine tag of machine used during balling - only if LeaveHayAfterCrop is true
        /// </summary>
        public string BalingMachine = "";

        /// <summary>
        /// Machine tag of machine used during collecting - only if LeaveHayAfterCrop is true
        /// </summary>
        public string CollectingMachine = "";

        /// <summary>
        /// Distance between plants in X axis (world space X axis)
        /// </summary>
        public float DensityX = 1.0f;

        /// <summary>
        /// Distance between plants in Y (world space Z) axis 
        /// </summary>
        public float DensityY = 1.0f;

        public bool Validate(string plantName)
        {
            if (minPh >= maxPh)
            {
                Debug.LogError($"Min pH {minPh} on Plant {plantName}, is larger or equal then Max pH {maxPh}, validation failed");
                return false;
            }

            if (GrowEfficiency <= 0.0f)
            {
                Debug.LogError($"Grow efficiency on Plant {plantName}, is smaller or equal then 0, validation failed");
                return false;
            }

            if (GrowingTime <= 0)
            {
                Debug.LogError($"Growing Time on Plant {plantName}, is smaller or equal then 0, validation failed");
                return false;
            }

            if (DensityX < 0.0f || DensityY < 0.0f)
            {
                Debug.LogError($"Density (x: {DensityX}, y: {DensityY}) on Plant {plantName}, is smaller or equal then 0, validation failed");
                return false;
            }

            if (MinSurvivableTemperature >= MinGrowthTemeprature)
            {
                Debug.LogError($"MinSurvivableTemperature {MinSurvivableTemperature} on Plant {plantName}, is larger or equal then MinGrowthTemeprature {MinGrowthTemeprature}, validation failed");
                return false;
            }

            if (MinGrowthTemeprature >= MaxGrowthTemperature)
            {
                Debug.LogError($"MinGrowthTemeprature {MinGrowthTemeprature} on Plant {plantName}, is larger or equal then MaxGrowthTemperature {MaxGrowthTemperature}, validation failed");
                return false;
            }

            if (MaxGrowthTemperature >= MaxSurvivableTemperature)
            {
                Debug.LogError($"MaxGrowthTemperature {MaxGrowthTemperature} on Plant {plantName}, is larger or equal then MaxSurvivableTemperature {MaxSurvivableTemperature}, validation failed");
                return false;
            }

            if (LeaveHayAfterCrop && string.IsNullOrEmpty(BalingMachine))
            {
                Debug.LogError($"BallingMachine on Plant {plantName} is not filled, either fill it or set LeaveHayAfterCrop to false");
                return false;
            }

            if (LeaveHayAfterCrop && string.IsNullOrEmpty(CollectingMachine))
            {
                Debug.LogError($"CollectingMachine on Plant {plantName} is not filled, either fill it or set LeaveHayAfterCrop to false");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Describes Growing state of the plant starting from float Start 
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "GrowingState")]
    public class GrowingStateProperties
    {
        /// <summary>
        /// When does GrowingState starts taking effect. Both Start and End are expressed in [0, 1] range, 
        /// where 1 means 100% Growth/fully grown Plant
        /// </summary>
        public float Start = 0f;

        /// <summary>
        /// Used in scaling, refer to Start for more information
        /// </summary>
        public float End = 1f;

        /// <summary>
        /// List of MeshFilters that will be used when displaying plant
        /// </summary>
        [XmlIgnore]
        public GameObject[] Objects;

        /// <summary>
        /// Mesh of fruit that will be spawned at FruitDummies
        /// </summary>
        [XmlIgnore]
        public Mesh FruitMesh;

        /// <summary>
        /// Fruit spawn points
        /// </summary>
        [XmlIgnore]
        public Transform[] FruitsDumies;

        /// <summary>
        /// Static object i.e. wooden support for plant that will not be scaled
        /// </summary>
        [XmlIgnore]
        public GameObject StaticObject;
    }

    public enum PlantType
    {
        Grain, Bush, Tree
    }

    /// <summary>
    /// Describes influence of the plant on soil area field is located. Bonus to harvest yield from nitrogen is applied before soil statistics are affected.
    /// </summary>
    public enum NitrogenInfluence
    {
        /// <summary>
        /// Will drain nitrogen out of soil when harvested
        /// </summary>
        Drain, 

        /// <summary>
        /// Will replenish nitrogen back into the soil when harvested
        /// </summary>
        Replenish
    }
    
}
