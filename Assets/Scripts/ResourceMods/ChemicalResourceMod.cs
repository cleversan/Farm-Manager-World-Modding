using FarmManagerWorld.Modding.ObjectProperties;
using UnityEngine;
namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// ChemicalResourceMod component that is attached to GameObject
    /// </summary>
    public class ChemicalResourceMod : ResourceMod
    {
        public ChemicalResourceProperties Resource;

        public override ResourceProperties resource { get => Resource; set => Resource = (ChemicalResourceProperties)value; }
    }
}