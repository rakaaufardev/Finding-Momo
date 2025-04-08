using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Networking;
using System.IO;
using System;
using VD;

[Serializable]
public class BundleData
{
    public string bundleUrl;
    public string manifestUrl;
    public string localBundlePath;
    public string localVersionPath;
}

public enum BundleName
{
    ShopThumbnail,
    ShopCostume,
    ShopCostumeMaterials,
    COUNT
}

public class FMAssetBundleController : MonoBehaviour
{
    Dictionary<BundleName, BundleData> bundleContainer;

    private const int DOWNLOAD_PROCESS_COUNT = 2;

    private static FMAssetBundleController singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMAssetBundleController Get()
    {
        return singleton;
    }

    public void DownloadBundles()
    {
        bundleContainer = VDParameter.BUNDLE_CONTAINER;
        int bundleDataCount = bundleContainer.Count;
        FMDownloadProgressChecker.AddCheck(bundleDataCount * DOWNLOAD_PROCESS_COUNT);
        StartCoroutine(CheckAndUpdateAssetBundle());
    }

    IEnumerator CheckAndUpdateAssetBundle()
    {
        int bundleDataCount = bundleContainer.Count;
        BundleName[] bundleNames = new BundleName[bundleDataCount];
        BundleData[] bundleDatas = new BundleData[bundleDataCount];
        bundleContainer.Keys.CopyTo(bundleNames, 0);
        bundleContainer.Values.CopyTo(bundleDatas, 0);

        for (int i = 0; i < bundleDataCount; i++)
        {
            BundleName bundleName = bundleNames[i];
            BundleData bundleData = bundleDatas[i];

            string bundleUrl = bundleData.bundleUrl;
            string manifestUrl = bundleData.manifestUrl;
            string bundleLocalPath = bundleData.localBundlePath;
            string bundleLocalVersionPath = bundleData.localVersionPath;

            UnityWebRequest manifestRequest = UnityWebRequest.Get(manifestUrl);
            yield return manifestRequest.SendWebRequest();
            FMDownloadProgressChecker.SetAsComplete(1);

            if (manifestRequest.result != UnityWebRequest.Result.Success)
            {
                VDLog.LogError("Failed to fetch manifest: " + manifestRequest.error);
                yield break;
            }

            string manifestContent = manifestRequest.downloadHandler.text;
            string remoteVersion = string.Empty;

            foreach (var line in manifestContent.Split('\n'))
            {
                if (line.StartsWith("CRC:"))
                {
                    remoteVersion += line.Split(':')[1].Trim();
                }
            }

            string localVersion = File.Exists(bundleLocalVersionPath) ? File.ReadAllText(bundleLocalVersionPath) : null;

            if (localVersion != remoteVersion)
            {
                VDLog.Log("New AssetBundle detected. Downloading...");
                using (UnityWebRequest www = UnityWebRequest.Get(bundleUrl))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        VDLog.LogError("Error downloading AssetBundle: " + www.error);
                    }
                    else
                    {
                        File.WriteAllBytes(bundleLocalPath, www.downloadHandler.data);
                        File.WriteAllText(bundleLocalVersionPath, remoteVersion);
                        yield return LoadAssetBundleFromCache(bundleLocalPath, bundleName);
                        VDLog.Log("New AssetBundle downloaded!");
                    }
                }

            }
            else
            {
                yield return LoadAssetBundleFromCache(bundleLocalPath, bundleName);
                VDLog.Log("AssetBundle is up-to-date.");
            }

            FMDownloadProgressChecker.SetAsComplete(1);
        }
    }

    IEnumerator LoadAssetBundleFromCache(string localPath, BundleName bundleName)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(localPath);
        if (bundle == null)
        {
            VDLog.LogError("Failed to load AssetBundle from cache: " + localPath);
            yield break;
        }

        switch (bundleName)
        {
            case BundleName.ShopThumbnail:
                SpriteAtlas atlas = bundle.LoadAsset<SpriteAtlas>("ATL_Shop");
                if (atlas != null)
                {
                    FMShopController.Get.SetSpriteAtlasShop(atlas);
                }
                else
                {
                    VDLog.LogError("SpriteAtlas not found in AssetBundle: " + atlas);
                }
                break;
            case BundleName.ShopCostume:
                string[] costumeNames = bundle.GetAllAssetNames();
                GameObject[] costumes = bundle.LoadAllAssets<GameObject>();
                if (costumes != null)
                {
                    int count = costumes.Length;
                    for (int i = 0; i < count; i++)
                    {
                        //string costumeNameFull = costumeNames[i];
                        GameObject costume = costumes[i];
                        string costumeName = costume.name;
                        FMShopController.Get.SetCostumeModelViewer(costumeName, costume);
                    }
                }
                else
                {
                    VDLog.LogError("costumes not found in AssetBundle: " + costumes);
                }
                break;
        }        

        bundle.Unload(false);
    }
}
