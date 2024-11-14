using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// GlasshouseBuildingMod component that is attached to GameObject
    /// </summary>
    public class GlasshouseBuildingMod : BuildingMod
    {
        public GlasshouseBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (GlasshouseBuildingProperties)value; }
    }
}