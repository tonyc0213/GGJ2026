using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerUpdater : MonoBehaviour
    {
        Canvas canvas;
        CanvasScaler canvasScaler;

        private (float width, float height) screenResolution;
        void Start()
        {
            canvas = GetComponent<Canvas>();
            canvasScaler = GetComponent<CanvasScaler>();

            screenResolution = (Screen.width, Screen.height);
            
            UpdateCanvasScaler();
        }

        private void UpdateCanvasScaler()
        {
            canvasScaler.matchWidthOrHeight = screenResolution.width / screenResolution.height > 16 / 9f ? 1 : 0;
        }
    
        void Update()
        {
            if (!Mathf.Approximately(screenResolution.width, Screen.width) || !Mathf.Approximately(screenResolution.height, Screen.height))
            {
                screenResolution = (Screen.width, Screen.height);
                UpdateCanvasScaler();
            }
        }
    }
}
