using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Static;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors
{
    [CustomEditor(typeof(BaseMod), true)]
    [CanEditMultipleObjects]
    public class BaseModEditor : Editor
    {
        private bool canAddSaveableEditor
        {
            get
            {   
                if (!StaticInformation.EditorTypes.ContainsKey(BaseMod.properties.BasicType))
                    return false;

                return BaseMod.GetComponentInParent<SaveableEditor>() == null;
            }
        }

        private BaseMod _baseMod;
        private BaseMod BaseMod
        {
            get
            {
                if (_baseMod == null)
                    _baseMod = target as BaseMod;

                return _baseMod;
            }
        }

        public override void OnInspectorGUI()
        {
            if (canAddSaveableEditor)
            {
                if(GUILayout.Button("Add editor"))
                {
                    BaseMod.gameObject.AddComponent(StaticInformation.EditorTypes[BaseMod.properties.BasicType]);                    
                }
                GUILayout.Space(20);
            }

            DrawDefaultInspector();

        }
    }
}
