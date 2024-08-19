using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum Sound
{
    Bgm,
    Effect,
}

public class SoundManager : Singleton<SoundManager>
{
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioClip[] bgmaudioClips;
    [SerializeField]
    private AudioClip[] sfxaudioClips;
    [SerializeField]
    private AudioClip[] attackClip;

    [SerializeField]
    private AudioMixerGroup bgmMixerGroup;
    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    [HideInInspector]
    public Slider bgmSlider;
    [HideInInspector]
    public Slider sfxSlider;
    [HideInInspector]
    public Toggle bgmToggle;
    [HideInInspector]
    public Toggle sfxToggle;

    private const string BGM_VOLUME_PARAM = "BGM";
    private const string SFX_VOLUME_PARAM = "SFX";

    private const string BGM_TOGGLE_KEY = "BGM_Toggle";
    private const string SFX_TOGGLE_KEY = "SFX_Toggle";
    private const string BGM_VOLUME_KEY = "BGM_Volume";
    private const string SFX_VOLUME_KEY = "SFX_Volume";

    public override void Awake()
    {
        base.Awake();
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;

        bgmSource.volume = 0.3f;

        foreach (var clip in bgmaudioClips)
        {
            _audioClips[clip.name] = clip;
        }

        foreach (var clip in sfxaudioClips)
        {
            _audioClips[clip.name] = clip;
        }
    }

    public void InitializeSoundOption()
    {
        // 슬라이더 및 토글 초기 값 설정
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        bgmToggle.onValueChanged.AddListener(ToggleBGM);
        sfxToggle.onValueChanged.AddListener(ToggleSFX);

        LoadSettings(); // 저장된 설정을 불러와서 적용

        // 초기 볼륨을 슬라이더에 반영
        float bgmVolume;
        audioMixer.GetFloat(BGM_VOLUME_PARAM, out bgmVolume);
        bgmSlider.value = Mathf.Pow(10, bgmVolume / 20); // dB to linear

        float sfxVolume;
        audioMixer.GetFloat(SFX_VOLUME_PARAM, out sfxVolume);
        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20); // dB to linear

        // 초기 토글 상태 설정
        bgmToggle.isOn = bgmVolume < -80f; // -80dB는 음소거로 간주
        sfxToggle.isOn = sfxVolume < -80f;
    }

    public void PlayAudio(Sound soundType, string clipName)
    {
        if (!_audioClips.TryGetValue(clipName, out var clip))
        {
            return;
        }

        if (soundType == Sound.Bgm)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (soundType == Sound.Effect)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    // BGM 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        if (volume <= 0.001f) // 슬라이더 값이 0에 가깝다면
        {
            audioMixer.SetFloat(BGM_VOLUME_PARAM, -80f); // 음소거 (-80dB)
            bgmToggle.isOn = true; // 음소거 상태일 때 토글을 true로 설정
        }
        else
        {
            audioMixer.SetFloat(BGM_VOLUME_PARAM, Mathf.Log10(volume) * 20); // dB 단위로 변환
            bgmToggle.isOn = false; // 음소거 해제 상태일 때 토글을 false로 설정
        }

        SaveSettings(); // 설정 저장
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.001f) // 슬라이더 값이 0에 가깝다면
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, -80f); // 음소거 (-80dB)
            sfxToggle.isOn = true; // 음소거 상태일 때 토글을 true로 설정
        }
        else
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, Mathf.Log10(volume) * 20); // dB 단위로 변환
            sfxToggle.isOn = false; // 음소거 해제 상태일 때 토글을 false로 설정
        }

        SaveSettings(); // 설정 저장
    }

    // BGM 토글 처리
    public void ToggleBGM(bool isOn)
    {
        if (isOn)
        {
            // BGM 음소거 (볼륨을 매우 낮게 설정)
            audioMixer.SetFloat(BGM_VOLUME_PARAM, -80f);
            bgmSlider.value = 0; // 슬라이더 값을 0으로 설정
        }
        else
        {
            // 슬라이더 값으로 볼륨 복구
            SetBGMVolume(bgmSlider.value);
        }

        SaveSettings(); // 설정 저장
    }

    // SFX 토글 처리
    public void ToggleSFX(bool isOn)
    {
        if (isOn)
        {
            // SFX 음소거 (볼륨을 매우 낮게 설정)
            audioMixer.SetFloat(SFX_VOLUME_PARAM, -80f);
            sfxSlider.value = 0; // 슬라이더 값을 0으로 설정
        }
        else
        {
            // 슬라이더 값으로 볼륨 복구
            SetSFXVolume(sfxSlider.value);
        }

        SaveSettings(); // 설정 저장
    }

    private void SaveSettings()
    {
        // 저장된 토글 상태 저장
        PlayerPrefs.SetInt(BGM_TOGGLE_KEY, bgmToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(SFX_TOGGLE_KEY, sfxToggle.isOn ? 1 : 0);

        // 슬라이더의 현재 값을 저장
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmSlider.value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxSlider.value);

        PlayerPrefs.Save(); // 변경 사항을 저장
    }

    private void LoadSettings()
    {
        // 저장된 토글 상태 불러오기
        bool bgmIsMuted = PlayerPrefs.GetInt(BGM_TOGGLE_KEY, 0) == 1;
        bool sfxIsMuted = PlayerPrefs.GetInt(SFX_TOGGLE_KEY, 0) == 1;

        bgmToggle.isOn = bgmIsMuted;
        sfxToggle.isOn = sfxIsMuted;

        // 저장된 슬라이더 값 불러오기
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f); // 기본값 0.5
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f); // 기본값 0.5

        if (bgmIsMuted)
        {
            audioMixer.SetFloat(BGM_VOLUME_PARAM, -80f); // 음소거 (-80dB)
            bgmSlider.value = 0; // 음소거 상태에서는 슬라이더 값을 0으로 설정
        }
        else
        {
            SetBGMVolume(bgmVolume);
            bgmSlider.value = bgmVolume; // 저장된 슬라이더 값으로 설정
        }

        if (sfxIsMuted)
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, -80f); // 음소거 (-80dB)
            sfxSlider.value = 0; // 음소거 상태에서는 슬라이더 값을 0으로 설정
        }
        else
        {
            SetSFXVolume(sfxVolume);
            sfxSlider.value = sfxVolume; // 저장된 슬라이더 값으로 설정
        }
    }
}