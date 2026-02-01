using System;
using System.Collections.Generic;
using Data.ScriptableObject.Abstract;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue
{
    [Serializable]
    public struct Dialogues
    {
        [FormerlySerializedAs("dialogueList")] public List<string> dialogueListEng;
        public List<string> dialogueListCn;

        public List<string> GetDialogueList()
        {
            var langId=  PlayerPrefs.GetInt("language");
            return langId == 2 ? dialogueListCn : dialogueListEng;
        }
    }
    
    [CreateAssetMenu(menuName = "ScriptableObjects/DialogueIndex")]
    public class DialogueIndexSO : ScriptableObjectIndex<int, Dialogues> { }
}