using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// SowingMachineMod component that is attached to GameObject
    /// </summary>
    public class SowingMachineMod : MachineBaseMod
    {
        public SowingMachineProperties Machine;
        public override MachineProperties baseMachine { get => Machine; set => Machine = (SowingMachineProperties)value; }
    }
}