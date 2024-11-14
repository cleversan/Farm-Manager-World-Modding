using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// AnimalsBuildingMod component that is attached to GameObject
    /// </summary>
    public class AnimalsBuildingMod : BuildingMod
    {
        public AnimalsBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (AnimalsBuildingProperties)value; }
    }
}