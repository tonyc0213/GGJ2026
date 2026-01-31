using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

    public class PartsGenSystem : MonoBehaviour
    {
        [System.Serializable]
        public class Parts
        {
            public int id;
            public string partName;         
            public Sprite partSprite;   
            public int category;        // 1= Eyes 2= Nose 3= Mouth 4= Eyebrows 5= Face Shapes 6= Hair 
        }

        [Header("All Parts Data")]
        public List<Parts> allParts = new List<Parts>();

        [Header("Position")]
        public UnityEngine.UI.Image[] fixedPositions = new UnityEngine.UI.Image[6];

        // check ID (key=cat, value=ID)
        private Dictionary<int, int> drawnIDs = new Dictionary<int, int>();

        void Start()
        {
            // check 6 position 
            if (fixedPositions.Length != 6)
            {
                Debug.LogError("not enough image");
            }
        }

        
        public void DrawAllSixCategories()
        {
            drawnIDs.Clear();  // clear

            for (int cat = 1; cat <= 6; cat++)
            {
                DrawSingleCategory(cat);
            }

            Debug.Log("draw complete check " + string.Join(", ", drawnIDs.Select(kv => $"cat{kv.Key}: {kv.Value}")));
        }

        // draw single 
        private void DrawSingleCategory(int category)
        {
            // found all in that cat
            var categoryCards = allParts.Where(c => c.category == category).ToList();
            if (categoryCards.Count == 0)
            {
                Debug.LogWarning($"cat {category} no parts!");
                return;
            }

            // draw one part
            int randomIndex = Random.Range(0, categoryCards.Count);
            Parts drawn = categoryCards[randomIndex];

            // check ID
            drawnIDs[category] = drawn.id;

            // put on position (cat1 ¡÷ index 0, cat2 ¡÷ index 1...)
            int positionIndex = category - 1;
            if (positionIndex < fixedPositions.Length && fixedPositions[positionIndex] != null)
            {
                fixedPositions[positionIndex].sprite = drawn.partSprite;
                fixedPositions[positionIndex].enabled = true;
            }

            Debug.Log($"cat {category} draw¡G{drawn.partName} (ID: {drawn.id})");
        }

        // save all id
        public Dictionary<int, int> GetDrawnIDs()
        {
            return new Dictionary<int, int>(drawnIDs);
        }

        // clear all
        public void ClearAll()
        {
            drawnIDs.Clear();
            foreach (var img in fixedPositions)
            {
                if (img != null) img.enabled = false;
            }
        }

    private void Update()
    {
        //Debug.Log("draw complete check " + string.Join(", ", drawnIDs.Select(kv => $"cat{kv.Key}: {kv.Value}")));
    }


}

