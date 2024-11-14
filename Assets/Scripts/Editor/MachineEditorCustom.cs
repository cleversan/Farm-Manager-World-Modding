using UnityEditor;
using UnityEngine;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Utils;
using System.Linq;
using FarmManagerWorld.Static;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Editors
{
    [CustomEditor(typeof(MachineEditor))]
    [CanEditMultipleObjects]
    public class MachineEditorCustom : SaveableEditorCustom
    {
        public MachineEditor editor;

        private bool _customMachineTag = false;
        private int _selectedMachineTagIndex;
        private string  _machineTagToAdd;

        private bool isMachine;

        private bool _customVehicleTag = false;
        private int _selectedVehicleTagIndex;
        private string _vehicleTagToAdd;


        void OnEnable()
        {
            editor = (MachineEditor)target;
            isMachine = editor.machine is not VehicleMod;
        }       

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            ModPopup();

            if (GUILayout.Button("Initialize"))
            {
                editor.GetStarted();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create new parking space collider"))
            {
                editor.GenerateSpaceCollider();
            }

            if (GUILayout.Button("Check if collider is valid"))
            {
                if (!editor.IsColliderSizeGood4(editor.machine.parkingSpaceCollider.size))
                    editor.CorrectColliderSize4(editor.machine.parkingSpaceCollider.size);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);           
            
            GUILayout.BeginVertical();
            if (GUILayout.Button("Cancel placing sockets"))
            {
                SetIsPlacing(MachinePlacingMode.None);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Place staff socket standing"))
            {

                SetIsPlacing(MachinePlacingMode.StaffSocketStanding);
            }
            if (GUILayout.Button("Place staff socket sitting"))
            {

                SetIsPlacing(MachinePlacingMode.StaffSocketSitting);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (GUILayout.Button("Place machine socket"))
            {
                SetIsPlacing(MachinePlacingMode.MachineSocket);
            }

            if (GUILayout.Button("Place wheel"))
            {
                SetIsPlacing(MachinePlacingMode.CreateWheel);
            }

            if (editor.machine is SowingMachineMod)
            {
                if (GUILayout.Button("Place seedbox"))
                {
                    SetIsPlacing(MachinePlacingMode.Seedbox);
                }
            }

            if (isMachine)
            {
                AddTagToStringOnGUI(
                    ref _customVehicleTag,
                    _customVehicleTag ? "Disable custom vehicle tag" : "Enable custom vehicle tag",
                    "Input custom vehicle tag to add",
                    "Select vehicle tags that will be able to use this machine (multiple)",
                    "Vehicle tag to add",
                    ref _vehicleTagToAdd,
                    StaticInformation.VehicleTags,
                    ref _selectedVehicleTagIndex,
                    ref editor.machine.baseMachine.VehicleTag,
                    false,
                    true);
            }

            AddTagToStringOnGUI(
                ref _customMachineTag,
                _customMachineTag ? "Disable custom machine tag" : "Enable custom machine tag",
                "Input custom machine tag to add",
                $"Select machine tag to describe this {(isMachine ? "machine" : "vehicle")} (allows adding new workflow)",
                "Machine tag to add",
                ref _machineTagToAdd,
                StaticInformation.MachineTags,
                ref _selectedMachineTagIndex,
                ref editor.machine.baseMachine.MachineTag,
                true,
                true);

            if (Application.isPlaying && editor.machine is VehicleMod)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Place destination to drive"))
                {
                    SetIsPlacing(MachinePlacingMode.Destination);
                }

                if (GUILayout.Button("Attach machine to vehicle"))
                {
                    SetIsPlacing(MachinePlacingMode.AttachMachine);
                }

                if (GUILayout.Button("Detach machine to vehicle"))
                {
                    DetachMachine();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            if (!Application.isPlaying && GUILayout.Button("Finalize for asset bundle"))
            {
                if (editor.machine.Validate() && CheckMod(editor.gameObject, modID, true, true) && CheckForStaffSockets())
                    FinalizeForAssetBundle(editor, editor.gameObject, modID, "machines");
            }
        }

        private void SetIsPlacing(MachinePlacingMode mode)
        {
            editor.PlacingMode = mode;
            switch (editor.PlacingMode)
            {
                default:
                case MachinePlacingMode.None:
                    editor.DestroyMeshColliders();
                    break;

                case MachinePlacingMode.Seedbox:
                case MachinePlacingMode.MachineSocket:
                case MachinePlacingMode.StaffSocketStanding:
                case MachinePlacingMode.StaffSocketSitting:
                case MachinePlacingMode.CreateWheel:
                    editor.GenerateMeshColliders();
                    break;
            }
        }

        public void OnSceneGUI()
        {
            switch (editor.PlacingMode)
            {
                default:
                case MachinePlacingMode.None:
                    break;

                case MachinePlacingMode.Seedbox:
                    PlaceSeedbox();
                    break;

                case MachinePlacingMode.Destination:
                    PlaceDestination();
                    break;

                case MachinePlacingMode.MachineSocket:
                    PlaceMachineSocket();
                    break;

                case MachinePlacingMode.StaffSocketStanding:
                    PlaceStaffSocket(StaffSocketType.Standing);
                    break;

                case MachinePlacingMode.StaffSocketSitting:
                    PlaceStaffSocket(StaffSocketType.Sitting);
                    break;

                case MachinePlacingMode.AttachMachine:
                    AttachMachine();
                    break;

                case MachinePlacingMode.CreateWheel:
                    PlaceWheel();
                    break;
            }
        }

        void PlaceStaffSocket(StaffSocketType type)
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

                        if (hitInfo.transform.gameObject.GetComponentInParent<MachineEditor>() != null)
                        {
                            editor.GenerateStaffSocket(hitInfo.point, type);
                        }
                    };
                }
            }
        }

        void PlaceMachineSocket()
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
                        if (hitInfo.transform.gameObject.GetComponentInParent<MachineEditor>() != null)
                        {
                            editor.GenerateMachineSocket(hitInfo.point);
                        }
                    }

                }
            }
        }

        void PlaceSeedbox()
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
                        if (hitInfo.transform.gameObject.GetComponentInParent<MachineEditor>() is MachineEditor editor && editor.machine.parkingSpaceCollider.gameObject != hitInfo.transform.gameObject)
                        {
                            editor.GenerateSeedlingbox(hitInfo.transform.gameObject);
                        }
                    }
                }
            }
        }

        void PlaceDestination()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    float distance;
                    if (new Plane(Vector3.up, Vector3.zero).Raycast(worldRay, out distance))
                    {
                        editor.PlaceDestination(worldRay.GetPoint(distance));
                    }
                }
            }
        }

        private void PlaceWheel()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo, 1000f, LayerMask.GetMask("Ground", "Default")))
                    {
                        editor.CreateWheel(hitInfo.point);
                        SetIsPlacing(MachinePlacingMode.None);
                    }
                }
            }
        }

        void AttachMachine()
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
                        MachineBaseMod machine = hitInfo.transform.gameObject.GetComponentInParent<MachineBaseMod>();

                        if (machine != null)
                        {
                            editor.AttachMachine(machine);
                        }
                    }
                }
            }
        }

        void DetachMachine()
        {
            editor.DetachMachine();
        }                

        bool CheckForStaffSockets()
        {
            bool validation = true;
            if (!isMachine && editor.machine.baseMachine is VehicleProperties vehicle && !vehicle.HideStaffWhenDriving)
            {
                var staffSockets = editor.GetComponentsInChildren<StaffSocketMarker>();
                if (staffSockets.FirstOrDefault(item => item.Type == StaffSocketType.Sitting) is null)
                {  
                    Debug.LogError("Vehicles is set to show Staff that entered it but there is no StaffSocketMarker with Type set to \"Sitting\", validation failed");
                    validation = false;
                }
            }

            return validation;
        }
    }
}

