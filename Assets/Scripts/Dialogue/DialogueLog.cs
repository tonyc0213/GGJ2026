using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Dialogue
{
    public class DialogueLog : MonoBehaviour
    {
        public DialogueItem detectiveDialoguePrefab;
        public DialogueItem suspectDialoguePrefab;
        
        public RectTransform layoutRoot;
        public ScrollRect scroll;
        public float delay;

        private Dialogues myDialogues;
        private int currentIndex;

        private List<DialogueItem> spawnedItems = new();
        
        public void SetDialogue(Dialogues dialogues, float delay = 1)
        {
            Clear();
            myDialogues = dialogues;
            currentIndex = 0;
            this.delay = delay;
            timer = delay;
            SpawnNextDialogue();
        }

        private float timer;

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    SpawnNextDialogue();
                }
            } 
        }

        void SpawnNextDialogue()
        {
            var text = myDialogues.dialogueListEng[currentIndex];
            var prefab = currentIndex % 2 == 0 ? detectiveDialoguePrefab : suspectDialoguePrefab;

            var dialogueItem = Instantiate(prefab, layoutRoot);
            dialogueItem.SetText(text);
            dialogueItem.transform.SetAsFirstSibling();
            spawnedItems.Add(dialogueItem);

            currentIndex++;
            if (currentIndex < myDialogues.dialogueListEng.Count)
            {
                timer = delay;
            }
        }

        public void Clear()
        {
            foreach (var dialogueItem in spawnedItems)
            {
                Destroy(dialogueItem.gameObject);
            }
            spawnedItems.Clear();
        }
    }
}