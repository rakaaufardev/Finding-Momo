using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System;
using UnityEngine.Networking;
using System.Reflection;
using System.IO;
using VD;

public class FMPowerUpImporter
{
    static string url = "https://docs.google.com/spreadsheets/d/1oepZzMpratHv8B9RK0Ur5gBOTGqsILvsc8fkPgMNi20/pub?gid=0&single=true&output=tsv";

    [MenuItem("Tools/Visual Dart/Import Power Up")]
    static void ImportPowerUp()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(ImportProcess());
    }

    static IEnumerator ImportProcess()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            VDLog.LogError(string.Format("Import Database Tutorial Error : {0}", www.error));
        }
        else
        {
            string data = www.downloadHandler.text;
            ProcessData(data);
        }        
    }

    static void ProcessData(string data)
    {
        List<string> enums = new List<string>();
        string[] rows = data.Split('\n');
        Dictionary<string, PowerUpData> powerUpList = new Dictionary<string, PowerUpData>();
        Type importerType = typeof(FMPowerUpImporter);
        string classContent = 
            "using System;\n" +
            "using System.Collections.Generic;" +
            "\n" +
            "public class FMPowerUpCollection\n" +
            "{" +
            "\n    " +
            "public static Dictionary<string, PowerUpData> powerUpList = new Dictionary<string, PowerUpData>()\n" +
            "{\n";
        int count = rows.Length;

        for (int i = 1; i < count; i++)
        {
            string row = rows[i];
            if (string.IsNullOrWhiteSpace(row))
                continue;

            string[] columns = row.Split('\t');

            if (columns.Length < 4)
                continue;

            string name = columns[0];
            string value = columns[1];
            string duration = columns[2];
            string actionName = columns[3];

            enums.Add(name);

            // Generate the power-up entry
            string powerUpEntry = $"        {{ \"{name}\", new PowerUpData(\"{value}\", \"{duration}\", (string val) => new FMPowerUp_{name}().PowerUpEffect(val)) }},\n";
            classContent += powerUpEntry;
        }

        classContent += 
            "};\n" +
            "}\n";

        classContent += 
            "public enum PowerUpType\n" +
            "{\n";

        int enumCount = enums.Count;
        for (int i = 0; i < enumCount; i++)
        {
            string name = enums[i];
            classContent += name + ",\n";
        }

        classContent += "}";

        string path = "Assets/Scripts/FMPowerUp/FMPowerUpCollection.cs";

        File.WriteAllText(path, classContent);
        AssetDatabase.Refresh();

        VDLog.Log("FMPowerUp class successfully generated.");
    }
}
