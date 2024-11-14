using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// ProductionBuildingMod component that is attached to GameObject
    /// </summary>
    public class ProductionBuildingMod : BuildingMod
    {
        public ProductionBuildingProperties Building;

        public override BuildingProperties building { get => Building; set => Building = (ProductionBuildingProperties)value; }
    }

}