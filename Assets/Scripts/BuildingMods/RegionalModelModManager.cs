using System.Collections.Generic;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// Component added to base of regional building which specifices sprites for different regions. <see cref="RegionalModelMod"/> meanwhile is object in one specific region
    /// </summary>
    public class RegionalModelModManager : MonoBehaviour, ICopyTo
    {
        public Sprite SpriteEurope;
        public Sprite SpriteAsia;
        public Sprite SpriteSouthAmerica;

        public Vector3 ParkingSelectorSnapHelperEurope = Vector3.zero;
        public Vector3 ParkingSelectorSnapHelperAsia = Vector3.zero;
        public Vector3 ParkingSelectorSnapHelperSouthAmerica = Vector3.zero;

        public bool SetParkingSnapRegional = false;

        public void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy)
        {
            RegionalModelModManager oldRegional = oldObject as RegionalModelModManager;
            RegionalModelModManager newRegional = newObject as RegionalModelModManager;

            newRegional.SpriteEurope = oldRegional.SpriteEurope;
            newRegional.SpriteAsia = oldRegional.SpriteAsia;
            newRegional.SpriteSouthAmerica = oldRegional.SpriteSouthAmerica;

            newRegional.ParkingSelectorSnapHelperEurope = oldRegional.ParkingSelectorSnapHelperEurope;
            newRegional.ParkingSelectorSnapHelperAsia = oldRegional.ParkingSelectorSnapHelperAsia;
            newRegional.ParkingSelectorSnapHelperSouthAmerica = oldRegional.ParkingSelectorSnapHelperSouthAmerica;
        }
    }
}