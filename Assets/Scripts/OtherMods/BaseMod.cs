using System.Collections;
using System.Collections.Generic;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Translations;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    [ExecuteInEditMode]
    public abstract class BaseMod : MonoBehaviour
    {
        public abstract Properties properties { get; set; }

        private void Start()
        {
            if (properties.Translation == null)
                properties.Translation = new Translation(properties);
        }

        public bool ValidateTranslation()
        {
            return properties.Translation != null && properties.Translation.ValidateTranslation(properties.Name);
        }

        public virtual bool Validate()
        {
            return properties.ValidateProperties();
        }
    }
}