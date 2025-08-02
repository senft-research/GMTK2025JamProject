using System;
using System.Collections.Generic;
using KBCore.Refs;
using TMPro;

namespace _Scripts.Model.Level.Canvas
{
    public class LevelObjectives : ValidatedMonoBehaviour
    {
        private TMP_Text _text;
        private string _stringToSend;

        void Awake()
        {
            _text = gameObject.GetComponent<TMP_Text>();
        }
        
        public void SetObjectives(List<string> objectives)
        {
            _stringToSend = String.Empty;
            foreach (string objective in objectives)
            {
                _stringToSend += String.Format("• {0}\n", objective);
            }

            //_text.text = _stringToSend;
            _text.SetText(_stringToSend);
        }
    }
}
