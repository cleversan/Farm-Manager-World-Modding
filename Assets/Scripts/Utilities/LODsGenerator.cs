using System.Linq;
using UnityEngine;

public static class LODsGenerator
{
    public static GameObject GenerateEmptyLOD(int lodCount, bool createEmptyLODs) 
    {
        GameObject parentMesh = new GameObject();
        GameObject lodMeshes = new GameObject();

        lodMeshes.transform.SetParent(parentMesh.transform);
        lodMeshes.name = "LODGroup";

        lodMeshes.AddComponent<LODGroup>();
        LODGroup group = lodMeshes.GetComponent<LODGroup>();

        if (createEmptyLODs)
        {
            for (int lodIndex = 0; lodIndex < lodCount; ++lodIndex)
            {
                GameObject emptyLOD = new GameObject($"LOD{lodIndex}");
                emptyLOD.transform.SetParent(lodMeshes.transform);
            }
        }

        LOD[] lods = new LOD[lodCount];

        for (int lodIndex = 0; lodIndex < lodCount; ++lodIndex)        
            lods[lodIndex] = new LOD(1.0f / (lodIndex + 5.0f), null);

        lods[lods.Length-1].screenRelativeTransitionHeight = 0.01f;

        group.SetLODs(lods);

        return parentMesh;
    }
}
