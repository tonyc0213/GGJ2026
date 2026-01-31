using System;
using System.Collections.Generic;
using System.Linq;
using Identification;
using PartsGen;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameFlow
{
    public class IdentificationPart : MonoBehaviour
    {
        public IdentificationSuspect suspectPrefab;

        public RectTransform layoutRoot;
        public List<IdentificationSuspect> suspects = new List<IdentificationSuspect>();

        public List<RealCulpritSnippet> faceGenerators;

        private void Start()
        {
            GenerateSuspects();
        }

        void GenerateSuspects()
        {
            var faces = FaceAndDrawings.singleton.drawnFaces.ToList();
            faces.ShuffleList();
            foreach (var (hash, face) in faces)
            {
                var newSuspect = Instantiate(suspectPrefab, layoutRoot);
                newSuspect.SetMask(hash,face);
                newSuspect.SetCallback(OnClickSuspect);
                suspects.Add(newSuspect);
            }

            List<Vector2> usedPositions = new List<Vector2>();
            foreach (var faceGenerator in faceGenerators)
            {
                faceGenerator.face.DrawFace(FaceAndDrawings.singleton.suspectFaceHash[FaceAndDrawings.singleton.realCulpritIndex]);
                faceGenerator.RandomizePosition(usedPositions);
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