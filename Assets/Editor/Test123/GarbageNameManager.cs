using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class GarbageNameManager : MonoBehaviour
{
    private static List<string> cachedGarbageNames;

    public static string GetRandomGarbageName()
    {
        // Load names if not cached
        if (cachedGarbageNames == null || cachedGarbageNames.Count == 0)
        {
            LoadGarbageNames();
        }

        // Get random name from cache
        if (cachedGarbageNames != null && cachedGarbageNames.Count > 0)
        {
            return cachedGarbageNames[UnityEngine.Random.Range(0, cachedGarbageNames.Count)];
        }

        return "DefaultGarbage";
    }

    private static void LoadGarbageNames()
    {
        string jsonPath = Application.dataPath + "/Resources/FMDatabase/GarbagePoolDatabase.json";
        if (File.Exists(jsonPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonPath);
                GarbagePoolWrapper wrapper = JsonUtility.FromJson<GarbagePoolWrapper>(jsonContent);

                if (wrapper != null && wrapper.garbageNames != null)
                {
                    cachedGarbageNames = wrapper.garbageNames;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading garbage pool JSON: " + e.Message);
                cachedGarbageNames = new List<string>();
            }
        }
    }

    // Call this when you want to force reload the names from JSON
    public static void ReloadGarbageNames()
    {
        cachedGarbageNames = null;
        LoadGarbageNames();
    }
}