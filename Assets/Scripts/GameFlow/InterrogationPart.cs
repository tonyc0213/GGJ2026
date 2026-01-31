using System;
using System.Collections.Generic;
using PartsGen;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    public class InterrogationPart : MonoBehaviour
    {
        public int suspectCount;
        public float drawTime;

        public PartsGenSystem faceGenerator;
        public MaskDrawer maskDrawer;
        private List<long> suspectFaceHash;

        private int currentIndex;

        public Dictionary<long, Texture2D> drawnFaces = new Dictionary<long, Texture2D>();
        
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
            suspectFaceHash = faceGenerator.GenerateFaces(suspectCount);
            maskDrawer.Clear();
        }

        void ShowSuspect()
        {
            Debug.Log($"Showing suspect {currentIndex}");
            faceGenerator.DrawGeneratedFace(currentIndex);
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
            drawnFaces.Add(suspectFaceHash[currentIndex], maskDrawer.ExportTexture());
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