using System;
using _Scripts.UI.Buttons;
using UnityEngine;

namespace _Scripts.UI
{
    public class RectBar : MonoBehaviour, IUiBar
    {
        public BarType barType;

        protected float MaxSize;

        protected RectTransform RectTransform;

        void Start()
        {
            UiBarManager.Instance.RegisterBar(this);
            MaxSize = gameObject.GetComponentInParent<RectTransform>().rect.width;
            RectTransform = gameObject.GetComponent<RectTransform>();
        }
        
        public BarType GetBarType()
        {
            return this.barType;
        }

        public void ChangeBarPercent(float percent)
        {
            RectTransform.offsetMax = new Vector2(-((1 - percent) * MaxSize), RectTransform.offsetMax.y);
        }

        public float GetBarPercent()
        {
            return 1 - (-RectTransform.offsetMax.x / MaxSize);
        }
    }
}