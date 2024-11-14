using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// DecoBuildingMod component that is attached to GameObject
    /// </summary>
    public class DecoBuildingMod : BuildingMod
    {
        public DecoBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (DecoBuildingProperties)value; }
    }
}