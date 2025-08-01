using UnityEngine;

namespace _Scripts.UI
{
    public class RectBar : MonoBehaviour, IUiBar
    {
        public UiElementType barType;

        protected float MaxSize;

        protected RectTransform RectTransform;

        void Start()
        {
            UiManager.Instance.RegisterElement(this);
            MaxSize = gameObject.GetComponentInParent<RectTransform>().rect.width;
            RectTransform = gameObject.GetComponent<RectTransform>();
        }
        
        

        public void ChangeBarPercent(float percent)
        {
            RectTransform.offsetMax = new Vector2(-((1 - percent) * MaxSize), RectTransform.offsetMax.y);
        }

        public float GetBarPercent()
        {
            return 1 - (-RectTransform.offsetMax.x / MaxSize);
        }

        public UiElementType GetElementType()
        {
            return this.barType;
        }
    }
}