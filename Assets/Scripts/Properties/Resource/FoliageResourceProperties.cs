using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Foliage resources are produced by Plants that are specificed in <see cref="PlantProperties.FoliageName"/>
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "FoliageResource")]
	public class FoliageResourceProperties : ResourceProperties
    {
		/// <summary>
		/// Should harvest material be used on storage mesh, otherwise it will use material stored in harvestPlane (i.e. tarpulin on the chest trailer)
		/// </summary>
        public bool UseHarvestMaterialOnMesh = false;

		/// <summary>
		/// Should put harvest material on kisten (otherwise it will cover kisten with lid)
		/// </summary>
        public bool UseHarvestMaterialOnKisten = false;

		/// <summary>
		/// Material that is put on harvest storage mesh;
		/// </summary>
		[XmlIgnore]
		public Material HarvestMaterial;

		/// <summary>
		/// Mesh that is put on chest trailer plane. If this is null, random tarpulin will be set instead.
		/// </summary>
		[XmlIgnore]
		public GameObject HarvestChestTrailerPlane;
    }
}