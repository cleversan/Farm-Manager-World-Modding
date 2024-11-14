using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// LogisticBuildingMod component that is attached to GameObject
    /// </summary>
    public class LogisticBuildingMod : BuildingMod
    {
        public LogisticBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (LogisticBuildingProperties)value; }
    }
}