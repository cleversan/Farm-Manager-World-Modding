using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FarmManagerWorld.Modding.ObjectProperties
{
    /// <summary>
    /// Base building properties class 
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Building")]
    public abstract class BuildingProperties : Properties
    {
        /// <summary>
        /// Icon of the nonregional building as seen in game on UI
        /// </summary>
        [XmlIgnore]
        public Sprite Icon;

        /// <summary>
        /// Designated point that staff and vehicles will try to reach before entering the building if building does not contain Entrance. 
        /// When trying to go from Building A (Entrance exists) to Building B (only waypoint) and back this will be path:
        /// <list type="number">
        /// <item>Staff leaves Building A by going to closest doors, opening it and trying to reach RoadConnector assigned to this entrance</item>
        /// <item>Staff uses pathing algorith to reach WayPoint of Building B</item>
        /// <item>Staff finishes their job at building B</item>
        /// <item>Staff leaves Building B by going to WayPoint</item>
        /// <item>Staff uses pathfinding algorithm to reach closest entrance relative to their position in Building A</item>
        /// <item>Staff will enter open the door and try to enter inside building through it</item>
        /// </list> 
        /// </summary>
        public Vector3 WayPoint;        

        /// <summary>
        /// SFX volume 
        /// </summary>
        public float SoundFXVolume;       

        /// <summary>
        /// Duration of pause after construction ends expressed in second
        /// </summary>
        public float BuildTime = 60;

        /// <summary>
        /// Monthly maintenance cost
        /// </summary>
        public float Monthly = 10;

        /// <summary>
        /// Price of the building
        /// </summary>
        public float Price = 100;

        /// <summary>
        /// List of Training IDs required to build building, refer to documentation for specific information. 
        /// </summary>
        public List<int> RequiredTraining = new List<int>();

        /// <summary>
        /// Trainings that building will use for certain actions or upgrades like Storage Space Optimization (ID: 43) or Regrigeration Systems (ID: 44). 
        /// Please refer to training section of documentation for more information 
        /// </summary>
        public List<int> AdditionalTraining = new List<int>();

        /// <summary>
        /// List of ResourceType that this building can hold
        /// </summary>
        public List<ResourceType> StoredResourceType;

        /// <summary>
        /// Max staff you can assign to this building. 
        /// </summary>
        public int MaxAssignedStaffCount = 3;

        /// <summary>
        /// Max storage capacity for holding resources, ingredients for production, food for animals or fuel for machines
        /// </summary>
        public int MaxResourcesCapacity = 10;

        /// <summary>
        /// Max storage capacity for resources produced by animals or production line. It is separate to avoid clogging and other unforseen consequences
        /// </summary>
        public int MaxProductionResourcesCapacity = 10;

        /// <summary>
        /// Range of effect expressed in meters (1 tile equals 4 meters), used in buildings to specify in what area they are detected/work. It affects:
        /// <list type="bullet">
        /// <item>warehouses as for it to be detected by i.e. AnimalBuildings or ProductionBuilding </item>
        /// <item>irrigation, mechanic and veterinarian buildings when determining their working radius</item>
        /// </list>
        /// </summary>
        public int RangeRadius = 50;

        /// <summary>
        /// Should building be built instantly i.e. Silos
        /// </summary>
        public bool BuildImmediate = false;

        /// <summary>
        /// Should building snap to grid or not (like most decorations)
        /// </summary>
        public bool SnapToGrid = true;

        /// <summary>
        /// Should game adjust ground height to avoid graphical glitches
        /// </summary>
        public bool SetGroundHeight = false;

        /// <summary>
        /// Should building be visible in Build Menu i.e. hives are buildings but are not visible in Build Menu.
        /// </summary>
        public bool VisibleInBuildMenu = true;

        /// <summary>
        /// Marks if Staff can enter the building. If true, it will try to enter the building via door
        /// </summary>
        public bool CanEntrance = true;

        /// <summary>
        /// Does building need electricity to operate.
        /// </summary>
        public bool NeedElectricity = true;

        /// <summary>
        /// Used during saving to determine if WayPoint has been added or not
        /// </summary>
        [XmlIgnore]
        public bool HasWayPoint;

        /// <summary>
        /// Adjust parking selector snapping to better align machines so they do not take 2 parking spaces.
        /// </summary>
        public Vector3 ParkingSelectorSnapHelper;

        public override bool ValidateProperties()
        {
            bool validation = true;
            if (Icon == null)
            {
                Debug.LogError($"Icon at Building {Name} cannot be null, validation failed");
                validation = false;
            }

            if (Price <= 0)
            {
                Debug.LogError($"Price is lower or equal then 0, was it intentional?");
            }

            // moved to separate line to avoid potential short-circuiting and to unify with other Validate methods
            bool baseValidation = base.ValidateProperties();
            return baseValidation && validation;
        }
    }
}
