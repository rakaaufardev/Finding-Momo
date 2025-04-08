using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMRandomStartSign : MonoBehaviour
{
    [SerializeField] private List<GameObject> logoSigns;
    private GameObject selectedSign;

    public void GenerateStartSign()
    {
        if (selectedSign != null)
        {
            Destroy(selectedSign);
        }

        int randomIndex = 0;
        randomIndex = Random.Range(0, logoSigns.Count);

        selectedSign = Instantiate(logoSigns[randomIndex], transform);
        selectedSign.transform.localPosition = Vector3.zero;
        selectedSign.transform.localRotation = Quaternion.identity;
        selectedSign.transform.localScale = Vector3.one;
    }
}
