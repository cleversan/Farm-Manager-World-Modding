#if UNITY_EDITOR
using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MachineBaseMod))]
    public class MachineEditor : SaveableEditor
    {
        public MachineBaseMod machine;

        public string FolderPath;
        [HideInInspector]
        public GameObject machineSocketHolder;
        [HideInInspector]
        public GameObject staffSocketHolder;
        [SerializeField]
        private bool ShowParkingSpaceCollider = true;

        public MachinePlacingMode PlacingMode = MachinePlacingMode.None;

        private static WheelProperties WheelPrefab;

        [HideInInspector] 
        public List<MeshCollider> gizmoMeshColliders = new List<MeshCollider>();

        private Vector3 ParkingSpaceSize
        {
            get
            {
                if (machine != null && machine.parkingSpaceCollider != null)
                    return machine.parkingSpaceCollider.size;

                return Vector3.zero;
            }
        }

        private void OnEnable()
        {
            GetStarted();
            if (WheelPrefab == null)
                WheelPrefab = Resources.Load<WheelProperties>("Prefabs/Wheel");
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
            machine = GetComponent<MachineBaseMod>();
            Debug.Log("Got started BuildingEditor");
            GenerateHolders();

            if (machine.parkingSpaceCollider != null && !IsColliderSizeGood4(machine.parkingSpaceCollider.size))
                CorrectColliderSize4(machine.parkingSpaceCollider.size);
        }

        public void GenerateStaffSocket(Vector3 firePoint, StaffSocketType type)
        {
            GameObject staffSocket = new GameObject();
            string socketType = type == StaffSocketType.Sitting ? "Sitting" : "Standing";

            staffSocket.name = "StaffSocket" + socketType + firePoint;
            staffSocket.transform.position = firePoint;
            staffSocket.transform.localScale = new Vector3(1, 1, 1);
            staffSocket.transform.SetParent(staffSocketHolder.transform);
            var marker = staffSocket.AddComponent<StaffSocketMarker>();
            marker.Type = type;
        }

        public void GenerateMachineSocket(Vector3 socketPoint)
        {
            GameObject machineSocket = new GameObject();
            machineSocket.name = "MachineSocket " + socketPoint;
            machineSocket.transform.position = socketPoint;
            machineSocket.transform.localScale = new Vector3(1, 1, 1);
            machineSocket.transform.SetParent(machineSocketHolder.transform);
            var socket = machineSocket.AddComponent<MachineSocketMarker>();
            socket.Parent = machine;
        }

        public void CreateWheel(Vector3 wheelPosition)
        {
            GameObject wheelObject = Instantiate(WheelPrefab).gameObject;
            wheelObject.name = "Wheel_ " + machine.GetComponentsInChildren<WheelProperties>(true).Length;
            wheelObject.transform.position = wheelPosition;
            wheelObject.transform.localScale = new Vector3(1, 1, 1);
            wheelObject.transform.SetParent(machine.transform);
        }

        public void GenerateSeedlingbox(GameObject gameObject)
        {
            gameObject.name = "seedlingsbox";
            if (gameObject.GetComponent<MeshFilter>() is null)
                gameObject.AddComponent<MeshFilter>();

            if (gameObject.GetComponent<MeshRenderer>() is null)
                gameObject.AddComponent<MeshRenderer>();

            if (gameObject.GetComponent<BoxCollider>() is null)
                gameObject.AddComponent<BoxCollider>();

            if (gameObject.GetComponent<SeedlingBox>() is null)
                gameObject.AddComponent<SeedlingBox>();

            PlacingMode = MachinePlacingMode.None;

            Selection.activeGameObject = gameObject;
        }

        void GenerateHolders()
        {
            if (transform.Find("MachineSocketHolder"))
            {
                machineSocketHolder = transform.Find("MachineSocketHolder").gameObject;
            }
            else
            {
                machineSocketHolder = new GameObject();
                machineSocketHolder.name = "MachineSocketHolder";
                machineSocketHolder.transform.position = new Vector3(0, 0, 0);
                machineSocketHolder.transform.localScale = new Vector3(1, 1, 1);
                machineSocketHolder.transform.SetParent(machine.transform);
            }

            if (transform.Find("StaffSocketHolder"))
            {
                staffSocketHolder = transform.Find("StaffSocketHolder").gameObject;
            }
            else
            {
                staffSocketHolder = new GameObject();
                staffSocketHolder.name = "StaffSocketHolder";
                staffSocketHolder.transform.position = new Vector3(0, 0, 0);
                staffSocketHolder.transform.localScale = new Vector3(1, 1, 1);
                staffSocketHolder.transform.SetParent(machine.transform);
            }
        }

        public void GenerateSpaceCollider()
        {
            Bounds bigBounds = new Bounds();

            if (machine.GetComponent<BoxCollider>() != null)
                DestroyImmediate(machine.GetComponent<BoxCollider>());

            if (machine.parkingSpaceCollider != null)
                DestroyImmediate(machine.parkingSpaceCollider.gameObject);
            
            bigBounds.center = machine.transform.position;

            foreach (var r in machine.GetComponentsInChildren<Renderer>())
                bigBounds.Encapsulate(r.bounds);

            BoxCollider parkingCollider = machine.gameObject.AddComponent<BoxCollider>();
            machine.parkingSpaceCollider = parkingCollider;
            parkingCollider.size = bigBounds.size;
            if (parkingCollider.size.magnitude < 1)
                parkingCollider.size = new(4, 4, 4);

            // set size of collider to bounds and position to center
            parkingCollider.size = bigBounds.size;

            // if magnitude is smaller then 1 then its clear sign that collider created is not valid, create proper one instead
            if (parkingCollider.size.magnitude < 1)
            {
                parkingCollider.center += new Vector3(0, 2, 0);
                parkingCollider.size = new(4, 4, 4);
            }

            machine.parkingSpaceCollider = parkingCollider;

            CorrectColliderSize4(parkingCollider.size);

            UnityEditorInternal.ComponentUtility.MoveComponentDown(this);
        }

        public bool IsColliderSizeGood4(Vector3 vector)
        {
            bool result = true;
            if (vector.x % 4 != 0)
                result = false;

            if (vector.z % 4 != 0)
                result = false;

            if (!result)
                Debug.LogError("Collider size is invalid, width and lenght must be multiple of 4, size will be corrected automatically");
            else
                Debug.Log("Collider size is valid");

            return result;
        }

        public void PlaceDestination(Vector3 destination)
        {
            if (machine is VehicleMod vehicle)
            {
                destination.y = vehicle.transform.position.y;
                vehicle.CurrentDestination = destination;
            }
        }

        public void AttachMachine(MachineBaseMod machineBase)
        {
            if (machine is VehicleMod vehicle && machineBase != null && machineBase != machine)
            {
                vehicle.AttachMachine(machineBase);
            }
        }

        public void DetachMachine()
        {
            if (machine is VehicleMod vehicle)
            {
                vehicle.DetachMachines();
            }
        }

        public void CorrectColliderSize4(Vector3 vector)
        {
            Debug.Log("Correcting collider size");
            Debug.Log("Vector before: " + vector);
            vector.x = Extensions.ToNearestMultiple(vector.x, 4, Extensions.ROUNDING.UP);
            vector.z = Extensions.ToNearestMultiple(vector.z, 4, Extensions.ROUNDING.UP);
            Debug.Log("Vector after: " + vector);

            machine.parkingSpaceCollider.size = vector;
        }

        public void GenerateMeshColliders()
        {
            List<MeshRenderer> renderers = machine.GetComponentsInChildren<MeshRenderer>(true).ToList();
            for(int i = 0; i < renderers.Count; i++)             
                gizmoMeshColliders.Add(renderers[i].gameObject.AddComponent<MeshCollider>());            
        }

        public void DestroyMeshColliders()
        {
            for (int i = gizmoMeshColliders.Count - 1; i >= 0; --i)            
                DestroyImmediate(gizmoMeshColliders[i]);            
        }

        private void OnDrawGizmos()
        {
            if (machine == null)
                return;

            #region Sockets

            foreach (var machineSocket in machine.transform.GetComponentsInChildren<MachineSocketMarker>())
            {
                Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
                Gizmos.DrawSphere(machineSocket.transform.position, 0.2f);
            }

            foreach (var staffSocket in machine.transform.GetComponentsInChildren<StaffSocketMarker>())
            {
                Gizmos.color = new Color(1.0f, 0.64f, 0.0f, 0.5f); //orange color
                Gizmos.DrawSphere(staffSocket.transform.position, 0.2f);
            }

            #endregion

            #region Parking Space Collider

            if (ShowParkingSpaceCollider)
            {
                Handles.color = Color.white;
                Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.5f);
                Vector3 labelPos = machine.transform.position + (ParkingSpaceSize / 2);
                if (machine.parkingSpaceCollider != null)
                {
                    Handles.Label(labelPos, "Parking Space Collider");
                    Gizmos.DrawCube(machine.transform.position + machine.parkingSpaceCollider.center, ParkingSpaceSize);
                }
                else
                    Handles.Label(labelPos, "No parking Space Collider detected");
            }

            #endregion
        }
    }

    public enum MachinePlacingMode
    {
        None, MachineSocket, StaffSocketStanding, StaffSocketSitting, Seedbox, Destination, AttachMachine, CreateWheel
    }

}
#endif