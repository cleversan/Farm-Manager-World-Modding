using UnityEditor;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Editors.Wizards;
using System.Linq;
using System.IO;
using System.Reflection;
using System;
using System.Collections;
using NUnit.Framework;
using FarmManagerWorld.Static;
using System.Collections.Generic;

namespace FarmManagerWorld.Editors
{

    [CustomEditor(typeof(BuildingEditor))]
    [CanEditMultipleObjects]
    public class BuildingEditorCustom : SaveableEditorCustom
    {
        public BuildingEditor editor;

        public override void OnEnable()
        {
            base.OnEnable();
            editor = (BuildingEditor)target;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            ModPopup();

            EditorGUI.EndChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
                editor.GetStarted();
            }

            if (GUILayout.Button("Choose waypoint"))
            {
                editor.GeneratePossibleWaypoints();
            }

            if (GUILayout.Button("Choose roadconnector"))
            {
                editor.GeneratePossibleRoadconnectors();
            }

            if (GUILayout.Button("Show/Hide parkingSpaces"))
                editor.ShowParkingSpaceCollider();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Place roof fire"))
            {
                SetIsPlacing(BuildingPlacingMode.FireRoof);
            }
            if (GUILayout.Button("Place wall fire"))
            {
                SetIsPlacing(BuildingPlacingMode.FireWall);
            }

            GUILayout.EndHorizontal();

            if ((editor.PlacingMode == BuildingPlacingMode.FireRoof || editor.PlacingMode == BuildingPlacingMode.FireWall) && GUILayout.Button("Stop placing fire"))
            {
                SetIsPlacing(BuildingPlacingMode.None);
            }


            GUILayout.Space(20);

            if (GUILayout.Button("Place door"))
            {
                SetIsPlacing(BuildingPlacingMode.Door);
            }

            if (GUILayout.Button("Place gate"))
            {
                SetIsPlacing(BuildingPlacingMode.Gate);
            }

            if (GUILayout.Button("Place slide up"))
            {
                SetIsPlacing(BuildingPlacingMode.SlideUp);
            }

            if ((editor.PlacingMode == BuildingPlacingMode.Door || editor.PlacingMode == BuildingPlacingMode.Gate || editor.PlacingMode == BuildingPlacingMode.SlideUp)
                && GUILayout.Button("Stop placing entrance"))
            {
                SetIsPlacing(BuildingPlacingMode.None);
            }

            if (editor.building.building.BasicType == "AnimalsBuilding")
            {
                if (editor.PlacingMode == BuildingPlacingMode.AnimalSpawn)
                {
                    if (GUILayout.Button("Stop placing animal spawns"))
                    {
                        SetIsPlacing(BuildingPlacingMode.None);
                    }
                }
                else if (GUILayout.Button("Place animal spawns"))
                {
                    SetIsPlacing(BuildingPlacingMode.AnimalSpawn);
                }
            }

            if (editor.building.building.BasicType == "HiveBuilding")
            {
                if (editor.PlacingMode == BuildingPlacingMode.HiveFeedingLocation)
                {
                    if (GUILayout.Button("Stop placing hive feeding location"))
                    {
                        SetIsPlacing(BuildingPlacingMode.None);
                    }
                }
                else if (GUILayout.Button("Place hive feeding location"))
                {
                    SetIsPlacing(BuildingPlacingMode.HiveFeedingLocation);
                }
            }

            if (editor.building.NeedParkingSpaceCandidate)
            {
                if (editor.PlacingMode == BuildingPlacingMode.ParkingSpaceCandidate)
                {
                    if (GUILayout.Button("Stop placing machine parking space candidate"))
                    {
                        SetIsPlacing(BuildingPlacingMode.None);
                    }
                }
                else if (GUILayout.Button("Place machine parking space"))
                {
                    SetIsPlacing(BuildingPlacingMode.ParkingSpaceCandidate);
                }
            }

            if (!editor.AllowSkinnedMeshRenderers && GUILayout.Button("Convert Skinned Mesh Renderers"))
            {
                Extensions.ConvertSkinnedMeshRenderers(editor.gameObject);
            }

            if (!Application.isPlaying && GUILayout.Button("Finalize for asset bundle"))
            {
                if (editor.building.Validate() && CheckMod(editor.gameObject, _modID, true, true) && CheckRegionalBuildings())
                {
                    var regionalModels = editor.building.GetComponentsInChildren<RegionalModelMod>(true);
                    for (int i = regionalModels.Length - 1; i >= 0; i--)
                        FinalizeForAssetBundle(null, regionalModels[i].gameObject, _modID, "buildings", regionalModels[i].RegionalModel.Region);

                    FinalizeForAssetBundle(editor, editor.gameObject, _modID, "buildings");
                }
            }
        }

        private void SetIsPlacing(BuildingPlacingMode mode)
        {
            editor.PlacingMode = mode;
            switch (editor.PlacingMode)
            {
                default:
                case BuildingPlacingMode.None:
                    editor.DestroyMeshColliders();
                    break;

                case BuildingPlacingMode.FireRoof:
                case BuildingPlacingMode.FireWall:
                case BuildingPlacingMode.AnimalSpawn:
                case BuildingPlacingMode.HiveFeedingLocation:
                case BuildingPlacingMode.ParkingSpaceCandidate:
                case BuildingPlacingMode.Door:
                case BuildingPlacingMode.Gate:
                case BuildingPlacingMode.SlideUp:
                    editor.GenerateMeshColliders();
                    break;
            }
        }


        public void OnSceneGUI()
        {
            switch (editor.PlacingMode)
            {
                default:
                case BuildingPlacingMode.None:
                    break;

                case BuildingPlacingMode.FireRoof:
                case BuildingPlacingMode.FireWall:
                    PlaceFire();
                    break;

                case BuildingPlacingMode.Waypoint:
                    SelectWaypoint();
                    break;

                case BuildingPlacingMode.RoadConnector:
                    SelectRoadconnector();
                    break;


                case BuildingPlacingMode.AnimalSpawn:
                    PlaceAnimalSpawn();
                    break;

                case BuildingPlacingMode.HiveFeedingLocation:
                    PlaceHiveFeedingLocation();
                    break;

                case BuildingPlacingMode.ParkingSpaceCandidate:
                    PlaceParkingSpaceCandidate();
                    break;

                case BuildingPlacingMode.Door:
                case BuildingPlacingMode.Gate:
                case BuildingPlacingMode.SlideUp:
                    PlaceEntrance();
                    break;
            }
        }

        void SelectWaypoint()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f))
                    {

                        if (hitInfo.transform.gameObject.GetComponent<WaypointCandidate>() != null)
                        {
                            editor.selectedWaypoint = hitInfo.transform.gameObject.GetComponent<WaypointCandidate>();
                            SetIsPlacing(BuildingPlacingMode.None);
                            editor.ApplyWaypoint();
                        }
                    }

                    Event.current.Use();
                }
            }
        }

        void SelectRoadconnector()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f))
                    {

                        if (hitInfo.transform.gameObject.GetComponent<RoadconnectorCandidate>() != null)
                        {
                            editor.selectedRoadconnector = hitInfo.transform.gameObject.GetComponent<RoadconnectorCandidate>();
                            SetIsPlacing(BuildingPlacingMode.None);
                            editor.ApplyRoadconnector();
                            //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Keyboard));
                        }
                    }

                    Event.current.Use();
                }
            }
        }

        void PlaceFire()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f))
                    {
                        if (hitInfo.transform.gameObject.GetComponentInParent<BuildingMod>() != null)
                        {
                            if (editor.PlacingMode == BuildingPlacingMode.FireRoof)
                                editor.GenerateRoofFirepoint(hitInfo.point);
                            else if (editor.PlacingMode == BuildingPlacingMode.FireWall)
                                editor.GenerateWallFirepoint(hitInfo.point);
                        }
                    }
                }
            }
        }

        private void PlaceAnimalSpawn()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f, LayerMask.GetMask("Ground")))
                    {
                        editor.GenerateAnimalSpawn(hitInfo.point);
                    }
                }
            }
        }

        private void PlaceHiveFeedingLocation()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f, LayerMask.GetMask("Ground")))
                    {
                        editor.GenerateHiveFeedingLocation(hitInfo.point);
                    }
                }
            }
        }

        private void PlaceParkingSpaceCandidate()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f, LayerMask.GetMask("Ground")))
                    {
                        editor.GenerateParkingSpaceCandidate(hitInfo.point);
                    }
                }
            }
        }

        private void PlaceEntrance()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f, LayerMask.GetMask("Ground")))
                    {
                        editor.GenerateEntrance(hitInfo.point);
                        SetIsPlacing(BuildingPlacingMode.None);
                    }
                }
            }
        }

        private bool CheckRegionalBuildings()
        {
            RegionalModelModManager regionalManager = editor.GetComponentInChildren<RegionalModelModManager>(true);
            if (regionalManager == null)
                return true;

            List<StaticInformation.Region> regions = new();
            var models = editor.GetComponentsInChildren<RegionalModelMod>(true);

            bool regionsValidated = true;
            bool lodValidated = true;
            foreach(var item in models)
            {
                if (!CheckMod(item.gameObject, "", true, false))
                {
                    Debug.LogError($"LOD validation failed at {item.name}");
                    lodValidated = false;
                }

                if (regions.Contains(item.RegionalModel.Region))
                {
                    Debug.LogError($"There are more then 1 regional model for {item.RegionalModel.Region} in {editor.building.building.Name}");
                    regionsValidated = false;
                }
                else
                    regions.Add(item.RegionalModel.Region);                
            }

            return regionsValidated && lodValidated;
        }
    }
}