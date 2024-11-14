// Create an AssetBundle for Windows.
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuildAssetBundles
{
    public static DirectoryInfo BuildAllAssetBundles()
    {
        string assetBundleDirectory = "AssetBundles";

        DirectoryInfo directoryInfo;

        if (!Directory.Exists(assetBundleDirectory))        
            directoryInfo = Directory.CreateDirectory(assetBundleDirectory);        
        else
            directoryInfo = new DirectoryInfo("AssetBundles");

        foreach (FileInfo file in directoryInfo.GetFiles())        
            file.Delete();
        
        foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            dir.Delete(true);        

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);
        return directoryInfo;
    }
}
