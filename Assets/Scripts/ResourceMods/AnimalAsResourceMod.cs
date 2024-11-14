using FarmManagerWorld.Modding;
using FarmManagerWorld.Modding.ObjectProperties;
using System.Linq;
using UnityEngine;
namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// AnimalAsResourceMod component that is attached to GameObject
    /// </summary>
    public class AnimalAsResourceMod : ResourceMod
    {
        public AnimalAsResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (AnimalAsResourceProperties)value; }
    }
}