using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Translations
{
    /// <summary>
    /// Creates translation files for every mod
    /// </summary>
    public static class TranslationSerializer
    {
        public static void GenerateTranslationFiles(string path, List<BuildingMod> buildingMods, List<MachineBaseMod> machineBaseMods, List<ResourceMod> resourceMods, List<AnimalMod> animalMods)
        {
            string[] paths = Directory.GetFiles(path);
            List<FileInfo> files = new List<FileInfo>();
            for (int i = 0; i < paths.Length; ++i)
                files.Add(new FileInfo(paths[i]));

            string[] filenamesToDelete = new string[]
            {
                "machineTranslations.json",
                "resourceTranslations.json",
                "buildingTranslations.json",
                "animalTranslations.json"
            };

            for(int i = files.Count - 1; i >= 0; --i)
            {
                if (filenamesToDelete.Contains(files[i].Name))
                {
                    files[i].Delete();
                    continue;
                }
            }

            if (buildingMods.Count > 0)            
                WriteTranslationToFile(path, "buildingTranslations.json", buildingMods.Select(item => item as BaseMod).ToList());            

            if (resourceMods.Count > 0)            
                WriteTranslationToFile(path, "resourceTranslations.json", resourceMods.Select(item => item as BaseMod).ToList());            

            if (machineBaseMods.Count > 0)            
                WriteTranslationToFile(path, "machineTranslations.json", machineBaseMods.Select(item => item as BaseMod).ToList());            

            if (animalMods.Count > 0)            
                WriteTranslationToFile(path, "animalTranslations.json", animalMods.Select(item => item as BaseMod).ToList());            
        }

        private static void WriteTranslationToFile(string path, string fileName, List<BaseMod> mods)
        {
            Container container = new Container();
            container.Translations = new List<Translation>();
            foreach (BaseMod mod in mods)
                container.Translations.Add(mod.ValidateTranslation() ? mod.properties.Translation : new Translation(mod.properties));

            string serializedTranslations = JsonUtility.ToJson(container, true);
            File.WriteAllText(Path.Combine(path, fileName), serializedTranslations);
        }

        internal static List<LocalizationData> GenerateLanguageLocalizationData(string fillerString)
        {
            List<LocalizationData> localizationDataList = new List<LocalizationData>();

            for(int i = 0; i < 12; ++i)
            {
                LocalizationData localizationData = new LocalizationData();
                localizationData.Language = GetLocalizationIndex(i);
                localizationData.LocalizedName = fillerString;
                localizationData.LocalizedDescription = fillerString;
                localizationDataList.Add(localizationData);
            }

            return localizationDataList;
        }

        internal static string GetLocalizationIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return "pl";

                default:
                case 1:
                    return "en";

                case 2:
                    return "de";

                case 3:
                    return "pt-BR";

                case 4:
                    return "es";

                case 5:
                    return "fr";

                case 6:
                    return "tr";

                case 7:
                    return "zh-CN";

                case 8:
                    return "it";

                case 9:
                    return "ko";

                case 10:
                    return "ja";

                case 11:
                    return "ru";
            }
        }
    }
    
    /// <summary>
    /// Wrapper for translation that stores translation for every object
    /// </summary>
    [Serializable()]
    public class Container
    {
        public List<Translation> Translations;
    }

    /// <summary>
    /// Basic translation for object specified by <see cref="Name"/> with list of all localization data.
    /// </summary>
    [Serializable()]
    public class Translation
    {
        public string Name;
        public List<LocalizationData> LocalizationData;

        public Translation(Properties properties)
        {
            Name = properties.Name;
            LocalizationData = TranslationSerializer.GenerateLanguageLocalizationData(properties.Name);
        }

        public bool ValidateTranslation(string identificationName)
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            if (Name != identificationName)
                return false;

            if (LocalizationData.Count != 12)
                return false;

            for(int i = 0; i < LocalizationData.Count; i++)
            {
                if (LocalizationData[i] is null)
                    return false;

                if (LocalizationData[i].Language != TranslationSerializer.GetLocalizationIndex(i))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Basic structure for localization of object in <see cref="Language"/>
    /// </summary>
    [Serializable()]
    public class LocalizationData
    {
        /// <summary>
        /// Defines which language it will use based by <see cref="TranslationSerializer.GetLocalizationIndex(int)"/>
        /// </summary>
        public string Language;

        /// <summary>
        /// Localized name of the object
        /// </summary>
        public string LocalizedName;

        /// <summary>
        /// Localized description of the object. Only matters in:
        /// <list type="bullet">
        /// <item><see cref="MachineProperties"/></item>
        /// <item><see cref="BuildingProperties"/></item>
        /// </list>
        /// </summary>
        public string LocalizedDescription;
    }
}
