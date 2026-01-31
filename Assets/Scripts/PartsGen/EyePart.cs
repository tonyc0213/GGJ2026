using UnityEngine;
using UnityEngine.UI;

namespace PartsGen
{
    public class EyePart : MonoBehaviour
    {
        public Image Iris;

        public void SetIrisColor(Color color)
        {
            Iris.color = color;
        }
    }
}