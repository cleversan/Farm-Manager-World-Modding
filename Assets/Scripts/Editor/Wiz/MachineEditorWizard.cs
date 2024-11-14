using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;
using FarmManagerWorld.Utils;

namespace FarmManagerWorld.Editors.Wizards
{
    public class MachineEditorWizard : ScriptableWizard
    {
        [HideInInspector]
        public MachineEditor machineEditor;
        [HideInInspector]
        public MachineBaseMod machineMod;

        [HideInInspector]
        public bool hasWheels;
        [Space(30)]
        [Header("Wheels rotate around Y axis")]
        public List<GameObject> wheels = new List<GameObject>();

        public void Initialize()
        {
            machineEditor = machineMod.GetComponent<MachineEditor>();
            if (!machineMod.gameObject)
            {
                return;
            }
            if (machineMod.baseMachine.MachineType == MachineType.Tractor)
            {
                DisplayWheelFields(true);
            }
            else
            {
                DisplayWheelFields(false);
            }            
        }

        private void OnWizardCreate()
        {
            foreach (var wheel in wheels)
            {
                machineMod.Wheels.Add(wheel.AddComponent<WheelProperties>());
            }
        }

        void DisplayWheelFields(bool value)
        {
            hasWheels = value;
        }
    }
}