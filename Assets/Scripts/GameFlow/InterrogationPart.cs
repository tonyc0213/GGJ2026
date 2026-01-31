using System;
using System.Collections.Generic;
using PartsGen;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameFlow
{
    public class InterrogationPart : MonoBehaviour
    {
        public int suspectCount;
        public float drawTime;

        public PartsGenSystem faceGenerator;
        public MaskDrawer maskDrawer;
        public Timer timer;

        public UnityEvent OnShowNewSuspect;
        private int currentIndex;
        
        private void Start()
        {
            Initialize();
            ShowSuspect();
        }

        void Initialize()
        {
            currentIndex = 0;
            FaceAndDrawings.singleton.suspectFaceHash = faceGenerator.GenerateFaces(suspectCount);
            FaceAndDrawings.singleton.realCulpritIndex = Random.Range(0, FaceAndDrawings.singleton.suspectFaceHash.Count);
            maskDrawer.Clear();

            timer.OnTimesUp += OnSuspectDone;
        }

        void ShowSuspect()
        {
            Debug.Log($"Showing suspect {currentIndex}");
            faceGenerator.DrawGeneratedFace(currentIndex);
            OnShowNewSuspect.Invoke();
            StartTimer();
        }


        private void StartTimer()
        {
            timer.StartTimer(drawTime);
        }

        public void ForceSuspectDone()
        {
            if (timer.CurrentTime <= 0) return;
            timer.StopTimer();
            OnSuspectDone();
        }

        void OnSuspectDone()
        {
            FaceAndDrawings.singleton.drawnFaces.Add(FaceAndDrawings.singleton.suspectFaceHash[currentIndex], maskDrawer.ExportTexture());
            maskDrawer.Clear();
            currentIndex++;

            if (currentIndex == suspectCount)
            {
                MoveToNextScene();
            }
            else
            {
                ShowSuspect();
            }
        }

        private void MoveToNextScene()
        {
            SceneManager.LoadScene("Scenes/Identification", LoadSceneMode.Single);
        }
    }
}