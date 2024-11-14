using System;
using System.Xml.Serialization;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Empty class that does not contain any parameters but is still used in few cases like craetion of SeedingBox, that are used for visualising seeds and seedlings during field work.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Vehicle")]
    public class SowingMachineProperties : MachineProperties
    {

    }
}