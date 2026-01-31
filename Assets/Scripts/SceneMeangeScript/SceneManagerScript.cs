using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SceneManagerScript : MonoBehaviour
{

    [SerializeField]
    private string StartGameScene;

    public int languageId =1; //1 =eng 2=can

    public GameObject engOptionMenu;
    public GameObject canOptionMenu;

    public GameObject engStartBtn;
    public GameObject engOptionBtn;

    public GameObject canStartBtn;
    public GameObject canOptionBtn;

    public GameObject MainUi;

    public AudioMixer mixer;
    public Toggle bgmSound;
    public Toggle sfxSound;


    public AudioSource sfx;
    public GameObject bgm;


    private void Start()
    {

        if (bgmSound) bgmSound.onValueChanged.AddListener(ToggleBGM);
        if (sfxSound) sfxSound.onValueChanged.AddListener(ToggleSFX);
        if (bgmSound) bgmSound.isOn = true;
        if (sfxSound) sfxSound.isOn = true;

        ToggleBGM(bgmSound.isOn);
        ToggleSFX(sfxSound.isOn);


    }


    public void StartGame()
    {

        PlayerPrefs.SetInt("language", languageId);
        SceneManager.LoadScene(StartGameScene);
    }

    public void OpenMenu()
    {
        MainUi.SetActive(true);


        if (languageId ==1)
        {
            engOptionMenu.SetActive(true);
        }
        if (languageId == 1)
        {
            canOptionMenu.SetActive(true);
        }
    }

    public void CloseMenu()
    {
        MainUi.SetActive(false);

        if (languageId == 1)
        {
            engOptionMenu.SetActive(false);
        }
        if (languageId == 1)
        {
            canOptionMenu.SetActive(false);
        }
    }


    public void ChangeToEng()
    {
        languageId = 1;
        engOptionBtn.SetActive(true);
        engStartBtn.SetActive(true);
        canOptionBtn.SetActive(false);
        canStartBtn.SetActive(false);
    }

    public void ChangeToCan()
    {
        languageId = 2;
        engOptionBtn.SetActive(true);
        engStartBtn.SetActive(true);
        canOptionBtn.SetActive(false);
        canStartBtn.SetActive(false);
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
