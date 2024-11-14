using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Animals represented as resources when bought or moved to Slaughterhouse.
	/// </summary>
    [Serializable]
	[XmlRoot(ElementName = "AnimalAsResource")]
	public class AnimalAsResourceProperties : ResourceProperties
	{
		/// <summary>
		/// Sprite of young animal variant
		/// </summary>
		[XmlIgnore]
		public Sprite YoungSprite;
	}
}