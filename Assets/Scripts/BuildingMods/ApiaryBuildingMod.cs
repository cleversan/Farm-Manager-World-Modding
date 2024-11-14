using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// ApiaryBuildingMod component that is attached to GameObject
    /// </summary>
    public class ApiaryBuildingMod : BuildingMod
    {
        public ApiaryBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (ApiaryBuildingProperties)value; }
    }
}