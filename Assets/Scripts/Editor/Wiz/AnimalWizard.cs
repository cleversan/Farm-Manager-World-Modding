using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Static;
using static FarmManagerWorld.Static.StaticInformation;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors.Wizards
{ 
    public class AnimalWizard : ScriptableWizard
    {
        AnimalMod animalMod;

        public Animator animalAnimator;
        public float sicknessChance = 10;
        public AnimalSize animalSize = AnimalSize.L;
        public Vector2 reproductionPerYear = new Vector2(1, 1);
        public string asResourceName;
        public float productionAge = 2;
        public bool isHidden = false;
        public List<string> allowedFoodPaths;
        public float maxFoodAmountPerFeed = 3;
        public List<ResourceAnimalProduction> production;
        public float basePrice;
        public Vector2 wanderTime = new Vector2(1, 1);
        public float walkingDistance;
        public float walkingSpeedBaseValue;
        public float walkingChance;
        public Sprite Sprite;
        public List<Material> MaterialVariants;
        public List<AudioClip> SoundFX;
        public float volume;
        public float soundDistance;
        public StaffMeshState vettingState;
        public StaffMeshState feedingState;
        public float radius = 1;


        private void OnEnable()
        {
            isValid = true;
            ShowUtility();
        }

        private void OnWizardCreate()
        {
            CreationProcess();
            AnimalAsResourceWizard window = (AnimalAsResourceWizard)EditorWindow.GetWindow(typeof(AnimalAsResourceWizard), false, "Wizard for AnimalAsResource");
            window.animalMod = animalMod;
            window.Name = animalMod.Animal.Name;
            window.resourceWizardType = StaticInformation.ResourceEnums.AnimalAsResource;
            window.Type = ResourceModType.AnimalsAsResources;
            window.Show();
            this.Close();
        }

        private void CreationProcess()
        {
            AnimalProperties properties = new AnimalProperties
            {
                SicknessChance = sicknessChance,
                AnimalSize = animalSize,
                ReproductionPerYear = reproductionPerYear,
                AsResourceName = asResourceName,
                ProductionAge = productionAge,
                IsHidden = isHidden,
                AllowedFood = allowedFoodPaths,
                MaxFoodAmountPerFeed = maxFoodAmountPerFeed,
                Production = production,
                BasePrice = basePrice,
                WanderTime = wanderTime,
                WalkingDistance = walkingDistance,
                WalkingChance = walkingChance,
                WalkingSpeedBaseValue = walkingSpeedBaseValue,
                SoundFX = SoundFX,
                Volume = volume,
                SoundDistance = soundDistance,
                Radius = radius
            };

            GameObject animalObject = new GameObject();
            animalObject.AddComponent<AnimalMod>();
            animalMod = animalObject.GetComponent<AnimalMod>();
            animalMod.Animal = properties;
            animalMod.Animal.Name = asResourceName;
            animalMod.gameObject.name = animalMod.Animal.Name + "_Object";

            Animator animator = animalMod.gameObject.AddComponent<Animator>();
            if (animalAnimator != null)
            {
                if (animalAnimator.avatar != null)
                    animator.avatar = animalAnimator.avatar;


                if (animalAnimator.runtimeAnimatorController != null)
                    animator.runtimeAnimatorController = animalAnimator.runtimeAnimatorController;
            }

            animator.cullingMode = AnimatorCullingMode.CullCompletely;
            animalMod.gameObject.AddComponent<AnimalEditor>();
        }
    }

    public class AnimalAsResourcePopup : EditorWindow
    {
        [HideInInspector]
        public AnimalMod MainObject;

        public static void ShowWindow(GameObject mainObject)
        {
        
            var instance = (AnimalAsResourcePopup)GetWindow(typeof(AnimalAsResourcePopup));
            instance.MainObject = mainObject.GetComponent<AnimalMod>();
        }

        private void OnGUI()
        {
            this.minSize = new Vector2(300, 75);
            this.maxSize = new Vector2(500, 75);
            GUILayout.Label("Would you like to create a AnimalAsResource \nfor this animal (can be created manually later)?", EditorStyles.boldLabel);
            if (GUILayout.Button("Yes"))
            {
                AnimalAsResourceWizard window = (AnimalAsResourceWizard)EditorWindow.GetWindow(typeof(AnimalAsResourceWizard), false, "Wizard for AnimalAsResource");
                window.animalMod = MainObject;
                window.Name = MainObject.Animal.Name;
                window.resourceWizardType = StaticInformation.ResourceEnums.AnimalAsResource;
                window.Type = ResourceModType.AnimalsAsResources;
                window.Show();
                this.Close();
            }

            if (GUILayout.Button("No"))
            {
                Close();
            }
        }
    }
}


