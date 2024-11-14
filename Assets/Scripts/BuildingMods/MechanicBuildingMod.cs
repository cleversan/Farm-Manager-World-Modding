using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// MechanicBuildingMod component that is attached to GameObject
    /// </summary>
    public class MechanicBuildingMod : BuildingMod
    {
        public MechanicBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (MechanicBuildingProperties)value; }
    }
}