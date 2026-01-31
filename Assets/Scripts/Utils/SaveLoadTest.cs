using UnityEngine;

namespace Utils
{
    public class SaveLoadTest : MonoBehaviour
    {
        public MaskDrawer maskDrawer;

        public void SaveTexture()
        {
            CodeUtils.SaveTexture2D(maskDrawer.generatedTexture,"test");
        }

        public void LoadTexture()
        {
            var texture = CodeUtils.LoadTexture2D("test");
            if (texture != null)
            {
                maskDrawer.SetTexture(texture);
            }
            Destroy(texture);
        }
    }
}