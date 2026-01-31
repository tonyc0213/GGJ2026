using System;
using System.Collections.Generic;
using Data.ScriptableObject.Abstract;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    public struct Dialogues
    {
        public List<string> dialogueList;
    }
    
    [CreateAssetMenu(menuName = "ScriptableObjects/DialogueIndex")]
    public class DialogueIndexSO : ScriptableObjectIndex<int, Dialogues> { }
}