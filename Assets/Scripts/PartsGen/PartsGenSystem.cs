using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PartsGen
{
    public class PartsGenSystem : MonoBehaviour
    {
        [Serializable]
        public struct PartsInfo
        {
            public PartsType partsType;
            public PartsIndex partsIndex;
        }
        
        [Serializable]
        public struct PartSlotInfo
        {
            public PartsType partsType;
            public Image imageComponent;
        }
        public ColorTable IrisColorTable;

        public List<PartsInfo> partsInfos;
        private Dictionary<PartsType, PartsIndex> partsInfoDict;
        
        private Dictionary<PartsType, int> generatedIDs = new Dictionary<PartsType, int>();
        private int irisColorID;
        private Dictionary<PartsType, GameObject> generatedParts = new Dictionary<PartsType, GameObject> ();

        public List<long> generatedFaces = new List<long>();

        void Awake()
        {
            partsInfoDict = partsInfos.ToDictionary(x => x.partsType, x => x.partsIndex);
        }

        public List<long> GenerateFaces(int count)
        {
            generatedFaces.Clear();
            for (int i = 0; i < count; i++)
            {
                var rndIndex = Random.Range(0, IrisColorTable.itemList.Count);
                irisColorID = IrisColorTable.itemList[rndIndex].key;
                for (int i1 = 1; i1 <= 7; i1++)
                {
                    RandomizePart((PartsType)i1);
                }
                if(generatedFaces.Contains(GetFaceHashCode()))
                {
                    // do again if duplicated faces
                    i--; 
                }else
                {
                    generatedFaces.Add(GetFaceHashCode());
                }
            }

            return generatedFaces;
        }
        
        long GetFaceHashCode()
        {
            long hash = 0;
            hash += irisColorID;
            foreach (var (partsType,id) in generatedIDs)
            {
                hash += (long)Mathf.Pow(100, (int)partsType) * id;
            }
            
            return hash;
        }

        private void RandomizePart(PartsType partType)
        {
            var categoryCards = partsInfoDict[partType].itemList;

            int randomIndex = Random.Range(0, categoryCards.Count);
            generatedIDs[partType] = categoryCards[randomIndex].key;
        }

        public void DrawGeneratedFace(int index)
        {
            DrawFace(generatedFaces[index]);
        }

        public void DrawFace(long hash)
        {
            var irisColor = (int)(hash % 100);
            hash /= 100;

            for (int partsType = 1; partsType <= 7; partsType++)
            {
                var id = (int)(hash % 100);
                hash /= 100;
                
                var generatedPart = Instantiate(partsInfoDict[(PartsType)partsType].GetItem(id).partPrefab, transform);
                generatedPart.transform.SetAsLastSibling();
                generatedParts[(PartsType)partsType] =  generatedPart;

                if ((PartsType)partsType == PartsType.Eyes)
                {
                    var eyePart = generatedPart.GetComponent<EyePart>();
                    eyePart.SetIrisColor(IrisColorTable.GetItem(irisColor));
                }
            }
        }
        
        public void ClearAll()
        {
            generatedIDs.Clear();
            foreach (var (partsType, generatedPart) in generatedParts)
            {
                Destroy(generatedPart);
            }
            generatedParts.Clear();
        }
    }
}