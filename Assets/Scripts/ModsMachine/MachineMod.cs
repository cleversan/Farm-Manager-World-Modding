using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// MachineMod component that is attached to GameObject
    /// </summary>
    public class MachineMod : MachineBaseMod
    {
        public MachineProperties Machine;
        public override MachineProperties baseMachine { get => Machine; set => Machine = value; }
    }
}