#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FarmManagerWorld.Static;
using FarmManagerWorld.Modding.Mods;

namespace FarmManagerWorld.Editors
{
    [CustomEditor(typeof(ResourceEditor))]
    public class ResourceEditorCustom : SaveableEditorCustom
    {
        public ResourceEditor editor;

        private string _transportToolTag = "";
        private int _selectedTransportToolIndex = -1;

        private bool _customSowingMachine = false;
        private string _sowingMachineTag = "";
        private int _selectedSowingMachineIndex = -1;

        public void OnEnable()
        {
            editor = target as ResourceEditor;
        }

        public override void OnInspectorGUI()
        {
            ModPopup();
            if (!Application.isPlaying)
            {
                // Transport tool tag
                AddTagToStringOnFromDropdownGUI("Select transport tool for resource", 
                    "Transport tool tag to select", 
                    ref editor.resourceMod.resource.TransportToolTag, 
                    StaticInformation.TransportToolTags, 
                    ref _selectedTransportToolIndex, 
                    ref _transportToolTag, 
                    true);

                if (editor.resourceMod is SeedResourceMod seedResource)
                {
                    AddTagToStringOnGUI(
                        ref _customSowingMachine,
                        _customSowingMachine ? "Disable custom machine tag" : "Enable custom machine tag",
                        "Input custom machine tag to add",
                        "Select sowing machine type for seed",
                        "Sowing machine to select",
                        ref _sowingMachineTag,
                        StaticInformation.SowingMachinesTags,
                        ref _selectedSowingMachineIndex,
                        ref seedResource.Resource.SowingMachine,
                        true);
                }

                GUILayout.Space(20);
                BoolDrawer(ref editor.OverrideModObject, "Override Mod Object");

                if (GUILayout.Button("Finalize for asset bundle"))
                {
                    if (editor.resourceMod.Validate() && CheckMod(editor.gameObject, _modID, false, true, _overrideModObject))
                        FinalizeForAssetBundle(editor, editor.gameObject, _modID, "resources");
                }
            }
        }
    }
}
#endif