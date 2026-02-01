using System.Collections;
using System.Collections.Generic;
using GameFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SceneManagerScript : MonoBehaviour
{

    [SerializeField]
    private string StartGameScene;

    public int languageId =1; //1 =eng 2=can

    public GameObject engStartBtn;
    public GameObject canStartBtn;

    public GameObject MainUi;

    public AudioMixer mixer;
    public Toggle bgmSound;
    public Toggle sfxSound;

    public GameObject enTitle;
    public GameObject cnTitle;

    public Text bgmSoundText;
    public Text sfxSoundText;

    public Font enFont;
    public Font cnFont;

    public AudioSource sfx;
    public GameObject bgm;

    public bool resetDifficultyOnEnter;

    private void Start()
    {

        if (bgmSound) bgmSound.onValueChanged.AddListener(ToggleBGM);
        if (sfxSound) sfxSound.onValueChanged.AddListener(ToggleSFX);
        if (bgmSound) bgmSound.isOn = true;
        if (sfxSound) sfxSound.isOn = true;

        ToggleBGM(bgmSound.isOn);
        ToggleSFX(sfxSound.isOn);

        if(resetDifficultyOnEnter) FaceAndDrawings.singleton.Reset();
    }


    public void StartGame()
    {

        PlayerPrefs.SetInt("language", languageId);
        SceneManager.LoadScene(StartGameScene);
    }




    public void ChangeToEng()
    {
        languageId = 1;
        engStartBtn.SetActive(true);
        canStartBtn.SetActive(false);
        enTitle.SetActive(true);
        cnTitle.SetActive(false);
        bgmSoundText.font = enFont;
        sfxSoundText.font = enFont;
        bgmSoundText.text = ("Music");
        sfxSoundText.text = ("SFX");

    }

    public void ChangeToCan()
    {
        languageId = 2;
        engStartBtn.SetActive(false);
        canStartBtn.SetActive(true);
        enTitle.SetActive(false);
        cnTitle.SetActive(true);
        bgmSoundText.font = cnFont;
        sfxSoundText.font = cnFont;
        bgmSoundText.text = ("音樂");
        sfxSoundText.text = ("音效");

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

   public void TestSfx()
    {

        sfx.Play();
    }



}
