using System;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Chemical resources are used during spraying to prevent/remove disease from field. 
	/// Plants have different resistance to diseases specified by <see cref="PlantAttributeProperties.FungsChance"/>, <see cref="PlantAttributeProperties.HerbsChance"/> and <see cref="PlantAttributeProperties.InsectsChance"/>
    /// </summary>
    [Serializable]
	[XmlRoot(ElementName = "ChemicalResource")]
	public class ChemicalResourceProperties : ResourceProperties
	{
		/// <summary>
		/// How much resource is needed to spray the field per square meter. It ignores plants and density as it sprays whole area. 
		/// Field 52m x 52m (2704 m2) with 0.01f per square it will need 27.04 resource (UI will round it)
		/// </summary>
		public float AmountPerSquareMeter = 0.01f;

		/// <summary>
		/// What kind of chemical resource it is. Yes, it can be None
		/// </summary>
		public ChemicalType ChemicalType;
	}

	/// <summary>
	/// Defines 
	/// </summary>
	public enum ChemicalType
	{
		None,
		Fungs = 1,
		Herbs = 2,
		Insects = 4
	}
}