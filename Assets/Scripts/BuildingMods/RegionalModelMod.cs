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
    public class RegionalModelMod : BaseMod, ICopyTo
    {
        public RegionalModelProperties RegionalModel;

        public override Properties properties { get => RegionalModel; set => RegionalModel = (RegionalModelProperties)value; }

        private void Start()
        {
            // excplicit null assignment as translation is not needed for regional models
            properties.Translation = null;
        }

        public void CopyTo(Object oldObject, Object newObject, ref List<Component> componentsToDestroy)
        {
            RegionalModelMod oldModel = oldObject as RegionalModelMod;
            RegionalModelMod newModel = newObject as RegionalModelMod;

            newModel.RegionalModel = oldModel.RegionalModel;

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