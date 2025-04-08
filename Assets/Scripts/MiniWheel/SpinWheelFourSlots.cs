using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheelFourSlots : SpinWheelSlots
{
    public override void AssignIcons(string targetIcon)
    {
        int count = iconImages.Count;

        for (int i = 0; i < 2; i++)
        {
            iconImages[i].sprite = FMAssetFactory.GetGarbageIcon(targetIcon);
        }

        for (int j = 2; j < count; j++)
        {
            iconImages[j].sprite = FMAssetFactory.GetGarbageIcon(GarbageName());
        }
    }

    public override bool SpinResult(string target)
    {
        pickedIconIndex = (int)(rootVisual.transform.localEulerAngles.z / 90);
        
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
