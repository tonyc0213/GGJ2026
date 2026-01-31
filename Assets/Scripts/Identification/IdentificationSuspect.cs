using System;
using UnityEngine;
using UnityEngine.UI;

namespace Identification
{
    public class IdentificationSuspect : MonoBehaviour
    {
        public Button myButton;
        public RawImage faceRawImage;
        private long suspectHash;
        private Texture2D faceTexture;
        private void Start()
        {
            myButton.onClick.AddListener(OnClickMyButton);
        }

        public void SetMask(long hash, Texture2D face)
        {
            suspectHash = hash;
            faceTexture = face;
            faceRawImage.texture = faceTexture;
        }

        private Action<long> callback;
        public void SetCallback(Action<long> callback)
        {
            this.callback = callback;
        }

        void OnClickMyButton()
        {
            callback.Invoke(suspectHash);
        }
    }
}