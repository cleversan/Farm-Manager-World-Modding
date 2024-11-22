#if UNITY_EDITOR
using FarmManagerWorld.Modding.Mods;
using UnityEngine;

namespace FarmManagerWorld.Editors
{
    [RequireComponent(typeof(AnimalMod))]
    [ExecuteInEditMode]
    public class AnimalEditor : SaveableEditor
    {
        public AnimalMod animalMod;
        public ResourceMod animalAsResource;

        private void Awake()
        {
            animalMod = GetComponent<AnimalMod>();
        }

        void OnEnable()
        {
            animalMod = GetComponent<AnimalMod>();
        }
    }
}

#endif