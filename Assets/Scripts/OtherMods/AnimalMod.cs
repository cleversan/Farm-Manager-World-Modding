using FarmManagerWorld.Modding.ObjectProperties;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// AnimalMod component that is attached to GameObject
    /// </summary>
    public class AnimalMod : BaseMod
    {
        public AnimalProperties Animal;
        public override Properties properties { get => Animal; set => Animal = (AnimalProperties)value; }
        public GameObject animalBones;
        public Material[] AdultMaterialVariants;
        public Material[] YoungMaterialVariants;

        public GameObject YoungAnimalGameObject;
        public GameObject AdultAnimalGameObject;

        public override bool Validate()
        {
            bool validation = true;
            if (animalBones == null)
            {
                Debug.LogError("AnimalBoxes cannot be null, validation failed");
                validation = false;
            }

            bool isYoungMaterialVariants = YoungMaterialVariants != null;
            bool isAdultMaterialVariants = AdultMaterialVariants != null;
            if (isYoungMaterialVariants != isAdultMaterialVariants)
            {
                Debug.LogError("Disparity with materials variants detected, either young or adult material variants is null, validation failed");
                validation = false;
            }
            else if (isYoungMaterialVariants && YoungMaterialVariants.Length != AdultMaterialVariants.Length)
            {
                Debug.LogError("Disparity with materials variants detected, adult and young animals should have equal amount of materials, validation failed");
                validation = false;
            }

            bool isYoungAnimalGameObject = YoungAnimalGameObject != null;
            bool isAdultAnimalGameObject = AdultAnimalGameObject != null;
            if (isYoungAnimalGameObject != isAdultAnimalGameObject)
            {
                Debug.LogError("Disparity with young and adult gameobjects detected, one of them is null and other should not be, validation failed");
                validation = false;
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool animalPropertiesValidation = Animal.ValidateProperties();
            return animalPropertiesValidation && validation;
        }
    }
}