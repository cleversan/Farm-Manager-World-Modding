using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// GarageBuildingMod component that is attached to GameObject
    /// </summary>
    public class GarageBuildingMod : BuildingMod
    {
        public GarageBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (GarageBuildingProperties)value; }
    }
}