using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// AnimalResourceMod component that is attached to GameObject
    /// </summary>
    public class AnimalResourceMod : ResourceMod
    {
        public AnimalResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (AnimalResourceProperties)value; }
    }
}