using FarmManagerWorld.Modding.ObjectProperties;
using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Utils
{
    /// <summary>
    /// Unifies <see cref="PlantProperties"/>, <see cref="FoliageResourceProperties"/> and <see cref="SeedResourceProperties"/> during saving to .xml file
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "PlantData")]
    public class PlantDataContainer : ScriptableObject
    {
        public PlantProperties plant;
        public FoliageResourceProperties foliage;
        public SeedResourceProperties seed;
    }
}