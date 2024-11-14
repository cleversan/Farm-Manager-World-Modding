using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Static;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors.Wizards
{
    public class PlantWizard : ScriptableWizard
    {
        GameObject mod;

        public Sprite Icon;
        public string Name;
        [Header("If set to \"Bush\", Growth State can be added through window \n(Growth State can still be edited via Inspector)")]
        public PlantType PlantType;

        private void OnWizardCreate()
        {
            mod = new GameObject("PlantModel");
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(mod.transform);
            cube.transform.localPosition = Vector3.zero;

            PlantProperties properties = new PlantProperties();
            properties.Icon = Icon;

            PlantMod plantMod = new GameObject().AddComponent<PlantMod>();
            plantMod.Plant = properties;
            plantMod.Plant.SetBasicType();
            plantMod.Plant.Name = Name;
            plantMod.gameObject.name = plantMod.Plant.Name + "_Plant";
            mod.transform.SetParent(plantMod.transform);
            plantMod.model = mod;
            plantMod.gameObject.AddComponent<PlantEditor>();
            plantMod.Plant.Type = PlantType;
            if (plantMod.Plant.Type == PlantType.Bush)
            {
                PlantPopup window = (PlantPopup)GetWindow(typeof(PlantPopup), true, "");
                window.MainObject = plantMod;
                window.Show();
            }
            else
            {
                FoliageResourceWizard.ShowWizard(plantMod);
            }
            Close();
        }

        private void Awake()
        {
            this.minSize = new Vector2(400, 240);
        }
    }

    public class GrowthWizard : ScriptableWizard
    {
        [HideInInspector]
        public PlantMod MainObject;

        [Range(0.0f, 1.0f)]
        [Header("When does GrowingState starts taking effect, \n" +
            "used in calculation of the plant scale based on the statement:\n" +
            "Mathf.Clamp(Growth - state.Start + state.Offset, 0, state.End - state.Start) * state.Multiplier.\n" +
            "This will only take effect in the axis that has GrowScale marked as true")]
        public float Start = 0f;
        [Range(0.0f, 1.0f)]
        public float End = 1f;
        public float Offset = 0;
        public float Multiplier = 1f;
        [Header("Object that will be affected by this Growth State \n(will still be affected by other scaling measures)")]
        public GameObject[] Objects;

        [Header("Fruit mesh that will be rendered at locations of Fruit Dummies")]
        public Mesh FruitMesh;
        [Header("Possible locations for fruits to spawn")]
        public Transform[] FruitsDummies;

        [Header("Object that should not be affe by any scaling whatsoever")]
        public GameObject StaticObject;

        private void Awake()
        {
            this.minSize = new Vector2(570, 500);
        }

        private void OnWizardCreate()
        {
            GrowingStateProperties growingState = new GrowingStateProperties
            {
                Start = Start,
                End = End,
                Offset = Offset,
                Multiplier = Multiplier,
                Objects = Objects,
                FruitMesh = FruitMesh,
                FruitsDumies = FruitsDummies,
                StaticObject = StaticObject
            };

            MainObject.Plant.GrowStates.Add(growingState);

            PlantPopup window = (PlantPopup)GetWindow(typeof(PlantPopup), false, "");
            window.MainObject = MainObject;
            window.Show();
            this.Close();
        }
    }

    public class PlantPopup : EditorWindow
    {
        [HideInInspector]
        public PlantMod MainObject;

        public static void ShowWindow()
        {
            GetWindow(typeof(PlantPopup));
        }

        private void OnGUI()
        {
            this.minSize = new Vector2(500, 100);
            if (MainObject.Plant.Type == PlantType.Bush)
            {
                GUILayout.Label($"Would you like to add Growth State? (currently {MainObject.Plant.GrowStates.Count})", EditorStyles.boldLabel);
                GUILayout.Label("Growth states are used primarly when rendering plants with fruits or objects unaffected by growth (i.e. wooden support )");
                
                if (GUILayout.Button("Yes"))
                {
                    GrowthWizard window = (GrowthWizard)GetWindow(typeof(GrowthWizard), false, "Adding new Growth State");
                    window.MainObject = MainObject;
                    window.Show();
                    Close();
                }

                if (GUILayout.Button("No"))
                {
                    FoliageResourceWizard window = (FoliageResourceWizard)GetWindow(typeof(FoliageResourceWizard), false, "Wizard for foliage");
                    window.Name = MainObject.GetComponent<PlantMod>().Plant.Name;
                    window.plant = MainObject;
                    window.resourceWizardType = StaticInformation.ResourceEnums.FoliageResource;
                    window.Type = ResourceModType.FoliageResources;
                    window.Show();
                    Close();
                }
            }            
        }
    }

    public class SeedPopup : EditorWindow
    {
        [HideInInspector]
        public PlantMod MainObject;

        public static void ShowWindow()
        {
            GetWindow(typeof(SeedPopup));
        }

        private void OnGUI()
        {
            this.minSize = new Vector2(500, 100);
            SeedResourceWizard window = (SeedResourceWizard)EditorWindow.GetWindow(typeof(SeedResourceWizard), false, "Wizard for Seeds");
            window.Name = MainObject.Plant.Name + "Seed";
            window.PlantName = MainObject.Plant.Name;
            window.plant = MainObject;

            window.resourceWizardType = StaticInformation.ResourceEnums.SeedResource;
            window.Type = ResourceModType.SeedResources;
            window.Show();
            this.Close();
        }
    }

}