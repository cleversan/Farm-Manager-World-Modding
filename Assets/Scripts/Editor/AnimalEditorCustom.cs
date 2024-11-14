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

        void OnEnable()
        {
            editor = (AnimalEditor)target;
        }

        public override void OnInspectorGUI()
        {
            ModPopup();

            if (!Application.isPlaying && GUILayout.Button("Finalize for asset bundle"))
            {
                if (editor.animalMod.Validate() && editor.animalAsResource.Validate() && CheckMod(editor.gameObject, modID, false, true))
                {
                    FinalizeForAssetBundle(editor.animalAsResource.GetComponent<ResourceEditor>(), editor.animalAsResource.gameObject, modID, "resources");
                    FinalizeForAssetBundle(editor, editor.gameObject, modID, "animals");
                }
            }
        }
    }
}
#endif