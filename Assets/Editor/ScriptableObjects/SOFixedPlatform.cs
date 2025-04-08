using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOFixedPlatform", menuName = "Platform Editor/Fixed Platform")]
public class SOFixedPlatform : ScriptableObject
{
    [HideInInspector] public string folderPath;
    [HideInInspector] public List<PlatformPoolData> fixedPlatforms;
}
