using FarmManagerWorld.Editors;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Translations;
using FarmManagerWorld.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// MachineBaseMod component that is attached to GameObject
    /// </summary>
    public abstract class MachineBaseMod : BaseMod, ICopyTo
    {
        [Header("Testing section - items below are only used when testing \nand will be automatically assigned in Play mode if none were assigned")]
        [Space]
        public List<WheelProperties> Wheels = null;
        [SerializeField] protected MachineSocketMarker[] MachineSockets;
        [SerializeField] protected float testSpeed = -1.0f;
        [Header("End of testing section")]
        [Space]

        public GameObject model;
        public BoxCollider parkingSpaceCollider;
        public MachineSocketMarker CurrentSocket;
        public abstract MachineProperties baseMachine { get; set; }
        public override Properties properties { get => baseMachine; set => baseMachine = (MachineProperties)value; }
        private void Start()
        {
            MachineSockets = GetComponentsInChildren<MachineSocketMarker>();
            Wheels = GetComponentsInChildren<WheelProperties>().ToList();
        }
        public void UpdateWheelRotation(float speed)
        {
            for (int wheelIndex = 0; wheelIndex < Wheels.Count; wheelIndex++)
                Wheels[wheelIndex].RotateWheel(speed);
        }

        public float SockefOffsetXZ
        {
            get
            {
                if (CurrentSocket != null)
                    return transform.position.XZDistance(CurrentSocket.transform.position);

                if (MachineSockets.FirstOrDefault() != null)
                    return transform.position.XZDistance(MachineSockets.FirstOrDefault().transform.position);

                return 0.0f;
            }
        }

        public void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy)
        {
            MachineBaseMod oldMachineBaseMod = oldObject as MachineBaseMod;
            MachineBaseMod newMachineBaseMod = newObject as MachineBaseMod;
            newMachineBaseMod.baseMachine = oldMachineBaseMod.baseMachine;
            newMachineBaseMod.parkingSpaceCollider = oldMachineBaseMod.parkingSpaceCollider;

            List<WheelProperties> wheelProperties = newMachineBaseMod.gameObject.GetComponentsInChildren<WheelProperties>(true).ToList();

            for (int i = 0; i < wheelProperties.Count; ++i)
            {
                componentsToDestroy.Add(wheelProperties[i]);
                WheelProperties newEntrance = wheelProperties[i].gameObject.AddComponent<WheelProperties>();
                newEntrance.CopyTo(wheelProperties[i], newEntrance, ref componentsToDestroy);
            }
        }

        public override bool Validate()
        {
            if (model == null)
            {
                Debug.LogError($"Model cannot be null, validation failed");
                return false;
            }

            if (parkingSpaceCollider == null)
            {
                Debug.LogError($"ParkingSpaceCollider cannot be null, validation failed");
                return false;
            }

            if (TryGetComponent(out MachineEditor machineEditor))            
                machineEditor.CorrectColliderSize4(parkingSpaceCollider.size);  

            return baseMachine.ValidateProperties();
        }
    }
}