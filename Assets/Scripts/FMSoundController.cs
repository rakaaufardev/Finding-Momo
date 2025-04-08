using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFX
{
    SFX_Button,
    SFX_Coin,
    SFX_Garbage,
    SFX_Jump,
    SFX_Splash,
    SFX_Trampoline,
    SFX_TransitionSurf,
    COUNT
}

public enum BGM
{
    BGM_Lobby,
    BGM_Main,
    COUNT
}

public class FMSoundController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    private Dictionary<SFX, AudioClip> sfxClips;
    private Dictionary<BGM, AudioClip> bgmClips;

    private static FMSoundController singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMSoundController Get()
    {
        return singleton;
    }

    public void Init()
    {
        sfxClips = new Dictionary<SFX, AudioClip>();
        bgmClips = new Dictionary<BGM, AudioClip>();

        int count = (int)SFX.COUNT;
        for (int i = 0; i < count; i++)
        {
            SFX sfx = (SFX)i;
            AudioClip clip = FMAssetFactory.GetSound(sfx.ToString());
            sfxClips.Add(sfx, clip);
        }

        count = (int)BGM.COUNT;
        for (int i = 0; i < count; i++)
        {
            BGM bgm = (BGM)i;
            AudioClip clip = FMAssetFactory.GetSound(bgm.ToString());
            bgmClips.Add(bgm, clip);
        }

        float bgmVolume = FMUserDataService.Get().LoadBGMVolume();
        float sfxVolume = FMUserDataService.Get().LoadSFXVolume();

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetBGMVolume(float volumePercentage)
    {
        audioMixer.SetFloat("bgm", Mathf.Log10(volumePercentage) * 20);
    }

    public void SetSFXVolume(float volumePercentage)
    {
        audioMixer.SetFloat("sfx", Mathf.Log10(volumePercentage) * 20);
    }

    public void PlaySFX(SFX sfx)
    {
        AudioClip clip = sfxClips[sfx];

        /*sfxSource.Stop();*/
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(BGM bgm)
    {
        AudioClip clip = bgmClips[bgm];
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
