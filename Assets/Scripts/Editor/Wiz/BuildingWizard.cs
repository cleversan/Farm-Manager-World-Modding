using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Static;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.ComponentModel;
namespace FarmManagerWorld.Editors.Wizards
{
    public class BuildingWizard : ScriptableWizard
    {
        GameObject modObject;

        [Range(1, 6)]
        public int numberOfLods = 1;

        public bool createEmptyLODs = true;

        public bool IsRegional = false;

        public Sprite icon;
        [HideInInspector]
        public StaticInformation.BuildingEnums type;
        public string Name;

        public virtual void OnUpdate()
        {
            isValid = true;
        }

        public virtual void Enable()
        {
            isValid = false;
        }

        public virtual void Create()
        {
            string BuildingType;
            BuildingType = type.ToString();
            GameObject gameObject = new GameObject();
            BuildingMod buildingMod = (BuildingMod)gameObject.AddComponent(StaticInformation.Modtypes[type.ToString()]);
            buildingMod.building = (BuildingProperties)Activator.CreateInstance(StaticInformation.BuildingTypes[BuildingType]);
            buildingMod.building.BasicType = type.ToString();
            buildingMod.gameObject.name = Name + "_" + type.ToString();
            buildingMod.building.Name = Name;

            if (IsRegional)
            {
                modObject = new GameObject("RegionalModelModManager", typeof(RegionalModelModManager));
                modObject.transform.SetParent(buildingMod.transform);
                foreach (StaticInformation.Region region in Enum.GetValues(typeof(StaticInformation.Region)))
                {
                    if (region == StaticInformation.Region.None)
                        continue;

                    RegionalModelMod regionObject = LODsGenerator.GenerateEmptyLOD(numberOfLods, createEmptyLODs).AddComponent<RegionalModelMod>();
                    regionObject.name = $"{Name}{region}";
                    regionObject.RegionalModel = (RegionalModelProperties)Activator.CreateInstance(typeof(RegionalModelProperties));
                    regionObject.RegionalModel.Name = Name;
                    regionObject.RegionalModel.Region = region;
                    regionObject.transform.SetParent(modObject.transform);
                }
            }
            else
            {
                modObject = LODsGenerator.GenerateEmptyLOD(numberOfLods, createEmptyLODs);
                modObject.transform.SetParent(buildingMod.transform);
                modObject.gameObject.name = buildingMod.building.Name + "_Model";
                buildingMod.model = modObject;
            }

            buildingMod.building.Icon = icon;

            // Object size
            gameObject.AddComponent<BoxCollider>();

            foreach (var obj in buildingMod.GetComponentsInChildren<Transform>())
            {
                obj.name = obj.name.Replace("(Clone)", "");
            }

            buildingMod.building.WayPoint = new Vector3(0, 0, 0);
            buildingMod.gameObject.AddComponent<BuildingEditor>();
            buildingMod.building = ExtraValuesInput(buildingMod.building);

            this.Close();
        }

        protected void OnWizardCreate()
        {
            Create();

            //EditorUtility.DisplayCustomMenu(new Rect(0, 0, 100, 100), new GUIContent[] { new GUIContent("aa"), new GUIContent("bb") }, 0, SelectMenuItemFunction, "asdasd");
        }

        void SelectMenuItemFunction(object userData, string[] options, int selected)
        {
            Create();
        }

        public virtual BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            Debug.Log("Basic building no additives");
            return building;
        }
    }

    public class BuildTypeEditor : EditorWindow
    {
        public Dictionary<string, Type> BuildingWizardTypes = new Dictionary<string, Type>
    {
            {"AnimalsBuilding" ,typeof(AnimalsBuildingWizard)},
            {"ApiaryBuilding" ,typeof(ApiaryBuildingWizard)},
            {"DecoBuilding" ,typeof(DecoBuildingWizard)},
            {"GarageBuilding" ,typeof(GarageBuildingWizard)},
            {"GlasshouseBuilding" ,typeof(GlassHouseBuildingWizard)},
            {"HiveBuilding" ,typeof(HiveBuildingWizard)},
            {"IrrigationBuilding" ,typeof(IrrigationBuildingWizard)},
            {"LogisticBuilding" ,typeof(LogisticBuildingWizard)},
            {"MechanicBuilding" ,typeof(MechanicBuildingWizard)},
            {"ProductionBuilding" ,typeof(ProductionBuildingWizard)},
            {"StaffBuilding" ,typeof(StaffBuildingWizard)},
            {"VetBuilding" ,typeof(VetBuildingWizard)},
            {"WarehouseBuilding" ,typeof(WarehouseBuildingWizard)}
        };

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(BuildTypeEditor));
        }

        private void OnGUI()
        {
            GUILayout.Label("Choose building type", EditorStyles.boldLabel);

            foreach (string build in BuildingWizardTypes.Keys)
            {
                if (GUILayout.Button(build))
                {
                    BuildingWizard window = (BuildingWizard)ScriptableObject.CreateInstance(BuildingWizardTypes[build]);

                    window.type = (StaticInformation.BuildingEnums)System.Enum.Parse(typeof(StaticInformation.BuildingEnums), build);
                    window.Show();
                }
            }

        }
    }

    public class AnimalsBuildingWizard : BuildingWizard
    {
        public string[] PossibleAnimals;
        public int MaxAnimalsCount;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            AnimalsBuildingProperties b = (AnimalsBuildingProperties)building;
            b.PossibleAnimals = PossibleAnimals;
            b.MaxAnimalsCount = MaxAnimalsCount;

            return b;
        }
    }

    public class ApiaryBuildingWizard : BuildingWizard
    {
        public float MaxDistanceToHive = 15;
        public int MaxHiveNumber = 10;
        public string HiveBuildingName;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            ApiaryBuildingProperties b = (ApiaryBuildingProperties)building;

            b.MaxDistanceToHive = MaxDistanceToHive;
            b.MaxHiveNumber = MaxHiveNumber;
            b.HiveBuildingName = HiveBuildingName;

            return b;
        }
    }

    public class DecoBuildingWizard : BuildingWizard
    {
        public Vector3 Offset;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            DecoBuildingProperties b = (DecoBuildingProperties)building;
            b.Offset = Offset;

            return b;
        }
    }

    public class GarageBuildingWizard : BuildingWizard
    {
        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            GarageBuildingProperties b = (GarageBuildingProperties)building;

            return b;
        }

    }
    public class GlassHouseBuildingWizard : BuildingWizard
    {
        public BoxCollider PassageCollider;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            GlasshouseBuildingProperties b = (GlasshouseBuildingProperties)building;            
            b.PassageCollider = PassageCollider;

            return b;
        }
    }

    public class HiveBuildingWizard : BuildingWizard
    {
        public ParticleSystem Emitter;
        public GameObject HiveFeedingLocation;

        public float MaxDistanceToPlants = 75;
        public float HoneyPerHarvest = 1f;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            HiveBuildingProperties b = (HiveBuildingProperties)building;
            b.MaxDistanceToPlants = MaxDistanceToPlants;
            b.HoneyPerHarvest = HoneyPerHarvest;
            b.Emitter = Emitter;
            b.HiveFeedingLocation = HiveFeedingLocation;


            return b;
        }
    }
    public class IrrigationBuildingWizard : BuildingWizard
    {
        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            IrrigationBuildingProperties b = (IrrigationBuildingProperties)building;

            return b;
        }
    }
    public class LogisticBuildingWizard : BuildingWizard
    {
        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            LogisticBuildingProperties b = (LogisticBuildingProperties)building;
            b.StoredResourceType = new List<ResourceType>();
            foreach (ResourceType storageType in Enum.GetValues(typeof(ResourceType)))
                b.StoredResourceType.Add(storageType);

            return b;
        }
    }
    public class MechanicBuildingWizard : BuildingWizard
    {
        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            MechanicBuildingProperties b = (MechanicBuildingProperties)building;

            return b;
        }
    }

    public class WarehouseBuildingWizard : BuildingWizard
    {
        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            WarehouseBuildingProperties b = (WarehouseBuildingProperties)building;

            b.StoredResourceType = new List<ResourceType>();
            foreach (ResourceType storageType in Enum.GetValues(typeof(ResourceType)))
                b.StoredResourceType.Add(storageType);

            return b;
        }
    }
    public class VetBuildingWizard : BuildingWizard
    {
        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            VetBuildingProperties b = (VetBuildingProperties)building;

            return b;
        }
    }

    public class StaffBuildingWizard : BuildingWizard
    {
        public bool IsSeasonalBuilding;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            StaffBuildingProperties b = (StaffBuildingProperties)building;
            b.IsSeasonalBuilding = IsSeasonalBuilding;

            return b;
        }
    }

    public class ProductionBuildingWizard : BuildingWizard
    {
        public string[] MachinesAndVehicles;
        public ProductionProperties[] AvailableProduction;

        public override BuildingProperties ExtraValuesInput(BuildingProperties building)
        {
            ProductionBuildingProperties b = (ProductionBuildingProperties)building;

            b.MachinesAndVehicles = MachinesAndVehicles;
            b.AvailableProduction = AvailableProduction;

            return b;
        }
    }
}