using System;
using System.Collections.Generic;
using Identification;
using UnityEngine;
using UnityEngine.UI;

namespace GameFlow
{
    public class IdentificationPart : MonoBehaviour
    {
        public IdentificationSuspect suspectPrefab;

        public RectTransform layoutRoot;
        public List<IdentificationSuspect> suspects = new List<IdentificationSuspect>();

        private void Start()
        {
            GenerateSuspects();
        }

        void GenerateSuspects()
        {
            foreach (var (hash, face) in FaceAndDrawings.singleton.drawnFaces)
            {
                var newSuspect = Instantiate(suspectPrefab, layoutRoot);
                newSuspect.SetMask(hash,face);
                newSuspect.SetCallback(OnClickSuspect);
                suspects.Add(newSuspect);
            }
        }

        void OnClickSuspect(long suspectHash)
        {
            if (suspectHash == FaceAndDrawings.singleton.suspectFaceHash[FaceAndDrawings.singleton.realCulpritIndex])
            {
                Debug.Log("Correct Answer!");
            }
            else
            {
                Debug.Log("Wrong Answer");
            }
        }
    }
}