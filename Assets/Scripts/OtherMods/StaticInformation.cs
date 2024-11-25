using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmManagerWorld.Static
{
    public static class StaticInformation
    {
        public static Dictionary<string, Type> BuildingTypes = new Dictionary<string, Type>
        {
            {"AnimalsBuilding" ,typeof(AnimalsBuildingProperties)},
            {"ApiaryBuilding" ,typeof(ApiaryBuildingProperties)},
            {"DecoBuilding" ,typeof(DecoBuildingProperties)},
            {"GarageBuilding" ,typeof(GarageBuildingProperties)},
            {"GlasshouseBuilding" ,typeof(GlasshouseBuildingProperties)},
            {"HiveBuilding" ,typeof(HiveBuildingProperties)},
            {"IrrigationBuilding" ,typeof(IrrigationBuildingProperties)},
            {"LogisticBuilding" ,typeof(LogisticBuildingProperties)},
            {"MechanicBuilding" ,typeof(MechanicBuildingProperties)},
            {"ProductionBuilding" ,typeof(ProductionBuildingProperties)},
            {"StaffBuilding" ,typeof(StaffBuildingProperties)},
            {"VetBuilding" ,typeof(VetBuildingProperties)},
            {"WarehouseBuilding" ,typeof(WarehouseBuildingProperties)}
        };

        public static Dictionary<string, Type> MachinesDictionary = new Dictionary<string, Type>
        {
            {"Vehicle", typeof(VehicleProperties)},
            {"SowingMachine", typeof(SowingMachineProperties)},
            {"Machine" ,typeof(MachineProperties)}
        };

        public static Dictionary<string, Type> ResourceTypes = new Dictionary<string, Type>
        {
            {"AnimalAsResource" , typeof(AnimalAsResourceProperties)  },
            {"AnimalResource",typeof(AnimalResourceProperties) },
            {"ChemicalResource",typeof(ChemicalResourceProperties) },
            {"FertilizingResource" ,typeof(FertilizingResourceProperties)},
            {"FoliageResource" ,typeof(FoliageResourceProperties)},
            {"HoneyResource" ,typeof(HoneyResourceProperties)},
            {"SeedResource" , typeof(SeedResourceProperties)},
            {"ProductionResource",typeof(ProductionResourceProperties) },
            {"ProductionMultiResource" ,typeof(ProductionMultiResourceProperties)},
            {"OtherResource" ,typeof(OtherResourceProperties)}
        };

        public static Dictionary<string, Type> Modtypes = new Dictionary<string, Type>
        {
            ///Buildings
            {"AnimalsBuilding" ,typeof(AnimalsBuildingMod)},
            {"ApiaryBuilding" ,typeof(ApiaryBuildingMod)},
            {"DecoBuilding" ,typeof(DecoBuildingMod)},
            {"GarageBuilding" ,typeof(GarageBuildingMod)},
            {"GlasshouseBuilding" ,typeof(GlasshouseBuildingMod)},
            {"HiveBuilding" ,typeof(HiveBuildingMod)},
            {"IrrigationBuilding" ,typeof(IrrigationBuildingMod)},
            {"LogisticBuilding" ,typeof(LogisticBuildingMod)},
            {"MechanicBuilding" ,typeof(MechanicBuildingMod)},
            {"ProductionBuilding" ,typeof(ProductionBuildingMod)},
            {"StaffBuilding" ,typeof(StaffBuildingMod)},
            {"VetBuilding" ,typeof(VetBuildingMod)},
            {"WarehouseBuilding" ,typeof(WarehouseBuildingMod)},
            ///Resources
            {"AnimalAsResource" , typeof(AnimalAsResourceMod)  },
            {"AnimalResource",typeof(AnimalResourceMod) },
            {"ChemicalResource",typeof(ChemicalResourceMod) },
            {"FertilizingResource" ,typeof(FertilizingResourceMod)},
            {"FoliageResource" ,typeof(FoliageResourceMod)},
            {"HoneyResource" ,typeof(HoneyResourceMod)},
            {"SeedResource" , typeof(SeedResourceMod)},
            {"ProductionResource",typeof(ProductionResourceMod) },
            {"OtherResource" ,typeof(OtherResourceMod)},
            //Machines
            {"Machine", typeof(MachineMod)},
            {"Vehicle" ,typeof(VehicleMod)},
            {"SowingMachine" ,typeof(SowingMachineMod)},
            //Other
            {"RegionalModel", typeof(RegionalModelMod)},
        };

        public static Dictionary<Type, bool> BuildingsWithRoadconnector = new Dictionary<Type, bool>
        {
            {typeof(AnimalsBuildingMod),    true},
            {typeof(ApiaryBuildingMod),     true},
            {typeof(DecoBuildingMod),       false},
            {typeof(GarageBuildingMod),     true},
            {typeof(GlasshouseBuildingMod), true},
            {typeof(HiveBuildingMod),       false},
            {typeof(IrrigationBuildingMod), false},
            {typeof(LogisticBuildingMod),   true},
            {typeof(MechanicBuildingMod),   true},
            {typeof(ProductionBuildingMod), true},
            {typeof(StaffBuildingMod),      true},
            {typeof(VetBuildingMod),        true},
            {typeof(WarehouseBuildingMod),  true}
        };

        private static MachineType[] _vehicleTypes = new MachineType[]
        {
            MachineType.Tractor,
            MachineType.Combine,
            MachineType.DeliveryTruck,
            MachineType.PickUp,
            MachineType.FireTruck,
            MachineType.CombineForWine
        };

        private static MachineType[] _sowingMachines = new MachineType[]
        {
            MachineType.Seeder,
            MachineType.Planter,
            MachineType.TreePlanter,
            MachineType.PointPlanter,
            MachineType.PotatoPlanter,
        };

        public static string GetMachinePropertyType(MachineType machineType)
        {
            if (_vehicleTypes.Contains(machineType))
                return "Vehicle";

            if (_sowingMachines.Contains(machineType))
                return "SowingMachine";

            return "Machine";
        }

        public enum ResourceEnums
        {
            AnimalAsResource,
            AnimalResource,
            ChemicalResource,
            FertilizingResource,
            FoliageResource,
            HoneyResource,
            SeedResource,
            ProductionResource,
            OtherResource
        }

        public enum BuildingEnums
        {
            AnimalsBuilding,
            ApiaryBuilding,
            DecoBuilding,
            GarageBuilding,
            GlasshouseBuilding,
            HiveBuilding,
            IrrigationBuilding,
            LogisticBuilding,
            MechanicBuilding,
            ProductionBuilding,
            StaffBuilding,
            VetBuilding,
            WarehouseBuilding
        }

        public enum TypeEnums
        {
            Building,
            Plant,
            Resource,
            Animal,
            Machine,
            PlantFull
        }

        public static string[] AllowedBundleVariants = new string[]
        {
            ".default",
            ".europe",
            ".asia",
            ".southamerica"
        };

        public enum Region
        {
            None, Europe, Asia, SouthAmerica
        }

        public enum StaffMeshState
        {
            Idle = 0,
            Apiary = 1,
            Digging = 2,
            HarvestingLow = 3,
            HarvestingMedium = 4,
            HarvestingTrees = 5,
            Hoeing = 6,
            OpeningDoors = 7,
            Planting = 8,
            Raking = 9,
            Scything = 10,
            Sowing = 11,
            Spraying = 12,
            Sitting = 13,
            Throwing = 14,
            VettingMediumLargeAnimal = 15,
            VettingSmallAnimal = 16,
            Walking = 17,
            Watering = 18,
            SprayingTree = 19,
            ManureSpreading = 20,
            TreePlanting = 21,
            MachineSitting = 22,
            PrecisionPlanter = 23,
            IdleKisten = 24,
            WalkingKisten = 25,
            HarvestingLowKisten = 26,
            HarvestingMediumKisten = 27,
            HarvestingTreesKisten = 28,
            Mechanic = 30,
            WalkingLeft = 31,
            WalkingRight = 32,
            Grinding = 33,
            SwoingLoop = 60
        }

        [Serializable()]
        public class RegionModifier
        {
            public Region Region = Region.None;
            public float PriceModifier = 1.0f;

            public RegionModifier()
            {

            }

            public RegionModifier(Region region, float priceModifier)
            {
                Region = region;
                PriceModifier = priceModifier;
            }
        }

        public interface ICopyTo
        {
            void CopyTo(UnityEngine.Object oldObject, UnityEngine.Object newObject, ref List<UnityEngine.Component> componentsToDestroy);
        }

        public static string[] VehicleTags =
        {
            "choppercornmachine",
            "tractor",
            "combineharvester",
            "orchardtractor",
            "pickup"
        };

        public static string[] MachineTags =
        {
            "choppercornmachine",
            "chopperheader",
            "cultivator",
            "plow",
            "potatoplanter",
            "reaperforcorn",
            "orchardsprayer",
            "tractor",
            "treeplanter",
            "solidmanurespreaders",
            "planter",
            "precisionseeder",
            "potatoharvester",
            "roundbaler",
            "trailerforbales",
            "reapers",
            "combineharvester",
            "mower",
            "selfloadingtrailer",
            "chesttrailer",
            "windrower",
            "watering",
            "liquidmanurespreaders",
            "pickup",
            "sprayer",
            "animaltrailer",
            "deliverytruck",
            "fertilizer",
            "seederforplantingcreals" // i know that is has typo but ya'll know the drill, someone did the typo and nobody questioned it and now its everywhere and its that one piece that holds fabric or (farming) reality together
        };

        public static string[] TransportToolTags =
        {
            "bag",
            "bucket",
            "kisten",
            "watercan"
        };

        public static string[] SowingMachinesTags =
        {
            "potatoplanter",
            "treeplanter",
            "planter",
            "precisionseeder",
            "seederforplantingcreals"
        };

        public static string[] AllowedBushShaderNames =
        {
            "Farm Manager World/PlantCutout",
            "Farm Manager World/PlantCutoutNoWind"
        };

        public static string[] AllowedGrainShaderNames =
        {
            "Farm Manager World/GrainWithWindInstanced"
        };
    }
}
