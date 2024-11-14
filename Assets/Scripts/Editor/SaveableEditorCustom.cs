#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FarmManagerWorld.Editors.Wizards;
using System.IO;
using System.Linq;
using System.Drawing;
using FarmManagerWorld.Static;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding;
using System;

namespace FarmManagerWorld.Editors
{ 
    public class SaveableEditorCustom : Editor
    {
        protected int _selectedMod = 0;
        protected string modID = "";

        protected void FinalizeForAssetBundle(MonoBehaviour editor, GameObject modObject, string modName, string folderName, StaticInformation.Region region = StaticInformation.Region.None)
        {
            var go = modObject;
            var mod = ModLoader.GetMods().FirstOrDefault(item => item.id == modName);
            modObject.GetComponent<BaseMod>().properties.Mod = mod;
            var assetPath = Path.Combine(mod.Folder, folderName, modObject.name + ".prefab");
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            if (editor != null)
                DestroyImmediate(editor);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, assetPath);

            assetPath = AssetDatabase.GetAssetPath(prefab);

            var importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = mod.id;
            
            switch(region)
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

        protected void ModPopup(string header = "Mod for compilation")
        {
            EditorGUI.BeginChangeCheck();
            string[] _options = ModLoader.GetMods().Select(item => item.id).ToArray();
            _selectedMod = EditorGUILayout.Popup(header, _selectedMod, _options);
            if (_selectedMod > -1)
                modID = _options[_selectedMod];

            EditorGUI.EndChangeCheck();
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

        protected bool CheckMod(GameObject objectToCheck, string modID, bool checkLOD, bool checkMod)
        {
            if (checkMod && string.IsNullOrEmpty(modID))
            {
                EditorUtility.DisplayDialog("Error", "You have to choose a mod", "ok");
                return false;
            }

            bool validation = true;
            BaseMod mod = objectToCheck.GetComponent<BaseMod>();
            if (mod != null && ModLoader.GetModProperties().Any(item => item.BasicType == mod.properties.BasicType && item.Name == mod.properties.Name))
            {
                EditorUtility.DisplayDialog("Error", $"There is already a Mod object with name \"{mod.properties.Name}\" of type \"{mod.properties.BasicType}\" inside mod with ID \"{mod.properties.Mod.id}\"", "ok");
                validation = false;
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
    }
}

#endif