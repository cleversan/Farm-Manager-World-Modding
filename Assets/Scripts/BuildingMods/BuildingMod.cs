using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;
using System.Collections.Generic;
using FarmManagerWorld.Static;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using FarmManagerWorld.Editors;
#endif
namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// BuildingMod component that is attached to GameObject
    /// </summary>
    public abstract class BuildingMod : BaseMod, ICopyTo
    {
        [HideInInspector]
        public GameObject model;
        public abstract BuildingProperties building { get; set; }

        public override Properties properties { get => building; set => building = (BuildingProperties)value; }

        public List<GameObject> roofFirePoints = new List<GameObject>();
        public List<GameObject> wallFirePoints = new List<GameObject>();
        
        private bool allowSkinnedMeshRenderers
        {
            get
            {
#if UNITY_EDITOR
                if (TryGetComponent(out BuildingEditor editor))
                    return editor.AllowSkinnedMeshRenderers;
#endif
                return false;
            }
        }

        private bool checkMaterials
        {
            get
            {
#if UNITY_EDITOR
                if (IsRegional) // ignore checking materials in regional buildings
                    return false;

                if (TryGetComponent(out BuildingEditor editor))
                    return editor.CheckMaterials;
#endif
                return false;
            }
        }

        public bool NeedParkingSpaceCandidate
        {
            get { return building.BasicType == "LogisticBuilding" || building.BasicType == "VetBuilding" || building.BasicType == "MechanicBuilding"; }
        }

        public bool IsRegional
        {
            get
            {
                return gameObject.GetComponentsInChildren<RegionalModelModManager>(true).Length > 0;
            }
        }

        public override bool Validate()
        {
            bool validateRoadconnectors = ValidateRoadconnectors();
            bool validateParkingSpaceColliders = ValidateParkingSpaceColliders();
            bool validateParkingSpaceCandidates = ValidateParkingSpaceCandidates();
            bool validateSkinnedMeshRenderers = ValidateSkinnedMeshRenderers();
            bool validateMaterials = ValidateMaterials();
            bool validateRegionals = ValidateRegionalModels();
            bool validateBuildingProperties = building.ValidateProperties();

            return validateRoadconnectors && validateParkingSpaceColliders && validateParkingSpaceCandidates && validateSkinnedMeshRenderers && validateMaterials && validateRegionals && validateBuildingProperties;
        }

        private bool ValidateRoadconnectors()
        {
            if (StaticInformation.BuildingsWithRoadconnector.ContainsKey(GetType()))
            {
                if (!StaticInformation.BuildingsWithRoadconnector[GetType()])
                {
                    Debug.Log("Building does not need roadconnectors, continuing");
                    return true;
                }
            }
            else
            {
                Debug.LogError($"Given object {GetType()} is not included in BuildingsWithRoadconnector Dictionary, possible corruption of the asset, returning false");
                return false;
            }

            int roadconnectorCount = Extensions.FindChildrenWithNameContains(gameObject, true, true, "roadconnector").Count;

            if (roadconnectorCount <= 0)
                Debug.LogError($"Building {building.Name} has no roadconnectors but require it, validation failed");

            return roadconnectorCount > 0;
        }

        private bool ValidateParkingSpaceColliders()
        {
            if (properties.BasicType != "GarageBuilding")
                return true;

            bool valid = false;

            List<GameObject> parkings = gameObject.FindChildrenWithNameContains(false, true, "ParkingSpaceCollider");
            for (int i = 0; i < parkings.Count; ++i)
            {
                if (parkings[i].GetComponent<BoxCollider>() != null)
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
                Debug.LogError($"Building {building.Name} has no ParkingSpaceCollider object or that object does not have BoxCollider component attached, validation failed");

            return valid;
        }

        private bool ValidateParkingSpaceCandidates()
        {
            if (properties.BasicType != "LogisticBuilding" && properties.BasicType != "MechanicBuilding" && properties.BasicType != "VetBuilding")
                return true;

            bool validated = GetComponentsInChildren<ParkingSpaceProperties>().Length > 0;

            if (!validated)
                Debug.LogError($"Building {building.Name} has no ParkingSpaceProperties components in children, validation failed");

            return validated;
        }

        private bool ValidateSkinnedMeshRenderers()
        {
            int skinnedMeshCount = GetComponentsInChildren<SkinnedMeshRenderer>().Length;
            if (skinnedMeshCount > 0)
            {
                if (allowSkinnedMeshRenderers)
                {
                    Debug.LogWarning($"SkinnedMeshRenderers detected in building {building.Name}. Meshes using SkinnedMeshRenderer will not have outlines.");
                    return true;
                }
                else
                {
                    Debug.LogError($"SkinnedMeshRenderers detected in building {building.Name}, set \"AllowSkinnedMeshRenderers\" to true in BuildingEditor " +
                        $"component to allow compilation or press \"Convert SkinnedMeshRenderers\" button to convert to them to MeshRenderers");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateMaterials()
        {
            if (checkMaterials)            
                return ValidateRenderersAndMaterials(StaticInformation.AllowedBuildingShaderNames);
            
            return true;
        }      

        private bool ValidateRegionalModels()
        {
            if (IsRegional)
            {
                var regionalModels = GetComponentsInChildren<RegionalModelMod>();
                for(int  i = 0; i < regionalModels.Length; ++i)                
                    regionalModels[i].RegionalModel.Name = building.Name;
            }

            return true;
        }

        public void CopyTo(UnityEngine.Object oldObject, UnityEngine.Object newObject, ref List<Component> componentsToDestroy)
        {
            BuildingMod oldBuildingMod = oldObject as BuildingMod;
            BuildingMod newBuildingMod = newObject as BuildingMod;

            newBuildingMod.model = oldBuildingMod.model;
            newBuildingMod.building = oldBuildingMod.building;
            newBuildingMod.roofFirePoints = oldBuildingMod.roofFirePoints;
            newBuildingMod.wallFirePoints = oldBuildingMod.wallFirePoints;
            newBuildingMod.building.Mod = oldBuildingMod.building.Mod;

            List<EntranceProperties> entranceProperties = newBuildingMod.gameObject.GetComponentsInChildren<EntranceProperties>(true).ToList();

            for (int i = 0; i < entranceProperties.Count; ++i)
            {
                componentsToDestroy.Add(entranceProperties[i]);
                EntranceProperties newEntrance = entranceProperties[i].gameObject.AddComponent<EntranceProperties>();
                newEntrance.CopyTo(entranceProperties[i], newEntrance, ref componentsToDestroy);
            }

            List<RegionalModelModManager> regionalManagers = newBuildingMod.gameObject.GetComponentsInChildren<RegionalModelModManager>(true).ToList();

            for (int i = 0; i < regionalManagers.Count; ++i)
            {
                componentsToDestroy.Add(regionalManagers[i]);
                RegionalModelModManager newRegionalManager = regionalManagers[i].gameObject.AddComponent<RegionalModelModManager>();
                newRegionalManager.CopyTo(regionalManagers[i], newRegionalManager, ref componentsToDestroy);
            }
        }
    }
}