using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    [SerializeField]
    private int lauguageId;
    
        void Start()
    {
        lauguageId = PlayerPrefs.GetInt("language");
        if(lauguageId == 1)
        {



        }
        else
        {



        }





    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            gameObject.SetActive(false);
        }
    }
}
