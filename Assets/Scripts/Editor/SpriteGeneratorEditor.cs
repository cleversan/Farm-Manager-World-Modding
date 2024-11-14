using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FarmManagerWorld.Editors.Wizards;
using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Utils;
using UnityEditor;
using UnityEngine;
using static FarmManagerWorld.Static.StaticInformation;

namespace FarmManagerWorld.Editors
{
    [CustomEditor(typeof(SpriteGenerator))]
    public class SpriteGeneratorEditor : SaveableEditorCustom
    {
        private SpriteGenerator spriteGenerator;

        private bool renderMachines = false;
        private bool renderBuidlings = true;
        private bool ignoreRegions = false;
        private string selectedRegion = "";

        // used for UI that im not really inclined to do rn, maybe in future
        private bool nullRegion;
        private int regionIndex = -1;
        private string[] regionOptions =
        {
            "Non regional buildings",   // 0
            "Europe",                   // 1
            "Asia",                     // 2
            "South America"             // 3
        };

        private void OnEnable()
        {
            spriteGenerator = target as SpriteGenerator;
            spriteGenerator.MainCamera = Camera.main;
        }

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("Rendering is only allowed when application is playing.");
                return;
            }

            ModPopup();

            if (GUILayout.Button(renderMachines ? "Disable machine rendering" : "Enable machine rendering"))
                renderMachines = !renderMachines;

            if (GUILayout.Button(renderBuidlings ? "Disable building rendering" : "Enable building rendering"))            
                renderBuidlings = !renderBuidlings;     
            
            if (renderBuidlings)
            {
                if (ignoreRegions)
                {
                    GUILayout.Label("All buildings, including regional versions will be rendered");
                    if (GUILayout.Button("Enable regional rendering"))
                        ignoreRegions = false;
                }
                else
                {
                    GUILayout.Label("Only regional buildings will be rendered, select region below");

                    AddTagToStringOnFromDropdownGUI(
                        "Select region to render",
                        "Region",
                        ref selectedRegion,
                        regionOptions,
                        ref regionIndex,
                        ref selectedRegion,
                        true);

                    if (GUILayout.Button("Disable regional rendering"))
                        ignoreRegions = true;
                }
            }

            if (string.IsNullOrEmpty(modID))
            {
                GUILayout.Label("Select mod before rendering");
                return;
            }

            if (renderMachines || renderBuidlings)
            {                
                if (GUILayout.Button("Render button"))
                {
                    List<GameObject> objectsToRender;
                    List<string> objectNames;
                    Mod renderMod = ModLoader.GetMods().FirstOrDefault(item => item.id == modID);
                    spriteGenerator.GetObjectsToRender(
                        renderMod, 
                        renderMachines, 
                        renderBuidlings, 
                        ignoreRegions, 
                        GetRenderRegion(), 
                        out objectsToRender, 
                        out objectNames);

                    spriteGenerator.RenderObjects(objectsToRender, objectNames);
                }                
            }
            else
            {
                GUILayout.Label("Select what you want to render before continuing");
            }
        }


        private Region GetRenderRegion()
        {
            switch(regionIndex)
            {
                default:
                case 0:
                    return Region.None;

                case 1:
                    return Region.Europe;

                case 2:
                    return Region.Asia;

                case 3:
                    return Region.SouthAmerica;
            }
        }
    }
}
