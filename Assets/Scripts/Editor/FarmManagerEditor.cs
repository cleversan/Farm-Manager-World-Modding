using FarmManagerWorld.Editors.Wizards;
using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.Mods;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors
{    
    [CustomEditor(typeof(FarmManagerEditor))]
    public class FarmManagerEditor : EditorWindow
    {
        [MenuItem("Farm Manager/Farm Manager Editor Plugin")]

        public static void ShowWindow()
        {
            var editor = EditorWindow.GetWindow(typeof(FarmManagerEditor));
            editor.minSize = new Vector2(350, 350);
        }

        private void OnGUI()
        {
            titleContent.text = "Farm Manager Modding Panel";

            GUILayout.Label("Farm Manager Modding Panel", EditorStyles.boldLabel);
            GUILayout.Space(20);

            GUILayout.Label("1. Start with 'Create New Mod'");

            if (GUILayout.Button("Create new mod"))
            {
                ScriptableWizard.DisplayWizard("Mod Creator", typeof(CreateModWizard), "Create");
            }
            GUILayout.Space(20);
            GUILayout.Label("2. Add some content to it by using creators options below");
            GUILayout.Label("ENABLE GIZMOS IN THE SCENE. \nIT'S REQUIRED FOR PROPER WORKING OF THE WIZARDS", EditorStyles.boldLabel);

            if (GUILayout.Button("Building creator"))
            {
                BuildTypeEditor window = (BuildTypeEditor)EditorWindow.GetWindow(typeof(BuildTypeEditor), false, "Create building");
                window.minSize = new Vector2(150,550);
                window.maxSize = 2 * window.minSize;
                window.Show();
            }

            if (GUILayout.Button("Plant creator"))
            {
                ScriptableWizard.DisplayWizard("Plant creator", typeof(PlantWizard), "Create");
            }

            if (GUILayout.Button("Resource creator"))
            {
                ResourceTypeEditor window = (ResourceTypeEditor)EditorWindow.GetWindow(typeof(ResourceTypeEditor), false, "Create resource"); 
                window.minSize = new Vector2(150, 550);
                window.maxSize = 2 * window.minSize;
                window.Show();
            }

            if (GUILayout.Button("Animal creator"))
            {
                ScriptableWizard.DisplayWizard("Animal creator", typeof(AnimalWizard), "Create");
            }

            if (GUILayout.Button("Machine creator"))
            {
                MachineWizard wizard = (MachineWizard)ScriptableWizard.DisplayWizard("Machine creator", typeof(MachineWizard), "Create");
            }

            GUILayout.Space(20);
            GUILayout.Label("3. Compile your mod");

            if (GUILayout.Button("Compile mod"))
            {
                ScriptableWizard.DisplayWizard("Compile mod", typeof(SaveModWizard), "Compile");
            }            
        }
    }
}
