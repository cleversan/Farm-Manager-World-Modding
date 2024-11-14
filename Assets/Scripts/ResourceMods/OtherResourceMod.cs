using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// OtherResourceMod component that is attached to GameObject
    /// </summary>
    public class OtherResourceMod : ResourceMod
    {
        public OtherResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (OtherResourceProperties)value; }
    }
}