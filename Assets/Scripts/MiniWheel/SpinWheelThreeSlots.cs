using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelThreeSlots : SpinWheelSlots
{
    public override void AssignIcons(string targetIcon)
    {
        int count = iconImages.Count;

        for (int i = 0; i < 1; i++)
        {
            iconImages[i].sprite = FMAssetFactory.GetGarbageIcon(targetIcon);
        }

        for (int j = 1; j < count; j++)
        {
            iconImages[j].sprite = FMAssetFactory.GetGarbageIcon(GarbageName());
        }
    }

    public override bool SpinResult(string target)
    {
        pickedIconIndex = (int)(rootVisual.transform.localEulerAngles.z / 120);

        bool isTrue = CheckReward(target, pickedIconIndex);

        if (isTrue)
        {
            PunchIcon(pickedIconIndex);
        }
        else
        {
            ShakeIcon(pickedIconIndex);
        }

        return isTrue;
    }
}
