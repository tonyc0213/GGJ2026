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
        
        private Dictionary<PartsType, int> drawnIDs = new Dictionary<PartsType, int>();
        private int irisColorID;
        private Dictionary<PartsType, GameObject> generatedParts = new Dictionary<PartsType, GameObject> ();

        void Start()
        {
            partsInfoDict = partsInfos.ToDictionary(x => x.partsType, x => x.partsIndex);
        }

        public void DrawAllSixCategories()
        {
            ClearAll();

            for (int i = 1; i <= 7; i++)
            {
                RandomizePart((PartsType)i);
                irisColorID = IrisColorTable.itemList[Random.Range(0, IrisColorTable.itemList.Count)].key;
            }
            DrawAllParts();
        }

        private void RandomizePart(PartsType partType)
        {
            var categoryCards = partsInfoDict[partType].itemList;

            int randomIndex = Random.Range(0, categoryCards.Count);
            drawnIDs[partType] = categoryCards[randomIndex].key;
        }

        void DrawAllParts()
        {
            foreach (var (partsType,id) in drawnIDs.ToList().OrderBy(x => x.Key))
            {
                var generatedPart = Instantiate(partsInfoDict[partsType].GetItem(id).partPrefab, transform);
                generatedPart.transform.SetAsLastSibling();
                generatedParts[partsType] =  generatedPart;

                if (partsType == PartsType.Eyes)
                {
                    var eyePart = generatedPart.GetComponent<EyePart>();
                    eyePart.SetIrisColor(IrisColorTable.GetItem(irisColorID));
                }
            }
        }

        public long GetFaceHashCode()
        {
            long hash = 0;
            hash += irisColorID;
            foreach (var (partsType,id) in drawnIDs)
            {
                hash += (long)Mathf.Pow(100, (int)partsType) * id;
            }
            
            return hash;
        }
        
        public void ClearAll()
        {
            drawnIDs.Clear();
            foreach (var (partsType, generatedPart) in generatedParts)
            {
                Destroy(generatedPart);
            }
            generatedParts.Clear();
        }
    }
}