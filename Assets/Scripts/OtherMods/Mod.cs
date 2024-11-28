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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not Mod) 
                return false;

            Mod modToCheck = obj as Mod;
            return modToCheck.id == id && modToCheck.Title == Title && modToCheck.Author == Author && modToCheck.Version == Version && modToCheck.About == About && modToCheck.Folder == Folder;
        }
    }
}