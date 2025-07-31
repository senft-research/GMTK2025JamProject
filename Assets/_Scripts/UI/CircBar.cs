using _Scripts.UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class CircBar : MonoBehaviour,IUiBar
    {
        public BarType barType;

        protected Image Circle;

        void Start()
        {
            UiBarManager.Instance.RegisterBar(this);
            Circle = gameObject.GetComponent<Image>();
        }
        
        public BarType GetBarType()
        {
            return this.barType;
        }

        public void ChangeBarPercent(float percent)
        {
            Circle.fillAmount = percent;
        }

        public float GetBarPercent()
        {
            return Circle.fillAmount;
        }
    }
}