using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMTutorialDatabase : MonoBehaviour
{
    static private Dictionary<string,List<Tutorial>> tutorialDatabase;

    private const string PATH_TUTORIAL_DATABASE = "FMDatabase/TutorialDatabase";

    protected void Start()
    {
        Load();
    }

    public static Dictionary<string, List<Tutorial>> GetTutorialDatabase()
    {
        return tutorialDatabase;
    }

    private void Load()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(PATH_TUTORIAL_DATABASE);

        if (jsonFile != null)
        {
            tutorialDatabase = new Dictionary<string, List<Tutorial>>();
            TutorialWrapper wrapper = JsonUtility.FromJson<TutorialWrapper>(jsonFile.text);
            Tutorial[] tutorialSteps = wrapper.tutorial;
            string lastTutorialSection = string.Empty;

            for (int i = 0; i < tutorialSteps.Length; i++)
            {
                Tutorial tutorial = tutorialSteps[i];
                string tutorialSection = tutorial.TutorialSection;
                if (lastTutorialSection != tutorialSection)
                {
                    lastTutorialSection = tutorialSection;

                    List<Tutorial> specificTutorialSteps = new List<Tutorial>();
                    specificTutorialSteps.Add(tutorial);

                    tutorialDatabase.Add(tutorialSection, specificTutorialSteps);
                }
                else
                {
                    tutorialDatabase[tutorialSection].Add(tutorial);
                }
            }
        }
    }
}
