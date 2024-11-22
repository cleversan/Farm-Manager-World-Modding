#if UNITY_EDITOR

using UnityEngine;
using FarmManagerWorld.Modding.Mods;

namespace FarmManagerWorld.Editors
{
    [RequireComponent(typeof(ResourceMod))]
    [ExecuteInEditMode]
    public class ResourceEditor : SaveableEditor
    {
        public ResourceMod resourceMod;

        private void Awake()
        {
            resourceMod = GetComponent<ResourceMod>();
        }

        void OnEnable()
        {
            resourceMod = GetComponent<ResourceMod>();
        }
    }
}

#endif