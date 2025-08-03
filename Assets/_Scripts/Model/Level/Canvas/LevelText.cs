using KBCore.Refs;
using TMPro;

namespace _Scripts.Model.Level.Canvas
{
    public class LevelText : ValidatedMonoBehaviour
    {
        private TMP_Text _text;

        void Awake()
        {
            _text = gameObject.GetComponent<TMP_Text>();
        }
        
        public void SetText(string text)
        {
            _text.SetText(text);
        }
        
        
    }
}
