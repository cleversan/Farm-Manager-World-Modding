using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;

namespace FarmManagerWorld.Utils
{
    /// <summary>
    /// Allows visualisation of grown plants
    /// </summary>
    [ExecuteInEditMode]
    public class FieldShowcase : MonoBehaviour
    {
        [SerializeField] private Transform PlantContainer;
        private Bounds Bounds;

        private void Awake()
        {
            Bounds = new Bounds(PlantContainer.position, new Vector3(52, 1, 52));
        }

        public void VisualisePlant(PlantMod plant)
        {
            // Remove all children as a cleanup before visualising
            Extensions.RemoveAllChildren(PlantContainer.gameObject);

            float densityX = plant.Plant.PlantAttributes.DensityX;
            float densityY = plant.Plant.PlantAttributes.DensityY;

            float padding = plant.Plant.Padding;

            int plantsInX = (int)(Mathf.Floor(52 - padding * 2) / densityX);
            int plantsInY = (int)(Mathf.Floor(52 - padding * 2) / densityY);

            float posVariationX;
            float posVariationY;

            if (plant.Plant.PosVariation == 0)
            {
                posVariationX = plant.Plant.PosVariationVector.x;
                posVariationY = plant.Plant.PosVariationVector.y;
            }
            else
            {
                posVariationX = plant.Plant.PosVariation;
                posVariationY = plant.Plant.PosVariation;
            }

            float sizeX = plantsInX * densityX;
            float sizeY = plantsInY * densityY;

            float xPadding = Mathf.Max(posVariationX / 2 + padding, Mathf.Clamp(Bounds.size.x - sizeX, 0, float.PositiveInfinity));
            float yPadding = Mathf.Max(posVariationY / 2 + padding, Mathf.Clamp(Bounds.size.z - sizeY, 0, float.PositiveInfinity));


            float baseXTemp = Bounds.min.x + xPadding;
            float baseZTemp = Bounds.min.z + yPadding;

            GameObject meshGroup = new GameObject("Mesh group");
            meshGroup.transform.SetParent(PlantContainer);
            meshGroup.transform.position = new Vector3(baseXTemp, 0, baseZTemp);

            for (int x = 0; x < plantsInX; x++)
            {
                for (int y = 0; y < plantsInY; y++)
                {
                    GameObject plantObject = Instantiate(plant.model, meshGroup.transform);
                    plantObject.transform.localPosition = new Vector3(
                        x * densityX + Random.value * posVariationX - posVariationX / 2,
                        0,
                        y * densityY + Random.value * posVariationY - posVariationY / 2);

                }
            }
        }

        private static FieldShowcase _prefab;
        public static FieldShowcase Get(Transform parent)
        {
            if (_prefab == null)
                _prefab = Resources.Load<FieldShowcase>("Prefabs/FieldShowcase");

            FieldShowcase instance = Instantiate(_prefab, parent);

            return instance;
        }
    }
}