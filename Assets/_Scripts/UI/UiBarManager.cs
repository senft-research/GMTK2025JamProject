using System.Collections.Generic;
using _Scripts.UI.Buttons;
using UnityEngine;

namespace _Scripts.UI
{
    public class UiBarManager : MonoBehaviour
    {
        public static UiBarManager Instance { get; private set; }
        
        private Dictionary<BarType, IUiBar> _bars;

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
                _bars = new Dictionary<BarType, IUiBar>();
            }
        }

        public void RegisterBar(IUiBar bar)
        {
            this._bars.Add(bar.GetBarType(), bar);
        }

        public void UnregisterBar(BarType barType)
        {
            this._bars.Remove(barType);
        }

        public void ChangeBarPercent(BarType barType, float percent)
        {
            _bars[barType].ChangeBarPercent(percent);
        }
    }
}