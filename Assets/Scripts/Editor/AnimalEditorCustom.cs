#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FarmManagerWorld.Editors;

namespace FarmManagerWorld.Editors
{
    [CustomEditor(typeof(AnimalEditor))]
    public class AnimalEditorCustom : SaveableEditorCustom
    {
        public AnimalEditor editor;

        public void OnEnable()
        {
            editor = target as AnimalEditor;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ModPopup();

            GUILayout.Space(20);
            BoolDrawer(ref editor.OverrideModObject, "Override Mod Object when finalizing?");

            if (!Application.isPlaying)
            {
                if (GUILayout.Button("Finalize for asset bundle") && editor.animalMod.Validate() && editor.animalAsResource.Validate() && CheckMod(editor.gameObject, _modID, false, true, _overrideModObject))
                {
                    FinalizeForAssetBundle(editor.animalAsResource.GetComponent<ResourceEditor>(), editor.animalAsResource.gameObject, _modID, "resources");
                    FinalizeForAssetBundle(editor, editor.gameObject, _modID, "animals");
                }

                RemoveEditorComponentButton();
            }
        }
    }
}
#endif