using UnityEditor;
using UnityEngine;

namespace FarmManagerWorld.Editors
{
    [CustomEditor(typeof(BoxCollider))]
    [CanEditMultipleObjects]
    public class BoxColliderEditor : Editor
    {
        private BoxCollider BoxCollider { get { return (BoxCollider)(target); } }
        public override void OnInspectorGUI () 
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Round to 4"))
            {
                Vector3 size = BoxCollider.size;
                size.x = Extensions.ToNearestMultiple(size.x, 4, Extensions.ROUNDING.CLOSEST);
                size.z = Extensions.ToNearestMultiple(size.z, 4, Extensions.ROUNDING.CLOSEST);
                BoxCollider.size = size;
            }
        }
    }
}