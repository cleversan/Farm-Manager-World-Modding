using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Static;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors.Wizards
{
    public class ResourceWizard : ScriptableWizard
    {
        [HideInInspector]
        public ResourceMod resourceMod;
        public string Name;
        public Sprite Sprite;
        public UnitOfMeasure Unit;
        [HideInInspector]
        public StaticInformation.ResourceEnums resourceWizardType;
        public ResourceModType Type;
        public ResourceType StorageType;
        //public Sprite sprite;
        public string TransportToolTag;
        public float BaseMarketAmount = 100;
        public float ExpirationDate = 100;
        public float InitialBasePrice = 2;
        public float MaxDailyPriceGrowth = 0.0005f;
        public float MaxDailyPriceDrop = -0.0005f;
        public float FoodQuality = 0.3f;

        private void Awake()
        {
            minSize = new Vector2(300, 300);
        }

        public virtual GameObject Create(bool addResourceEditor = true)
        {
            var gameObject = new GameObject();
            ResourceMod resourceMod = (ResourceMod)gameObject.AddComponent(StaticInformation.Modtypes[resourceWizardType.ToString()]);
            resourceMod.resource = (ResourceProperties)Activator.CreateInstance(StaticInformation.ResourceTypes[resourceWizardType.ToString()], new object[] { });

            resourceMod.resource.Sprite = Sprite;
            resourceMod.resource.Unit = Unit;
            resourceMod.resource.Type = Type;
            resourceMod.resource.StorageType = StorageType;

            resourceMod.resource.TransportToolTag = TransportToolTag;
            resourceMod.resource.BaseMarketAmount = BaseMarketAmount;
            resourceMod.resource.ExpirationDate = ExpirationDate;
            resourceMod.resource.InitialBasePrice = InitialBasePrice;
            resourceMod.resource.MaxPercentageDailyPriceDrop = MaxDailyPriceDrop;
            resourceMod.resource.MaxPercentageDailyPriceGrowth = MaxDailyPriceGrowth;
            resourceMod.resource.FoodQuality = FoodQuality;
            resourceMod.resource.BasicType = resourceWizardType.ToString();
            gameObject.name = resourceMod.resource.Name + "_Resource";

            resourceMod.resource.Name = Name;
            resourceMod.gameObject.name = resourceMod.resource.Name + "_Resource";
            Debug.Log(resourceMod.resource.GetType());
            resourceMod.resource = ExtraValuesInput(resourceMod.resource);

            if (addResourceEditor)
                gameObject.AddComponent<ResourceEditor>();

            this.Close();
            return gameObject;
        }

        public virtual ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            Debug.Log("Basic building no additives");
            return resource;
        }
    }

    public class ResourceTypeEditor : EditorWindow
    {
        Dictionary<string, Type> ResourceWizardTypes = new Dictionary<string, Type>
     {
            {"AnimalAsResource" , typeof(AnimalAsResourceWizard)  },
            {"AnimalResource",typeof(AnimalResourceWizard) },
            {"ChemicalResource",typeof(ChemicalResourceWizard) },
            {"FertilizingResource" ,typeof(FertilizingResourceWizard)},
            {"FoliageResource" ,typeof(FoliageResourceWizard)},
            {"HoneyResource" ,typeof(HoneyResourceWizard)},
            {"SeedResource" , typeof(SeedResourceWizard)},
            {"ProductionResource",typeof(ProductionResourceWizard) },
            {"OtherResource" ,typeof(OtherResourceWizard)},
        };

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(BuildTypeEditor));
        }

        private void OnGUI()
        {
            GUILayout.Label("Choose Type", EditorStyles.boldLabel);

            foreach (string build in ResourceWizardTypes.Keys)
            {
                if (GUILayout.Button(build))
                {
                    ResourceWizard window = (ResourceWizard)Activator.CreateInstance(ResourceWizardTypes[build]);
                    window.resourceWizardType = (StaticInformation.ResourceEnums)Enum.Parse(typeof(StaticInformation.ResourceEnums), build);
                    window.Show();
                }
            }

        }
    }

    public class AnimalAsResourceWizard : ResourceWizard
    {
        [HideInInspector]
        public AnimalMod animalMod;

        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            AnimalAsResourceProperties b = (AnimalAsResourceProperties)resource;

            return resource;
        }

        private void OnWizardCreate()
        {
            var animalAsResource = Create(animalMod == null);
            if (animalMod != null)
            {
                var editor = animalMod.GetComponent<AnimalEditor>();
                if (editor != null)
                    editor.animalAsResource = animalAsResource.GetComponent<AnimalAsResourceMod>();
            }
        }
    }

    public class AnimalResourceWizard : ResourceWizard
    {
        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            AnimalResourceProperties b = (AnimalResourceProperties)resource;
            return resource;
        }
        private void OnWizardCreate()
        {
            Create();
        }
    }

    public class ChemicalResourceWizard : ResourceWizard
    {
        public float AmountPerSquareMeter = 0.01f;
        public ChemicalType ChemicalType;

        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            ChemicalResourceProperties b = (ChemicalResourceProperties)resource;
            b.AmountPerSquareMeter = AmountPerSquareMeter;
            b.ChemicalType = ChemicalType;

            return resource;
        }
        
        private void OnWizardCreate()
        {
            Create();
        }
    }

    public class FertilizingResourceWizard : ResourceWizard
    {
        public string FertilizingMachine;
        public float AmountPerSquareMeter = 0.1f;

        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            FertilizingResourceProperties b = (FertilizingResourceProperties)resource;
            b.FertilizingMachine = FertilizingMachine;
            b.AmountPerSquareMeter = AmountPerSquareMeter;
            return resource;
        }

        private void OnWizardCreate()
        {
            Create();
        }
    }

    public class FoliageResourceWizard : ResourceWizard
    {
        [HideInInspector]
        public PlantMod plant;

        public Material HarvestMaterial;
        public GameObject HarvestTrailerPlane;
        public bool UseMaterialOnMesh = false;
        public bool UseMaterialOnKisten = false;
        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            FoliageResourceProperties b = (FoliageResourceProperties)resource;
            b.HarvestMaterial = HarvestMaterial;
            b.HarvestChestTrailerPlane = HarvestTrailerPlane;
            b.UseHarvestMaterialOnMesh = UseMaterialOnMesh;
            b.UseHarvestMaterialOnKisten = UseMaterialOnKisten;

            return resource;
        }

        private void OnWizardCreate()
        {
            GameObject ob = Create(plant == null);
            if (plant)
            {
                plant.Plant.FoliageName = Name;
                if (plant.GetComponent<PlantEditor>() is PlantEditor editor)
                    editor.foliageResourceMod = ob.GetComponent<FoliageResourceMod>();

                SeedPopup window2 = (SeedPopup)EditorWindow.GetWindow(typeof(SeedPopup), false, "Seed Popup");
                window2.MainObject = plant;
                window2.Show();
                Close();
            }
        }

        public static void ShowWizard(PlantMod mainObject)
        {
            FoliageResourceWizard window = (FoliageResourceWizard)EditorWindow.GetWindow(typeof(FoliageResourceWizard), false, "Wizard for foliage");
            window.Name = mainObject.Plant.Name;
            window.plant = mainObject;
            window.resourceWizardType = StaticInformation.ResourceEnums.FoliageResource;
            window.Type = ResourceModType.FoliageResources;
            window.Show();
        }

        private void OnWizardUpdate()
        {
            EditorGUIUtility.labelWidth = 250;
        }
    }
    
    public class HoneyResourceWizard : ResourceWizard
    {
        public string NeededPlantName;
        public bool NeedPlant = true;

        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            HoneyResourceProperties b = (HoneyResourceProperties)resource;
            b.NeededPlantName = NeededPlantName;
            b.NeedPlant = NeedPlant;
            return resource;
        }

        private void OnWizardCreate()
        {
            Create();
        }
    }

    public class SeedResourceWizard : ResourceWizard
    {
        [HideInInspector]
        public PlantMod plant;

        public string SowingMachine;
        public bool IsSeedling;
        public string PlantName;
        public float SeedAmountPerSqaureMeter = 1f;
        public float EnergyDropPerSow = 0.05f;
        public bool CanPlantInGlassHouse = false;
        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            SeedResourceProperties b = (SeedResourceProperties)resource;
            b.SowingMachine = SowingMachine;
            b.IsSeedling = IsSeedling;
            b.PlantName = PlantName;
            b.SeedAmountPerSqaureMeter = SeedAmountPerSqaureMeter;
            b.CanPlantInGlassHouse = CanPlantInGlassHouse;
            b.plant = plant.gameObject;
            return resource;
        }

        private void OnWizardCreate()
        {
            GameObject ob = Create(plant == null);
            if (plant)
            {
                if (plant.GetComponent<PlantEditor>() is PlantEditor editor)
                    editor.seedResourceMod = ob.GetComponent<SeedResourceMod>();

                plant.Plant.SeedResource = ob.GetComponent<ResourceMod>().resource.Name;
            }
        }
    }

    public class ProductionResourceWizard : ResourceWizard
    {
        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            ProductionResourceProperties b = (ProductionResourceProperties)resource;
            return resource;
        }

        private void OnWizardCreate()
        {
            Create();
        }
    }

    public class OtherResourceWizard : ResourceWizard
    {
        public Material StorageMaterial;
        public GameObject ChestTrailerPlane;
        public override ResourceProperties ExtraValuesInput(ResourceProperties resource)
        {
            OtherResourceProperties b = (OtherResourceProperties)resource;
            b.StorageMaterial = StorageMaterial;
            b.ChestTrailerPlane = ChestTrailerPlane;
            return resource;
        }

        private void OnWizardCreate()
        {
            Create();
        }
    }
}