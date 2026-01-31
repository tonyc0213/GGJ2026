using System;
using System.Collections;
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
        public float transitionInTime;
        public float transitionOutTime;

        public PartsGenSystem faceGenerator;
        public MaskDrawer maskDrawer;
        public Timer timer;

        public UnityEvent OnShowNewSuspect;
        public UnityEvent OnSSuspectLeave;
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
            FaceAndDrawings.singleton.drawnFaces.Clear(); 
            maskDrawer.Clear();

            timer.OnTimesUp += OnSuspectDone;
        }

        void ShowSuspect()
        {
            Debug.Log($"Showing suspect {currentIndex}");
            faceGenerator.DrawGeneratedFace(currentIndex);
            OnShowNewSuspect.Invoke();
            Delay(transitionInTime, StartTimer);
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
            
            OnSSuspectLeave.Invoke();
            Delay(transitionOutTime,CheckNextSuspect);
        }

        void CheckNextSuspect()
        {
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

        void Delay(float delay, Action callback)
        {
            StartCoroutine(DelayCoroutine(delay, callback));
        }

        IEnumerator DelayCoroutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }
    }
}