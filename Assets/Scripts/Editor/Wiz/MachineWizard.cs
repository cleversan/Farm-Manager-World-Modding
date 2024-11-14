using FarmManagerWorld.Static;
using UnityEditor;
using UnityEngine;
using System;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Modding.Mods;

namespace FarmManagerWorld.Editors.Wizards
{
    public class MachineWizard : ScriptableWizard
    {
        MachineBaseMod machineMod;

        GameObject mod;

        public GameObject[] colliderModels;
        [Range(1, 6)]
        public int numberOfLods = 1;

        [HideInInspector]
        public string MachineType;
        public MachineType type;
        public string Name;
        public Sprite sprite;
        public Sprite spriteDestroyed;

        private void OnWizardCreate()
        {
            CreationProcess();
        }

        private void CreationProcess()
        {
            MachineType = type.ToString();
            GameObject gameObject = new GameObject();

            string machineType = StaticInformation.GetMachinePropertyType(type);

            machineMod = (MachineBaseMod)gameObject.AddComponent(StaticInformation.Modtypes[machineType]);
            MachineProperties comp = (MachineProperties)Activator.CreateInstance(StaticInformation.MachinesDictionary[machineType]);

            comp.MachineType = type;
            comp.BasicType = comp.IsMachine() ? "Vehicle" : "Machine";

            machineMod.gameObject.name = comp.Name + "_" + comp.BasicType;
            machineMod.baseMachine = comp;

            machineMod.baseMachine.Name = Name;
            machineMod.gameObject.name = machineMod.baseMachine.Name + "_" + comp.BasicType;

            mod = LODsGenerator.GenerateEmptyLOD(numberOfLods, true);

            mod.transform.SetParent(machineMod.transform);
            mod.gameObject.name = machineMod.baseMachine.Name + "_Model";

            machineMod.model = mod;
                      
            foreach (var obj in machineMod.GetComponentsInChildren<Transform>())            
                obj.name = obj.name.Replace("(Clone)", "");

            MachineEditor editor = machineMod.gameObject.AddComponent<MachineEditor>();
            editor.GenerateSpaceCollider();
            machineMod.gameObject.layer = 20; // Machine layer
            machineMod.baseMachine.Icon = sprite;
            machineMod.baseMachine.IconDestroyed = spriteDestroyed;
            MachineEditorWizard wizard = (MachineEditorWizard)DisplayWizard("Machine Editor", typeof(MachineEditorWizard), "Finish");
            wizard.machineMod = machineMod;
            wizard.Initialize();
        }
    }
}
