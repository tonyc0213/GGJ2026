using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    [SerializeField]
    private int lauguageId;
    [SerializeField] public string Scene;
    [SerializeField] private GameObject Cn;
    [SerializeField] private GameObject Eng;
    
    [SerializeField] private GameObject CnTutor;
    [SerializeField] private GameObject EngTutor;

    [SerializeField] private GameObject Intro;
    [SerializeField] private GameObject Tutor;

    private bool showingIntro = false;

    void Start()
    {
        Intro.SetActive(true);
        Tutor.SetActive(false);
        lauguageId = PlayerPrefs.GetInt("language");
        if(lauguageId == 1)
        {
            Cn.SetActive(false);
            CnTutor.SetActive(false);
            Eng.SetActive(true);
            EngTutor.SetActive(true);
        }
        else
        {
            Cn.SetActive(true);
            CnTutor.SetActive(true);
            Eng.SetActive(false);
            EngTutor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!showingIntro)
            {
                Intro.SetActive(false);
                Tutor.SetActive(true);
                showingIntro = true;
            }
            else
            {
                //gameObject.SetActive(false);
                SceneManager.LoadScene(Scene);
            }
           
        }
    }
}
