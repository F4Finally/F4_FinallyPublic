using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionInitializer : MonoBehaviour
{
    [SerializeField]
    private Slider BGMSlider;
    [SerializeField]
    private Slider SFXSlider;
    [SerializeField]
    private Toggle BGMToggle;
    [SerializeField]
    private Toggle SFXToggle;

    private void Start()
    {
        SoundManager.Instance.bgmSlider = BGMSlider;
        SoundManager.Instance.sfxSlider = SFXSlider;
        SoundManager.Instance.bgmToggle = BGMToggle;
        SoundManager.Instance.sfxToggle = SFXToggle;

        SoundManager.Instance.InitializeSoundOption();
    }


}
