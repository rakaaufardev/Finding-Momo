using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class SpinWheelSlots : MonoBehaviour
{
    [SerializeField] protected RectTransform rootVisual;
    [SerializeField] protected List<Image> iconImages;
    protected string garbageName;
    protected int pickedIconIndex;

    private bool hasPressed;
    private bool isMaxSpeed;
    private float currentSpeed;

    public int PickedIconIndex
    {
        get => pickedIconIndex;
    }

    public bool IsMaxSpeed
    {
        get => isMaxSpeed;
    }

    public bool HasPressed
    {
        set => hasPressed = value;
    }

    public abstract bool SpinResult(string target);
    public abstract void AssignIcons(string targetIcon);

    public void Show(bool isShow, string? targetIcon)
    {
        rootVisual.gameObject.SetActive(isShow);
        if (targetIcon != null) AssignIcons(targetIcon);
    }

    public void SpinWheel()
    {
        if (!rootVisual.gameObject.activeInHierarchy)
        {
            return;
        }

        if (currentSpeed < VDParameter.SPIN_WHEEL_MAX_SPEED)
        {
            currentSpeed += VDParameter.SPIN_WHEEL_ACCELERATION * Time.deltaTime;
        }
        else
        {
            isMaxSpeed = true;
        }

        if (hasPressed)
        {
            currentSpeed = 0;
            isMaxSpeed = false;
            return;
        }

        rootVisual.Rotate(0, 0, currentSpeed);
    }

    protected string GarbageName()
    {
        int randomNumber = Random.Range(0, 7);

        switch (randomNumber)
        {
            case 0:
                garbageName = "Battery";
                break;
            case 1:
                garbageName = "GlassBottle";
                break;
            case 2:
                garbageName = "MilkCarton";
                break;
            case 3:
                garbageName = "PaperCup";
                break;
            case 4:
                garbageName = "PlasticBag";
                break;
            case 5:
                garbageName = "PlasticBottle";
                break;
            case 6:
                garbageName = "SodaCan";
                break;
        }

        return garbageName;
    }

    protected string ConvertName(string targetIcon)
    {
        string convertedName = "";

        switch (targetIcon)
        {
            case "WheelBattery":
                convertedName = "Battery";
                break;
            case "WheelGlassBottle":
                convertedName = "GlassBottle";
                break;
            case "WheelMilkCarton":
                convertedName = "MilkCarton";
                break;
            case "WheelPlasticCup":
                convertedName = "PaperCup";
                break;
            case "WheelPlasticBag":
                convertedName = "PlasticBag";
                break;
            case "WheelPlasticBottle":
                convertedName = "PlasticBottle";
                break;
            case "WheelSodaCan":
                convertedName = "SodaCan";
                break;
        }

        return convertedName;
    }

    protected bool CheckReward(string target, int iconIndex)
    {
        Dictionary<string, string> rewardMappings = new Dictionary<string, string>
        {
            { "Battery", "GUI_Icon_Battery(Clone)" },
            { "PlasticBag", "GUI_Icon_PlasticBag(Clone)" },
            { "PaperCup", "GUI_Icon_PaperCup(Clone)" },
            { "PlasticBottle", "GUI_Icon_PlasticBottle(Clone)" },
            { "SodaCan", "GUI_Icon_SodaCan(Clone)" },
            { "MilkCarton", "GUI_Icon_MilkCarton(Clone)" },
            { "GlassBottle", "GUI_Icon_GlassBottle(Clone)" }
        };

        if (rewardMappings.TryGetValue(target, out string expectedSpriteName))
        {
            return iconImages[iconIndex].sprite.name.Contains(expectedSpriteName);
        }

        return false;
    }

    protected void PunchIcon(int index)
    {
        iconImages[index].transform.DOPunchScale(Vector3.one * 1.25f, 0.5f, 1);
    }

    protected void ShakeIcon(int index)
    {
        iconImages[index].transform.DOShakePosition(0.5f, 25f, 25, 90, false, true, ShakeRandomnessMode.Harmonic);
    }
}
