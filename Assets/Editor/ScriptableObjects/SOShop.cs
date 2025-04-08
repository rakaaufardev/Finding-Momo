using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using VD;

[CustomEditor(typeof(SOShop))]
public class FMShopEditor : Editor
{
    private SOShop scriptableObject;

    private const string PATH_SAVE = "/Resources/FMDatabase/ShopConfig.json";

    private void OnEnable()
    {
        scriptableObject = (SOShop)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save"))
        {
            Save();
        }

        if (GUILayout.Button("Load"))
        {
            Load();
        }

        if (GUILayout.Button("Download"))
        {
            Download();
        }
    }

    public async void Download()
    {
        Tuple<string, List<ShopData>> result = await DoDownload();
        scriptableObject.version = result.Item1;
        scriptableObject.shopDatas = result.Item2;
    }

    public static async Task<Tuple<string,List<ShopData>>> DoDownload()
    {
        Tuple<string, List<ShopData>> result = null;
        string version = null;
        List<ShopData> shopDatas = null;

        OnlineConfigData shopOnlineConfigData = VDParameter.SHOP_ONLINE_CONFIG_DATA;
        string configUrl = shopOnlineConfigData.configUrl;
        string versionUrl = shopOnlineConfigData.versionUrl;

        VDLog.Log("Start fetch shop online config");

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(versionUrl);
            if (!response.IsSuccessStatusCode)
            {
                VDLog.Log("Failed to fetch version: " + response.ReasonPhrase);
                return result;
            }

            version = await response.Content.ReadAsStringAsync();

            VDLog.Log("Fetch version complete: " + response.ReasonPhrase);
        }

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(configUrl);
            if (!response.IsSuccessStatusCode)
            {
                VDLog.Log("Failed to fetch config: " + response.ReasonPhrase);
                return result;
            }

            string configContent = await response.Content.ReadAsStringAsync();

            ShopConfigData shopWrapper = JsonUtility.FromJson<ShopConfigData>(configContent);
            shopDatas = shopWrapper.shopDatas;

            VDLog.Log("Fetch config complete: " + response.ReasonPhrase);
        }

        VDLog.Log("Fetch online config success!");
        result = new Tuple<string, List<ShopData>>(version, shopDatas);
        return result;
    }

    public void Load()
    {
        string loadPath = Application.dataPath + PATH_SAVE;

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                ShopConfigData shopWrapper = JsonUtility.FromJson<ShopConfigData>(json);

                if (shopWrapper != null)
                {
                    scriptableObject.shopDatas = shopWrapper.shopDatas;
                    VDLog.Log("Load Shop Setup Database Success");
                }
                else
                {
                    VDLog.LogError("Load Shop Setup Database Failed: Data is null");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError("Load Shop Setup Database Failed: " + e.Message);
            }
        }
    }

    public async void Upload()
    {
        await DoUpload();
    }

    public async Task DoUpload()
    {
        OnlineConfigData shopOnlineConfigData = VDParameter.SHOP_ONLINE_CONFIG_DATA;
        string configUrl = shopOnlineConfigData.configUrl;
        string versionUrl = shopOnlineConfigData.versionUrl;

        VDLog.Log("Start upload shop online config");

        using (HttpClient client = new HttpClient())
        {
            string jsonData = JsonUtility.ToJson(scriptableObject.version);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(versionUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                VDLog.Log("Failed to upload version: " + response.ReasonPhrase);
                return;
            }

            VDLog.Log("Version upload complete: " + response.ReasonPhrase);
        }

        using (HttpClient client = new HttpClient())
        {
            string jsonData = JsonUtility.ToJson(scriptableObject.shopDatas);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(configUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                VDLog.Log("Failed to upload config: " + response.ReasonPhrase);
                return;
            }

            VDLog.Log("Config upload complete: " + response.ReasonPhrase);
        }

        VDLog.Log("Upload shop online config complete!");
    }

    public void Save()
    {
        ShopConfigData shopWrapper = new ShopConfigData();
        shopWrapper.shopDatas = scriptableObject.shopDatas;

        string json = string.Empty;
        json = JsonUtility.ToJson(shopWrapper);

        string savePath = Application.dataPath + PATH_SAVE;

        try
        {
            File.WriteAllText(savePath, json);
            VDLog.Log("Save Shop Database Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Shop Database Failed: " + e.Message);
        }
    }
}

[CreateAssetMenu(fileName = "Shop", menuName = "Shop Setup")]
public class SOShop : ScriptableObject
{
    public string version;
    public List<ShopData> shopDatas;
}
