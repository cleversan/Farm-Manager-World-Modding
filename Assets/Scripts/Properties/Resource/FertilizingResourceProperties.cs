using System;
using System.Xml.Serialization;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Fertilizer is used to complete obligatory Fertilizing step that occurs before planting resource.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "FertilizingResource")]
	public class FertilizingResourceProperties : ResourceProperties
	{
		/// <summary>
		/// Tag of fertilizing machine that is needed to fertilize using this resource
		/// </summary>
		public string FertilizingMachine;

        /// <summary>
        /// How much of this resource is needed to fertilize whole field. It ignores plants and density as it fertilize whole area. 
        /// Field 52m x 52m (2704 m2) with 0.01f per square it will need 27.04 resource (UI will round it)
        /// </summary>
        public float AmountPerSquareMeter = 0.1f;
	}
}