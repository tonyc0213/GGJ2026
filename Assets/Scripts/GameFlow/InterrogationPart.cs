using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MaskDrawer;
using PartsGen;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace GameFlow
{
    public class InterrogationPart : MonoBehaviour
    {
        public DifficultySettingSO difficultySettings;
        public int startingDifficulty;

        public float reappearingSuspectStayTime;
        
        private int suspectCount;
        private float drawTime;
        private int reappearingSuspectsCount;
        
        public float transitionInTime;
        public float transitionOutTime;

        public PartsGenSystem faceGenerator;
        public MaskSketchbook maskSketchbook;
        public Timer timer;

        public UnityEvent OnShowNewSuspect;
        public UnityEvent OnSSuspectLeave;
        private int currentIndex;
        
        public GameObject suspectMask;
        public RawImage suspectMaskTexture;
        
        private void Start()
        {
            Initialize();
            ShowNextSuspect();
        }

        void Initialize()
        {
            var difficulty = Math.Min(startingDifficulty + FaceAndDrawings.singleton.difficultyIncrease, difficultySettings.itemList.Count);
            var difficultySetting = difficultySettings.GetItem(difficulty);

            suspectCount = difficultySetting.suspectCount;
            drawTime = difficultySetting.drawTime;
            reappearingSuspectsCount = difficultySetting.reappearingSuspectCounts;
            
            currentIndex = 0;

            if (reappearingSuspectsCount > FaceAndDrawings.singleton.drawnFaces.Count)
            {
                FaceAndDrawings.singleton.suspectFaceHash = faceGenerator.GenerateFaces(suspectCount);
            }
            else
            {
                var list = faceGenerator.GenerateFaces(suspectCount - reappearingSuspectsCount);

                var drawnSuspectIds = FaceAndDrawings.singleton.drawnFaces.Keys.ToList();
                for (int i = 0; i < reappearingSuspectsCount; i++)
                {
                    var hash = drawnSuspectIds[Random.Range(0, drawnSuspectIds.Count)];
                    if (list.Contains(hash))
                    {
                        i--;
                        continue;
                    }
                    list.Add(hash);
                }
                list.ShuffleList();
                FaceAndDrawings.singleton.suspectFaceHash = list;
            }
            FaceAndDrawings.singleton.realCulpritIndex = Random.Range(0, FaceAndDrawings.singleton.suspectFaceHash.Count);
            
            maskSketchbook.Clear();

            timer.OnTimesUp += OnSuspectDone;
        }

        void ShowNextSuspect()
        {
            if (FaceAndDrawings.singleton.drawnFaces.ContainsKey(FaceAndDrawings.singleton.suspectFaceHash[currentIndex]))
            {
                ShowReappearingSuspect();
            }
            else
            {
                ShowNewSuspect();
            }
        }

        void ShowNewSuspect()
        {
            maskSketchbook.gameObject.SetActive(true);
            suspectMask.SetActive(false);
            faceGenerator.DrawGeneratedFace(currentIndex);
            OnShowNewSuspect.Invoke();
            Delay(transitionInTime, StartDrawTimer);
            //Delay(transitionInTime + 2, StartDeformFace);
        }

        void ShowReappearingSuspect()
        {
            faceGenerator.ClearFace();
            
            var faceHash = FaceAndDrawings.singleton.suspectFaceHash[currentIndex];
            SetSuspectMask(faceHash);

            maskSketchbook.Clear();
            maskSketchbook.gameObject.SetActive(false);
            
            timer.StartTimer(reappearingSuspectStayTime);
        }

        private void SetSuspectMask(long hash)
        {
            suspectMask.SetActive(true);
            suspectMaskTexture.texture = FaceAndDrawings.singleton.drawnFaces[hash];
        }

        private void StartDrawTimer()
        {
            timer.StartTimer(drawTime);
        }

        public float deformDuration;
        private void StartDeformFace()
        {
            deformStart = deformDuration;
            deformTime = deformDuration;
        }

        private float deformStart;
        private float deformTime;
        private void Update()
        {
            if (deformTime > 0)
            {
                var deformFactor = deformTime / deformStart;
                faceGenerator.SetOpacity(deformFactor);

                deformTime -= Time.deltaTime;
            }
        }

        public void ForceSuspectDone()
        {
            if (timer.CurrentTime <= 0) return;
            timer.StopTimer();
            OnSuspectDone();
        }

        void OnSuspectDone()
        {
            var faceHash = FaceAndDrawings.singleton.suspectFaceHash[currentIndex];
            FaceAndDrawings.singleton.drawnFaces.TryAdd(faceHash, maskSketchbook.ExportTexture());
            
            SetSuspectMask(faceHash);
            maskSketchbook.Clear();
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
                ShowNextSuspect();
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