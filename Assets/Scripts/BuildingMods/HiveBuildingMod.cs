using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// HiveBuildingMod component that is attached to GameObject
    /// </summary>
    public class HiveBuildingMod : BuildingMod
    {
        public HiveBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (HiveBuildingProperties)value; }
    }
}