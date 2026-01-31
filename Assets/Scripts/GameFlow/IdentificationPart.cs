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

namespace GameFlow
{
    public class IdentificationPart : MonoBehaviour
    {
        public IdentificationSuspect suspectPrefab;

        public RectTransform layoutRoot;
        public List<IdentificationSuspect> suspects = new List<IdentificationSuspect>();

        public List<RealCulpritSnippet> faceGenerators;
        public List<PartsGenSystem> answerUIFaces;

        public GameObject answerUI;
        public TextMeshProUGUI answerText;
        public Button restartButton;

        public UnityEvent OnCorrectAnswer;
        public UnityEvent OnIncorrectAnswer;

        private void Start()
        {
            GenerateSuspects();
            restartButton.onClick.AddListener(Restart);
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

            List<Vector2> usedPositions = new List<Vector2>();
            foreach (var faceGenerator in faceGenerators)
            {
                faceGenerator.face.DrawFace(FaceAndDrawings.singleton.suspectFaceHash[FaceAndDrawings.singleton.realCulpritIndex]);
                faceGenerator.RandomizePosition(usedPositions);
            }
        }

        void OnClickSuspect(long suspectHash)
        {
            foreach (var partsGenSystem in answerUIFaces)
            {
                partsGenSystem.DrawFace(suspectHash);
            }
            
            if (suspectHash == FaceAndDrawings.singleton.suspectFaceHash[FaceAndDrawings.singleton.realCulpritIndex])
            {
                FaceAndDrawings.singleton.difficultyIncrease++;
                answerText.SetText("Correct Answer!");
                OnCorrectAnswer.Invoke();
            }
            else
            {
                answerText.SetText("Wrong Answer");
                OnIncorrectAnswer.Invoke();
            }
            answerUI.SetActive(true);
        }

        void Restart()
        {
            SceneManager.LoadScene("Scenes/Interrogation");
        }
    }
}