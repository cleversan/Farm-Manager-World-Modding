using FarmManagerWorld.Modding.ObjectProperties;
using FarmManagerWorld.Static;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmManagerWorld.Modding.Mods
{
    /// <summary>
    /// Empty mod model to ease loading of regional models
    /// </summary>
    public class RegionalModelMod : MonoBehaviour, ICopyTo
    {
        public string parentBuildingName;
        public StaticInformation.Region Region;

        public void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy)
        {
            RegionalModelMod oldModel = oldObject as RegionalModelMod;
            RegionalModelMod newModel = newObject as RegionalModelMod;

            newModel.parentBuildingName = oldModel.parentBuildingName;

            List<EntranceProperties> entranceProperties = newModel.gameObject.GetComponentsInChildren<EntranceProperties>(true).ToList();

            for (int i = 0; i < entranceProperties.Count; ++i)
            {
                componentsToDestroy.Add(entranceProperties[i]);
                EntranceProperties newEntrance = entranceProperties[i].gameObject.AddComponent<EntranceProperties>();
                newEntrance.CopyTo(entranceProperties[i], newEntrance, ref componentsToDestroy);
            }
        }
    }
}