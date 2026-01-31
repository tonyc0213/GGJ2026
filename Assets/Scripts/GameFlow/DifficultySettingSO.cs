using System;
using Data.ScriptableObject.Abstract;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public struct DifficultySetting
    {
        public int suspectCount;
        public float drawTime;
        public int reappearingSuspectCounts;
        public int irisColorCount;
        public int snippetListId;
    }
    
    [CreateAssetMenu(menuName = "ScriptableObjects/DifficultySetting")]
    public class DifficultySettingSO : ScriptableObjectIndex<int, DifficultySetting> { }
}