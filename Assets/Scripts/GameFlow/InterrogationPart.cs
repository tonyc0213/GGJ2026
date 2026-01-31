using System;
using System.Collections.Generic;
using PartsGen;
using UnityEngine;
using UnityEngine.Events;
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


        public UnityEvent OnShowNewSuspect;
        private int currentIndex;
        
        private void Start()
        {
            Initialize();
            ShowSuspect();
        }

        private void Update()
        {
            if (timer <= 0)
            {
                return;
            }
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                OnSuspectDone();
            }
        }

        void Initialize()
        {
            currentIndex = 0;
            FaceAndDrawings.singleton.suspectFaceHash = faceGenerator.GenerateFaces(suspectCount);
            FaceAndDrawings.singleton.realCulpritIndex = Random.Range(0, FaceAndDrawings.singleton.suspectFaceHash.Count);
            maskDrawer.Clear();
        }

        void ShowSuspect()
        {
            Debug.Log($"Showing suspect {currentIndex}");
            faceGenerator.DrawGeneratedFace(currentIndex);
            OnShowNewSuspect.Invoke();
            StartTimer();
        }

        private float timer;
        private void StartTimer()
        {
            timer = drawTime;
        }

        public void ForceSuspectDone()
        {
            if (timer <= 0) return;
            timer = -1;
            OnSuspectDone();
        }

        void OnSuspectDone()
        {
            FaceAndDrawings.singleton.drawnFaces.Add(FaceAndDrawings.singleton.suspectFaceHash[currentIndex], maskDrawer.ExportTexture());
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
        }
    }
}