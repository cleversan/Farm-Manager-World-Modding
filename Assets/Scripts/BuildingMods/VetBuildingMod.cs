using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// VetBuildingMod component that is attached to GameObject
    /// </summary>
    public class VetBuildingMod : BuildingMod
    {
        public VetBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (VetBuildingProperties)value; }
    }
}