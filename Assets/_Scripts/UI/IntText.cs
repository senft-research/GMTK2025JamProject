using TMPro;
using UnityEngine;


namespace _Scripts.UI
{
    public class IntText : MonoBehaviour,IUIText
    {
        public UiElementType textType;

        protected TMP_Text _text;
        
        public UiElementType GetElementType()
        {
            throw new System.NotImplementedException();
        }
        
        public void ChangeText(string text)
        {
            
        }

        public int GetIntegerText()
        {
            throw new System.NotImplementedException();
        }
    }
}