using System;
using System.Collections.Generic;
using Data.ScriptableObject.Abstract;
using UnityEngine;

namespace PartsGen
{
    [Serializable]
    public struct Parts
    {
        public string partName;
        public GameObject partPrefab;
        public string partDescription;
    }

    public enum PartsType
    {
        FaceShapes = 1,
        Ears,
        Nose,
        Eyes,
        Mouth,
        Eyebrows,
        Hair,
    }
    [CreateAssetMenu(menuName = "ScriptableObjects/Parts")]
    public class PartsIndex : ScriptableObjectIndex<int, Parts> { }
}