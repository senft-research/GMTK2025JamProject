//using TMPro;
using UnityEngine;


namespace _Scripts.UI
{
    public class IntText : MonoBehaviour,IUIText
    {
        public UiElementType textType;

        //protected TextMeshProUGUI _text;
        
        public UiElementType GetElementType()
        {
            throw new System.NotImplementedException();
        }
        
        public void ChangeText(string text)
        {
            //_text.text
        }

        public int GetIntegerText()
        {
            throw new System.NotImplementedException();
        }
    }
}