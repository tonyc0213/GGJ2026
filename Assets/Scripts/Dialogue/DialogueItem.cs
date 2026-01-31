using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class DialogueItem : MonoBehaviour
    {
        public TextMeshProUGUI myText;
        
        public void SetText(string text)
        {
            myText.text = text;
        }
    }
}