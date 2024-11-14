using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;
using System.Linq;
using UnityEngine;
namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// HoneyResourceMod component that is attached to GameObject
    /// </summary>
    public class HoneyResourceMod : ResourceMod
    {
        public HoneyResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (HoneyResourceProperties)value; }
    }
}