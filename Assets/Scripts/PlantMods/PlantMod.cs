using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Static;
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
            bool materialValidation = true;
            if (Plant.Type == PlantType.Grain)
                materialValidation = ValidateRenderersAndMaterials(StaticInformation.AllowedGrainShaderNames);
            else if (Plant.Type == PlantType.Bush)
                materialValidation = ValidateRenderersAndMaterials(StaticInformation.AllowedBushShaderNames);
            else
                materialValidation = ValidateRenderersAndMaterials(null);

            bool propertiesValidation = Plant.ValidateProperties();
            return propertiesValidation && materialValidation;
        }        
    }    
}