using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.UI
{
    public class UiManager : MonoBehaviour
    {
        public static UiManager Instance { get; private set; }

        private Dictionary<UiElementType, IUiElement> _elements;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new System.Exception("A UiBarManager already exists.");
            }
            else
            {
                Instance = this;
                _elements = new Dictionary<UiElementType, IUiElement>();
            }
        }

        public void RegisterElement(IUiElement element)
        {
            this._elements.Add(element.GetElementType(), element);
        }

        public void UnregisterElement(UiElementType type)
        {
            this._elements.Remove(type);
        }

        public void ChangeBarPercent(UiElementType barType, float percent)
        {
            try
            {
                ((IUiBar)_elements[barType]).ChangeBarPercent(percent);

            }
            catch (System.InvalidCastException)
            {
                Debug.LogError(String.Format("UiBarManager: Given UiElementType ({0}) is Not an instance of IUiBar",barType.ToString()));
            }
        }

        public void ChangeText(UiElementType type, string text)
        {
            try
            {
                ((IUIText)_elements[type]).ChangeText(text);
            }
            catch (System.InvalidCastException)
            {
                Debug.LogError(String.Format("UiBarManager: Given UiElementType ({0}) is Not an instance of IUiBar",type.ToString()));
            }
        }
    }
}