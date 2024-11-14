using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// StaffBuildingMod component that is attached to GameObject
    /// </summary>
    public class StaffBuildingMod : BuildingMod
    {
        public StaffBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (StaffBuildingProperties)value; }
    }
}