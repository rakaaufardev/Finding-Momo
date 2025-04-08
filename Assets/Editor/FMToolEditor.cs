using UnityEditor;
using System.IO;

public class FMToolEditor
{
    [MenuItem("Tools/Visual Dart/Clear User Data")]
    static void ClearUserData()
    {
        UnityEngine.PlayerPrefs.DeleteAll();

        string path = UnityEngine.Application.persistentDataPath;

        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            File.Delete(file);
        }

        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            Directory.Delete(directory, true);
        }
    }

    [MenuItem("Tools/Visual Dart/Build Asset Bundles Android")]
    static void BuildAssetBundles_Android()
    {
        string outputDirectory = "Assets/AssetBundleOutputAndroid";
        BuildAssetBundles(outputDirectory, BuildTarget.Android);
    }

    [MenuItem("Tools/Visual Dart/Build Asset Bundles Desktop")]
    static void BuildAssetBundles_Desktop()
    {
        string outputDirectory = "Assets/AssetBundleOutputDesktop";
        BuildAssetBundles(outputDirectory, BuildTarget.StandaloneWindows);
    }

    static void BuildAssetBundles(string outputDirectory, BuildTarget buildTarget)
    {
        string assetBundlePath_thumbnailShop = "Assets/AssetBundleThumbnailShop";
        string assetBundlePath_costumeShop = "Assets/AssetBundleCostumeShop";

        AssetBundleBuild[] setup = new AssetBundleBuild[2]
        {
            new AssetBundleBuild
            {
                assetBundleName = "shopThumbnail",
                assetNames = new[]
                {
                    assetBundlePath_thumbnailShop,
                }
            },
            new AssetBundleBuild
            {
                assetBundleName = "shopCostume",
                assetNames = new[]
                {
                    assetBundlePath_costumeShop
                }
            }
        };

        BuildPipeline.BuildAssetBundles(
            outputDirectory,
            setup,
            BuildAssetBundleOptions.None,
            buildTarget
        );
    }
}
