using MaskDrawer;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utils
{
    public class SaveLoadTest : MonoBehaviour
    {
        [FormerlySerializedAs("maskDrawer")] public MaskSketchbook maskSketchbook;

        public void SaveTexture()
        {
            CodeUtils.SaveTexture2D(maskSketchbook.generatedTexture,"test");
        }

        public void LoadTexture()
        {
            var texture = CodeUtils.LoadTexture2D("test");
            if (texture != null)
            {
                maskSketchbook.SetTexture(texture);
            }
            Destroy(texture);
        }
    }
}