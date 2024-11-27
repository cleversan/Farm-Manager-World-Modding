using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Validate Renderenrs and Materials. Checks if object has any Renderers, those Renderers have any materials attached and if materials have correct shaders
        /// </summary>
        /// <param name="gameObject">Object to check</param>
        /// <param name="allowedShaderNames">Allowed shader names. If its null, checking shader names will be ignored.</param>
        /// <returns>
        /// <listheader>False if:</listheader>
        /// <list type="bullet">Any Renderer does not have material attached</list>
        /// <list type="bullet">Any Material is null</list>
        /// <list type="bullet">Any Shader used in Material is not specified in <paramref name="allowedShaderNames"/></list>
        /// Otherwise return true.
        /// </returns>
        public bool ValidateRenderersAndMaterials(string[] allowedShaderNames)
        {
            bool validation = true;
            bool ignoreShaderNames = allowedShaderNames == null;
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);

            // check if there are even any renderers
            if (renderers.Length <= 0)
            {
                Debug.LogError($"No renderers at GameObject {gameObject.name} detected, validation failed");
                validation = false;
            }
            else
            {
                List<Renderer> invalidRenderers = new List<Renderer>();
                List<Material> invalidMaterials = new List<Material>();
                for (int rendererIndex = 0; rendererIndex < renderers.Length; ++rendererIndex)
                {
                    // check if renderer has any materials, if not, set validation to false, save name and iterate
                    if (renderers[rendererIndex].sharedMaterials.Length <= 0)
                    {
                        invalidRenderers.Add(renderers[rendererIndex]);
                        validation = false;
                        continue;
                    }

                    // check if each materials shader is ok
                    for (int materialIndex = 0; materialIndex < renderers[rendererIndex].sharedMaterials.Length; ++materialIndex)
                    {
                        // if material is null, add to invalid Renderers list
                        if (renderers[rendererIndex].sharedMaterials[materialIndex] == null)
                        {
                            invalidRenderers.Add(renderers[rendererIndex]);
                            validation = false;
                            continue;
                        }

                        // if ignore shader names, continue
                        if (ignoreShaderNames)
                            continue;

                        // check if shader name appears in allowedShaderNames, if not, add to invalid materials list
                        if (!allowedShaderNames.Contains(renderers[rendererIndex].sharedMaterials[materialIndex].shader.name))
                        {
                            invalidMaterials.Add(renderers[rendererIndex].sharedMaterials[materialIndex]);
                            validation = false;
                        }
                    }
                }

                // print errors
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


                    message += $"Allowed shaders for gameObject {gameObject.name} are:\n";

                    for (int shaderIndex = 0; shaderIndex < allowedShaderNames.Length; ++shaderIndex)
                        message += $"{allowedShaderNames[shaderIndex]}";

                    Debug.LogError(message);
                }
            }

            return validation;
        }
    }
}