using System;
using System.Linq;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Utils;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// VehicleMod component that is attached to GameObject
    /// </summary>
    public class VehicleMod : MachineBaseMod
    {
        public VehicleProperties Machine;
        public override MachineProperties baseMachine { get => Machine; set => Machine = (VehicleProperties)value; }

#if UNITY_EDITOR

        public Vector3? CurrentDestination;
        private float currentSpeed;

        private Vector3 GetVehiclePosition { get { return transform.position; } }
        private Vector3 GetLastVehiclePosition;
        private Vector3 WantedForward;
        private Vector3 rotationVelocity;
        private Vector3 CurrentVelocity;

        private void Update()
        {
            UpdateAttachedMachine();

            if (!CurrentDestination.HasValue)
                return;

            currentSpeed = (GetVehiclePosition - GetLastVehiclePosition).magnitude;
            GetLastVehiclePosition = GetVehiclePosition;
            var wantedDirection = Extensions.GetDirection(CurrentDestination.Value, GetVehiclePosition);
            if (Vector3.Dot(wantedDirection, transform.forward) > -0.9)
                WantedForward = Vector3.SmoothDamp(transform.forward, wantedDirection, ref rotationVelocity, 0.085f);
            else
                WantedForward = wantedDirection;

            var wantedSpeed = (Machine.MaxSpeed / 20);

            if (Single.IsNaN(CurrentVelocity.x))
                return;

            var newPosition = Vector3.SmoothDamp(GetVehiclePosition, GetVehiclePosition + (wantedDirection * wantedSpeed), ref CurrentVelocity, 0.3f);
            var distanceToTarget = transform.position.XZDistance(CurrentDestination.Value);
            var dsitanceToNewPosition = transform.position.XZDistance(newPosition);
            if (distanceToTarget < dsitanceToNewPosition)
            {
                newPosition = transform.position + (wantedDirection * distanceToTarget);
                CurrentDestination = null;
            }

            if (!Single.IsNaN(newPosition.x) && !Single.IsNaN(newPosition.y) && !Single.IsNaN(newPosition.z))
                transform.position = newPosition;
            else
                Debug.LogError($"Tried to set NaN as new position, old position{transform.position}, new position: {newPosition}");

            UpdateWheelRotation(currentSpeed * 30);
            transform.forward = WantedForward;
        }

        private void UpdateAttachedMachine()
        {
            if (MachineSockets == null)
                return;

            for (int i = 0; i < MachineSockets.Length; i++)
            {
                if (MachineSockets[i] == null || MachineSockets[i].AttachedMachine == null)
                    continue;

                if (currentSpeed > 0 && CurrentDestination.HasValue)
                    MachineSockets[i].AttachedMachine.UpdateWheelRotation(currentSpeed * 30);

                var newAttachedMachinePosition = MachineSockets[i].transform.position + (MachineSockets[i].AttachedMachine.transform.forward * MachineSockets[i].AttachedMachine.SockefOffsetXZ * (MachineSockets[i].IsFrontSocket ? 1 : -1));
                float posY = 0;

                MachineSockets[i].AttachedMachine.transform.position = new Vector3(newAttachedMachinePosition.x, posY, newAttachedMachinePosition.z);

                if (!MachineSockets[i].AttachedMachine.baseMachine.IsFixedJoint && Vector3.Dot(MachineSockets[i].AttachedMachine.transform.forward, transform.forward) > -0.9)
                {
                    MachineSockets[i].AttachedMachine.transform.forward = Vector3.SmoothDamp(MachineSockets[i].AttachedMachine.transform.forward, transform.forward, ref MachineSockets[i].ForwardVelocity, 0.5f);
                }
                else
                {
                    MachineSockets[i].AttachedMachine.transform.forward = transform.forward;
                }
            }
        }

        public void AttachMachine(MachineBaseMod machine)
        {
            MachineSocketMarker socket = MachineSockets.FirstOrDefault();
            if (socket != null)
            {
                socket.AttachedMachine = machine;
            }
        }

        public void DetachMachines()
        {
            for (int i = 0; i < MachineSockets.Length; i++)
            {
                MachineSockets[i].AttachedMachine = null;
            }
        }
#endif

    }
}