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

        private void Awake()
        {
            faceTransform = face.transform as RectTransform;
        }

        public void SetPositionAndScale(Vector2 position, float scale)
        {

            faceTransform.anchoredPosition = position;
            faceTransform.localScale = new Vector3(scale, scale, 1);
        }
    }
}