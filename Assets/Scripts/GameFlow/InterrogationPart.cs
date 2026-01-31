using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialogue;
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
        public DialogueIndexSO newSuspectDialogueIndex;
        public DialogueIndexSO reappearingSuspectDialogueIndex;
        public int startingDifficulty;

        public float reappearingSuspectStayTime;
        
        private int suspectCount;
        private float drawTime;
        private int reappearingSuspectsCount;
        private int irisColorCount;
        
        public float transitionInTime;
        public float transitionOutTime;

        public PartsGenSystem faceGenerator;
        public MaskSketchbook maskSketchbook;
        public Timer timer;
        public DialogueLog dialogueLog;

        public UnityEvent OnShowNewSuspect;
        public UnityEvent OnShowReappearingSuspect;
        public UnityEvent OnStartDrawingSuspect;
        public UnityEvent OnNewSuspectLeave;
        public UnityEvent OnReappearingSuspectLeave;
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
            irisColorCount = difficultySetting.irisColorCount;
            
            currentIndex = 0;

            var colors = faceGenerator.GetRandomIrisColors(irisColorCount);
            if (reappearingSuspectsCount > FaceAndDrawings.singleton.drawnFaces.Count)
            {
                FaceAndDrawings.singleton.suspectFaceHash = faceGenerator.GenerateFaces(suspectCount, colors);
            }
            else
            {
                var list = faceGenerator.GenerateFaces(suspectCount - reappearingSuspectsCount, colors);

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
            maskSketchbook.SetAllowDrawing(false);
            
            timer.OnTimesUp += OnSuspectDone;
        }

        void ShowNextSuspect()
        {
            dialogueLog.Clear();
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
            suspectMask.SetActive(false);
            faceGenerator.DrawGeneratedFace(currentIndex);
            OnShowNewSuspect.Invoke();
            Delay(transitionInTime, StartDrawTimer);
        }

        void ShowReappearingSuspect()
        {
            faceGenerator.ClearFace();
            
            var faceHash = FaceAndDrawings.singleton.suspectFaceHash[currentIndex];
            SetSuspectMask(faceHash);

            maskSketchbook.Clear();
            OnShowReappearingSuspect.Invoke();

            var dialogue = reappearingSuspectDialogueIndex.itemList[Random.Range(0, newSuspectDialogueIndex.itemList.Count)].item;
            var delay = Mathf.Min(1, reappearingSuspectStayTime / dialogue.dialogueList.Count);
            dialogueLog.SetDialogue(dialogue, delay);
            timer.StartTimer(reappearingSuspectStayTime);
        }

        private void SetSuspectMask(long hash)
        {
            suspectMask.SetActive(true);
            suspectMaskTexture.texture = FaceAndDrawings.singleton.drawnFaces[hash];
        }

        private void StartDrawTimer()
        {
            OnStartDrawingSuspect.Invoke();
            maskSketchbook.SetAllowDrawing(true);
            timer.StartTimer(drawTime);
            
            var dialogue = newSuspectDialogueIndex.itemList[Random.Range(0, newSuspectDialogueIndex.itemList.Count)].item;
            var delay = Mathf.Min(1, drawTime / dialogue.dialogueList.Count);
            dialogueLog.SetDialogue(dialogue,delay);
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
            var reappearing = FaceAndDrawings.singleton.drawnFaces.TryAdd(faceHash, maskSketchbook.ExportTexture());
            
            SetSuspectMask(faceHash);
            maskSketchbook.Clear();
            maskSketchbook.SetAllowDrawing(false);
            currentIndex++;

            if (reappearing)
            {
                OnReappearingSuspectLeave.Invoke();
            }
            else
            {
                OnNewSuspectLeave.Invoke();
            }
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