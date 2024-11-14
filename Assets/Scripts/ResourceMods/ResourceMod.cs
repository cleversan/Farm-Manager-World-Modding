using UnityEngine;
using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Translations;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// ResourceMod component that is attached to GameObject
    /// </summary>
    public abstract class ResourceMod : BaseMod
    {
        public abstract ResourceProperties resource { get; set; }

        public override Properties properties { get => resource; set => resource = (ResourceProperties)value; }

        public bool ValidateResource()
        {
            return resource.ValidateProperties();
        }
    }
}