#if UNITY_EDITOR
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Modding.ObjectProperties;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BuildingMod))]
    public class BuildingEditor : SaveableEditor
    {
        public BuildingMod building;
        public GameObject doorObject;
        public GameObject gateObject;

        [HideInInspector]
        public WaypointCandidate selectedWaypoint;

        [HideInInspector]
        public RoadconnectorCandidate selectedRoadconnector;

        [HideInInspector]
        public GameObject firePointsHolder;

        [HideInInspector]
        public GameObject roofFires;

        [HideInInspector]
        public GameObject wallFires;

        [SerializeField]
        private bool ShowBuildingSize = true;

        public BuildingPlacingMode PlacingMode = BuildingPlacingMode.None;

        GameObject waypointHolder;
        GameObject roadconnectorHolder;

        //for handling waypoint generation
        GameObject editorColliderHolder;
        BoxCollider buildingSizeCol;
        BoxCollider waypointCutoffCol;
        BoxCollider roadconnectorCutoffCol;

        [HideInInspector]
        public List<MeshCollider> gizmoMeshColliders = new List<MeshCollider>();

        public Vector3 BuildingSize
        {
            get
            {
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                if (boxCollider != null)
                    return boxCollider.size;

                return Vector3.zero;
            }
        }

        GameObject ParkingSpaceContainer;

        private void OnEnable()
        {
            LoadEntrances();
            GetStarted();
        }
        private void OnDisable()
        {
            DestroyMeshColliders();
        }

        private void OnDestroy()
        {
            DestroyMeshColliders();
        }

        public void GetStarted()
        {
            building = GetComponent<BuildingMod>();
            Debug.Log("Got started BuildingEditor");

            if (building.building.BasicType == "GarageBuilding" || building.building.BasicType == "LogisticBuilding")
                GenerateParkingSpaceCollider();

            if (building.building.BasicType == "GlasshouseBuilding")
                GeneratePassageCollider();

            if (transform.Find("FirePointHolder"))
            {
                firePointsHolder = transform.Find("FirePointHolder").gameObject;
                roofFires = firePointsHolder.transform.Find("RoofFires").gameObject;
                wallFires = firePointsHolder.transform.Find("WallFires").gameObject;
                Debug.Log("Found FirePointHolder");
            }
            else
            {
                firePointsHolder = new GameObject();
                firePointsHolder.name = "FirePointHolder";
                firePointsHolder.transform.position = new Vector3(0, 0, 0);
                firePointsHolder.transform.localScale = new Vector3(1, 1, 1);
                firePointsHolder.transform.SetParent(building.transform);
                roofFires = new GameObject();
                roofFires.name = "RoofFires";
                roofFires.transform.position = new Vector3(0, 0, 0);
                roofFires.transform.localScale = new Vector3(1, 1, 1);
                roofFires.transform.SetParent(firePointsHolder.transform);

                wallFires = new GameObject();
                wallFires.name = "WallFires";
                wallFires.transform.position = new Vector3(0, 0, 0);
                wallFires.transform.localScale = new Vector3(1, 1, 1);
                wallFires.transform.SetParent(firePointsHolder.transform);
            }
        }

        #region Waypoints

        public void GeneratePossibleWaypoints()
        {
            if (waypointHolder != null)
            {
                DestroyImmediate(waypointHolder);
            }
            if (editorColliderHolder != null)
            {
                DestroyImmediate(editorColliderHolder);
            }

            editorColliderHolder = new GameObject();
            editorColliderHolder.name = "EditorColliderHolder";
            editorColliderHolder.transform.position = building.transform.position;
            editorColliderHolder.transform.localScale = new Vector3(1, 1, 1);
            buildingSizeCol = editorColliderHolder.AddComponent<BoxCollider>();
            waypointCutoffCol = editorColliderHolder.AddComponent<BoxCollider>();
            buildingSizeCol.size = BuildingSize;
            buildingSizeCol.isTrigger = true;

            waypointCutoffCol.size = new Vector3(BuildingSize.x + 8, BuildingSize.y, BuildingSize.z + 8);
            waypointCutoffCol.isTrigger = true;

            waypointHolder = new GameObject();
            waypointHolder.transform.parent = building.gameObject.transform;
            waypointHolder.transform.localPosition = new Vector3(0, 0, 0);
            waypointHolder.transform.localScale = new Vector3(1, 1, 1);
            waypointHolder.name = "WaypointHolder";

            Vector3Int roundedSize = BuildingSize.RoundToInt();
            roundedSize += new Vector3Int(8, 0, 8);
            roundedSize.y = 0;
            Vector3Int offsetVector = roundedSize / 2 - new Vector3Int(2, 0, 2);
            List<WaypointCandidate> candidateList = new List<WaypointCandidate>();
            for (int i = 0; i < roundedSize.x; i += 4)
            {
                for (int j = 0; j < roundedSize.z; j += 4)
                {
                    GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    capsule.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    capsule.name = "Waypoint";
                    capsule.transform.SetParent(waypointHolder.transform);
                    capsule.transform.localPosition = new Vector3(i, 0, j) - offsetVector;
                    candidateList.Add(capsule.AddComponent<WaypointCandidate>());
                    capsule.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;
                }
            }

            foreach (WaypointCandidate candidate in candidateList)
            {
                candidate.CheckIfValid(waypointCutoffCol, buildingSizeCol);
            }

            DestroyImmediate(editorColliderHolder);
            PlacingMode = BuildingPlacingMode.Waypoint;
        }

        public void ApplyWaypoint()
        {
            if (selectedWaypoint != null)
            {
                building.building.HasWayPoint = true;
                building.building.WayPoint = selectedWaypoint.transform.localPosition;
                selectedWaypoint.GetComponent<Renderer>().sharedMaterial.color = Color.white;
                DestroyImmediate(waypointHolder);
            }
        }

        #endregion

        #region Roadconnectors

        private static Mesh _roadConnectorMesh;

        private static Mesh RoadConnectorMesh
        {
            get 
            {
                if (_roadConnectorMesh == null)
                    _roadConnectorMesh = Resources.Load<Mesh>("Prefabs/arrow");

                return _roadConnectorMesh;
            }
        }

        private static GameObject _roadConnectorDummy;

        private static GameObject RoadConnectorDummy
        {
            get
            {
                if (_roadConnectorDummy == null)
                    _roadConnectorDummy= Resources.Load<GameObject>("Prefabs/RoadConnectorDummy");

                return _roadConnectorDummy;
            }
        }

        public void GeneratePossibleRoadconnectors()
        {
            if (waypointHolder != null)
            {
                DestroyImmediate(roadconnectorHolder);
            }

            if (editorColliderHolder != null)
            {
                DestroyImmediate(editorColliderHolder);
            }

            editorColliderHolder = new GameObject();
            editorColliderHolder.name = "EditorColliderHolder";
            editorColliderHolder.transform.position = building.transform.position;
            editorColliderHolder.transform.localScale = new Vector3(1, 1, 1);
            buildingSizeCol = editorColliderHolder.AddComponent<BoxCollider>();
            roadconnectorCutoffCol = editorColliderHolder.AddComponent<BoxCollider>();
            buildingSizeCol.size = BuildingSize;
            buildingSizeCol.isTrigger = true;

            roadconnectorCutoffCol.size = new Vector3(BuildingSize.x + 8, BuildingSize.y, BuildingSize.z + 8);
            roadconnectorCutoffCol.isTrigger = true;

            roadconnectorHolder = new GameObject();
            roadconnectorHolder.transform.parent = building.transform;
            roadconnectorHolder.transform.localPosition = new Vector3(0, 0, 0);
            roadconnectorHolder.transform.localScale = new Vector3(1, 1, 1);
            roadconnectorHolder.name = "RoadconnectorHolder";

            Vector3Int roundedSize = BuildingSize.RoundToInt();
            roundedSize += new Vector3Int(8, 0, 8);
            roundedSize.y = 0;
            Vector3Int offsetVector = roundedSize / 2 - new Vector3Int(2, 0, 2);
            List<RoadconnectorCandidate> candidateList = new List<RoadconnectorCandidate>();
            for (int i = 0; i < roundedSize.x; i += 4)
            {
                for (int j = 0; j < roundedSize.z; j += 4)
                {
                    GameObject roadConnectorDummy = Instantiate(RoadConnectorDummy, roadconnectorHolder.transform);

                    roadConnectorDummy.transform.localPosition = new Vector3(i, 0, j) - offsetVector;
                    roadConnectorDummy.transform.localRotation = Quaternion.LookRotation(roadConnectorDummy.transform.position.GetSnappedForwardVector(transform.position));
                    candidateList.Add(roadConnectorDummy.AddComponent<RoadconnectorCandidate>());
                }
            }

            foreach (RoadconnectorCandidate candidate in candidateList)
            {
                candidate.CheckIfValid(roadconnectorCutoffCol, buildingSizeCol);
            }

            DestroyImmediate(editorColliderHolder);
            PlacingMode = BuildingPlacingMode.RoadConnector;
        }

        public void ApplyRoadconnector()
        {
            if (selectedRoadconnector != null)
            {
                GameObject roadConnector = new GameObject("roadconnector");
                // Represents roadConnector layer
                roadConnector.layer = 13;
                roadConnector.transform.localPosition = selectedRoadconnector.transform.position;
                roadConnector.transform.localRotation = selectedRoadconnector.transform.localRotation;
                roadConnector.transform.parent = building.transform;
                DestroyImmediate(roadconnectorHolder);
            }
        }

        #endregion

        #region Fires

        public void GenerateRoofFirepoint(Vector3 firePoint)
        {
            GameObject roofFire = new GameObject();
            roofFire.name = "RoofFire " + firePoint;
            roofFire.transform.position = firePoint;
            roofFire.transform.localScale = new Vector3(1, 1, 1);
            roofFire.transform.SetParent(roofFires.transform);
            building.roofFirePoints.Add(roofFire);
        }

        public void GenerateWallFirepoint(Vector3 firePoint)
        {
            GameObject wallFire = new GameObject();
            wallFire.name = "WallFire " + firePoint;
            wallFire.transform.position = firePoint;
            wallFire.transform.localScale = new Vector3(1, 1, 1);
            wallFire.transform.SetParent(wallFires.transform);
            building.wallFirePoints.Add(wallFire);
        }

        #endregion

        #region AnimalSpawn

        public void GenerateAnimalSpawn(Vector3 animalSpawnPoint)
        {
            GameObject animalSpawn = new GameObject("spawn");
            animalSpawn.transform.position = animalSpawnPoint;
            animalSpawn.transform.localScale = new Vector3(1, 1, 1);
            animalSpawn.transform.SetParent(building.transform);
            var boxCollider = animalSpawn.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(1.5f, 0.5f, 1.5f);
        }

        #endregion

        #region Hive Feeding Location

        public void GenerateHiveFeedingLocation(Vector3 hiveFeedingLocation)
        {
            List<GameObject> oldHiveFeedingLocation = Extensions.FindChildrenWithNameContains(gameObject, true, true, "HiveFeedingLocation");
            for (int i = oldHiveFeedingLocation.Count - 1; i >= 0; --i)
                DestroyImmediate(oldHiveFeedingLocation[i]);

            GameObject animalSpawn = new GameObject();
            animalSpawn.name = "HiveFeedingLocation";
            animalSpawn.transform.position = hiveFeedingLocation;
            animalSpawn.transform.localScale = new Vector3(1, 1, 1);
            animalSpawn.transform.SetParent(building.transform);
            PlacingMode = BuildingPlacingMode.None;
        }

        #endregion

        #region Parking Space Colliders

        private bool _showParkingSpaces = true;

        public void GenerateParkingSpaceCollider()
        {
            if (GetPotentialParkingSpaceCollider() != null)
                return;

            GameObject parkingSpaceColliderObject = new GameObject("ParkingSpaceCollider");
            parkingSpaceColliderObject.transform.parent = building.transform;
            BoxCollider parkingSpaceCollider = parkingSpaceColliderObject.AddComponent<BoxCollider>();
            parkingSpaceCollider.size = BuildingSize;
            // Represents ParkingSpace layer
            parkingSpaceColliderObject.layer = 21;
            ShowParkingSpaceCollider();
        }

        public BoxCollider GetPotentialParkingSpaceCollider()
        {
            BoxCollider parkingSpaceCollider = null;
            var potentialSpaces = gameObject.FindChildrenWithNameContains(false, true, "ParkingSpaceCollider");
            foreach (var space in potentialSpaces)
            {
                var boxCollider = space.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    parkingSpaceCollider = boxCollider;
                    break;
                }
            }

            return parkingSpaceCollider;
        }

        public void ShowParkingSpaceCollider()
        {
            if (ParkingSpaceContainer == null)
            {
                ParkingSpaceContainer = new GameObject();
                ParkingSpaceContainer.transform.SetParent(transform);
            }


            ParkingSpaceContainer.RemoveAllChildren();

            _showParkingSpaces = !_showParkingSpaces;
            if (_showParkingSpaces)
            {
                DestroyImmediate(ParkingSpaceContainer);
                return;
            }

            BoxCollider parkingSpaceCollider = GetPotentialParkingSpaceCollider();

            if (parkingSpaceCollider != null)
            {
                var minX = parkingSpaceCollider.bounds.min.x;
                var maxX = parkingSpaceCollider.bounds.max.x;

                var minZ = parkingSpaceCollider.bounds.min.z;
                var maxZ = parkingSpaceCollider.bounds.max.z;


                for (float i = minX + 2.0f; i <= maxX; i += 4)
                {
                    for (float j = minZ + 2.0f; j <= maxZ; j += 4)
                    {
                        var pGrid = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        pGrid.transform.SetParent(ParkingSpaceContainer.transform);

                        pGrid.name = "ParkingGrid";
                        pGrid.transform.localScale = new Vector3(3.99f, 1, 3.99f);
                        pGrid.transform.localPosition = new Vector3(i, 1, j); ;
                    }
                }
            }
            else
            {
                GenerateParkingSpaceCollider();
                return;
            }
        }

        public void GenerateParkingSpaceCandidate(Vector3 position)
        {
            GameObject parkingSpaceCandidate = new GameObject();
            parkingSpaceCandidate.AddComponent<ParkingSpaceProperties>();
            parkingSpaceCandidate.name = "ParkingSpaceCandidate";
            parkingSpaceCandidate.transform.position = position;
            parkingSpaceCandidate.transform.localScale = new Vector3(1, 1, 1);
            parkingSpaceCandidate.transform.SetParent(building.transform);
        }

        #endregion

        #region Entrances

        private static GameObject DoorPrefab;
        private static GameObject GatePrefab;
        private static GameObject SlideUpPrefab;

        public void GenerateEntrance(Vector3 position) 
        {
            GameObject entrance;
            switch(PlacingMode)
            {
                default:
                case BuildingPlacingMode.Door:
                    entrance = Instantiate(DoorPrefab, position, Quaternion.identity);
                    doorObject = entrance;
                    break;
                case BuildingPlacingMode.Gate:
                    entrance = Instantiate(GatePrefab, position, Quaternion.identity);
                    gateObject = entrance;
                    break;
                case BuildingPlacingMode.SlideUp:
                    entrance = Instantiate(SlideUpPrefab, position, Quaternion.identity);
                    break;
            }

            entrance.transform.localScale = new Vector3(1, 1, 1);
            entrance.transform.SetParent(building.transform);
        }

        private static void LoadEntrances()
        {
            // Yeah it could probably be done much better
            if (DoorPrefab == null)
                DoorPrefab = Resources.Load<GameObject>("Prefabs/Door");

            if (GatePrefab == null)
                GatePrefab = Resources.Load<GameObject>("Prefabs/Gate");

            if (SlideUpPrefab == null)
                SlideUpPrefab = Resources.Load<GameObject>("Prefabs/SlideUp");
        }

        #endregion

        #region PassageCollider

        private void GeneratePassageCollider()
        {
            GlasshouseBuildingProperties glasshouseBuilding = (GlasshouseBuildingProperties)building.properties;
            if (glasshouseBuilding.PassageCollider != null)
                return;

            BoxCollider passageCollider = new GameObject("PassageCollider").AddComponent<BoxCollider>();
            passageCollider.transform.SetParent(building.transform);
            passageCollider.transform.position = Vector3.zero;
            passageCollider.transform.localScale = Vector3.one;
            glasshouseBuilding.PassageCollider = passageCollider;            
        }

        #endregion

        public void GenerateMeshColliders()
        {
            List<MeshRenderer> renderers = building.GetComponentsInChildren<MeshRenderer>(true).ToList();
            for (int i = 0; i < renderers.Count; i++)
                gizmoMeshColliders.Add(renderers[i].gameObject.AddComponent<MeshCollider>());
        }

        public void DestroyMeshColliders()
        {
            for (int i = gizmoMeshColliders.Count - 1; i >= 0; --i)
                DestroyImmediate(gizmoMeshColliders[i]);
        }

        private void OnDrawGizmos()
        {
            if (building == null)
                return;

            #region Waypoint

            if (building.building.HasWayPoint)
            {
                Handles.color = Color.white;
                Handles.Label(building.building.WayPoint + transform.position, "Waypoint");
                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f);
                Gizmos.DrawSphere(building.building.WayPoint + transform.position, 0.4f);
            }

            #endregion

            #region RoadConnectors

            List<GameObject> roadconnectors = Extensions.FindChildrenWithNameContains(gameObject, true, true, "roadconnector");

            for(int i = 0; i <  roadconnectors.Count; i++) 
            {
                Handles.color = Color.white;
                Handles.Label(roadconnectors[i].transform.position, "Roadconnector");
                Gizmos.color = new Color(1, 0, 0, 0.5f);

                // as;khkjhasdkjh;asd;hjuiasd
                Gizmos.DrawMesh(RoadConnectorMesh, roadconnectors[i].transform.position, roadconnectors[i].transform.rotation * Quaternion.Euler(90, 0, -90), new Vector3(0.25f, 0.25f, 0.25f));
            }

            #endregion

            #region BuildingSize Gizmo

            if (ShowBuildingSize)
            {
                Handles.color = Color.white;
                Vector3 labelPos = new Vector3(building.transform.position.x + BuildingSize.x / 2,
                    building.transform.position.y + BuildingSize.y / 2,
                    building.transform.position.z + BuildingSize.z / 2);
                Handles.Label(labelPos, "Building Size");
                Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.5f);

                BoxCollider buildingCollider = building.GetComponent<BoxCollider>();
                Gizmos.DrawCube(building.transform.position + (buildingCollider != null ? buildingCollider.center : Vector3.zero),
                                BuildingSize);
            }

            #endregion

            #region Doors

            var thickness = 8;
            if (doorObject != null)
            {
                Renderer firstRenderer = doorObject.GetComponentInChildren<Renderer>();                
                Vector3 startPos = firstRenderer != null ? firstRenderer.bounds.center : doorObject.transform.position;
                Vector3 targetPos = building.building.WayPoint + transform.position;
                Handles.color = Color.white;
                Handles.Label(startPos, "Door");
                startPos.y = 1;
                targetPos.y = 1;
                Handles.DrawBezier(startPos, targetPos, startPos, targetPos, Color.yellow, null, thickness);
            }
            if (gateObject != null)
            {
                Renderer firstRenderer = gateObject.GetComponentInChildren<Renderer>();
                Vector3 startPos = firstRenderer != null ? firstRenderer.bounds.center : gateObject.transform.position;
                Vector3 targetPos = building.building.WayPoint + transform.position;
                Handles.color = Color.white;
                Handles.Label(startPos, "Gate");
                startPos.y = 1;
                targetPos.y = 1;
                Handles.DrawBezier(startPos, targetPos, startPos, targetPos, Color.yellow, null, thickness);
            }

            #endregion

            #region Fire positions

            foreach (Transform firePoint in roofFires.transform.GetComponentsInChildren<Transform>())
            {
                if (firePoint == roofFires.transform) continue;
                if (Selection.activeGameObject != building.gameObject && Selection.Contains(firePoint.gameObject))
                    Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
                else
                    Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
                Gizmos.DrawSphere(firePoint.position, 0.2f);
            }
            foreach (Transform firePoint in wallFires.transform.GetComponentsInChildren<Transform>())
            {
                if (firePoint == wallFires.transform) continue;
                if (Selection.activeGameObject != building.gameObject && Selection.Contains(firePoint.gameObject))
                    Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
                else
                    Gizmos.color = new Color(1.0f, 0.64f, 0.0f, 0.5f); //orange color
                Gizmos.DrawSphere(firePoint.position, 0.2f);
            }

            #endregion

            #region Animal spawns

            if (building is AnimalsBuildingMod)
            {
                List<GameObject> animalSpawnPoints = building.gameObject.FindChildrenWithNameContains(true, true, "spawn");
                Gizmos.color = new Color32(255, 55, 55, 125);
                foreach (var point in animalSpawnPoints)
                {
                    Vector3 labelPos = point.transform.position + Vector3.up;
                    Handles.Label(labelPos, "Animal Spawn");
                    var boxCollider = point.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                        Gizmos.DrawCube(point.transform.position, boxCollider.size);
                }
            }

            #endregion

            #region Hive feeding location

            if (building is HiveBuildingMod)
            {
                List<GameObject> hiveFeedinglocations = building.gameObject.FindChildrenWithNameContains(true, true, "HiveFeedingLocation");
                foreach (var point in hiveFeedinglocations)
                {
                    Gizmos.color = new Color32(255, 55, 55, 125);
                    Gizmos.DrawSphere(point.transform.position, 0.5f);
                }
            }

            #endregion

            #region Parking Space Candidates

            if (building is LogisticBuildingMod || building is MechanicBuildingMod || building is VetBuildingMod)
            {
                ParkingSpaceProperties[] parkingSpaceProperties = building.GetComponentsInChildren<ParkingSpaceProperties>();

                Gizmos.color = new Color32(100, 255, 100, 125);
                foreach (var parkingSpaceProperty in parkingSpaceProperties)                
                    Gizmos.DrawCube(parkingSpaceProperty.transform.position, new Vector3(4, 4, 8));                
            }

            #endregion

            SetMaterialPropertyBlock();
        }

        void SetMaterialPropertyBlock()
        {
            var t = gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in t)
            {
                var props = new MaterialPropertyBlock();

                props.SetColor("_ColorInstanced", Color.white);
                props.SetFloat("_QualityLayer", 0);
                props.SetInt("_ActiveLayer", 0);
                props.SetColor("_ColorBase", Color.white);
                props.SetColor("_Color", Color.white);
                props.SetColor("_Color2", Color.white);
                props.SetFloat("_BlendAmount", 0);

                renderer.SetPropertyBlock(props);
            }
        }

    }

    public enum BuildingPlacingMode
    { 
        None, Waypoint, RoadConnector, FireRoof, FireWall, AnimalSpawn, HiveFeedingLocation, ParkingSpaceCandidate, Door, Gate, SlideUp
    }
}

#endif