using FarmManagerWorld.Static;
using FarmManagerWorld.Utils;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;

namespace FarmManagerWorld.Modding
{
    public static class ModLoader
    {
        public static void Awake()
        {
            foreach (var item in toLoadMods)
            {
                LoadMod(item);
            }
            toLoadMods.Clear();
        }

        public static void Initialize()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            ClearMods();
            InitializeMods();
        }

        public static void ClearMods(bool clearAssetBundles = false)
        {
            if (clearAssetBundles)
                AssetBundle.UnloadAllAssetBundles(true);

            loadedMods.Clear();
            availableMods.Clear();
            loadedAnimalsPrefabs.Clear();
            loadedBuildingsPrefabs.Clear();
            loadedMachinePrefabs.Clear();
            loadedResourcesPrefabs.Clear();
            loadedPlantsPrefabs.Clear();
            loadedRegionalPrefabs.Clear();
        }

        public static List<GameObject> loadedAnimalsPrefabs = new List<GameObject>();
        public static List<GameObject> loadedBuildingsPrefabs = new List<GameObject>();
        public static List<GameObject> loadedPlantsPrefabs = new List<GameObject>();
        public static List<GameObject> loadedResourcesPrefabs = new List<GameObject>();
        public static List<GameObject> loadedMachinePrefabs = new List<GameObject>();
        public static List<GameObject> loadedRegionalPrefabs = new List<GameObject>();

        public static List<Mod> availableMods = new List<Mod>();
        public static List<Mod> loadedMods = new List<Mod>();
        public static List<Mod> toLoadMods = new List<Mod>();
        public static Mod FindModByTitle(string title) => availableMods.Find((x) => x.Title == title);

        static void InitializeMods()
        {
            foreach (string folder in Directory.GetDirectories(Path.Combine(Application.persistentDataPath, "mods")))
            {
                var modPath = Path.Combine(folder, "Mod.xml");
                if (File.Exists(modPath))
                {
                    var mod = Deserialize<Mod>(modPath);
                    mod.Folder = folder;
                    availableMods.Add(mod);
                }
            }
        }

        public static void LoadMod(Mod mod)
        {
            if (availableMods.Contains(mod))
            {
                InitializeAssetBundles(mod);
                InitializeObjects(mod, "animals");
                InitializeObjects(mod, "buildings");
                InitializeObjects(mod, "plants");
                InitializeObjects(mod, "plantFull");
                InitializeObjects(mod, "machines");
                InitializeObjects(mod, "resources");
                loadedMods.Add(mod);
            }
        }

        public static void InitializeAssetBundles(Mod mod)
        {
            string[] paths = Directory.GetFiles(Path.Combine(mod.Folder, "assets"));
            AssetBundleImport.ImportAssetBundle(mod, paths);
        }

        public static void InitializeObjects(Mod mod, string type, Transform parent = null)
        {
            string[] paths = Directory.GetFiles(Path.Combine(mod.Folder, type));
            foreach (string file in paths)
            {
                InitializeXML(file, type, mod, parent);
            }
        }

        public static void Serialize(object item, string path)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                XmlSerializer serializer = new XmlSerializer(item.GetType());
                using StreamWriter writer = new StreamWriter(path);
                serializer.Serialize(writer.BaseStream, item);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception thrown during serialization of an object {item} at path {path} - {e.Message}");
            }
        }

        public static T Deserialize<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            string fileText = File.ReadAllText(path);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader reader = new StringReader(fileText);
            return (T)serializer.Deserialize(reader);
        }

        private static Properties Deserialize(string path, string type)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            string fileText = File.ReadAllText(path);
            XmlSerializer serializer = new XmlSerializer(Type.GetType(type));
            using StringReader reader = new StringReader(fileText);
            return (Properties)serializer.Deserialize(reader);
        }

        private static void InitializeXML(string file, string type, Mod mod = null, Transform parent = null)
        {
            switch (type)
            {
                case "animals":
                    break;

                case "buildings":
                    LoadBuildingObjectFromPath(file, mod, parent);
                    break;

                case "resources":
                    LoadResourceObjectFromPath(file, mod, parent);
                    break;

                case "plants":
                    PlantProperties load = Deserialize<PlantProperties>(file);
                    load.Mod = mod;
                    LoadPlantObjectFromProperties(load, parent);
                    break;

                case "plantFull":
                    PlantDataContainer data = (PlantDataContainer)ScriptableObject.CreateInstance("PlantDataContainer");
                    data = Deserialize<PlantDataContainer>(file);
                    PlantProperties plant = data.plant;
                    SeedResourceProperties seed = data.seed;
                    FoliageResourceProperties foliage = data.foliage;
                    plant.Mod = mod;
                    seed.Mod = mod;
                    foliage.Mod = mod;
                    GameObject P = LoadPlantObjectFromProperties(plant, parent);
                    GameObject S = LoadResourceObjectFromProperties(seed, "SeedResource", parent);
                    GameObject F = LoadResourceObjectFromProperties(foliage, "FoliageResource", parent);

                    P.GetComponent<PlantMod>().Plant.SeedResource = S.GetComponent<ResourceMod>().resource.Name;
                    P.GetComponent<PlantMod>().Plant.FoliageName = F.GetComponent<ResourceMod>().resource.Name;

                    S.GetComponent<SeedResourceMod>().plant = P;

                    break;

                case "machines":
                    LoadMachineObjectFromPath(file, mod, parent);
                    break;
            }
        }

        private static void LoadResourceObjectFromPath(string file, Mod mod, Transform parent)
        {
            string type = ReadType(file);
            if (type == null) 
                return;

            var properties = Deserialize(file, StaticInformation.ResourceTypes[type].ToString());
            properties.Mod = mod;
            LoadResourceObjectFromProperties((ResourceProperties)properties, type, parent);
        }

        private static GameObject LoadResourceObjectFromProperties(ResourceProperties properties, string type, Transform parent)
        {
            var resource = new GameObject();
            resource.transform.SetParent(parent);
            resource.SetActive(false);
            ResourceMod comp = (ResourceMod)resource.AddComponent(StaticInformation.Modtypes[type]);
            comp.resource = properties;

            resource.name = properties.Name;
            return resource;
        }

        private static void LoadBuildingObjectFromPath(string file, Mod mod, Transform parent)
        {
            BuildingProperties basicProperties = Deserialize<BuildingProperties>(file);
            if (basicProperties == null) 
                return;

            var actualProperties = Deserialize(file, StaticInformation.BuildingTypes[basicProperties.BasicType].ToString());
            actualProperties.Mod = mod;
            LoadBuildingObjectFromProperties((BuildingProperties)actualProperties, basicProperties.BasicType, parent);
        }

        private static GameObject LoadBuildingObjectFromProperties(BuildingProperties properties, string type, Transform parent)
        {
            var build = new GameObject();
            build.transform.SetParent(parent);
            build.SetActive(false);
            BuildingMod comp = (BuildingMod)build.AddComponent(StaticInformation.Modtypes[type]);            
            comp.building = properties;
            build.name = comp.building.Name + "_XML";
            //comp.Load();
            build.SetActive(false);
            
            return build;
        }

        private static void LoadMachineObjectFromPath(string file, Mod mod, Transform parent)
        {
            string type = ReadType(file);
            if (string.IsNullOrEmpty(type)) 
                return;

            var properties = Deserialize(file, StaticInformation.MachinesDictionary[type].ToString());
            properties.Mod = mod;
            LoadMachineObjectFromProperties((MachineProperties)properties, type, parent);
        }

        private static GameObject LoadMachineObjectFromProperties(MachineProperties properties, string type, Transform parent)
        {
            var machine = new GameObject();
            machine.transform.SetParent(parent);
            machine.SetActive(false);
            MachineBaseMod comp = (MachineBaseMod)machine.AddComponent(StaticInformation.Modtypes[type]);
            comp.baseMachine = properties;
            machine.name = comp.baseMachine.Name;
            return machine;
        }

        private static GameObject LoadPlantObjectFromProperties(PlantProperties properties, Transform parent)
        {
            var build = new GameObject();
            build.transform.SetParent(parent);
            build.SetActive(false);
            PlantMod comp = build.AddComponent<PlantMod>();
            comp.Plant = properties;
            build.name = properties.Name;

            return build;
        }

        public static string ReadType(string path)
        {
            using (XmlReader xmlReader = XmlReader.Create(path))
            {
                if (xmlReader.MoveToContent() == XmlNodeType.Element)
                    return xmlReader.Name;
            }
            return null;
        }

        public static void LoadAnimalFromAssetBundle(ref GameObject modAnimalObject, Mod mod, out AnimalMod oldAnimalMod)
        {
            loadedAnimalsPrefabs.Add(modAnimalObject);
            oldAnimalMod = modAnimalObject.GetComponent<AnimalMod>();
            AnimalMod newAnimalMod = modAnimalObject.AddComponent<AnimalMod>();
            newAnimalMod.animalBones = oldAnimalMod.animalBones;
            newAnimalMod.Animal = oldAnimalMod.Animal;
            newAnimalMod.Animal.Mod = mod;
        }

        public static void LoadBuildingFromAssetBundle(ref GameObject modBuildingObject, Mod mod, out List<Component> componentsToDestroy)
        {
            componentsToDestroy = new List<Component>();
            loadedBuildingsPrefabs.Add(modBuildingObject);
            BuildingMod oldBuildingMod = modBuildingObject.GetComponent<BuildingMod>();
            oldBuildingMod.building.Mod = mod;
            componentsToDestroy.Add(oldBuildingMod);
            BuildingMod newBuildingMod = modBuildingObject.AddComponent(StaticInformation.Modtypes[oldBuildingMod.building.BasicType]) as BuildingMod;
            oldBuildingMod.CopyTo(oldBuildingMod, newBuildingMod, ref componentsToDestroy);
        }

        public static void LoadPlantFromAssetBundle(ref GameObject modPlantObject, Mod mod, out PlantMod oldPlantMod)
        {
            loadedPlantsPrefabs.Add(modPlantObject);
            oldPlantMod = modPlantObject.GetComponent<PlantMod>();
            PlantMod newPlantMod = modPlantObject.AddComponent<PlantMod>();
            newPlantMod.model = oldPlantMod.model;
            newPlantMod.Plant = oldPlantMod.Plant;
            newPlantMod.Plant.Mod = mod;
        }

        public static void LoadResourceFromAssetBundle(ref GameObject modResourceObject, Mod mod, out ResourceMod oldResourceMod)
        {
            loadedResourcesPrefabs.Add(modResourceObject);
            oldResourceMod = modResourceObject.GetComponent<ResourceMod>();
            ResourceMod newResourceMod = modResourceObject.AddComponent(StaticInformation.Modtypes[oldResourceMod.resource.BasicType]) as ResourceMod;
            newResourceMod.resource = oldResourceMod.resource;
            newResourceMod.resource.Mod = mod;
        }

        public static void LoadMachineBaseFromAssetBundle(ref GameObject modMachineBaseObject, Mod mod, out List<Component> componentsToDestroy)
        {
            componentsToDestroy = new List<Component>();
            loadedMachinePrefabs.Add(modMachineBaseObject);
            MachineBaseMod oldMachineBaseMod = modMachineBaseObject.GetComponent<MachineBaseMod>();
            MachineBaseMod newMachineBaseMod = modMachineBaseObject.AddComponent(StaticInformation.Modtypes[oldMachineBaseMod.baseMachine.BasicType]) as MachineBaseMod;
            oldMachineBaseMod.baseMachine.Mod = mod;
            oldMachineBaseMod.CopyTo(oldMachineBaseMod, newMachineBaseMod, ref componentsToDestroy);
            componentsToDestroy.Add(oldMachineBaseMod);
        }

        public static void LoadRegionalModelFromAssetBundle(ref GameObject modRegionalModelObject, out List<Component> componentsToDestroy)
        {
            componentsToDestroy = new List<Component>();
            loadedRegionalPrefabs.Add(modRegionalModelObject);
            RegionalModelMod oldRegionalModelMod = modRegionalModelObject.GetComponent<RegionalModelMod>();
            RegionalModelMod newRegionalModelMod = modRegionalModelObject.AddComponent<RegionalModelMod>();
            newRegionalModelMod.parentBuildingName = oldRegionalModelMod.parentBuildingName;
            oldRegionalModelMod.CopyTo(oldRegionalModelMod, newRegionalModelMod, ref componentsToDestroy);
            componentsToDestroy.Add(oldRegionalModelMod);
        }

        public static List<Mod> GetMods()
        {
            var files = Directory.GetFiles(Application.dataPath, "Mod.xml", SearchOption.AllDirectories);
            List<Mod> mods = new List<Mod>();

            mods.Add(new Mod() { id = "" });

            foreach (var file in files)
            {
                var mod = ModLoader.Deserialize<Mod>(file);
                mod.Folder = Path.GetDirectoryName(file);
                mods.Add(mod);
            }

            return mods;
        }

        /// <summary>
        /// Get all mod properties and their paths to get information about possible collisions when saving. WARNING - ExampleMod folder will be ignored
        /// </summary>
        public static List<Properties> GetModProperties(out List<string> paths)
        {
#if UNITY_EDITOR
            return Extensions.FindGameObjectsByType<BaseMod>(out paths).Select(item => item.properties).ToList();
#endif
            // im just trying to fix one issue, ignore this duct tape solution
            return new List<Properties>();
        }

        public static Mod GetMod(string path)
        {
            var files = Directory.GetFiles(path, "Mod.xml", SearchOption.AllDirectories);
            List<Mod> mods = new List<Mod>();

            foreach (var file in files)
            {
                var mod = ModLoader.Deserialize<Mod>(file);
                mod.Folder = Path.GetDirectoryName(file);
                return mod;
            }

            return null;
        }
    }    
}