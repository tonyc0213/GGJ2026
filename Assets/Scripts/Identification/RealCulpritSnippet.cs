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

        public List<Vector3> positionsAndScale;

        private void Awake()
        {
            faceTransform = face.transform as RectTransform;
        }

        public void RandomizePosition(List<Vector3> usedPositions)
        {
            Vector3 position;
            do
            {
                position = positionsAndScale[UnityEngine.Random.Range(0, positionsAndScale.Count)];
            } 
            while (usedPositions.Contains(position));
            
            usedPositions.Add(position);
            faceTransform.anchoredPosition = position;
            faceTransform.localScale = new Vector3(position.z, position.z, 1);
        }
    }
}