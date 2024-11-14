using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    public enum ROUNDING { UP, DOWN, CLOSEST }

    public static float ToNearestMultiple(this float f, int multiple, ROUNDING roundTowards = ROUNDING.CLOSEST)
    {
        f /= multiple;

        return (roundTowards == ROUNDING.UP ? Mathf.Ceil(f) : (roundTowards == ROUNDING.DOWN ? Mathf.Floor(f) : Mathf.Round(f))) * multiple;
    }

    /// <summary>
    /// Using a multiple with a maximum of two decimal places, will round to this value based on the ROUNDING method chosen
    /// </summary>
    public static float ToNearestMultiple(this float f, float multiple, ROUNDING roundTowards = ROUNDING.CLOSEST)
    {
        f = float.Parse((f * 100).ToString("f0"));
        multiple = float.Parse((multiple * 100).ToString("f0"));

        f /= multiple;

        f = (roundTowards == ROUNDING.UP ? Mathf.Ceil(f) : (roundTowards == ROUNDING.DOWN ? Mathf.Floor(f) : Mathf.Round(f))) * multiple;

        return f / 100;
    }

    /// <summary>
    /// Round Vector3 and return it as Vector3Int
    /// </summary>
    public static Vector3Int RoundToInt(this Vector3 v, ROUNDING roundTowards = ROUNDING.CLOSEST) 
    {
        if (roundTowards == ROUNDING.CLOSEST)
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z), Mathf.RoundToInt(v.z));

        if (roundTowards == ROUNDING.UP)
            return new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.z), Mathf.CeilToInt(v.z));

        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.z), Mathf.FloorToInt(v.z));
    }

    public static Vector3 GetForwardVectorAwayFrom(this Vector3 position, Vector3 pointAwayFrom)
    {
        return -(pointAwayFrom - position).normalized;
    }

    public static Vector3 GetSnappedForwardVector(this Vector3 position, Vector3 pointAwayFrom)
    {
        float dotProduct = Vector3.Dot(GetForwardVectorAwayFrom(position, pointAwayFrom), Vector3.forward);
        if (dotProduct >= 0.75f)
        {
            return Vector3.forward;
        }
        else if (dotProduct >= -0.75f)
        {
            if (position.x > pointAwayFrom.x)
                return Vector3.right;
            else return Vector3.left;
        }   
        else
        {
            return Vector3.back;
        }
    }

    /// <summary>
    /// Returns all children of parent GameObject that have name starting with prefix
    /// </summary>
    /// <param name="parent">Parent GameObject which children should be returned</param>
    /// <param name="namePrefix">Prefix which should be present in children names</param>
    /// <param name="exact">If set to false it converts names to lower case else it looks for exact names</param>
    /// <returns>List of found children</returns>
    public static List<GameObject> FindChildrenWithNameContains(this GameObject parent, bool exact, bool recursive, params string[] prefixes)
    {
        var children = new List<GameObject>();

        if (parent == null || prefixes == null || prefixes.Length == 0)
            return children;

        if (!exact)
            for (var i = 0; i < prefixes.Length; i++)
                prefixes[i] = prefixes[i].ToLower();

        for (var i = 0; i < parent.transform.childCount; i++)
        {
            var child = parent.transform.GetChild(i).gameObject;
            var childName = child.name;
            if (!exact)
                childName = childName.ToLower();
            for (var j = 0; j < prefixes.Length; j++)
            {
                if (!childName.Contains(prefixes[j]))
                    continue;

                children.Add(child);
                break;
            }

            if (recursive)
                children.AddRange(FindChildrenWithNameContains(child, exact, true, prefixes));
        }

        return children;
    }

    public static void RemoveAllChildren(this GameObject gameObject)
    {
        for (int i = gameObject.transform.childCount - 1; i >= 0; --i)
        {
            Object.DestroyImmediate((gameObject.transform.GetChild(i).gameObject));
        }
    }

    public static void DestroyComponents(List<Component> componentsToDestroy)
    { 
        for(int i = componentsToDestroy.Count - 1; i >= 0; --i)
        {
            Object.DestroyImmediate(componentsToDestroy[i], true);
        }
    }

    public static Vector3 GetDirection(Vector3 to, Vector3 from, bool skipY = true)
    {
        if (skipY)
        {
            var x = to.x - from.x;
            var z = to.z - from.z;
            var d = Mathf.Sqrt(x * x + z * z);
            return new Vector3(x / d, 0, z / d);
        }
        else
        {
            return (to - from).normalized;
        }
    }
    public static float XZDistance(this Vector3 v, Vector3 v2)
    {
        var x = v.x - v2.x;
        var z = v.z - v2.z;
        return Mathf.Sqrt(x * x + z * z);
    }

#if UNITY_EDITOR
    [MenuItem("Farm Manager/Set Material Property Block", false)]
    public static void SetMaterialPropertyBlock()
    {
        var t = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in t)
        {
            var props = new MaterialPropertyBlock();

            props.SetColor("_ColorInstanced", Color.white);
            props.SetFloat("_QualityLayer", 0);
            props.SetInt("_ActiveLayer", 0);
            props.SetColor("_ColorBase", Color.white);
            props.SetColor("_Color", Color.white);
            props.SetColor("_Color2", Color.white);
            props.SetFloat("_BlendAmount", 0);

            renderer.SetPropertyBlock(props);
        }
    }

    public static void UpdateMaterialPropertyBlock(GameObject selectedObject, float amount, string propertyName)
    {
        var t = selectedObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in t)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);

            block.SetFloat(propertyName, amount);

            renderer.SetPropertyBlock(block);
        }
    }

    /// <summary>
    /// Im just tired, so im gonna copy this random function so future reader, please forgive me
    /// </summary>
    public static T[] FindGameObjectsByType<T>() where T : MonoBehaviour
    {
        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<T> result = new List<T>();
        foreach (string s in temp)
        {
            if (s.Contains(".prefab"))
            {
                UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(s);
                GameObject go = o as GameObject;
                if (go != null)
                {
                    Component[] components = go.GetComponentsInChildren<Component>(true);
                    if (go.GetComponentInChildren<T>() != null)
                    {
                        result.Add(go.GetComponentInChildren<T>());
                    }
                }
            }
        }

        return result.ToArray();
    }

    public static bool IsObjectVisible(this UnityEngine.Camera @this, Bounds bounds)
    {
        GameObject box = new GameObject();
        List<BoxCollider> boxColliders = new List<BoxCollider>();

        BoxCollider p1 = box.AddComponent<BoxCollider>();
        p1.center = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        p1.size = Vector3.one;

        BoxCollider p2 = box.AddComponent<BoxCollider>();
        p2.center = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        p2.size = Vector3.one;

        BoxCollider p3 = box.AddComponent<BoxCollider>();
        p3.center = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        p3.size = Vector3.one;

        BoxCollider p4 = box.AddComponent<BoxCollider>();
        p4.center = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        p4.size = Vector3.one;

        BoxCollider p5 = box.AddComponent<BoxCollider>();
        p5.center = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        p5.size = Vector3.one;

        BoxCollider p6 = box.AddComponent<BoxCollider>();
        p6.center = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
        p6.size = Vector3.one;

        BoxCollider p7 = box.AddComponent<BoxCollider>();
        p7.center = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        p7.size = Vector3.one;

        BoxCollider p8 = box.AddComponent<BoxCollider>();
        p8.center = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        p8.size = Vector3.one;

        boxColliders.Add(p1);
        boxColliders.Add(p2);
        boxColliders.Add(p3);
        boxColliders.Add(p4);
        boxColliders.Add(p5);
        boxColliders.Add(p6);
        boxColliders.Add(p7);
        boxColliders.Add(p8);

        bool isVisible = true;

        foreach (var b in boxColliders)
        {
            var test = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(@this), b.bounds);
            if (!test)
            {
                isVisible = false;
                break;
            }
                
        }

        GameObject.Destroy(box);
        return isVisible;
    }

#endif
}