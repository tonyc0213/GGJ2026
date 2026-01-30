using UnityEngine;

namespace Utils
{
    public class SaveLoadTest : MonoBehaviour
    {
        public MaskDrawer maskDrawer;

        public void SaveTexture()
        {
            SaveLoadUtils.SaveTexture2D(maskDrawer.generatedTexture,"test");
        }

        public void LoadTexture()
        {
            var texture = SaveLoadUtils.LoadTexture2D("test");
            if (texture != null)
            {
                maskDrawer.SetTexture(texture);
            }
            Destroy(texture);
        }
    }
}