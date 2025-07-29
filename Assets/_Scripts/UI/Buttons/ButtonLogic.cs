using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.UI.Buttons
{
    public abstract class ButtonLogic : ValidatedMonoBehaviour
    {
        [FormerlySerializedAs("_button")]
        [SerializeField, Self]
        protected Button button;

        void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();
    }
}
