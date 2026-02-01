using System;
using System.Collections.Generic;
using System.Linq;
using Identification;
using PartsGen;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using UnityEngine.Audio;

namespace GameFlow
{
    public class IdentificationPart : MonoBehaviour
    {
        public DifficultySettingSO difficultySettings;
        public SnippetPositionSO snippetPositions;
        public IdentificationSuspect suspectPrefab;

        public RectTransform layoutRoot;
        public List<IdentificationSuspect> suspects = new List<IdentificationSuspect>();

        public List<RealCulpritSnippet> faceGenerators;
        public List<PartsGenSystem> answerUIFaces;

        public GameObject answerUI;
        public TextMeshProUGUI answerTextEn;
        public TextMeshProUGUI answerTextCn;
        public TextMeshProUGUI restartButtonTextEn;
        public TextMeshProUGUI restartButtonTextCn;
        public TextMeshProUGUI sueDialogEn;
        public TextMeshProUGUI sueDialogCn;
        public Button restartButton;
        public Button backToMainMenuButton;

        public UnityEvent OnCorrectAnswer;
        public UnityEvent OnIncorrectAnswer;

        public AudioSource correctMusic;
        public AudioSource wrongMusic;


        private void Start()
        {
            GenerateSuspects();
            restartButton.onClick.AddListener(Restart);
            backToMainMenuButton.onClick.AddListener(GotoMainMenu);
            answerUI.SetActive(false);
        }

        void GenerateSuspects()
        {
            var faceHashes = FaceAndDrawings.singleton.suspectFaceHash;
            faceHashes.ShuffleList();
            foreach (var hash in faceHashes)
            {
                var face = FaceAndDrawings.singleton.drawnFaces[hash];
                var newSuspect = Instantiate(suspectPrefab, layoutRoot);
                newSuspect.SetMask(hash,face);
                newSuspect.SetCallback(OnClickSuspect);
                suspects.Add(newSuspect);
            }

            List<Vector3> usedPositions = new List<Vector3>();
            var difficulty = Math.Min(FaceAndDrawings.singleton.difficultyIncrease, difficultySettings.itemList.Count - 1);
            var difficultySetting = difficultySettings.GetItem(difficulty);
            var snippetPositionsList = snippetPositions.GetItem(difficultySetting.snippetListId);
            foreach (var faceGenerator in faceGenerators)
            {
                faceGenerator.face.DrawFace(FaceAndDrawings.singleton.suspectFaceHash[FaceAndDrawings.singleton.realCulpritIndex]);
                
                Vector3 position;
                do
                {
                    position = snippetPositionsList[UnityEngine.Random.Range(0, snippetPositionsList.Count)];
                } 
                while (usedPositions.Contains(position));
                usedPositions.Add(position);
                
                faceGenerator.SetPositionAndScale(position, position.z);
            }
        }

        void OnClickSuspect(long suspectHash)
        {
            foreach (var partsGenSystem in answerUIFaces)
            {
                partsGenSystem.DrawFace(suspectHash);
            }
            
            var langId=  PlayerPrefs.GetInt("language");
            if (langId == 2)
            {
                answerTextEn.gameObject.SetActive(false);
                restartButtonTextEn.gameObject.SetActive(false);
                sueDialogEn.gameObject.SetActive(false);
                answerTextCn.gameObject.SetActive(true);
                restartButtonTextCn.gameObject.SetActive(true);
                sueDialogCn.gameObject.SetActive(true);
            }
            else
            {
                answerTextEn.gameObject.SetActive(true);
                restartButtonTextEn.gameObject.SetActive(true);
                sueDialogEn.gameObject.SetActive(true);
                answerTextCn.gameObject.SetActive(false);
                restartButtonTextCn.gameObject.SetActive(false);
                sueDialogCn.gameObject.SetActive(false);
            }
            
            if (suspectHash == FaceAndDrawings.singleton.suspectFaceHash[FaceAndDrawings.singleton.realCulpritIndex])
            {
                FaceAndDrawings.singleton.difficultyIncrease++;
                answerTextEn.SetText("Correct Answer!");
                answerTextCn.SetText("正確！");
                restartButtonTextEn.SetText("Continue");
                restartButtonTextCn.SetText("繼續");
                correctMusic.Play();
                OnCorrectAnswer.Invoke();
            }
            else
            {
                answerTextEn.SetText("Wrong Answer");
                answerTextCn.SetText("錯誤");
                restartButtonTextEn.SetText("Restart");
                restartButtonTextCn.SetText( "再來");
                
                wrongMusic.Play();
                OnIncorrectAnswer.Invoke();
            }
            answerUI.SetActive(true);
        }

        void Restart()
        {
            SceneManager.LoadScene("Scenes/Interrogation");
        }

        void GotoMainMenu()
        {
            SceneManager.LoadScene("Scenes/MenuScene");
        }
    }
}