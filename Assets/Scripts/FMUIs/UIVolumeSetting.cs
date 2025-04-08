using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VD;


public class UIVolumeSetting : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private FMButton bgmFullButton;
    [SerializeField] private FMButton bgmMuteButton;
    [SerializeField] private FMButton sfxFullButton;
    [SerializeField] private FMButton sfxMuteButton;
    [SerializeField] private RectTransform toggleButtonHandler;
    [SerializeField] private TextMeshProUGUI playerName;

    [SerializeField] private FMButton buttonChangeName;
    [SerializeField] private FMButton buttonLogout;

    [SerializeField] Toggle toggleButton;

    Vector2 originalButtonHandlerPosition;

    private Image toggleButtonImage;
    private UIQuickAction uiQuickAction;
    private float sfxPercentage;
    private float bgmPercentage;
    private float lastSfxPercentage;
    private float lastBgmPercentage;

    private const float MUTE_VALUE = 0.0001f;

    public void Init(UIQuickAction inUiQuickAction)
    {
        uiQuickAction = inUiQuickAction;

        buttonChangeName.AddListener(OnClickInputName);
        buttonLogout.AddListener(OnClickLogout);

        bgmFullButton.AddListener(MuteBGMVolume);
        bgmMuteButton.AddListener(UnmuteBGMVolume);
        sfxFullButton.AddListener(MuteSFXVolume);
        sfxMuteButton.AddListener(UnmuteSFXVolume);

        bgmPercentage = FMUserDataService.Get().LoadBGMVolume();
        sfxPercentage = FMUserDataService.Get().LoadSFXVolume();

        lastBgmPercentage = bgmPercentage;
        lastSfxPercentage = sfxPercentage;

        bgmSlider.value = bgmPercentage;
        sfxSlider.value = sfxPercentage;


        bgmSlider.GetComponent<SliderDragEventHandler>().OnEndDrag.AddListener(OnBGMSliderEndDrag);
        sfxSlider.GetComponent<SliderDragEventHandler>().OnEndDrag.AddListener(OnSFXSliderEndDrag);

        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        IconHandler(bgmPercentage, bgmMuteButton, bgmFullButton);
        IconHandler(sfxPercentage, sfxMuteButton, sfxFullButton);

        bool isPlayerNameExist = FMUserDataService.Get().IsPlayerNameExist();
        if (isPlayerNameExist)
        {
            UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
            playerName.text = userInfo.currentPlayerName;
        }
        originalButtonHandlerPosition = toggleButtonHandler.anchoredPosition;
        toggleButton.isOn = FMUserDataService.Get().LoadToggleButtonValue();
        toggleButtonImage = toggleButtonHandler.GetComponent<Image>();
        OnSwitchVisibleArrow(toggleButton.isOn);
        toggleButton.onValueChanged.AddListener(OnSwitchVisibleArrow);
    }
    public void SaveVolume()
    {
        FMUserDataService.Get().SaveSFXVolume(sfxPercentage);
        FMUserDataService.Get().SaveBGMVolume(bgmPercentage);
    }

    private void SetBGMVolume(float val)
    {
        bgmPercentage = val;
        FMSoundController.Get().SetBGMVolume(bgmPercentage);
        IconHandler(bgmPercentage, bgmMuteButton, bgmFullButton);
    }

    private void MuteBGMVolume()
    {
        bgmSlider.value = bgmSlider.minValue;
        SetBGMVolume(bgmSlider.minValue);
    }

    private void UnmuteBGMVolume()
    {
        bgmSlider.value = lastBgmPercentage;
        SetBGMVolume(lastBgmPercentage);
    }

    private void SetSFXVolume(float val)
    {
        sfxPercentage = val;
        FMSoundController.Get().SetSFXVolume(sfxPercentage);
        IconHandler(sfxPercentage, sfxMuteButton, sfxFullButton);
    }

    private void MuteSFXVolume()
    {
        sfxSlider.value = sfxSlider.minValue;
        SetSFXVolume(sfxSlider.minValue);
    }

    private void UnmuteSFXVolume()
    {
        sfxSlider.value = lastSfxPercentage;
        SetSFXVolume(lastSfxPercentage);
    }

    private void OnSFXSliderEndDrag(float lastSFXPercentage)
    {
        lastSfxPercentage = lastSFXPercentage;
    }

    private void OnBGMSliderEndDrag(float lastBGMPercentage)
    {
        lastBgmPercentage = lastBGMPercentage;
    }


    private void IconHandler(float volumePercentage, FMButton iconMute, FMButton iconFull)
    {
        if (volumePercentage <= MUTE_VALUE)
        {
            iconMute.gameObject.SetActive(true);
            iconFull.gameObject.SetActive(false);
        }
        else
        {
            iconMute.gameObject.SetActive(false);
            iconFull.gameObject.SetActive(true);
        }
    }

    private void OnClickInputName()
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.InputName, uiQuickAction);
    }

    private void OnClickLogout()
    {

    }

    public void OnSwitchVisibleArrow(bool isOn)
    {
        if (isOn)
        {
            toggleButtonImage.color = Color.white;
            toggleButtonHandler.anchoredPosition = originalButtonHandlerPosition;
            //nyalain visible arrow
        }
        else
        {
            toggleButtonImage.color = Color.red;
            toggleButtonHandler.anchoredPosition = originalButtonHandlerPosition*-1;
            //matiin visible arrow
        }
        
        FMUserDataService.Get().SaveToggleButtonValue(isOn);
    }
}
