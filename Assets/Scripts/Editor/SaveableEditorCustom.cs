#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using FarmManagerWorld.Static;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding;
using System;
using System.Runtime.Remoting.Messaging;
using System.CodeDom;

namespace FarmManagerWorld.Editors
{ 
    public class SaveableEditorCustom : Editor
    {
        [HideInInspector] protected SaveableEditor _saveableEditor;

        [HideInInspector] protected virtual SaveableEditor saveableEditor
        {
            get
            {
                if (_saveableEditor == null)
                    _saveableEditor = target as SaveableEditor;

                return _saveableEditor;
            }
        }
        
        protected string _modID
        {
            get { return saveableEditor.ModID; }
            set { saveableEditor.ModID = value;}
        }

        protected int _selectedMod
        {
            get { return saveableEditor.SelectedMod; }
            set { saveableEditor.SelectedMod = value; }
        }

        protected bool _overrideModObject
        {
            get { return saveableEditor.OverrideModObject; }
            set { saveableEditor.OverrideModObject = value; }
        }

        protected void ModPopup(string header = "Mod for compilation")
        {
            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck();
            string[] _options = ModLoader.GetMods().Select(item => item.id).ToArray();
            saveableEditor.SelectedMod = EditorGUILayout.Popup(header, saveableEditor.SelectedMod, _options);
            if (_selectedMod > -1)
                _modID = _options[_selectedMod];

            EditorGUI.EndChangeCheck();
            GUILayout.Space(10);
        }

        protected void AddTagToString(ref string tagToAdd, ref string stringWithTags, ref int index, bool replaceTag)
        {
            if (replaceTag)
            {
                stringWithTags = tagToAdd;
            }
            else
            {

                if (stringWithTags.Length > 0)
                    stringWithTags += ",";

                stringWithTags += tagToAdd;
            }


            index = -1;
            tagToAdd = "";
        }

        protected string[] GetPossibleTags(string currentTagsString, string[] tags)
        {
            if (currentTagsString == null)
                return tags;

            return tags.Except(currentTagsString.Split(',')).ToArray();
        }

        protected void AddTagToStringOnFromDropdownGUI(string dropdownHeader, string dropdownLabel, ref string stringWithTags, string[] tagsToChoose, ref int index, ref string currentlySelectedTag, bool replaceTag, bool addSpace = true)
        {
            if (addSpace)
                GUILayout.Space(20);

            string[] tags = GetPossibleTags(stringWithTags, tagsToChoose);
            if (tags.Length <= 0)
            {
                GUILayout.Label("No more tags to add");
                return;
            }

            GUILayout.Label(dropdownHeader);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            index = EditorGUILayout.Popup(dropdownLabel, index, tags);
            if (index > -1)
                currentlySelectedTag = tags[index];

            EditorGUI.EndChangeCheck();
            if (GUILayout.Button(replaceTag ? "Select" : "Add"))
            {
                AddTagToString(ref currentlySelectedTag, ref stringWithTags, ref index, replaceTag);
            }
            GUILayout.EndHorizontal();
        }

        protected void AddCustomTagToStringOnGUI(string labelHeader, ref string tagToAdd, ref string stringWithTags, bool replaceTag, bool addSpace = true)
        {
            if (addSpace)
                GUILayout.Space(20);
            
            GUILayout.Label(labelHeader);
            GUILayout.BeginHorizontal();

            tagToAdd = EditorGUILayout.TextField(tagToAdd);

            if (GUILayout.Button(replaceTag ? "Select" : "Add"))
            {
                int dummyIndex = -1;
                AddTagToString(ref tagToAdd, ref stringWithTags, ref dummyIndex, replaceTag);
            }

            GUILayout.EndHorizontal();            
        }

        protected void AddTagToStringOnGUI(ref bool isCustomValue, string toggleMessage, string textFieldHeader, string dropdownHeader, string dropdownLabel, ref string tagToAdd, string[] tagsToChoose, ref int index, ref string stringWithTags, bool replaceTag, bool addSpace = true)
        {
            if (addSpace)
                GUILayout.Space(20);

            isCustomValue = GUILayout.Toggle(isCustomValue, toggleMessage);

            if (isCustomValue)            
                AddCustomTagToStringOnGUI(textFieldHeader, ref tagToAdd, ref stringWithTags, replaceTag, false);            
            else            
                AddTagToStringOnFromDropdownGUI(dropdownHeader, dropdownLabel, ref stringWithTags, tagsToChoose, ref index, ref tagToAdd, replaceTag, false);            
        }

        protected void BoolDrawer(ref bool valueToChange, string toggleMessage)
        {
            GUILayout.BeginHorizontal();
            valueToChange = GUILayout.Toggle(valueToChange, toggleMessage);
            GUILayout.Label(valueToChange ? "Will override mod" : "Override disabled");
            GUILayout.EndHorizontal();
        }

        protected bool CheckMod(GameObject objectToCheck, string modID, bool checkLOD, bool checkMod, bool allowOverride)
        {
            if (checkMod && string.IsNullOrEmpty(modID))
            {
                EditorUtility.DisplayDialog("Error", "You have to choose a mod, search for \"Mod for compilation\" Dropdown in this component", "Ok");
                return false;
            }

            bool validation = true;
            BaseMod mod = objectToCheck.GetComponent<BaseMod>();            
            if (mod != null)
            {
                // first check if this is a prefab and just check if basic information will be overriden
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(objectToCheck);
                if (prefabAssetType != PrefabAssetType.NotAPrefab && prefabAssetType != PrefabAssetType.MissingAsset)
                {
                    // Get the corresponding prefab asset
                    BaseMod overridenMod = PrefabUtility.GetCorrespondingObjectFromSource(objectToCheck).GetComponent<BaseMod>();
                    bool willOverride = mod.properties.BasicType != overridenMod.properties.BasicType || mod.properties.Name != overridenMod.properties.Name || !mod.properties.Mod.Equals(overridenMod.properties.Mod);

                    if (willOverride && !allowOverride)
                    {
                        Debug.LogError($"Override will not work due to change to properties.BasicType and properties.Name in object {mod.properties.Name}, {mod.name}\n" +
                            $"Prefab data: BasicType: {overridenMod.properties.BasicType}, Name: {overridenMod.properties.Name}");
                        validation = false;
                    }
                }

                if (!allowOverride)
                {
                    List<string> paths;
                    string collisionPaths = "";

                    // ignore example mods when searching for mods with collision path
                    List<Modding.ObjectProperties.Properties> modProperties = ModLoader.GetModProperties(out paths, "Assets/ExampleMod");
                    for (int modIndex = 0; modIndex < modProperties.Count; ++modIndex)
                    {
                        if (modProperties[modIndex].BasicType == mod.properties.BasicType && modProperties[modIndex].Name == mod.properties.Name)
                            collisionPaths += $"\"{paths[modIndex]}\"\n";
                    }

                    if (!string.IsNullOrEmpty(collisionPaths))
                    {
                        EditorUtility.DisplayDialog("Error", $"There is already a Mod object with name \"{mod.properties.Name}\" of type \"{mod.properties.BasicType}\".\n" +
                            $"Paths for colliding Mod objects: {collisionPaths}", "Ok");

                        validation = false;
                    }
                }

                
            }

            if (checkLOD) 
            {
                var lod = objectToCheck.gameObject.GetComponentInChildren<LODGroup>();
                if (lod.GetLODs().Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", $"You have to add LOD group at object {lod.name}", "ok");
                    validation = false;
                }
                for (int i = 0; i < lod.GetLODs().Length; i++)
                {
                    if (lod.GetLODs()[i].renderers.Length == 0)
                    {
                        if (lod.GetLODs().Length == 1)
                        {
                            lod.GetLODs()[i].renderers = lod.GetComponentsInChildren<Renderer>();
                        }
                        else if (lod.transform.childCount > i)
                        {
                            var child = lod.transform.GetChild(i);
                            if (child != null)
                                lod.GetLODs()[i].renderers = child.GetComponentsInChildren<Renderer>();
                        }
                    }

                    if (lod.GetLODs()[i].renderers.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "You have to add at least one renderer to LOD" + i + $" in LOD group at object {lod.name}", "ok");
                        validation = false;
                    }
                }
            }            

            return validation;
        }

        protected void FinalizeForAssetBundle(MonoBehaviour editor, GameObject modObject, string modName, string folderName, StaticInformation.Region region = StaticInformation.Region.None)
        {
            if (editor != null)
                DestroyImmediate(editor);

            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(modObject); // if this object is a prefab, just apply changes
            if (prefabAssetType != PrefabAssetType.NotAPrefab && prefabAssetType != PrefabAssetType.MissingAsset)
            {
                PrefabUtility.ApplyPrefabInstance(modObject, InteractionMode.UserAction);
                return;
            }

            var go = modObject;
            var mod = ModLoader.GetMods().FirstOrDefault(item => item.id == modName);
            modObject.GetComponent<BaseMod>().properties.Mod = mod;
            var assetPath = Path.Combine(mod.Folder, folderName, modObject.name + ".prefab");
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, assetPath);

            assetPath = AssetDatabase.GetAssetPath(prefab);

            var importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = mod.id;

            switch (region)
            {
                default:
                case StaticInformation.Region.None:
                    importer.assetBundleVariant = "default";
                    break;

                case StaticInformation.Region.Europe:
                    importer.assetBundleVariant = "europe";
                    break;


                case StaticInformation.Region.SouthAmerica:
                    importer.assetBundleVariant = "southamerica";
                    break;


                case StaticInformation.Region.Asia:
                    importer.assetBundleVariant = "asia";
                    break;
            }


            PrefabUtility.InstantiatePrefab(prefab);
            DestroyImmediate(go);
        }

        protected void RemoveEditorComponentButton()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Remove editor component"))
                DestroyImmediate(saveableEditor);
        }
    }

    
}

#endif