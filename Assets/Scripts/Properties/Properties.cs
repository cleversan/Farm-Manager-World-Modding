using System;
using System.Xml.Serialization;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Translations;

namespace FarmManagerWorld.Modding.ObjectProperties
{
	/// <summary>
	/// Base Properties class from which other will inherit.
	/// </summary>
	[Serializable]
	[XmlRoot(ElementName = "Properties")]
	public abstract class Properties
	{
		/// <summary>
		/// Reference to mod its assigned to
		/// </summary>
		[XmlIgnore]
		[HideInInspector]
		public Mod Mod;

		/// <summary>
		/// Name of the object, universal between different properties 
		/// </summary>
		public string Name;

		/// <summary>
		/// Type of properties, filled automatically
		/// </summary>
		public string BasicType;

		/// <summary>
		/// Stores translation for easier 
		/// </summary>
		[XmlIgnore]
		public Translation Translation;

        public void SetBasicType()
		{
			BasicType = GetType().Name.Replace("Properties", "");
		}

		public virtual bool ValidateProperties()
		{
			bool validation = true;
			if (string.IsNullOrEmpty(Name))
			{
				Debug.LogError($"Name cannot be empty, validation failed");
                validation = false;
			}

			return validation;
		}
	}
}