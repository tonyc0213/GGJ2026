using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{

    [SerializeField]
    private string StartGameScene;

    



    public void StartGame()
    {
        SceneManager.LoadScene(StartGameScene);
    }




}
