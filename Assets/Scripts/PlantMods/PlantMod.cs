using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Translations;
using FarmManagerWorld.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// PlantMod component that is attached to GameObject
    /// </summary>
    public class PlantMod : BaseMod
    {
        public GameObject model;
        public PlantProperties Plant;

        public override Properties properties { get => Plant; set => Plant = (PlantProperties)value; }
        public override bool Validate()
        {
            if (model.GetComponentsInChildren<Renderer>(true).Length <= 0)
            {
                Debug.LogError($"No renderers at Plant {Plant.Name} detected, validation failed");
                return false;
            }

            return Plant.ValidateProperties();
        }

    }
}