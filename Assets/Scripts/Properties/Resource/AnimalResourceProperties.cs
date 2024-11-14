using System;
using System.Xml.Serialization;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Resource produced by animals based on <see cref="AnimalProperties.Production"/>. 
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "AnimalResource")]
	public class AnimalResourceProperties : ResourceProperties
	{

	}
}