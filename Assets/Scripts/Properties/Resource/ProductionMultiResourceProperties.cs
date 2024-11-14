using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Production Multi Resource differs from <see cref="ProductionResourceProperties"/> by having one specific recipe that needs more then one resource to be produced.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "ProductionMultiResource")]
	public class ProductionMultiResourceProperties : ProductionResourceProperties
	{
		/// <summary>
		/// List of resources that are needed to produce this resource. Instead of resource ingredients being contained inside Production list in building,
		/// its stored inside resource.
		/// </summary>
		[XmlElement("BaseResources")]
		public List<string> BaseResourcesNames;

		/// <summary>
		/// Array of amounts needed for production. Amounts and resources are paired based on index. 
		/// To have amount of resource at Index 1 (indexing starts from 0) equal 300, you need to set 300 at index 1.
		/// </summary>
		public int[] BaseAmountResources;
	}
}