using System;
using System.Xml.Serialization;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Honey resources are produced through <see cref="ApiaryBuildingProperties"/> and <see cref="HiveBuildingProperties"/>.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "HoneyResource")]
	public class HoneyResourceProperties : AnimalResourceProperties
	{
		/// <summary>
		/// Name of the Plant that is required to be in vicinity for hive to produce this Honey
		/// </summary>
		[XmlElement("NeededPlant")]
		public string NeededPlantName;

		/// <summary>
		/// Does hive even require Plant to produce this honey resource
		/// </summary>
		public bool NeedPlant = true;
	}
}