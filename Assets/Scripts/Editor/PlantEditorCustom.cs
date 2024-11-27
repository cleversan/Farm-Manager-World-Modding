#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Static;

namespace FarmManagerWorld.Editors
{ 
    [CustomEditor(typeof(PlantEditor))]
    [CanEditMultipleObjects]
    public class PlantEditorCustom : SaveableEditorCustom
    {
        public PlantEditor editor;

        private bool _customHarvestingMachine = false;
        private int _harvestingMachineIndex = -1;
        private string _harvestingMachineTag = "";

        private bool _customCollectingMachine = false;
        private int _collectingingMachineIndex = -1;
        private string _collectingingMachineTag = "";

        private bool _customBallingMachine = false;
        private int _ballingMachineIndex = -1;
        private string _ballingMachineTag = "";
               
        public void OnEnable()
        {;
            editor = target as PlantEditor;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ModPopup();

            if (Application.isPlaying)
            {

                GUILayout.Space(20);
                if (editor.plantMod.model != null)
                {
                    if (GUILayout.Button("Visualise plant"))
                        editor.Visualise();   
                }
                else                
                    GUILayout.Label("Assign \"Model\" in PlantMod component to visualise plant");

                GUILayout.Space(20);
            }
            else
            {
                AddTagToStringOnGUI(
                    ref _customHarvestingMachine,
                    _customHarvestingMachine ? "Disable custom machine tag" : "Enable custom machine tag",
                    "Input custom harvesting machine tag to add",
                    "Select machine tag that will be used to harvest (default chesttrailer)",
                    "Machine tag to add",
                    ref _harvestingMachineTag,
                    StaticInformation.MachineTags,
                    ref _harvestingMachineIndex,
                    ref editor.plantMod.Plant.PlantAttributes.HarvestingMachine,
                    true);

                if (editor.plantMod.Plant.PlantAttributes.LeaveHayAfterCrop)
                {
                    AddTagToStringOnGUI(
                    ref _customBallingMachine,
                    _customBallingMachine ? "Disable custom machine tag" : "Enable custom machine tag",
                    "Input custom machine tag that will be used to during balling",
                    "Select machine tag that will be used to during balling",
                    "Machine tag to add",
                    ref _ballingMachineTag,
                    StaticInformation.MachineTags,
                    ref _ballingMachineIndex,
                    ref editor.plantMod.Plant.PlantAttributes.BalingMachine,
                    true);

                    AddTagToStringOnGUI(
                    ref _customCollectingMachine,
                    _customCollectingMachine ? "Disable custom machine tag" : "Enable custom machine tag",
                    "Input custom machine tag that will be used to collect leftovers after balling",
                    "Select machine tag that will be used to collect leftovers after balling",
                    "Machine tag to add",
                    ref _collectingingMachineTag,
                    StaticInformation.MachineTags,
                    ref _collectingingMachineIndex,
                    ref editor.plantMod.Plant.PlantAttributes.CollectingMachine,
                    true);
                }

                GUILayout.Space(20);

                if (GUILayout.Button("Apply harvest values"))
                {
                    float growEfficiency = editor.GetGrowEfficiency(editor.HarvestFromWholeField);
                    if (!float.IsNaN(growEfficiency) && !float.IsInfinity(growEfficiency))
                    {
                        editor.plantMod.Plant.PlantAttributes.GrowEfficiency = growEfficiency;
                        EditorUtility.SetDirty(editor.plantMod);
                    }
                }

                if (GUILayout.Button("Apply seed values"))
                {
                    SeedResourceMod seedResource = FindObjectsByType<SeedResourceMod>(FindObjectsSortMode.None).
                        FirstOrDefault(item => item.Resource != null && item.Resource.PlantName.Equals(editor.plantMod.Plant.Name));

                    if (seedResource != null)
                    { 
                        float seedPerSquareMeter = editor.CalculateSeedPerSquareMeter(editor.SeedsNeededForField);
                        if (!float.IsNaN(seedPerSquareMeter) && !float.IsInfinity(seedPerSquareMeter))
                        {
                            seedResource.Resource.SeedAmountPerSqaureMeter = seedPerSquareMeter;
                            EditorUtility.SetDirty(seedResource.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogError($"No seed resource with this plant name has been found: {editor.plantMod.Plant.Name}");
                    }
                }

                GUILayout.Space(20);
                GUILayout.Label("Plant visualisation available in play mode");;
            }

            if (!Application.isPlaying && GUILayout.Button("Finalize for asset bundle"))
            {
                bool plantValidated, seedResourceValidated, foliageResourceValidated;
                plantValidated = editor.plantMod.Validate() && CheckMod(editor.gameObject, _modID, false, true);
                seedResourceValidated = editor.seedResourceMod != null && editor.seedResourceMod.Validate() && CheckMod(editor.seedResourceMod.gameObject, _modID, false, false);
                foliageResourceValidated = editor.foliageResourceMod != null && editor.foliageResourceMod.Validate() && CheckMod(editor.foliageResourceMod.gameObject, _modID, false, false);

                if (plantValidated && seedResourceValidated && foliageResourceValidated)
                {
                    FinalizeForAssetBundle(editor.seedResourceMod.GetComponent<ResourceEditor>(), editor.seedResourceMod.gameObject, _modID, "resources");
                    FinalizeForAssetBundle(editor.foliageResourceMod.GetComponent<ResourceEditor>(), editor.foliageResourceMod.gameObject, _modID, "resources");
                    FinalizeForAssetBundle(editor, editor.gameObject, _modID, "plants");
                }
            }
        }
    }
}

#endif