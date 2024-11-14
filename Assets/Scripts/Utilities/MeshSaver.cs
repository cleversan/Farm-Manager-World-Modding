#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MeshSaver 
{
	public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance = false, bool optimizeMesh = false)
	{
		string path = Path.Combine(Application.dataPath, "tmp", "tmpMeshes", name);
		if (string.IsNullOrEmpty(path)) return;

		path = FileUtil.GetProjectRelativePath(path);

		Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

		if (optimizeMesh)
			MeshUtility.Optimize(meshToSave);

		AssetDatabase.CreateAsset(meshToSave, path);
		AssetDatabase.SaveAssets();
	}
}
#endif