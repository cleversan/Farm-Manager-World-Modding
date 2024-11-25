using FarmManagerWorld.Modding;
using FarmManagerWorld.Translations;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using FarmManagerWorld.Static;
using FarmManagerWorld.Utils;

namespace FarmManagerWorld.Editors.Wizards
{
    [CustomEditor(typeof(ModSaveWizard))]
    public class ModSaveWizard : EditorWindow
    {
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ModSaveWizard));
        }

        private void OnGUI()
        {
            this.maxSize = new Vector2(250, 100);
            GUILayout.Label("Would you like to create a new mod", EditorStyles.boldLabel);
            if (GUILayout.Button("Yes"))
            {
                ScriptableWizard.DisplayWizard("Mod Creator", typeof(CreateModWizard), "Create", "Change Path");

                this.Close();
            }

            if (GUILayout.Button("No"))
            {
                ScriptableWizard.DisplayWizard("Save to existing mod", typeof(SaveModWizard), "Save", "Change Path");

                this.Close();
            }
        }
    }

    public class CreateModWizard : ScriptableWizard
    {
        List<string> data = new List<string>();
        public string id = "Your mod id";
        public string Title = "Your mod name";
        public string Author = "Your name or nick";
        public string Version = "1.0";
        public string About = "Basic description";
        public string FolderPath;

        private void OnWizardCreate()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            foreach (string folder in data)
            {
                try
                {
                    if (!Directory.Exists(Path.Combine(FolderPath, folder)))
                    {
                        Directory.CreateDirectory(Path.Combine(FolderPath, folder));
                        AssetDatabase.Refresh();
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (File.Exists(Path.Combine(FolderPath, "Mod.xml")))
                File.Delete(Path.Combine(FolderPath, "Mod.xml"));

            Mod mod = new Mod
            {
                id = id,
                About = About,
                Title = Title,
                Version = Version,
                Author = Author
            };


            ModLoader.Serialize(mod, Path.Combine(FolderPath, "Mod.xml"));
        }

        private void OnWizardUpdate()
        {
            isValid = !(string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Author) || string.IsNullOrEmpty(Version));
        }

        private void OnEnable()
        {
            data.Add("animals"); data.Add("assets"); data.Add("buildings");
            data.Add("meshes"); data.Add("plants"); data.Add("resources");
            data.Add("textures"); data.Add("machines"); data.Add("plantFull");
        }

        private void Update()
        {
            FolderPath = Path.Combine(Application.dataPath, "mods", id);
        }
    }

    public class SaveModWizard : ScriptableWizard
    {
        private int leftColumnWidth = 375;
        protected int _selectedMod = 0;
        protected string modName = "";
        private string ModPath;

        public bool ShouldBuildAssetBundles = true;
        public bool ShouldGenerateEmptyTranslations = true;

        [Header("Preview image that will be exported as a thumbnail of this mod. " +
            "\n You can switch it to any Sprite that is available in project or by swapping " +
            "\nPreviewImage.jpg file in file system. " +
            "\nRember that Sprite needs to have option Read/Write set to true " +
            "\nin Advanced section")]
        public Sprite previewImage;
        private Sprite defaultPreviewImage;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Select mod you want to save");
            EditorGUI.BeginChangeCheck();
            string[] _options = ModLoader.GetMods().Select(item => item.id).ToArray();
            bool canCompile = true;
            _selectedMod = EditorGUILayout.Popup("Mod", _selectedMod, _options);
            if (_selectedMod > -1)
                modName = _options[_selectedMod];

            EditorGUI.EndChangeCheck();

            ModPath = Path.Combine(Application.dataPath, "Mods", modName);
            EditorGUILayout.LabelField($"Currently selected mod path: {ModPath}");
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Build asset bundles?", GUILayout.MinWidth(leftColumnWidth));
            ShouldBuildAssetBundles = EditorGUILayout.Toggle(ShouldBuildAssetBundles);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Generate empty translation files?", GUILayout.MinWidth(leftColumnWidth));
            ShouldGenerateEmptyTranslations = EditorGUILayout.Toggle(ShouldGenerateEmptyTranslations);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {             
                EditorGUILayout.LabelField(
                   "Preview image that will be exported as a thumbnail of this mod. " +
                "\nYou can switch it to any Sprite that is available in project or " +
                "\nby swapping PreviewImage.jpg file in file system. " +
                "\nRember that Sprite needs to have option Read/Write set to true " +
                "\nin Advanced section", GUILayout.MinWidth(leftColumnWidth), GUILayout.MinHeight(80), GUILayout.ExpandHeight(false));

                EditorGUILayout.BeginVertical();
                {
                    previewImage = (Sprite)EditorGUILayout.ObjectField(previewImage, typeof(Sprite), false);
                    if (previewImage != null && !previewImage.texture.isReadable)
                    {
                        EditorGUILayout.LabelField("SELECTED SPRITE \nIS NOT READABLE", GUILayout.ExpandHeight(true));
                        canCompile = false;
                    }
                    
                    if (string.IsNullOrEmpty(modName))
                    {
                        canCompile = false;
                        EditorGUILayout.LabelField("Select mod before compilation", GUILayout.ExpandHeight(true));
                    }

                    if (!Directory.Exists(ModPath))
                    {
                        canCompile = false;
                        EditorGUILayout.LabelField("Directory at Mod path does not exist", GUILayout.ExpandHeight(true));
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();        

            if (canCompile && GUILayout.Button("Compile", GUILayout.ExpandHeight(true)))            
                OnWizardCreate();           
        }

        private void OnWizardCreate()
        {
            Dictionary<string, bool> assetBundlesWithValidation = new Dictionary<string, bool>();

            List<BuildingMod> buildingModsToTranslate = new List<BuildingMod>();
            List<AnimalMod> animalModsToTranslate = new List<AnimalMod>();
            List<ResourceMod> resourceModsToTranslate = new List<ResourceMod>();
            List<MachineBaseMod> machineModsToTranslate = new List<MachineBaseMod>();

            var mod = ModLoader.GetMod(ModPath);
            var compiledModPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow"), "Cleversan\\FarmManagerWorld\\mods", mod.id);

            if (Directory.Exists(compiledModPath))
                Directory.Delete(compiledModPath, true);

            Directory.CreateDirectory(compiledModPath);

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { mod.RelativeDirectory });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                #region Checking validation

                string assetBundleImplicitName = "";
                if (ShouldBuildAssetBundles)
                {
                    assetBundleImplicitName = AssetDatabase.GetImplicitAssetBundleName(AssetDatabase.GetAssetPath(obj));
                    if (string.IsNullOrEmpty(assetBundleImplicitName))
                    {
                        Debug.LogError("Asset is not included in any assetBundle, ignoring object and moving on");
                        continue;
                    }
                    else if (assetBundlesWithValidation.ContainsKey(assetBundleImplicitName))
                    {
                        if (!assetBundlesWithValidation[assetBundleImplicitName])
                        {

                            Debug.LogError($"Asset bundle {assetBundleImplicitName} was not validated correctly previously, ignoring object and moving on");
                            continue;
                        }
                    }
                    else
                    {
                        assetBundlesWithValidation.Add(assetBundleImplicitName, true);
                    }

                    Debug.Log($"Asset bundle labels: {assetBundleImplicitName}");
                }

                #endregion

                #region Serialization

                Debug.Log($"Preparing to serialize: {obj.name}");
                if (obj.TryGetComponent(out BuildingMod buildMod))
                {                    
                    if (!buildMod.Validate())
                    {
                        Debug.LogError($"Building mod {buildMod.name} failed validation");

                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    buildMod.building.SetBasicType();
                    buildingModsToTranslate.Add(buildMod);
                    ModLoader.Serialize(buildMod.building, Path.Combine(compiledModPath, "buildings", buildMod.building.Name + ".xml"));
                }
                else if (obj.TryGetComponent(out RegionalModelMod regionalModel))
                {
                    regionalModel.RegionalModel.SetBasicType();
                    if (!regionalModel.Validate())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }
                    // RegionalModel does not need additional translation nor it requires saving via .xml
                }
                else if (obj.TryGetComponent(out ResourceMod resourceMod))
                {
                    resourceMod.resource.SetBasicType();

                    if (!resourceMod.ValidateResource())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    resourceModsToTranslate.Add(resourceMod);
                    ModLoader.Serialize(obj.GetComponent<ResourceMod>().resource, Path.Combine(compiledModPath, "resources", resourceMod.resource.Name + ".xml"));
                }
                else if (obj.TryGetComponent(out PlantMod plantModStandalone))
                {
                    plantModStandalone.Plant.SetBasicType();

                    if (!plantModStandalone.Validate())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    ModLoader.Serialize(plantModStandalone.Plant, Path.Combine(compiledModPath, "plants", plantModStandalone.Plant.Name + ".xml"));
                }
                else if (obj.TryGetComponent(out AnimalMod animalMod))
                {
                    if (!animalMod.Validate())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    animalModsToTranslate.Add(animalMod);
                    Debug.LogWarning("Animal is not supported with save option");
                }
                else if (obj.GetComponentInChildren<PlantMod>() && obj.GetComponentInChildren<FoliageResourceMod>() && obj.GetComponentInChildren<SeedResourceMod>())
                {
                    PlantDataContainer container = new PlantDataContainer();
                    PlantMod plantMod = obj.GetComponentInChildren<PlantMod>();
                    container.plant = plantMod.Plant;
                    container.seed = (SeedResourceProperties)obj.GetComponentInChildren<SeedResourceMod>().Resource;
                    container.foliage = (FoliageResourceProperties)obj.GetComponentInChildren<FoliageResourceMod>().Resource;
                    plantMod.Plant.SetBasicType();

                    if (!plantMod.Validate() || !container.seed.ValidateProperties() || !container.foliage.ValidateProperties())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    ModLoader.Serialize(container, Path.Combine(compiledModPath, "plantFull", plantMod.Plant.Name + ".xml"));
                }
                else if (obj.TryGetComponent(out VehicleMod vehicleMod))
                {                    
                    vehicleMod.baseMachine.SetBasicType();

                    if (!vehicleMod.Validate())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    machineModsToTranslate.Add(vehicleMod);
                    ModLoader.Serialize(vehicleMod, Path.Combine(compiledModPath, "machines", vehicleMod.baseMachine.Name + ".xml"));
                }
                else if (obj.GetComponent<MachineMod>())
                {
                    MachineMod machineMod = obj.GetComponent<MachineMod>();
                    machineMod.baseMachine.SetBasicType();

                    if (!machineMod.Validate())
                    {
                        if (ShouldBuildAssetBundles)
                            assetBundlesWithValidation[assetBundleImplicitName] = false;

                        continue;
                    }

                    machineModsToTranslate.Add(machineMod);
                    ModLoader.Serialize(machineMod, Path.Combine(compiledModPath, "machines", machineMod.baseMachine.Name + ".xml"));
                }

                if (File.Exists(Path.Combine(compiledModPath, "Mod.xml")))
                    File.Delete(Path.Combine(compiledModPath, "Mod.xml"));
                File.Copy(Path.Combine(ModPath, "Mod.xml"), Path.Combine(compiledModPath, "Mod.xml"));

                #endregion
            }

            Debug.Log("Validation finished");

            #region Building AssetBundles

            if (ShouldBuildAssetBundles)
            {
                for (int i = 0; i < assetBundlesWithValidation.Count; ++i)
                {
                    if (!assetBundlesWithValidation.ElementAt(i).Value)
                    {
                        Debug.LogError($"AssetBundle that was not validated found - {assetBundlesWithValidation.ElementAt(i).Key}, returning and not building asset Bundles");
                        return;
                    }
                }

                Debug.Log($"No errors found during validation, building assetBundles");
                DirectoryInfo assetBundleDirectory = BuildAssetBundles.BuildAllAssetBundles();

                if (assetBundleDirectory == null)
                {
                    Debug.LogError($"AssetBundle directory is null or was not created, not able to copy assetBundles to modFolder");
                    return;
                }

                foreach (var file in assetBundleDirectory.GetFiles())
                {
                    if (!assetBundlesWithValidation.ContainsKey(Path.GetFileNameWithoutExtension(file.Name)))
                        continue;

                    if (StaticInformation.AllowedBundleVariants.Contains(file.Extension))
                    {
                        string newPath = Path.Combine(compiledModPath, "assets");
                        if (!Directory.Exists(newPath))
                            Directory.CreateDirectory(newPath);

                        DirectoryInfo newLocation = new DirectoryInfo(newPath);
                        List<FileInfo> overlappingFiles = newLocation.GetFiles().ToList().FindAll(item => item.Name.Equals(file.Name));

                        for (int i = overlappingFiles.Count - 1; i >= 0; --i)
                        {
                            Debug.Log($"Removing overlapping file {overlappingFiles[i].FullName}");
                            overlappingFiles[i].Delete();
                        }

                        newPath = Path.Combine(newPath, file.Name);
                        Debug.Log($"Moving {file.Name} from {file.FullName} to {newPath}");
                        File.Move(file.FullName, newPath);
                    }
                }
            }

            #endregion

            #region Generating Empty Translations
            if (ShouldGenerateEmptyTranslations)
                TranslationSerializer.GenerateTranslationFiles(compiledModPath, buildingModsToTranslate, machineModsToTranslate, resourceModsToTranslate, animalModsToTranslate);

            #endregion

            #region Generate Preview Image

            string previewPath = Path.Combine(compiledModPath, "PreviewImage.jpg");
            if (!File.Exists(previewPath))
                File.WriteAllBytes(previewPath, previewImage.texture.EncodeToPNG());

            #endregion

            Close();
        }

        private void OnEnable()
        {
            this.minSize = new Vector2(600, 250);

            if (previewImage == null)
            {
                if (defaultPreviewImage == null)
                    defaultPreviewImage = Resources.Load<Sprite>("Sprites/moddingPreview");

                previewImage = defaultPreviewImage;
            }
        }
    }
}