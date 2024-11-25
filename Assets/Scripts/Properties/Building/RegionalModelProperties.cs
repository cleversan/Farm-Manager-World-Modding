using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Static;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Regional model properties that define in which region should this model be present
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "RegionalModel")]
    public class RegionalModelProperties : Properties
    {
        [XmlElement("Region")]
        public StaticInformation.Region Region;
    }
}