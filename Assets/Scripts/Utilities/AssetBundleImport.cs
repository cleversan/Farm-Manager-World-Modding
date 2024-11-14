using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.Mods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmManagerWorld.Utils
{
    public class AssetBundleImport : MonoBehaviour
    {
        public static AssetBundleImportCoroutine Instance;

        public static void ImportAssetBundle(Mod mod, string[] paths)
        {
            if (Instance == null)            
                Instance = new GameObject("AssetBundle Importer", typeof(AssetBundleImportCoroutine)).GetComponent<AssetBundleImportCoroutine>();
            
            Instance.StartCoroutine(Instance.Import(mod, paths));
        }
    }

    public class AssetBundleImportCoroutine : MonoBehaviour
    {
        public IEnumerator Import(Mod mod, string[] paths)
        {
            for(int pathIndex = 0; pathIndex < paths.Length; ++pathIndex)
                yield return GetAssetBundle(paths[pathIndex], mod);

            mod.assetBundles.Clear();
            DestroyImmediate(gameObject);
        }

        IEnumerator GetAssetBundle(string path, Mod mod)
        {
            var loadRequest = AssetBundle.LoadFromFileAsync(path);
            yield return loadRequest;
            var assetBundle = loadRequest.assetBundle;
            if (assetBundle != null)
            {
                if (!mod.assetBundles.Contains(assetBundle))
                {
                    mod.assetBundles.Add(assetBundle);
                }
            }
            var assetLoadRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
            
            yield return assetLoadRequest;

            #region Loading mods

            foreach (var asset in assetLoadRequest.allAssets)
            {
                var obj = asset as GameObject;
                if (obj.GetComponent<AnimalMod>() != null)
                {
                    ModLoader.LoadAnimalFromAssetBundle(ref obj, mod, out AnimalMod oldAnimalMod);
                    DestroyImmediate(oldAnimalMod, true);
                }
                else if (obj.GetComponent<BuildingMod>() != null)
                {
                    ModLoader.LoadBuildingFromAssetBundle(ref obj, mod, out List<Component> componentsToDestroy);
                    Extensions.DestroyComponents(componentsToDestroy);
                }
                else if (obj.GetComponent<PlantMod>() != null)
                {
                    ModLoader.LoadPlantFromAssetBundle(ref obj, mod, out PlantMod oldPlantMod);
                    DestroyImmediate(oldPlantMod, true);
                }
                else if (obj.GetComponent<ResourceMod>() != null)
                {
                    ModLoader.LoadResourceFromAssetBundle(ref obj, mod, out ResourceMod oldResourceMod);
                    DestroyImmediate(oldResourceMod, true);
                }
                else if (obj.GetComponent<MachineBaseMod>() != null)
                {
                    ModLoader.LoadMachineBaseFromAssetBundle(ref obj, mod, out List<Component> componentsToDestroy);
                    Extensions.DestroyComponents(componentsToDestroy);
                }
                else if (obj.GetComponent<RegionalModelMod>() != null)
                {
                    ModLoader.LoadRegionalModelFromAssetBundle(ref obj, out List<Component> componentsToDestroy);
                    Extensions.DestroyComponents(componentsToDestroy);
                }
            }

            #endregion

            #region Instancing objects

            GameObject modContainer = new GameObject($"{assetBundle.name} container");

            if (ModLoader.loadedAnimalsPrefabs.Count > 0)
                foreach (var animal in ModLoader.loadedAnimalsPrefabs)
                    InstantiateModObject(animal, modContainer);

            if (ModLoader.loadedBuildingsPrefabs.Count > 0)
                foreach (var building in ModLoader.loadedBuildingsPrefabs)
                    InstantiateModObject(building, modContainer);

            if (ModLoader.loadedMachinePrefabs.Count > 0)
                foreach (var machine in ModLoader.loadedMachinePrefabs)
                    InstantiateModObject(machine, modContainer);

            if (ModLoader.loadedResourcesPrefabs.Count > 0)
                foreach (var resource in ModLoader.loadedResourcesPrefabs)
                    InstantiateModObject(resource, modContainer);

            if (ModLoader.loadedPlantsPrefabs.Count > 0)
                foreach (var plant in ModLoader.loadedPlantsPrefabs)
                    InstantiateModObject(plant, modContainer);

            if (ModLoader.loadedRegionalPrefabs.Count > 0)
                foreach (var regionalModel in ModLoader.loadedRegionalPrefabs)
                    InstantiateModObject(regionalModel, modContainer);

            #endregion

            yield return assetBundle.UnloadAsync(false);

            ModLoader.ClearMods();
        }

        private void InstantiateModObject(GameObject modObject, GameObject modContainer = null)
        {
            GameObject obj = Instantiate(modObject);
            if (modContainer != null)
                obj.transform.parent = modContainer.transform;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}