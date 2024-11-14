using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// Base structure with basic information that is used to display mod in the proper game
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Mod")]
    public class Mod
    {
        public string id;
        public string Title;
        public string Author;
        public string Version;
        public string About;
        [XmlIgnore]
        public string Folder;
        [XmlIgnore]
        public string RelativeDirectory
        {
            get
            {
                return Folder.Replace("\\", "/").Replace(Application.dataPath, "Assets").TrimStart('/');
            }
        }
        [XmlIgnore]
        public List<AssetBundle> assetBundles = new List<AssetBundle>();
    }
}