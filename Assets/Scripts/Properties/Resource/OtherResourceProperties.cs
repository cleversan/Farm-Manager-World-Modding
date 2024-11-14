using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Backup resources that do not fall into any other categories. One of the is ingame Crops resource
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "OtherResource")]
	public class OtherResourceProperties : ResourceProperties
	{
		/// <summary>
		/// Storage material that will be put on ChestTrailerPlane
		/// </summary>
		[XmlIgnore]
		public Material StorageMaterial;

		/// <summary>
		/// Chest trailer plane that will be put inside ChestTrailer
		/// </summary>
		[XmlIgnore]
		public GameObject ChestTrailerPlane;
	}
}