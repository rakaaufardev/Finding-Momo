using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelSixSlots : SpinWheelSlots
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
        if (rootVisual.localEulerAngles.z >= 331 || rootVisual.localEulerAngles.z <= 30)
        {
            pickedIconIndex = 0;
        }
        else if (rootVisual.localEulerAngles.z >= 271 && rootVisual.localEulerAngles.z <= 330)
        {
            pickedIconIndex = 5;
        }
        else if (rootVisual.localEulerAngles.z >= 31 && rootVisual.localEulerAngles.z <= 270)
        {
            float x = rootVisual.localEulerAngles.z + 30;
            x /= 60;

            pickedIconIndex = (int)x;
        }

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
