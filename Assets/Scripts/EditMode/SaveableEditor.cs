#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors
{    
    public class SaveableEditor : MonoBehaviour 
    {
        [HideInInspector] public int SelectedMod;
        [HideInInspector] public string ModID;
    }
}
#endif