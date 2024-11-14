using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// ProductionResourceMod component that is attached to GameObject
    /// </summary>
    public class ProductionResourceMod : ResourceMod
    {
        public ProductionResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (ProductionResourceProperties)value; }
    }
}