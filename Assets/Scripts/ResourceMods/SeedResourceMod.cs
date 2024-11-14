using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;
using System.Linq;
using UnityEngine;
namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// SeedResourceMod component that is attached to GameObject
    /// </summary>
    public class SeedResourceMod : ResourceMod
    {
        public SeedResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (SeedResourceProperties)value; }
        public GameObject plant { get => Resource.plant; set => Resource.plant = value; }

    }
}