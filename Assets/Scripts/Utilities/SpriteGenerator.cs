using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using FarmManagerWorld.Editors;
#endif
using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.Mods;
using UnityEditor;
using UnityEngine;
using static FarmManagerWorld.Static.StaticInformation;

namespace FarmManagerWorld.Utils
{
#if UNITY_EDITOR
    public class SpriteGenerator : SaveableEditor
    {
        public Camera MainCamera;
        private Camera whiteCam;
        private Camera blackCam;

        private int screenWidth;
        private int screenHeight;

        private Texture2D textureBlack;
        private Texture2D textureWhite;
        private Texture2D textureTransparentBackground;

        [HideInInspector] public bool IsRendering = false;
        private GameObject ObjectSpawner;

        private string ScreenCapDirectory
        {
            get
            {
                var baseLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                var complexLocation = baseLocation + "\\FMW_Modding\\Screens\\";
                if (!Directory.Exists(complexLocation))
                    Directory.CreateDirectory(complexLocation);

                return complexLocation;
            }
        }

        private void Awake()
        {
            ObjectSpawner = GameObject.Find("ObjectSpawner");

            CreateSeparateCamera("White background camera", ref whiteCam, Color.white);
            CreateSeparateCamera("Black background camera", ref blackCam, Color.black);

            CacheAndInitializeFields();
        }

        void CreateSeparateCamera(string cameraName, ref Camera cameraToAssign, Color backgroundColor)
        {
            cameraToAssign = new GameObject(cameraName, typeof(Camera)).GetComponent<Camera>();
            cameraToAssign.CopyFrom(MainCamera);
            cameraToAssign.backgroundColor = backgroundColor;
            cameraToAssign.transform.SetParent(gameObject.transform, true);
        }

        void CacheAndInitializeFields()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            textureBlack = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
            textureWhite = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
            textureTransparentBackground = new Texture2D(screenWidth, screenHeight, TextureFormat.ARGB32, false);
        }

        void SavePng(string imgName)
        {
            var pngShot = textureTransparentBackground.EncodeToPNG();
            File.WriteAllBytes(ScreenCapDirectory + imgName + ".png", pngShot);
        }

        void RenderCamToTexture(Camera cam, Texture2D tex)
        {
            cam.enabled = true;
            cam.Render();
            WriteScreenImageToTexture(tex);
            cam.enabled = false;
        }

        void WriteScreenImageToTexture(Texture2D tex)
        {
            tex.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            tex.Apply();
        }

        void CalculateOutputTexture()
        {
            Color color;
            for (int y = 0; y < textureTransparentBackground.height; ++y)
            {
                // each row
                for (int x = 0; x < textureTransparentBackground.width; ++x)
                {
                    // each column
                    float alpha = 1.0f - (textureWhite.GetPixel(x, y).r - textureBlack.GetPixel(x, y).r);
                    color = alpha <= 0 ? Color.clear : textureBlack.GetPixel(x, y) / alpha;

                    color.a = alpha;
                    textureTransparentBackground.SetPixel(x, y, color);
                }
            }
        }
        
        public void RenderObjects(List<GameObject> objectsToRender, List<string> spriteNames)
        {
            StartCoroutine(RenderObjectsCoroutine(objectsToRender, spriteNames));
        }

        IEnumerator RenderObjectsCoroutine(List<GameObject> objectsToRender, List<string> spriteNames)
        {
            IsRendering = true;
            for (int i = 0; i < objectsToRender.Count; ++i) 
            {
                yield return RenderOneObject(objectsToRender[i], spriteNames[i], false);


                if (objectsToRender[i].TryGetComponent(out MachineBaseMod machine))
                {
                    yield return RenderOneObject(objectsToRender[i], spriteNames[i] + "_destroyed", true);
                }
            }

            Debug.Log($"Rendering has been finished");
            IsRendering = false;
        }

        IEnumerator RenderOneObject(GameObject objectToRender, string pngName, bool updateDestruction)
        {
            yield return new WaitForEndOfFrame();
            ObjectSpawner.RemoveAllChildren();
            var instance = Instantiate(objectToRender, ObjectSpawner.transform);
            Selection.SetActiveObjectWithContext(instance, null);
            Extensions.SetMaterialPropertyBlock();
            instance.transform.position = new Vector3(0, 0, 0);

            int z = 0;
            float scale = 1;
            var bounds = new Bounds(instance.transform.position, Vector3.zero);
            var meshRenders = instance.GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshRenders)
            {
                if (mesh.gameObject.name.Contains("ground"))
                    continue;

                Bounds meshBounds = mesh.bounds;
                bounds.Encapsulate(meshBounds);
            }

            if (updateDestruction)
                Extensions.UpdateMaterialPropertyBlock(instance, 1.0f, "_BlendAmount");

            var max = new Vector2(bounds.size.x, bounds.size.z).magnitude;
            if (max > bounds.size.y)
            {
                instance.transform.position = new Vector3(0, (max - bounds.size.y) / 3.14f, 0);

                while (Extensions.IsObjectVisible(MainCamera, bounds))
                {
                    if (z > 500)
                        break;

                    instance.transform.localScale = new Vector3(scale, scale, scale);

                    scale += 0.1f;
                    z++;

                    yield return new WaitForEndOfFrame();

                    bounds = new Bounds(instance.transform.position, Vector3.zero);
                    foreach (var mesh in meshRenders)
                    {
                        Bounds meshBounds = mesh.bounds;
                        bounds.Encapsulate(meshBounds);
                    }
                }

                scale -= 0.25f;
                instance.transform.localScale = new Vector3(scale, scale, scale);

                yield return new WaitForEndOfFrame();
                RenderCamToTexture(blackCam, textureBlack);
                RenderCamToTexture(whiteCam, textureWhite);
                CalculateOutputTexture();

                SavePng(pngName);
            }
        }

        /// <summary>
        /// Function to get all objects that will be rendered
        /// </summary>
        /// <param name="modToRender">Mod that will be considered when generating sprites</param>
        /// <param name="renderMachines">Should generate machine sprites</param>
        /// <param name="renderBuildings">Should generate building sprites</param>
        /// <param name="ignoreRegion">Should ignore regions (only when generating buildings)</param>
        /// <param name="regionToRender">Region of the building</param>
        /// <param name="objectsToRender">Objects that will be returned</param>
        /// <param name="objectNames">Objects names that will be used as sprite names</param>
        public void GetObjectsToRender(Mod modToRender, bool renderMachines, bool renderBuildings, bool ignoreRegion, Region regionToRender, out List<GameObject> objectsToRender, out List<string> objectNames)
        {
            objectsToRender = new List<GameObject>();
            objectNames = new List<string>();

            ModLoader.LoadMod(modToRender);

            // its not gonna be the most efficient but this function should be rarely used so whatever i guess
            if (renderMachines)
            {
                List<MachineBaseMod> machines = Extensions.FindGameObjectsByType<MachineBaseMod>().ToList().FindAll(item => item.properties.Mod.id.Equals(modToRender.id));
                for(int i = 0; i < machines.Count; i++) 
                {
                    if (objectNames.Contains(machines[i].properties.Name + "_MachineSprite"))
                        continue;

                    objectsToRender.Add(machines[i].gameObject);
                    objectNames.Add(machines[i].properties.Name + "_MachineSprite");
                }
            }

            if (renderBuildings)
            {
                List<string> buildingsToRender = new List<string>();
                List<BuildingMod> buildings = Extensions.FindGameObjectsByType<BuildingMod>().ToList().FindAll(item => item.properties.Mod.id.Equals(modToRender.id));
                for (int i = 0; i < buildings.Count; i++)
                {
                    if (buildings[i].IsRegional)
                    {
                        buildingsToRender.Add(buildings[i].building.Name);
                        continue;
                    }

                    if (!ignoreRegion && regionToRender != Region.None)
                        continue;


                    if (objectNames.Contains(buildings[i].properties.Name + "_BuildingSprite"))
                        continue;

                    objectsToRender.Add(buildings[i].gameObject);
                    objectNames.Add(buildings[i].properties.Name + "_BuildingSprite");
                }

                if (ignoreRegion || regionToRender != Region.None)
                {
                    List<RegionalModelMod> regionalModels = Extensions.FindGameObjectsByType<RegionalModelMod>().ToList().FindAll(item => buildingsToRender.Contains(item.RegionalModel.Name));
                    for (int i = 0; i < regionalModels.Count; i++)
                    {
                        if (!ignoreRegion && regionToRender != regionalModels[i].RegionalModel.Region)
                            continue;

                        if (objectNames.Contains($"{regionalModels[i].RegionalModel.Name}_{regionToRender}_BuildingSprite"))
                            continue;

                        objectsToRender.Add(regionalModels[i].gameObject);
                        objectNames.Add($"{regionalModels[i].RegionalModel.Name}_{regionToRender}_BuildingSprite");
                    }
                }       
            }
        }
    }
#endif
}