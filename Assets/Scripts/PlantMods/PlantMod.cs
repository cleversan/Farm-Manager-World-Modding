using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Static;
using System;
using System.Collections.Generic;
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
            bool validation = true;
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length <= 0)
            {
                Debug.LogError($"No renderers at Plant {Plant.Name} detected, validation failed");
                validation = false;
            }
            else
            {                
                List<Renderer> invalidRenderers = new List<Renderer>();
                List<Material> invalidMaterials = new List<Material>(); 
                for(int rendererIndex = 0; rendererIndex < renderers.Length; ++rendererIndex)
                {
                    if (renderers[rendererIndex].sharedMaterials.Length <= 0) // check if renderer has any materials, if not, set validation to false, save name and iterate
                    {
                        invalidRenderers.Add(renderers[rendererIndex]);
                        validation = false;
                        continue;
                    }

                    for(int materialIndex = 0; materialIndex < renderers[rendererIndex].sharedMaterials.Length; ++materialIndex)  // check if each materials shader is ok (only matters for grain and bushes)
                    {
                        if (renderers[rendererIndex].sharedMaterials[materialIndex] == null)
                        {
                            invalidRenderers.Add(renderers[rendererIndex]);
                            validation = false;
                            continue;
                        }

                        if (!IsShaderValid(Plant.Type, renderers[rendererIndex].sharedMaterials[materialIndex].shader.name))
                        {
                            invalidMaterials.Add(renderers[rendererIndex].sharedMaterials[materialIndex]);
                            validation = false;
                        }
                    }
                }

                if (invalidRenderers.Count > 0) 
                {
                    string message = $"There are renderers without material attached:\n";
                    for (int rendererIndex = 0; rendererIndex < invalidRenderers.Count; ++rendererIndex)
                        message += $"{invalidRenderers[rendererIndex].name}\n";

                    Debug.LogError(message);                    
                }

                if (invalidMaterials.Count > 0) 
                {
                    string message = $"There are materials with invalid shader attached:\n";
                    for (int materialIndex = 0; materialIndex < invalidMaterials.Count; ++materialIndex)
                        message += $"{invalidMaterials[materialIndex].name}, [Shader: {invalidMaterials[materialIndex].shader.name}]\n";

                    message += GetAllowedShaderNames(Plant.Type);

                    Debug.LogError(message);
                }
            }

            bool propertiesValidation = Plant.ValidateProperties();
            return propertiesValidation && validation;
        }
        
        private bool IsShaderValid(PlantType plantType, string shaderName)
        {
            switch (plantType)
            {
                case PlantType.Grain:
                    return StaticInformation.AllowedGrainShaderNames.Contains(shaderName);
                case PlantType.Bush:
                    return StaticInformation.AllowedBushShaderNames.Contains(shaderName);

                default:
                case PlantType.Tree:
                    return true;
            }
        }

        private string GetAllowedShaderNames(PlantType plantType) 
        {
            string message = $"Allowed shaders for type {plantType} are:\n";
            string[] names = new string[0];
            switch (plantType)
            {
                case PlantType.Grain:
                    names = StaticInformation.AllowedGrainShaderNames;
                    break;
                case PlantType.Bush:
                    names =  StaticInformation.AllowedBushShaderNames;
                    break;
            }


            for (int shaderIndex = 0; shaderIndex < names.Length; ++shaderIndex)
                message += $"{names[shaderIndex]}";

            return message;
        }
    }    
}