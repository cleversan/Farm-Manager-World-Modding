using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// FoliageResourceMod component that is attached to GameObject
    /// </summary>
    public class FoliageResourceMod : ResourceMod
    {
        public FoliageResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (FoliageResourceProperties)value; }

    }
}