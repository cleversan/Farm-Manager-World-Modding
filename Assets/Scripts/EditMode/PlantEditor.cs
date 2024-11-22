#if UNITY_EDITOR
using System;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Utils;

namespace FarmManagerWorld.Editors
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(PlantMod))]
    public class PlantEditor : SaveableEditor
    {        
        public PlantMod plantMod;
        public SeedResourceMod seedResourceMod;
        public FoliageResourceMod foliageResourceMod;

        [Header("Values for field size 52m x 52m")]
        public float HarvestFromWholeField;
        public float SeedsNeededForField;
        public FieldShowcase FieldShowcase;

        private void Awake()
        {
            plantMod = GetComponent<PlantMod>();
        }

        public void Visualise()
        {
            if (FieldShowcase == null)
                FieldShowcase = FieldShowcase.Get(transform);

            FieldShowcase.transform.SetParent(transform);
            FieldShowcase.transform.localPosition = Vector3.zero;
            FieldShowcase.VisualisePlant(plantMod);
        }

        public float GetGrowEfficiency(float harvest)
        {
            float densityX = plantMod.Plant.PlantAttributes.DensityX;
            float densityY = plantMod.Plant.PlantAttributes.DensityY;

            var padding = plantMod.Plant.Padding;
            if (plantMod.Plant.Type == Modding.ObjectProperties.PlantType.Grain)
                padding += 0.5f;

            int plantsInX = (int)Math.Ceiling((52 - padding * 2) / densityX);
            int plantsInY = (int)Math.Ceiling((52 - padding * 2) / densityY);

            float chestTrailerModifier = plantMod.Plant.PlantAttributes.HarvestingMachine.ToLower().Contains("chesttrailer") ? 0.77f : 1.0f;

            float efficiency = harvest / (plantsInX * plantsInY * chestTrailerModifier);

            Debug.Log($"Initial harvest: {harvest}, calculated harvest: {(plantsInX * plantsInY * chestTrailerModifier * efficiency)}");

            return efficiency;
        }

        public float CalculateSeedPerSquareMeter(float seedPerField)
        {
            float densityX = plantMod.Plant.PlantAttributes.DensityX;
            float densityY = plantMod.Plant.PlantAttributes.DensityY;

            var padding = plantMod.Plant.Padding;
            if (plantMod.Plant.Type == Modding.ObjectProperties.PlantType.Grain)
                padding += 0.5f;

            int plantsInX = (int)Math.Ceiling((52 - padding * 2) / densityX);
            int plantsInY = (int)Math.Ceiling((52 - padding * 2) / densityY);

            float amount = seedPerField / (densityX * densityY * plantsInX * plantsInY);

            Debug.Log($"Initial seed per field: {seedPerField}, calculated seed per field: {GetRequiredSeedAmount(amount, densityX, densityY, plantsInX, plantsInY)}, new seed per square meter value: {amount}");

            return amount;
        }
        
        private float GetRequiredSeedAmount(float seedPerSquareMeter, float densityX, float densityY, int plantsInX, int plantsInY)
        {
            float amount = Mathf.Ceil(seedPerSquareMeter * densityX * densityY * (plantsInX * plantsInY));

            if (amount < 1)
            {
                Debug.LogWarning($"Amount was rounded to 1, previous value: {amount}");
                amount = 1;
            }

            return amount;
        }
    }
}

#endif