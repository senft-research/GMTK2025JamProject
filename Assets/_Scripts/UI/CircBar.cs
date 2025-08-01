using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class CircBar : MonoBehaviour,IUiBar
    {
        public UiElementType barType;

        protected Image Circle;

        void Start()
        {
            UiBarManager.Instance.RegisterElement(this);
            Circle = gameObject.GetComponent<Image>();
        }

        public void ChangeBarPercent(float percent)
        {
            Circle.fillAmount = percent;
        }

        public float GetBarPercent()
        {
            return Circle.fillAmount;
        }

        public UiElementType GetElementType()
        {
            return this.barType;
        }
    }
}