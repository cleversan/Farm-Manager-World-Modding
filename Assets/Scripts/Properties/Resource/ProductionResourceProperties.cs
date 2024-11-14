using System;
using System.Xml.Serialization;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Production resources are produced in <see cref="ProductionBuildingProperties"/> with recipe specified in <see cref="ProductionBuildingProperties.AvailableProduction"/>
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "ProductionResource")]
	public class ProductionResourceProperties : ResourceProperties
	{

	}
}