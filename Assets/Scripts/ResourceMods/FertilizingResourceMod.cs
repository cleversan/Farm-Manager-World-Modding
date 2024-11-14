using FarmManagerWorld.Modding.ObjectProperties;
namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// FertilizingResourceMod component that is attached to GameObject
    /// </summary>
    public class FertilizingResourceMod : ResourceMod
    {        
        public FertilizingResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (FertilizingResourceProperties)value; }
    }
}