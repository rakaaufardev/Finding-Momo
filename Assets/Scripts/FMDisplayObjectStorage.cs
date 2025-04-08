using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMDisplayObjectStorage : MonoBehaviour
{
    [SerializeField] private Transform root;
    private static FMDisplayObjectStorage singleton;
    private Dictionary<string, List<FMDisplayObject>> displayObjectStorages;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMDisplayObjectStorage Get()
    {
        return singleton;
    }

    public FMDisplayObject Load(string name)
    { 
        FMDisplayObject displayObject = null;

        if (displayObjectStorages != null)
        {
            bool displayObjectExist = displayObjectStorages.ContainsKey(name);

            if (displayObjectExist)
            {
                List<FMDisplayObject> displayObjects = displayObjectStorages[name];

                if (displayObjects.Count > 0)
                {
                    displayObject = displayObjects[0];
                }
            }
        }

        return displayObject;
    }

    public void Save(FMDisplayObject displayObject)
    {
        if (displayObjectStorages == null)
        {
            displayObjectStorages = new Dictionary<string, List<FMDisplayObject>>();
        }

        displayObject.transform.SetParent(root);
        displayObject.transform.localPosition = Vector3.zero;
        displayObject.transform.localEulerAngles = Vector3.zero;

        string displayObjectName = displayObject.gameObject.name;

        if (!displayObjectStorages.ContainsKey(displayObjectName))
        {
            List<FMDisplayObject> listDisplayObject = new List<FMDisplayObject>();
            listDisplayObject.Add(displayObject);
            displayObjectStorages.Add(displayObjectName, listDisplayObject);
        }
    }
}
