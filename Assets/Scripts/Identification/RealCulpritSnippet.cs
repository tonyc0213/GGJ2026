using System;
using System.Collections.Generic;
using PartsGen;
using UnityEngine;

namespace Identification
{
    public class RealCulpritSnippet : MonoBehaviour
    {
        public PartsGenSystem face;
        private RectTransform faceTransform;

        public List<Vector2> positions;

        private void Awake()
        {
            faceTransform = face.transform as RectTransform;
        }

        public void RandomizePosition(List<Vector2> usedPositions)
        {
            Vector2 position;
            do
            {
                position = positions[UnityEngine.Random.Range(0, positions.Count)];
            } 
            while (usedPositions.Contains(position));
            
            usedPositions.Add(position);
            faceTransform.anchoredPosition = position;
        }
    }
}