using System.Collections.Generic;
using UnityEngine;

namespace GameFlow
{
    public class FaceAndDrawings
    {
        public static FaceAndDrawings singleton => _singleton ??= new FaceAndDrawings();
        private static FaceAndDrawings _singleton;
        
        public List<long> suspectFaceHash;
        public Dictionary<long, Texture2D> drawnFaces = new Dictionary<long, Texture2D>();
        public int realCulpritIndex;
        
        public List<long> reappearingSuspects = new List<long>();

        public int difficulty = 0;
    }
}