using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bundle : Editor
{
    [MenuItem("Assets/ Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(@"/Users/hongmingi//Desktop/AssetBundle", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);

    }
}
