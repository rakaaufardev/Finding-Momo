using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SOCoinSetup))]
public class FMCoinSetupEditor : Editor
{
    private SOCoinSetup scriptableObject;
    private string[] coinNames;
    private string path = "Assets/Resources/FMCoin";

    private void OnEnable()
    {
        scriptableObject = (SOCoinSetup)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Check Coins"))
        {
            CheckCoins();
        }

        if (GUILayout.Button("Load"))
        {
            scriptableObject.Load();
        }

        if (GUILayout.Button("Save"))
        {
            scriptableObject.Save();
        }
    }

    void CheckCoins()
    {
        scriptableObject.Init();
        string[] assetPaths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        coinNames = assetPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
        List<string> coinNameList = coinNames.ToList();
        scriptableObject.AddCoinList(coinNameList);
    }
}

[CreateAssetMenu(fileName = "SOCoinSetup", menuName = "Coin Setup")]
public class SOCoinSetup : ScriptableObject
{
    public List<CoinSetupData> coinList;

    public void Init()
    {
        coinList = new List<CoinSetupData>();
    }

    public void AddCoinList(List<string> coinNames)
    {
        int count = coinNames.Count;
        for (int i = 0; i < count; i++)
        {
            string name = coinNames[i];

            CoinSetupData newSetup = new CoinSetupData();
            newSetup.coinName = name;
            newSetup.collectCount = 0; // Default collect count

            coinList.Add(newSetup);
        }
    }

    public void Load()
    {
        string loadPath = Application.dataPath + "/Resources/FMDatabase/CoinDatabase.json";

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                CoinWrapper wrapper = JsonUtility.FromJson<CoinWrapper>(json);

                if (wrapper != null)
                {
                    coinList = wrapper.coinList;
                    Debug.Log("Load Database Coin Success.");
                }
                else
                {
                    Debug.LogError("Load Database Coin Failed: Data is null.");
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Load Database Coin Failed: " + e.Message);
            }
        }
    }

    public void Save()
    {
        CoinWrapper coinWrapper = new CoinWrapper();
        coinWrapper.coinList = coinList;

        string json = JsonUtility.ToJson(coinWrapper);
        string savePath = Application.dataPath + "/Resources/FMDatabase/CoinDatabase.json";

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("Save Database Coin Success. File is saved to " + savePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Save Database Coin Failed: " + e.Message);
        }
    }
}

[Serializable]
public class CoinSetupData
{
    public string coinName;
    public int collectCount;
}

[Serializable]
public class CoinWrapper
{
    public List<CoinSetupData> coinList;
}