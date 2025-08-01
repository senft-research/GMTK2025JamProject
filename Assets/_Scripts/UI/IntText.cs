using System;
using TMPro;
using UnityEngine;


namespace _Scripts.UI
{
    public class IntText : MonoBehaviour,IUIText
    {
        public UiElementType textType;

        public string defaultText;
        
        protected TMP_Text _text;

        public void Start()
        {
            _text = gameObject.GetComponent<TMP_Text>();
            UiManager.Instance.RegisterElement(this);
            ChangeText("0");
        }
        
        public UiElementType GetElementType()
        {
            return textType;
        }
        
        public void ChangeText(string text)
        {
            _text.text = String.Format(defaultText, text);
        }

        public string GetText()
        {
            return _text.text;
        }
    }
}