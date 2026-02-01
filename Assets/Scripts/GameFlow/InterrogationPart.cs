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
        public float stayAfterDrawTime;

        public PartsGenSystem faceGenerator;
        public MaskSketchbook maskSketchbook;
        public Timer timer;
        public DialogueLog dialogueLog;

        public UnityEvent OnStart;
        public UnityEvent OnShowNewSuspect;
        public UnityEvent OnStartDrawingSuspect;
        public UnityEvent OnSuspectLeave;
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

            if (currentIndex == 0)
            {
                OnStart.Invoke();
            }
            OnShowNewSuspect.Invoke();
        }

        void ShowNewSuspect()
        {
            var transitionTime = currentIndex == 0 ? transitionInTime + transitionOutTime : transitionInTime;
            
            suspectMask.SetActive(false);
            faceGenerator.DrawGeneratedFace(currentIndex);
            Delay(transitionTime, StartDrawTimer);
        }

        void ShowReappearingSuspect()
        {
            faceGenerator.ClearFace();
            var faceHash = FaceAndDrawings.singleton.suspectFaceHash[currentIndex];
            SetSuspectMask(faceHash);
            maskSketchbook.Clear();

            var transitionTime = currentIndex == 0 ? transitionInTime + transitionOutTime : transitionInTime;
            Delay(transitionTime, StartShowReappearingSuspect);
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

        void StartShowReappearingSuspect()
        {
            timer.StartTimer(reappearingSuspectStayTime);
            
            var dialogue = reappearingSuspectDialogueIndex.itemList[Random.Range(0, newSuspectDialogueIndex.itemList.Count)].item;
            var delay = Mathf.Min(1, reappearingSuspectStayTime / dialogue.dialogueList.Count);
            dialogueLog.SetDialogue(dialogue, delay);
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
            var reappearing = !FaceAndDrawings.singleton.drawnFaces.TryAdd(faceHash, maskSketchbook.ExportTexture());
            
            SetSuspectMask(faceHash);
            maskSketchbook.Clear();
            maskSketchbook.SetAllowDrawing(false);
            currentIndex++;
            
            if (!reappearing)
            {
                Delay(stayAfterDrawTime, SuspectLeave);
            }
            else
            {
                SuspectLeave();
            }
        }

        void SuspectLeave()
        {
            dialogueLog.Clear();
            OnSuspectLeave.Invoke();
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