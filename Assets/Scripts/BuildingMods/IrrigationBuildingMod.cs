using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// IrrigationBuildingMod component that is attached to GameObject
    /// </summary>
    public class IrrigationBuildingMod : BuildingMod
    {
        public IrrigationBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (IrrigationBuildingProperties)value; }
    }
}