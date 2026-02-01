using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }



    public AudioMixer mixer;
    public Toggle bgmSound;
    public Toggle sfxSound;

    public Text bgmSoundText;
    public Text sfxSoundText;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (bgmSound) bgmSound.onValueChanged.AddListener(ToggleBGM);
        if (sfxSound) sfxSound.onValueChanged.AddListener(ToggleSFX);
        if (bgmSound) bgmSound.isOn = true;
        if (sfxSound) sfxSound.isOn = true;

        ToggleBGM(bgmSound.isOn);
        ToggleSFX(sfxSound.isOn);
    }

    private void ToggleBGM(bool isOn)
    {
        float volume = isOn ? 0f : -80f;
        mixer.SetFloat("BGMVolume", volume);
    }


    private void ToggleSFX(bool isOn)
    {
        float volume = isOn ? 0f : -80f;
        mixer.SetFloat("SFXVolume", volume);
    }
}