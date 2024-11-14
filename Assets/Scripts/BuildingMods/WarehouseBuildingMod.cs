using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// WarehouseBuildingMod component that is attached to GameObject
    /// </summary>
    public class WarehouseBuildingMod : BuildingMod
    {
        public WarehouseBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (WarehouseBuildingProperties)value; }
    }

}