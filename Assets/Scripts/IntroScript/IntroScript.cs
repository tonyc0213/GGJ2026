using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    [SerializeField]
    private int lauguageId;
    public string Scene;
    private GameObject Cn;
    private GameObject Eng;



    void Start()
    {
        lauguageId = PlayerPrefs.GetInt("language");
        if(lauguageId == 1)
        {
            Cn.SetActive(false);
            Eng.SetActive(true);
        }
        else
        {
            Cn.SetActive(true);
            Eng.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            //gameObject.SetActive(false);
            SceneManager.LoadScene("Scene");
            
        }
    }
}
